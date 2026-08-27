using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SwiftFinancials.Utility
{
    internal static class BlobStoreInitializer
    {
        private const string FileGroupName = "BLOBFileGroup";

        public static void EnsureCreated(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("The BLOBStore connection string is missing.", nameof(connectionString));

            var blobBuilder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(blobBuilder.InitialCatalog))
                throw new InvalidOperationException("The BLOBStore connection string must specify an Initial Catalog.");

            string databaseName = blobBuilder.InitialCatalog;
            var masterBuilder = new SqlConnectionStringBuilder(blobBuilder.ConnectionString)
            {
                InitialCatalog = "master"
            };

            using (var connection = new SqlConnection(masterBuilder.ConnectionString))
            {
                connection.Open();
                EnableAndValidateFilestream(connection);

                if (!DatabaseExists(connection, databaseName))
                    CreateDatabase(connection, databaseName);

                EnsureFilestreamFilegroup(connection, databaseName);
            }

            EnsureSchema(blobBuilder.ConnectionString);
        }

        private static void EnableAndValidateFilestream(SqlConnection connection)
        {
            ExecuteNonQuery(connection,
                "EXEC sp_configure 'show advanced options', 1; RECONFIGURE; " +
                "EXEC sp_configure 'filestream access level', 2; RECONFIGURE;");

            using (var command = new SqlCommand(
                "SELECT CONVERT(int, SERVERPROPERTY('FilestreamEffectiveLevel'));", connection))
            {
                int effectiveLevel = Convert.ToInt32(command.ExecuteScalar());
                if (effectiveLevel < 2)
                {
                    throw new InvalidOperationException(
                        "SQL Server FILESTREAM file I/O access is not enabled. In SQL Server Configuration Manager, " +
                        "open the SQL Server instance Properties > FILESTREAM, enable both Transact-SQL access and " +
                        "file I/O streaming access, restart the SQL Server service, and run this utility again.");
                }
            }
        }

        private static bool DatabaseExists(SqlConnection connection, string databaseName)
        {
            using (var command = new SqlCommand("SELECT COUNT(1) FROM sys.databases WHERE name = @name;", connection))
            {
                command.Parameters.Add("@name", SqlDbType.NVarChar, 128).Value = databaseName;
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static void CreateDatabase(SqlConnection connection, string databaseName)
        {
            string dataPath;
            using (var command = new SqlCommand(
                "SELECT CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultDataPath'));", connection))
            {
                dataPath = command.ExecuteScalar() as string;
            }

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                using (var command = new SqlCommand(
                    "SELECT TOP (1) physical_name FROM sys.master_files WHERE database_id = 1 AND file_id = 1;", connection))
                {
                    dataPath = Path.GetDirectoryName(Convert.ToString(command.ExecuteScalar()));
                }
            }

            if (string.IsNullOrWhiteSpace(dataPath))
                throw new InvalidOperationException("SQL Server's default data directory could not be determined.");

            string safeFileStem = MakeSafeFileStem(databaseName);
            string dataFile = Path.Combine(dataPath, safeFileStem + ".mdf");
            string logFile = Path.Combine(dataPath, safeFileStem + ".ldf");
            string filestreamDirectory = Path.Combine(dataPath, safeFileStem + "_FSData");

            string sql = string.Format(
                "CREATE DATABASE {0} ON PRIMARY " +
                "(NAME = {1}, FILENAME = {2}), " +
                "FILEGROUP {3} CONTAINS FILESTREAM " +
                "(NAME = {4}, FILENAME = {5}) " +
                "LOG ON (NAME = {6}, FILENAME = {7});",
                QuoteIdentifier(databaseName),
                QuoteIdentifier(safeFileStem + "_data"), QuoteLiteral(dataFile),
                QuoteIdentifier(FileGroupName),
                QuoteIdentifier(safeFileStem + "_blobs"), QuoteLiteral(filestreamDirectory),
                QuoteIdentifier(safeFileStem + "_log"), QuoteLiteral(logFile));

            ExecuteNonQuery(connection, sql);
        }

        private static void EnsureFilestreamFilegroup(SqlConnection connection, string databaseName)
        {
            using (var command = new SqlCommand(
                "SELECT COUNT(1) FROM sys.master_files WHERE database_id = DB_ID(@databaseName) AND type = 2;",
                connection))
            {
                command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = databaseName;
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                    return;
            }

            string dataPath;
            using (var command = new SqlCommand(
                "SELECT CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultDataPath'));", connection))
            {
                dataPath = command.ExecuteScalar() as string;
            }

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                using (var command = new SqlCommand(
                    "SELECT TOP (1) physical_name FROM sys.master_files WHERE database_id = 1 AND file_id = 1;", connection))
                {
                    dataPath = Path.GetDirectoryName(Convert.ToString(command.ExecuteScalar()));
                }
            }

            if (string.IsNullOrWhiteSpace(dataPath))
                throw new InvalidOperationException("SQL Server's default data directory could not be determined.");

            string safeFileStem = MakeSafeFileStem(databaseName);
            string filestreamDirectory = Path.Combine(dataPath, safeFileStem + "_FSData");
            string sql = string.Format(
                "ALTER DATABASE {0} ADD FILEGROUP {1} CONTAINS FILESTREAM; " +
                "ALTER DATABASE {0} ADD FILE (NAME = {2}, FILENAME = {3}) TO FILEGROUP {1};",
                QuoteIdentifier(databaseName), QuoteIdentifier(FileGroupName),
                QuoteIdentifier(safeFileStem + "_blobs"), QuoteLiteral(filestreamDirectory));

            ExecuteNonQuery(connection, sql);
        }

        private static void EnsureSchema(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                ExecuteNonQuery(connection,
                    "IF OBJECT_ID(N'dbo.swift_media', N'U') IS NULL " +
                    "BEGIN " +
                    "CREATE TABLE dbo.swift_media (" +
                    "media_sku UNIQUEIDENTIFIER NOT NULL ROWGUIDCOL UNIQUE, " +
                    "file_name VARCHAR(256) NOT NULL, " +
                    "file_remarks VARCHAR(256) NULL, " +
                    "content_type VARCHAR(256) NOT NULL, " +
                    "content_coding VARCHAR(256) NULL, " +
                    "content VARBINARY(MAX) FILESTREAM NOT NULL, " +
                    "created_by VARCHAR(256) NOT NULL" +
                    ") FILESTREAM_ON " + QuoteIdentifier(FileGroupName) + "; " +
                    "END;");
            }
        }

        private static void ExecuteNonQuery(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 120;
                command.ExecuteNonQuery();
            }
        }

        private static string MakeSafeFileStem(string databaseName)
        {
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
                databaseName = databaseName.Replace(invalidCharacter, '_');

            return databaseName;
        }

        private static string QuoteIdentifier(string value)
        {
            return "[" + value.Replace("]", "]]" ) + "]";
        }

        private static string QuoteLiteral(string value)
        {
            return "N'" + value.Replace("'", "''") + "'";
        }
    }
}
