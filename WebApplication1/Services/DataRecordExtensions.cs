using System;
using System.Data;
using System.Diagnostics;

namespace WebApplication.Services
{
    public static class DataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord reader, string columnName)
        {
            if (reader == null) return false;
            if (string.IsNullOrWhiteSpace(columnName)) return false;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static Guid SafeGetGuid(this IDataRecord reader, string columnName)
        {
            try
            {
                if (reader == null) return Guid.Empty;
                if (string.IsNullOrWhiteSpace(columnName)) return Guid.Empty;
                if (!reader.HasColumn(columnName))
                {
                    Trace.TraceWarning($"Missing column: {columnName}");
                    return Guid.Empty;
                }

                int ord = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ord)) return Guid.Empty;
                return reader.GetGuid(ord);
            }
            catch (IndexOutOfRangeException)
            {
                Trace.TraceWarning($"Column not found when reading Guid: {columnName}");
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error reading Guid column {columnName}: {ex.Message}");
                return Guid.Empty;
            }
        }

        public static string SafeGetString(this IDataRecord reader, string columnName)
        {
            try
            {
                if (reader == null) return null;
                if (string.IsNullOrWhiteSpace(columnName)) return null;
                if (!reader.HasColumn(columnName))
                {
                    Trace.TraceWarning($"Missing column: {columnName}");
                    return null;
                }

                int ord = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ord)) return null;
                return reader.GetString(ord);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error reading string column {columnName}: {ex.Message}");
                return null;
            }
        }

        public static int SafeGetInt32(this IDataRecord reader, string columnName)
        {
            try
            {
                if (reader == null) return default;
                if (string.IsNullOrWhiteSpace(columnName)) return default;
                if (!reader.HasColumn(columnName))
                {
                    Trace.TraceWarning($"Missing column: {columnName}");
                    return default;
                }

                int ord = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ord)) return default;
                return reader.GetInt32(ord);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error reading int column {columnName}: {ex.Message}");
                return default;
            }
        }
    }
}
