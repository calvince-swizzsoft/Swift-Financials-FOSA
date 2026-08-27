using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SwiftFinancials.Utility
{
    internal static class StoredProcedureInitializer
    {
        private const string ResourceSuffix = ".Database.StoredProcedures.sql";

        private static readonly Regex BatchSeparator =
            new Regex(@"^\s*GO\s*(?:--.*)?$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static readonly Regex CreateObject = new Regex(
            @"\b(?<verb>CREATE|ALTER)\s+(?:OR\s+ALTER\s+)?(?<kind>PROC(?:EDURE)?|FUNCTION)\s+(?<name>(?:\[[^\]]+\]|\w+)(?:\s*\.\s*(?:\[[^\]]+\]|\w+))?)",
            RegexOptions.IgnoreCase);

        public static void EnsureCreated(string connectionString)
        {
            var sql = ReadEmbeddedScript();
            var batches = BatchSeparator.Split(sql)
                .Select(batch => batch.Trim())
                .Where(batch => batch.Length > 0 && !Regex.IsMatch(batch, @"^USE\s+", RegexOptions.IgnoreCase));

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var batch in batches)
                {
                    var match = CreateObject.Match(batch);
                    if (!match.Success)
                        continue;

                    var objectName = Regex.Replace(match.Groups["name"].Value, @"\s+", string.Empty);
                    if (ObjectExists(connection, objectName))
                        continue;

                    var executableBatch = batch;
                    if (match.Groups["verb"].Value.Equals("ALTER", StringComparison.OrdinalIgnoreCase))
                        executableBatch = batch.Remove(match.Groups["verb"].Index, match.Groups["verb"].Length)
                            .Insert(match.Groups["verb"].Index, "CREATE");

                    using (var command = new SqlCommand(executableBatch, connection))
                    {
                        command.CommandTimeout = 120;
                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine("Database object created>{0}", objectName);
                }
            }
        }

        private static bool ObjectExists(SqlConnection connection, string objectName)
        {
            using (var command = new SqlCommand("SELECT OBJECT_ID(@ObjectName)", connection))
            {
                command.Parameters.AddWithValue("@ObjectName", objectName.Replace("[", string.Empty).Replace("]", string.Empty));
                return command.ExecuteScalar() != DBNull.Value;
            }
        }

        private static string ReadEmbeddedScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(name => name.EndsWith(ResourceSuffix, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new InvalidOperationException("The embedded stored-procedure script was not found.");

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
