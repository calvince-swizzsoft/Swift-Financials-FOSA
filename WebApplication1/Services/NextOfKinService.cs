using Application.MainBoundedContext.DTO.RegistryModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace WebApplication1.Services
{
    public class NextOfKinService
    {
        private readonly string _connectionString;

        public NextOfKinService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        }

        public IEnumerable<NextOfKinDTO> GetAll()
        {
            var list = new List<NextOfKinDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT n.*, 
                                c.Type as CustomerType,
                                c.Individual_Salutation as CustomerIndividualSalutation,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription
                                FROM [swiftFin_NextOfKin] n
                                LEFT JOIN [swiftFin_Customers] c ON n.CustomerId = c.Id
                                ORDER BY n.CreatedDate DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public NextOfKinDTO GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT n.*, 
                                c.Type as CustomerType,
                                c.Individual_Salutation as CustomerIndividualSalutation,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription
                                FROM [swiftFin_NextOfKin] n
                                LEFT JOIN [swiftFin_Customers] c ON n.CustomerId = c.Id
                                WHERE n.Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }
            return null;
        }

        public IEnumerable<NextOfKinDTO> GetByCustomerId(Guid customerId)
        {
            var list = new List<NextOfKinDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT n.*, 
                                c.Type as CustomerType,
                                c.Individual_Salutation as CustomerIndividualSalutation,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription
                                FROM [swiftFin_NextOfKin] n
                                LEFT JOIN [swiftFin_Customers] c ON n.CustomerId = c.Id
                                WHERE n.CustomerId = @CustomerId
                                ORDER BY n.CreatedDate DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public double GetTotalPercentageForCustomer(Guid customerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ISNULL(SUM(NominatedPercentage), 0) 
                                FROM [swiftFin_NextOfKin] 
                                WHERE CustomerId = @CustomerId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDouble(result);
                }
            }
        }

        public double GetTotalPercentageForCustomerExcluding(Guid customerId, Guid excludeNextOfKinId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ISNULL(SUM(NominatedPercentage), 0)
                                FROM [swiftFin_NextOfKin]
                                WHERE CustomerId = @CustomerId
                                AND Id <> @ExcludeId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@ExcludeId", excludeNextOfKinId);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDouble(result);
                }
            }
        }

        public Dictionary<Guid, List<NextOfKinDTO>> GetNextOfKinsByCustomerIds(List<Guid> customerIds)
        {
            var result = new Dictionary<Guid, List<NextOfKinDTO>>();

            if (customerIds == null || customerIds.Count == 0)
                return result;

            // Initialize dictionary
            foreach (var customerId in customerIds)
            {
                result[customerId] = new List<NextOfKinDTO>();
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                // Create parameter list
                var parameters = new List<SqlParameter>();
                var paramNames = new List<string>();

                for (int i = 0; i < customerIds.Count; i++)
                {
                    var paramName = $"@CustomerId{i}";
                    paramNames.Add(paramName);
                    parameters.Add(new SqlParameter(paramName, customerIds[i]));
                }

                string query = $@"SELECT n.*, 
                         c.Type as CustomerType,
                         c.Individual_Salutation as CustomerIndividualSalutation,
                         c.Individual_FirstName as CustomerIndividualFirstName,
                         c.Individual_LastName as CustomerIndividualLastName,
                         c.NonIndividual_Description as CustomerNonIndividualDescription
                         FROM [swiftFin_NextOfKin] n
                         LEFT JOIN [swiftFin_Customers] c ON n.CustomerId = c.Id
                         WHERE n.CustomerId IN ({string.Join(",", paramNames)})
                         ORDER BY n.CustomerId, n.CreatedDate DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            var nextOfKin = Map(reader);
                            if (result.ContainsKey(nextOfKin.CustomerId))
                            {
                                result[nextOfKin.CustomerId].Add(nextOfKin);
                            }
                        }
                }
            }
            return result;
        }

        public Dictionary<Guid, PercentageSummaryDTO> GetPercentageSummariesByCustomerIds(List<Guid> customerIds)
        {
            var result = new Dictionary<Guid, PercentageSummaryDTO>();

            if (customerIds == null || customerIds.Count == 0)
                return result;

            using (var conn = new SqlConnection(_connectionString))
            {
                // Create parameter list
                var parameters = new List<SqlParameter>();
                var paramNames = new List<string>();

                for (int i = 0; i < customerIds.Count; i++)
                {
                    var paramName = $"@CustomerId{i}";
                    paramNames.Add(paramName);
                    parameters.Add(new SqlParameter(paramName, customerIds[i]));
                }

                string query = $@"SELECT 
                         CustomerId,
                         COUNT(*) as TotalNextOfKins,
                         ISNULL(SUM(NominatedPercentage), 0) as TotalPercentage,
                         CASE 
                            WHEN ISNULL(SUM(NominatedPercentage), 0) <= 100 
                            THEN 100 - ISNULL(SUM(NominatedPercentage), 0)
                            ELSE 0 
                         END as RemainingPercentage
                         FROM [swiftFin_NextOfKin] 
                         WHERE CustomerId IN ({string.Join(",", paramNames)})
                         GROUP BY CustomerId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            var summary = new PercentageSummaryDTO
                            {
                                CustomerId = (Guid)reader["CustomerId"],
                                TotalNextOfKins = Convert.ToInt32(reader["TotalNextOfKins"]),
                                TotalPercentage = Convert.ToDouble(reader["TotalPercentage"]),
                                RemainingPercentage = Convert.ToDouble(reader["RemainingPercentage"])
                            };
                            result[summary.CustomerId] = summary;
                        }
                }
            }

            // Add entries for customers without next of kins
            foreach (var customerId in customerIds)
            {
                if (!result.ContainsKey(customerId))
                {
                    result[customerId] = new PercentageSummaryDTO
                    {
                        CustomerId = customerId,
                        TotalNextOfKins = 0,
                        TotalPercentage = 0,
                        RemainingPercentage = 100
                    };
                }
            }

            return result;
        }

        public NextOfKinDTO Create(NextOfKinDTO nextOfKin)
        {
            // Validate percentage allocation
            double totalPercentage = GetTotalPercentageForCustomer(nextOfKin.CustomerId);
            double newTotal = totalPercentage + nextOfKin.NominatedPercentage;

            // Check for data integrity first
            if (totalPercentage > 100)
            {
                throw new InvalidOperationException(
                    $"Cannot add next of kin. Customer already has {totalPercentage:0.##}% allocated. " +
                    $"Please fix existing data first.");
            }

            if (newTotal > 100)
            {
                double remaining = 100 - totalPercentage;
                throw new InvalidOperationException(
                    $"Cannot add next of kin. Total percentage would be {newTotal:0.##}%. " +
                    $"Current total: {totalPercentage:0.##}%, Remaining: {remaining:0.##}%");
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                // Generate new Guid if not provided
                if (nextOfKin.Id == Guid.Empty)
                    nextOfKin.Id = Guid.NewGuid();

                nextOfKin.CreatedDate = DateTime.Now;

                string query = @"INSERT INTO [swiftFin_NextOfKin] 
                                ([Id], [CustomerId], [Salutation], [Gender], 
                                 [Relationship], [FirstName], [LastName], 
                                 [IdentityCardType], [IdentityCardNumber], 
                                 [Address_AddressLine1], [Address_AddressLine2], 
                                 [Address_Street], [Address_PostalCode], [Address_City], 
                                 [Address_Email], [Address_LandLine], [Address_MobileLine], 
                                 [NominatedPercentage], [Remarks], [CreatedBy], [CreatedDate])
                                VALUES 
                                (@Id, @CustomerId, @Salutation, @Gender, 
                                 @Relationship, @FirstName, @LastName, 
                                 @IdentityCardType, @IdentityCardNumber, 
                                 @Address_AddressLine1, @Address_AddressLine2, 
                                 @Address_Street, @Address_PostalCode, @Address_City, 
                                 @Address_Email, @Address_LandLine, @Address_MobileLine, 
                                 @NominatedPercentage, @Remarks, @CreatedBy, @CreatedDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, nextOfKin);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return GetById(nextOfKin.Id);
        }

        public void Update(NextOfKinDTO nextOfKin)
        {
            if (nextOfKin == null)
                throw new ArgumentNullException(nameof(nextOfKin));

            // Validate ID
            if (nextOfKin.Id == Guid.Empty)
                throw new ArgumentException("Next of kin ID is required");

            var existing = GetById(nextOfKin.Id);
            if (existing == null)
                throw new KeyNotFoundException("Next of kin not found");

            // Validate percentage
            if (nextOfKin.NominatedPercentage <= 0 || nextOfKin.NominatedPercentage > 100)
                throw new InvalidOperationException("Nominated percentage must be between 1 and 100");

            // Get total percentage excluding this record
            double allocatedExcludingThis = GetTotalPercentageForCustomerExcluding(
                nextOfKin.CustomerId,
                nextOfKin.Id
            );

            // Calculate what the new total would be
            double newTotal = allocatedExcludingThis + nextOfKin.NominatedPercentage;

            if (newTotal > 100)
            {
                double remaining = 100 - allocatedExcludingThis;
                throw new InvalidOperationException(
                    $"Cannot update next of kin. " +
                    $"Total percentage excluding this record: {allocatedExcludingThis:0.##}%, " +
                    $"New total would be: {newTotal:0.##}%. " +
                    $"Remaining capacity for this record: {remaining:0.##}%");
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                const string query = @"
                    UPDATE [swiftFin_NextOfKin]
                    SET
                        [CustomerId] = @CustomerId,
                        [Salutation] = @Salutation,
                        [Gender] = @Gender,
                        [Relationship] = @Relationship,
                        [FirstName] = @FirstName,
                        [LastName] = @LastName,
                        [IdentityCardType] = @IdentityCardType,
                        [IdentityCardNumber] = @IdentityCardNumber,
                        [Address_AddressLine1] = @Address_AddressLine1,
                        [Address_AddressLine2] = @Address_AddressLine2,
                        [Address_Street] = @Address_Street,
                        [Address_PostalCode] = @Address_PostalCode,
                        [Address_City] = @Address_City,
                        [Address_Email] = @Address_Email,
                        [Address_LandLine] = @Address_LandLine,
                        [Address_MobileLine] = @Address_MobileLine,
                        [NominatedPercentage] = @NominatedPercentage,
                        [Remarks] = @Remarks
                    WHERE [Id] = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddUpdateParams(cmd, nextOfKin);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new KeyNotFoundException("Next of kin not found");
                }
            }
        }

        public void Delete(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_NextOfKin] WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new KeyNotFoundException("Next of kin not found");
                }
            }
        }

        public void DeleteByCustomerId(Guid customerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_NextOfKin] WHERE CustomerId = @CustomerId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public PercentageSummaryDTO GetPercentageSummary(Guid customerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                                COUNT(*) as TotalNextOfKins,
                                ISNULL(SUM(NominatedPercentage), 0) as TotalPercentage,
                                CASE 
                                    WHEN ISNULL(SUM(NominatedPercentage), 0) <= 100 
                                    THEN 100 - ISNULL(SUM(NominatedPercentage), 0)
                                    ELSE 0 
                                END as RemainingPercentage
                                FROM [swiftFin_NextOfKin] 
                                WHERE CustomerId = @CustomerId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PercentageSummaryDTO
                            {
                                CustomerId = customerId,
                                TotalNextOfKins = Convert.ToInt32(reader["TotalNextOfKins"]),
                                TotalPercentage = Convert.ToDouble(reader["TotalPercentage"]),
                                RemainingPercentage = Convert.ToDouble(reader["RemainingPercentage"])
                            };
                        }
                    }
                }
            }

            return new PercentageSummaryDTO
            {
                CustomerId = customerId,
                TotalNextOfKins = 0,
                TotalPercentage = 0,
                RemainingPercentage = 100
            };
        }

        // NEW: Method to validate and fix data inconsistencies
        public Dictionary<Guid, double> ValidateCustomerPercentages()
        {
            var issues = new Dictionary<Guid, double>();

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT CustomerId, SUM(NominatedPercentage) as TotalPercentage
                    FROM [swiftFin_NextOfKin]
                    GROUP BY CustomerId
                    HAVING SUM(NominatedPercentage) > 100 OR SUM(NominatedPercentage) < 0";

                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customerId = (Guid)reader["CustomerId"];
                            var total = Convert.ToDouble(reader["TotalPercentage"]);
                            issues[customerId] = total;
                        }
                    }
                }
            }

            return issues;
        }

        // NEW: Method to fix data inconsistencies
        public void FixCustomerPercentage(Guid customerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Get all next of kins for this customer
                string query = @"
                    SELECT Id, NominatedPercentage
                    FROM [swiftFin_NextOfKin]
                    WHERE CustomerId = @CustomerId
                    ORDER BY CreatedDate";

                var nextOfKins = new List<Tuple<Guid, double>>();

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nextOfKins.Add(Tuple.Create(
                                (Guid)reader["Id"],
                                Convert.ToDouble(reader["NominatedPercentage"])
                            ));
                        }
                    }
                }

                // If total > 100%, normalize the percentages
                double total = nextOfKins.Sum(n => n.Item2);

                if (total > 100 && total > 0) // Avoid division by zero
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var nok in nextOfKins)
                            {
                                double normalizedPercentage = (nok.Item2 / total) * 100;
                                // Round to 2 decimal places
                                normalizedPercentage = Math.Round(normalizedPercentage, 2);

                                string updateQuery = @"
                                    UPDATE [swiftFin_NextOfKin]
                                    SET NominatedPercentage = @Percentage
                                    WHERE Id = @Id";

                                using (var updateCmd = new SqlCommand(updateQuery, conn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@Id", nok.Item1);
                                    updateCmd.Parameters.AddWithValue("@Percentage", normalizedPercentage);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        private void AddParams(SqlCommand cmd, NextOfKinDTO nextOfKin)
        {
            cmd.Parameters.AddWithValue("@Id", nextOfKin.Id);
            cmd.Parameters.AddWithValue("@CustomerId", nextOfKin.CustomerId);
            cmd.Parameters.AddWithValue("@Salutation", nextOfKin.Salutation);
            cmd.Parameters.AddWithValue("@Gender", nextOfKin.Gender);
            cmd.Parameters.AddWithValue("@Relationship", nextOfKin.Relationship);
            cmd.Parameters.AddWithValue("@FirstName", nextOfKin.FirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@LastName", nextOfKin.LastName ?? string.Empty);
            cmd.Parameters.AddWithValue("@IdentityCardType", nextOfKin.IdentityCardType);
            cmd.Parameters.AddWithValue("@IdentityCardNumber", nextOfKin.IdentityCardNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_AddressLine1", nextOfKin.AddressAddressLine1 ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_AddressLine2", nextOfKin.AddressAddressLine2 ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_Street", nextOfKin.AddressStreet ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_PostalCode", nextOfKin.AddressPostalCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_City", nextOfKin.AddressCity ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_Email", nextOfKin.AddressEmail ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_LandLine", nextOfKin.AddressLandLine ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_MobileLine", nextOfKin.AddressMobileLine ?? string.Empty);
            cmd.Parameters.AddWithValue("@NominatedPercentage", nextOfKin.NominatedPercentage);
            cmd.Parameters.AddWithValue("@Remarks", nextOfKin.Remarks ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedBy", nextOfKin.CreatedBy ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedDate", nextOfKin.CreatedDate);
        }

        private void AddUpdateParams(SqlCommand cmd, NextOfKinDTO nextOfKin)
        {
            cmd.Parameters.AddWithValue("@Id", nextOfKin.Id);
            cmd.Parameters.AddWithValue("@CustomerId", nextOfKin.CustomerId);
            cmd.Parameters.AddWithValue("@Salutation", nextOfKin.Salutation);
            cmd.Parameters.AddWithValue("@Gender", nextOfKin.Gender);
            cmd.Parameters.AddWithValue("@Relationship", nextOfKin.Relationship);
            cmd.Parameters.AddWithValue("@FirstName", nextOfKin.FirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@LastName", nextOfKin.LastName ?? string.Empty);
            cmd.Parameters.AddWithValue("@IdentityCardType", nextOfKin.IdentityCardType);
            cmd.Parameters.AddWithValue("@IdentityCardNumber", nextOfKin.IdentityCardNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_AddressLine1", nextOfKin.AddressAddressLine1 ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_AddressLine2", nextOfKin.AddressAddressLine2 ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_Street", nextOfKin.AddressStreet ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_PostalCode", nextOfKin.AddressPostalCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_City", nextOfKin.AddressCity ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_Email", nextOfKin.AddressEmail ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_LandLine", nextOfKin.AddressLandLine ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_MobileLine", nextOfKin.AddressMobileLine ?? string.Empty);
            cmd.Parameters.AddWithValue("@NominatedPercentage", nextOfKin.NominatedPercentage);
            cmd.Parameters.AddWithValue("@Remarks", nextOfKin.Remarks ?? string.Empty);
        }

        private NextOfKinDTO Map(IDataReader reader)
        {
            return new NextOfKinDTO
            {
                Id = (Guid)reader["Id"],
                CustomerId = (Guid)reader["CustomerId"],
                Salutation = Convert.ToByte(reader["Salutation"]),
                Gender = Convert.ToByte(reader["Gender"]),
                Relationship = Convert.ToByte(reader["Relationship"]),
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                IdentityCardType = Convert.ToByte(reader["IdentityCardType"]),
                IdentityCardNumber = reader["IdentityCardNumber"]?.ToString(),
                AddressAddressLine1 = reader["Address_AddressLine1"]?.ToString(),
                AddressAddressLine2 = reader["Address_AddressLine2"]?.ToString(),
                AddressStreet = reader["Address_Street"]?.ToString(),
                AddressPostalCode = reader["Address_PostalCode"]?.ToString(),
                AddressCity = reader["Address_City"]?.ToString(),
                AddressEmail = reader["Address_Email"]?.ToString(),
                AddressLandLine = reader["Address_LandLine"]?.ToString(),
                AddressMobileLine = reader["Address_MobileLine"]?.ToString(),
                NominatedPercentage = Convert.ToDouble(reader["NominatedPercentage"]),
                Remarks = reader["Remarks"]?.ToString(),
                CreatedBy = reader["CreatedBy"]?.ToString(),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                // Customer details from join
                CustomerType = Convert.ToByte(reader["CustomerType"]),
                CustomerIndividualSalutation = Convert.ToByte(reader["CustomerIndividualSalutation"]),
                CustomerIndividualFirstName = reader["CustomerIndividualFirstName"]?.ToString(),
                CustomerIndividualLastName = reader["CustomerIndividualLastName"]?.ToString(),
                CustomerNonIndividualDescription = reader["CustomerNonIndividualDescription"]?.ToString()
            };
        }
    }

    public class PercentageSummaryDTO
    {
        public Guid CustomerId { get; set; }
        public int TotalNextOfKins { get; set; }
        public double TotalPercentage { get; set; }
        public double RemainingPercentage { get; set; }
    }
}