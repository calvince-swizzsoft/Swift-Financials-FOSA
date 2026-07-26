using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Services
{
    public class SavingsProductService
    {
        private readonly string _connectionString;

        public SavingsProductService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        }

        public IEnumerable<SavingsProductDTO> GetAll()
        {
            var list = new List<SavingsProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT sp.*, 
                                coa.AccountType as ChartOfAccountAccountType,
                                coa.AccountCode as ChartOfAccountAccountCode,
                                coa.AccountName as ChartOfAccountAccountName,
                                coa.CostCenterId as ChartOfAccountCostCenterId,
                                cc.Description as ChartOfAccountCostCenterDescription
                                FROM [swiftFin_SavingsProducts] sp
                                LEFT JOIN [swiftFin_ChartOfAccounts] coa ON sp.ChartOfAccountId = coa.Id
                                LEFT JOIN [swiftFin_CostCenters] cc ON coa.CostCenterId = cc.Id
                                ORDER BY sp.Code";
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

        public SavingsProductDTO GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT sp.*, 
                                coa.AccountType as ChartOfAccountAccountType,
                                coa.AccountCode as ChartOfAccountAccountCode,
                                coa.AccountName as ChartOfAccountAccountName,
                                coa.CostCenterId as ChartOfAccountCostCenterId,
                                cc.Description as ChartOfAccountCostCenterDescription
                                FROM [swiftFin_SavingsProducts] sp
                                LEFT JOIN [swiftFin_ChartOfAccounts] coa ON sp.ChartOfAccountId = coa.Id
                                LEFT JOIN [swiftFin_CostCenters] cc ON coa.CostCenterId = cc.Id
                                WHERE sp.Id = @Id";
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

        public SavingsProductDTO GetByCode(int code)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT sp.*, 
                                coa.AccountType as ChartOfAccountAccountType,
                                coa.AccountCode as ChartOfAccountAccountCode,
                                coa.AccountName as ChartOfAccountAccountName,
                                coa.CostCenterId as ChartOfAccountCostCenterId,
                                cc.Description as ChartOfAccountCostCenterDescription
                                FROM [swiftFin_SavingsProducts] sp
                                LEFT JOIN [swiftFin_ChartOfAccounts] coa ON sp.ChartOfAccountId = coa.Id
                                LEFT JOIN [swiftFin_CostCenters] cc ON coa.CostCenterId = cc.Id
                                WHERE sp.Code = @Code";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
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

        public IEnumerable<SavingsProductDTO> GetActiveProducts()
        {
            var list = new List<SavingsProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT sp.*, 
                                coa.AccountType as ChartOfAccountAccountType,
                                coa.AccountCode as ChartOfAccountAccountCode,
                                coa.AccountName as ChartOfAccountAccountName,
                                coa.CostCenterId as ChartOfAccountCostCenterId,
                                cc.Description as ChartOfAccountCostCenterDescription
                                FROM [swiftFin_SavingsProducts] sp
                                LEFT JOIN [swiftFin_ChartOfAccounts] coa ON sp.ChartOfAccountId = coa.Id
                                LEFT JOIN [swiftFin_CostCenters] cc ON coa.CostCenterId = cc.Id
                                WHERE sp.IsLocked = 0
                                ORDER BY sp.Code";
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

        public IEnumerable<SavingsProductDTO> GetDefaultProducts()
        {
            var list = new List<SavingsProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT sp.*, 
                                coa.AccountType as ChartOfAccountAccountType,
                                coa.AccountCode as ChartOfAccountAccountCode,
                                coa.AccountName as ChartOfAccountAccountName,
                                coa.CostCenterId as ChartOfAccountCostCenterId,
                                cc.Description as ChartOfAccountCostCenterDescription
                                FROM [swiftFin_SavingsProducts] sp
                                LEFT JOIN [swiftFin_ChartOfAccounts] coa ON sp.ChartOfAccountId = coa.Id
                                LEFT JOIN [swiftFin_CostCenters] cc ON coa.CostCenterId = cc.Id
                                WHERE sp.IsDefault = 1 AND sp.IsLocked = 0
                                ORDER BY sp.Priority";
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

        public IEnumerable<SavingsProductDTO> GetMandatoryProducts()
        {
            var list = new List<SavingsProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT sp.*, 
                                coa.AccountType as ChartOfAccountAccountType,
                                coa.AccountCode as ChartOfAccountAccountCode,
                                coa.AccountName as ChartOfAccountAccountName,
                                coa.CostCenterId as ChartOfAccountCostCenterId,
                                cc.Description as ChartOfAccountCostCenterDescription
                                FROM [swiftFin_SavingsProducts] sp
                                LEFT JOIN [swiftFin_ChartOfAccounts] coa ON sp.ChartOfAccountId = coa.Id
                                LEFT JOIN [swiftFin_CostCenters] cc ON coa.CostCenterId = cc.Id
                                WHERE sp.IsMandatory = 1 AND sp.IsLocked = 0
                                ORDER BY sp.Priority";
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

        public SavingsProductDTO Create(SavingsProductDTO product)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(product.Description))
                throw new ArgumentException("Product description is required");

            if (product.ChartOfAccountId == Guid.Empty)
                throw new ArgumentException("Chart of Account is required");

            using (var conn = new SqlConnection(_connectionString))
            {
                // Generate new Guid if not provided
                if (product.Id == Guid.Empty)
                    product.Id = Guid.NewGuid();

                product.CreatedDate = DateTime.Now;

                // Generate code if not provided
                if (product.Code == 0)
                    product.Code = GenerateNextCode();

                // OPTION 1: Remove SequentialId from INSERT (if it's an IDENTITY column)
                string query = @"INSERT INTO [swiftFin_SavingsProducts] 
                                ([Id], [Code], [Description], [MaximumAllowedWithdrawal], 
                                 [MaximumAllowedDeposit], [MinimumBalance], [OperatingBalance], 
                                 [WithdrawalNoticeAmount], [WithdrawalNoticePeriod], 
                                 [WithdrawalInterval], [AnnualPercentageYield], [Priority], 
                                 [IsLocked], [IsDefault], [IsMandatory], [CreatedDate], 
                                 [ChartOfAccountId], [AutomateLedgerFeeCalculation], 
                                 [ThrottleOverTheCounterWithdrawals])
                                VALUES 
                                (@Id, @Code, @Description, @MaximumAllowedWithdrawal, 
                                 @MaximumAllowedDeposit, @MinimumBalance, @OperatingBalance, 
                                 @WithdrawalNoticeAmount, @WithdrawalNoticePeriod, 
                                 @WithdrawalInterval, @AnnualPercentageYield, @Priority, 
                                 @IsLocked, @IsDefault, @IsMandatory, @CreatedDate, 
                                 @ChartOfAccountId, @AutomateLedgerFeeCalculation, 
                                 @ThrottleOverTheCounterWithdrawals)";

                // OPTION 2: If SequentialId is required but not IDENTITY, use DEFAULT keyword
                // string query = @"INSERT INTO [swiftFin_SavingsProducts] 
                //                 ([Id], [Code], [SequentialId], [Description], [MaximumAllowedWithdrawal], 
                //                  [MaximumAllowedDeposit], [MinimumBalance], [OperatingBalance], 
                //                  [WithdrawalNoticeAmount], [WithdrawalNoticePeriod], 
                //                  [WithdrawalInterval], [AnnualPercentageYield], [Priority], 
                //                  [IsLocked], [IsDefault], [IsMandatory], [CreatedDate], 
                //                  [ChartOfAccountId], [AutomateLedgerFeeCalculation], 
                //                  [ThrottleOverTheCounterWithdrawals])
                //                 VALUES 
                //                 (@Id, @Code, DEFAULT, @Description, @MaximumAllowedWithdrawal, 
                //                  @MaximumAllowedDeposit, @MinimumBalance, @OperatingBalance, 
                //                  @WithdrawalNoticeAmount, @WithdrawalNoticePeriod, 
                //                  @WithdrawalInterval, @AnnualPercentageYield, @Priority, 
                //                  @IsLocked, @IsDefault, @IsMandatory, @CreatedDate, 
                //                  @ChartOfAccountId, @AutomateLedgerFeeCalculation, 
                //                  @ThrottleOverTheCounterWithdrawals)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, product);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return GetById(product.Id);
        }

        public void Update(SavingsProductDTO product)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(product.Description))
                throw new ArgumentException("Product description is required");

            if (product.ChartOfAccountId == Guid.Empty)
                throw new ArgumentException("Chart of Account is required");

            using (var conn = new SqlConnection(_connectionString))
            {
                // Don't include SequentialId in UPDATE if it's an IDENTITY column
                string query = @"UPDATE [swiftFin_SavingsProducts] 
                                SET [Code] = @Code,
                                    [Description] = @Description,
                                    [MaximumAllowedWithdrawal] = @MaximumAllowedWithdrawal,
                                    [MaximumAllowedDeposit] = @MaximumAllowedDeposit,
                                    [MinimumBalance] = @MinimumBalance,
                                    [OperatingBalance] = @OperatingBalance,
                                    [WithdrawalNoticeAmount] = @WithdrawalNoticeAmount,
                                    [WithdrawalNoticePeriod] = @WithdrawalNoticePeriod,
                                    [WithdrawalInterval] = @WithdrawalInterval,
                                    [AnnualPercentageYield] = @AnnualPercentageYield,
                                    [Priority] = @Priority,
                                    [IsLocked] = @IsLocked,
                                    [IsDefault] = @IsDefault,
                                    [IsMandatory] = @IsMandatory,
                                    [ChartOfAccountId] = @ChartOfAccountId,
                                    [AutomateLedgerFeeCalculation] = @AutomateLedgerFeeCalculation,
                                    [ThrottleOverTheCounterWithdrawals] = @ThrottleOverTheCounterWithdrawals
                                WHERE [Id] = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, product);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // First check if product is being used by any accounts
                string checkQuery = @"SELECT COUNT(*) FROM [swiftFin_CustomerAccounts] WHERE SavingsProductId = @Id";
                using (var checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    var count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                        throw new InvalidOperationException("Cannot delete savings product. It is being used by customer accounts.");
                }

                string deleteQuery = "DELETE FROM [swiftFin_SavingsProducts] WHERE Id = @Id";
                using (var cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Lock(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [swiftFin_SavingsProducts] SET IsLocked = 1 WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Unlock(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [swiftFin_SavingsProducts] SET IsLocked = 0 WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SetAsDefault(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // First, unset all other defaults
                string resetQuery = "UPDATE [swiftFin_SavingsProducts] SET IsDefault = 0 WHERE IsDefault = 1";
                using (var resetCmd = new SqlCommand(resetQuery, conn))
                {
                    resetCmd.ExecuteNonQuery();
                }

                // Then set this one as default
                string setQuery = "UPDATE [swiftFin_SavingsProducts] SET IsDefault = 1 WHERE Id = @Id";
                using (var setCmd = new SqlCommand(setQuery, conn))
                {
                    setCmd.Parameters.AddWithValue("@Id", id);
                    setCmd.ExecuteNonQuery();
                }
            }
        }

        private int GenerateNextCode()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT MAX(Code) FROM [swiftFin_SavingsProducts]";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                }
            }
        }

        private void AddParams(SqlCommand cmd, SavingsProductDTO product)
        {
            cmd.Parameters.AddWithValue("@Id", product.Id);
            cmd.Parameters.AddWithValue("@Code", product.Code);
            cmd.Parameters.AddWithValue("@Description", product.Description ?? "");
            cmd.Parameters.AddWithValue("@MaximumAllowedWithdrawal", product.MaximumAllowedWithdrawal);
            cmd.Parameters.AddWithValue("@MaximumAllowedDeposit", product.MaximumAllowedDeposit);
            cmd.Parameters.AddWithValue("@MinimumBalance", product.MinimumBalance);
            cmd.Parameters.AddWithValue("@OperatingBalance", product.OperatingBalance);
            cmd.Parameters.AddWithValue("@WithdrawalNoticeAmount", product.WithdrawalNoticeAmount);
            cmd.Parameters.AddWithValue("@WithdrawalNoticePeriod", product.WithdrawalNoticePeriod);
            cmd.Parameters.AddWithValue("@WithdrawalInterval", product.WithdrawalInterval);
            cmd.Parameters.AddWithValue("@AnnualPercentageYield", product.AnnualPercentageYield);
            cmd.Parameters.AddWithValue("@Priority", product.Priority);
            cmd.Parameters.AddWithValue("@IsLocked", product.IsLocked);
            cmd.Parameters.AddWithValue("@IsDefault", product.IsDefault);
            cmd.Parameters.AddWithValue("@IsMandatory", product.IsMandatory);
            cmd.Parameters.AddWithValue("@CreatedDate", product.CreatedDate);
            cmd.Parameters.AddWithValue("@ChartOfAccountId", product.ChartOfAccountId);
            cmd.Parameters.AddWithValue("@AutomateLedgerFeeCalculation", product.AutomateLedgerFeeCalculation);
            cmd.Parameters.AddWithValue("@ThrottleOverTheCounterWithdrawals", product.ThrottleOverTheCounterWithdrawals);
            // Removed ChargeBenefactor and ChargeType parameters since they don't exist in the database
        }

        private SavingsProductDTO Map(IDataReader reader)
        {
            return new SavingsProductDTO
            {
                Id = (Guid)reader["Id"],
                Code = Convert.ToInt32(reader["Code"]),
                // Optional: Add SequentialId mapping if you want to include it in the DTO
                // SequentialId = Convert.ToInt32(reader["SequentialId"]),
                Description = reader["Description"]?.ToString(),
                MaximumAllowedWithdrawal = Convert.ToDecimal(reader["MaximumAllowedWithdrawal"]),
                MaximumAllowedDeposit = Convert.ToDecimal(reader["MaximumAllowedDeposit"]),
                MinimumBalance = Convert.ToDecimal(reader["MinimumBalance"]),
                OperatingBalance = Convert.ToDecimal(reader["OperatingBalance"]),
                WithdrawalNoticeAmount = Convert.ToDecimal(reader["WithdrawalNoticeAmount"]),
                WithdrawalNoticePeriod = Convert.ToInt32(reader["WithdrawalNoticePeriod"]),
                WithdrawalInterval = Convert.ToInt32(reader["WithdrawalInterval"]),
                AnnualPercentageYield = Convert.ToDouble(reader["AnnualPercentageYield"]),
                Priority = Convert.ToInt32(reader["Priority"]),
                PriorityDescription = GetPriorityDescription(Convert.ToInt32(reader["Priority"])),
                IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                IsDefault = Convert.ToBoolean(reader["IsDefault"]),
                IsMandatory = Convert.ToBoolean(reader["IsMandatory"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                ChartOfAccountId = (Guid)reader["ChartOfAccountId"],
                ChartOfAccountAccountType = Convert.ToInt32(reader["ChartOfAccountAccountType"]),
                ChartOfAccountAccountCode = Convert.ToInt32(reader["ChartOfAccountAccountCode"]),
                ChartOfAccountAccountName = reader["ChartOfAccountAccountName"]?.ToString(),
                ChartOfAccountCostCenterId = reader["ChartOfAccountCostCenterId"] == DBNull.Value ? (Guid?)null : (Guid)reader["ChartOfAccountCostCenterId"],
                ChartOfAccountCostCenterDescription = reader["ChartOfAccountCostCenterDescription"]?.ToString(),
                AutomateLedgerFeeCalculation = Convert.ToBoolean(reader["AutomateLedgerFeeCalculation"]),
                ThrottleOverTheCounterWithdrawals = Convert.ToBoolean(reader["ThrottleOverTheCounterWithdrawals"]),
                // Set default values for ChargeBenefactor and ChargeType since they don't exist in the database
                ChargeBenefactor = 1, // Default to Customer
                ChargeType = 1 // Default to Fixed
            };
        }

        private string GetPriorityDescription(int priority)
        {
            // You can customize this based on your business logic
            switch (priority)
            {
                case 1: return "High";
                case 2: return "Medium";
                case 3: return "Low";
                default: return "Standard";
            }
        }
    }
}