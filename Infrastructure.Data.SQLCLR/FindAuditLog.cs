using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public partial class UserDefinedFunctions
{
    public class AuditLogResult
    {
        public SqlGuid AuditLogId;
        public SqlString EventType;
        public SqlString TableName;
        public SqlString AdditionalNarration;
        public SqlString CreatedBy;
        public SqlDateTime CreatedDate { get; set; }

        public AuditLogResult(SqlGuid auditLogId, SqlString eventType, SqlString tableName, SqlString additionalNarration, SqlString createdBy, SqlDateTime createdDate)
        {
            AuditLogId = auditLogId;
            EventType = eventType;
            TableName = tableName;
            AdditionalNarration = additionalNarration;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
    }

    [Serializable]
    public class AuditInfo
    {
        [XmlAttribute]
        public string ColumnName { get; set; }

        [XmlAttribute]
        public string OriginalValue { get; set; }

        [XmlAttribute]
        public string NewValue { get; set; }
    }

    [Serializable]
    public class AuditInfoWrapper
    {
        [XmlAttribute]
        public string TableName { get; set; }

        [XmlAttribute]
        public string EventType { get; set; }

        [XmlAttribute]
        public string RecordID { get; set; }

        public List<AuditInfo> AuditInfoCollection { get; set; }
    }

    static string FlattenAuditInfo(SqlString content)
    {
        var result = string.Empty;

        try
        {
            if (!content.IsNull)
            {
                if (!string.IsNullOrWhiteSpace(content.ToString()) && content.ToString().StartsWith("<?xml"))
                {
                    var serializer = new XmlSerializer(typeof(AuditInfoWrapper));

                    var wrapper = (AuditInfoWrapper)serializer.Deserialize(new StringReader(content.ToString()));

                    var sb = new StringBuilder();

                    if (wrapper.AuditInfoCollection != null)
                    {
                        foreach (var item in wrapper.AuditInfoCollection)
                            sb.AppendLine(string.Format("ColumnName: {0}, OriginalValue: {1}, NewValue: {2},", item.ColumnName, item.OriginalValue, item.NewValue));
                    }

                    result = string.Format("{0}", sb);
                }
            }

            return result;
        }
        catch { return result; }
    }

    [SqlFunction(
        DataAccess = DataAccessKind.Read,
        FillRowMethodName = "FindAuditLogsByUserName_FillRow",
        TableDefinition = "AuditLogId uniqueidentifier, EventType nvarchar(256), TableName nvarchar(256), AdditionalNarration nvarchar(MAX), CreatedBy nvarchar(256), CreatedDate datetime")]
    public static IEnumerable FindAuditLogsByUserName(string applicationUserName, DateTime startDate, DateTime endDate, int source)
    {
        ArrayList resultCollection = new ArrayList();

        using (SqlConnection connection = new SqlConnection("context connection=true"))
        {
            connection.Open();

            using (SqlCommand selectEmails = new SqlCommand(
                "SELECT " +
                "[Id], [EventType], [TableName], [AdditionalNarration], [CreatedBy], [CreatedDate] " +
               string.Format("{0}", source == 0 ? "FROM [dbo].[swift_AuditLogs] " : "FROM [dbo].[swift_AuditLogsArchive] ") +
                "WHERE [ApplicationUserName] = @applicationUserName AND [CreatedDate] >= @startDate AND [CreatedDate] <= @endDate",
                connection))
            {
                SqlParameter applicationUserNameParam = selectEmails.Parameters.Add("@applicationUserName", SqlDbType.NVarChar);
                applicationUserNameParam.Value = applicationUserName;

                SqlParameter startDateParam = selectEmails.Parameters.Add("@startDate", SqlDbType.DateTime);
                startDateParam.Value = startDate;

                SqlParameter endDateParam = selectEmails.Parameters.Add("@endDate", SqlDbType.DateTime);
                endDateParam.Value = endDate;

                using (SqlDataReader auditLogsReader = selectEmails.ExecuteReader())
                {
                    while (auditLogsReader.Read())
                    {
                        resultCollection.Add(new AuditLogResult(new Guid(string.Format("{0}", auditLogsReader[0])), auditLogsReader.GetSqlString(1), auditLogsReader.GetSqlString(2), FlattenAuditInfo(auditLogsReader.GetSqlString(3)), auditLogsReader.GetSqlString(4), auditLogsReader.GetSqlDateTime(5)));
                    }
                }
            }
        }

        return resultCollection;
    }

    public static void FindAuditLogsByUserName_FillRow(object auditLogResultObj, out SqlGuid auditLogId, out SqlString eventType, out SqlString tableName, out SqlString additionalNarration, out SqlString createdBy, out SqlDateTime createdDate)
    {
        AuditLogResult auditLogResult = (AuditLogResult)auditLogResultObj;

        auditLogId = auditLogResult.AuditLogId;
        eventType = auditLogResult.EventType;
        tableName = auditLogResult.TableName;
        additionalNarration = auditLogResult.AdditionalNarration;
        createdBy = auditLogResult.CreatedBy;
        createdDate = auditLogResult.CreatedDate;
    }

    [SqlFunction(
    DataAccess = DataAccessKind.Read,
    FillRowMethodName = "FindAuditLogs_FillRow",
    TableDefinition = "AuditLogId uniqueidentifier, EventType nvarchar(256), TableName nvarchar(256), AdditionalNarration nvarchar(MAX), CreatedBy nvarchar(256), CreatedDate datetime")]
    public static IEnumerable FindAuditLogs(DateTime startDate, DateTime endDate, int source)
    {
        ArrayList resultCollection = new ArrayList();

        using (SqlConnection connection = new SqlConnection("context connection=true"))
        {
            connection.Open();

            using (SqlCommand selectEmails = new SqlCommand(
                "SELECT " +
                "[Id], [EventType], [TableName], [AdditionalNarration], [CreatedBy], [CreatedDate] " +
               string.Format("{0}", source == 0 ? "FROM [dbo].[swift_AuditLogs] " : "FROM [dbo].[swift_AuditLogsArchive] ") +
                "WHERE [CreatedDate] >= @startDate AND [CreatedDate] <= @endDate",
                connection))
            {
                SqlParameter startDateParam = selectEmails.Parameters.Add("@startDate", SqlDbType.DateTime);
                startDateParam.Value = startDate;

                SqlParameter endDateParam = selectEmails.Parameters.Add("@endDate", SqlDbType.DateTime);
                endDateParam.Value = endDate;

                using (SqlDataReader auditLogsReader = selectEmails.ExecuteReader())
                {
                    while (auditLogsReader.Read())
                    {
                        resultCollection.Add(new AuditLogResult(new Guid(string.Format("{0}", auditLogsReader[0])), auditLogsReader.GetSqlString(1), auditLogsReader.GetSqlString(2), FlattenAuditInfo(auditLogsReader.GetSqlString(3)), auditLogsReader.GetSqlString(4), auditLogsReader.GetSqlDateTime(5)));
                    }
                }
            }
        }

        return resultCollection;
    }

    public static void FindAuditLogs_FillRow(object auditLogResultObj, out SqlGuid auditLogId, out SqlString eventType, out SqlString tableName, out SqlString additionalNarration, out SqlString createdBy, out SqlDateTime createdDate)
    {
        AuditLogResult auditLogResult = (AuditLogResult)auditLogResultObj;

        auditLogId = auditLogResult.AuditLogId;
        eventType = auditLogResult.EventType;
        tableName = auditLogResult.TableName;
        additionalNarration = auditLogResult.AdditionalNarration;
        createdBy = auditLogResult.CreatedBy;
        createdDate = auditLogResult.CreatedDate;
    }

    #region Flattened Audit Log

    public class FlattenedAuditLogResult
    {
        public SqlGuid AuditLogId;
        public SqlString EventType;
        public SqlString TableName;
        public SqlGuid RecordID;
        public SqlString ColumnName;
        public SqlString OriginalValue;
        public SqlString NewValue;
        public SqlString CreatedBy;
        public SqlDateTime CreatedDate { get; set; }

        public FlattenedAuditLogResult(SqlGuid auditLogId, SqlString eventType, SqlString tableName, SqlGuid recordID, SqlString columnName, SqlString originalValue, SqlString newValue, SqlString createdBy, SqlDateTime createdDate)
        {
            AuditLogId = auditLogId;
            EventType = eventType;
            TableName = tableName;
            RecordID = recordID;
            ColumnName = columnName;
            OriginalValue = originalValue;
            NewValue = newValue;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
    }

    static List<Tuple<SqlString, SqlString, SqlString>> DeserializeAdditionalNarration(SqlString content)
    {
        SqlString columnName = new SqlString(string.Empty);

        SqlString originalName = new SqlString(string.Empty);

        SqlString newValue = new SqlString(string.Empty);

        var result = new List<Tuple<SqlString, SqlString, SqlString>>();

        try
        {
            if (!content.IsNull)
            {
                if (!string.IsNullOrWhiteSpace(content.ToString()) && content.ToString().StartsWith("<?xml"))
                {
                    var serializer = new XmlSerializer(typeof(AuditInfoWrapper));

                    var wrapper = (AuditInfoWrapper)serializer.Deserialize(new StringReader(content.ToString()));

                    if (wrapper.AuditInfoCollection != null)
                    {
                        foreach (var item in wrapper.AuditInfoCollection)
                        {
                            columnName = new SqlString(item.ColumnName);

                            originalName = new SqlString(item.OriginalValue);

                            newValue = new SqlString(item.NewValue);

                            result.Add(new Tuple<SqlString, SqlString, SqlString>(columnName, originalName, newValue));
                        }
                    }
                }
            }

            return result;
        }
        catch { return result; }
    }

    [SqlFunction(
   DataAccess = DataAccessKind.Read,
   FillRowMethodName = "FindFlattenedAuditLogsByUserName_FillRow",
   TableDefinition = "AuditLogId uniqueidentifier, EventType nvarchar(256), TableName nvarchar(256), RecordID uniqueidentifier, ColumnName nvarchar(Max), OriginalValue nvarchar(Max), NewValue nvarchar(Max), CreatedBy nvarchar(256), CreatedDate datetime")]
    public static IEnumerable FindFlattenedAuditLogsByUserName(string applicationUserName, DateTime startDate, DateTime endDate, int source)
    {
        ArrayList resultCollection = new ArrayList();

        using (SqlConnection connection = new SqlConnection("context connection=true"))
        {
            connection.Open();

            using (SqlCommand selectEmails = new SqlCommand(
                "SELECT " +
                "[Id], [EventType], [TableName], [RecordID], [AdditionalNarration], [CreatedBy], [CreatedDate] " +
               string.Format("{0}", source == 0 ? "FROM [dbo].[swift_AuditLogs] " : "FROM [dbo].[swift_AuditLogsArchive] ") +
                "WHERE [ApplicationUserName] = @applicationUserName AND [CreatedDate] >= @startDate AND [CreatedDate] <= @endDate",
                connection))
            {
                SqlParameter applicationUserNameParam = selectEmails.Parameters.Add("@applicationUserName", SqlDbType.NVarChar);
                applicationUserNameParam.Value = applicationUserName;

                SqlParameter startDateParam = selectEmails.Parameters.Add("@startDate", SqlDbType.DateTime);
                startDateParam.Value = startDate;

                SqlParameter endDateParam = selectEmails.Parameters.Add("@endDate", SqlDbType.DateTime);
                endDateParam.Value = endDate;

                using (SqlDataReader auditLogsReader = selectEmails.ExecuteReader())
                {
                    while (auditLogsReader.Read())
                    {
                        var auditInfoTuples = DeserializeAdditionalNarration(auditLogsReader.GetSqlString(4));

                        foreach (var auditInfoTuple in auditInfoTuples)
                        {
                            resultCollection.Add(new FlattenedAuditLogResult(new Guid(string.Format("{0}", auditLogsReader[0])), auditLogsReader.GetSqlString(1), auditLogsReader.GetSqlString(2), new Guid(string.Format("{0}", auditLogsReader[3])), auditInfoTuple.Item1, auditInfoTuple.Item2, auditInfoTuple.Item3, auditLogsReader.GetSqlString(5), auditLogsReader.GetSqlDateTime(6)));
                        }
                    }
                }
            }
        }

        return resultCollection;
    }

    public static void FindFlattenedAuditLogsByUserName_FillRow(object auditLogResultObj, out SqlGuid auditLogId, out SqlString eventType, out SqlString tableName, out SqlGuid recordId, out SqlString columnName, out SqlString originalValue, out SqlString newValue, out SqlString createdBy, out SqlDateTime createdDate)
    {
        FlattenedAuditLogResult auditLogResult = (FlattenedAuditLogResult)auditLogResultObj;

        auditLogId = auditLogResult.AuditLogId;
        eventType = auditLogResult.EventType;
        tableName = auditLogResult.TableName;
        recordId = auditLogResult.RecordID;
        columnName = auditLogResult.ColumnName;
        originalValue = auditLogResult.OriginalValue;
        newValue = auditLogResult.NewValue;
        createdBy = auditLogResult.CreatedBy;
        createdDate = auditLogResult.CreatedDate;
    }

    [SqlFunction(
   DataAccess = DataAccessKind.Read,
   FillRowMethodName = "FindFlattenedAuditLogs_FillRow",
   TableDefinition = "AuditLogId uniqueidentifier, EventType nvarchar(256), TableName nvarchar(256), RecordID uniqueidentifier, ColumnName nvarchar(Max), OriginalValue nvarchar(Max), NewValue nvarchar(Max), CreatedBy nvarchar(256), CreatedDate datetime")]
    public static IEnumerable FindFlattenedAuditLogs(DateTime startDate, DateTime endDate, int source)
    {
        ArrayList resultCollection = new ArrayList();

        using (SqlConnection connection = new SqlConnection("context connection=true"))
        {
            connection.Open();

            using (SqlCommand selectEmails = new SqlCommand(
                "SELECT " +
                "[Id], [EventType], [TableName], [RecordID], [AdditionalNarration], [CreatedBy], [CreatedDate] " +
               string.Format("{0}", source == 0 ? "FROM [dbo].[swift_AuditLogs] " : "FROM [dbo].[swift_AuditLogsArchive] ") +
                "WHERE [CreatedDate] >= @startDate AND [CreatedDate] <= @endDate",
                connection))
            {
                SqlParameter startDateParam = selectEmails.Parameters.Add("@startDate", SqlDbType.DateTime);
                startDateParam.Value = startDate;

                SqlParameter endDateParam = selectEmails.Parameters.Add("@endDate", SqlDbType.DateTime);
                endDateParam.Value = endDate;

                using (SqlDataReader auditLogsReader = selectEmails.ExecuteReader())
                {
                    while (auditLogsReader.Read())
                    {
                        var auditInfoTuples = DeserializeAdditionalNarration(auditLogsReader.GetSqlString(4));

                        foreach (var auditInfoTuple in auditInfoTuples)
                        {
                            resultCollection.Add(new FlattenedAuditLogResult(new Guid(string.Format("{0}", auditLogsReader[0])), auditLogsReader.GetSqlString(1), auditLogsReader.GetSqlString(2), new Guid(string.Format("{0}", auditLogsReader[3])), auditInfoTuple.Item1, auditInfoTuple.Item2, auditInfoTuple.Item3, auditLogsReader.GetSqlString(5), auditLogsReader.GetSqlDateTime(6)));
                        }
                    }
                }
            }
        }

        return resultCollection;
    }

    public static void FindFlattenedAuditLogs_FillRow(object auditLogResultObj, out SqlGuid auditLogId, out SqlString eventType, out SqlString tableName, out SqlGuid recordId, out SqlString columnName, out SqlString originalValue, out SqlString newValue, out SqlString createdBy, out SqlDateTime createdDate)
    {
        FlattenedAuditLogResult auditLogResult = (FlattenedAuditLogResult)auditLogResultObj;

        auditLogId = auditLogResult.AuditLogId;
        eventType = auditLogResult.EventType;
        tableName = auditLogResult.TableName;
        recordId = auditLogResult.RecordID;
        columnName = auditLogResult.ColumnName;
        originalValue = auditLogResult.OriginalValue;
        newValue = auditLogResult.NewValue;
        createdBy = auditLogResult.CreatedBy;
        createdDate = auditLogResult.CreatedDate;
    }

    #endregion
}
