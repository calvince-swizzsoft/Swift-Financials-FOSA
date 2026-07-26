using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Services;
using Dapper;

namespace WebApplication.Services
{
    public class CustomerAccountService
    {
        private readonly string _connectionString;
        private readonly CompanyAttachedProductService _companyAttachedProductService;
        private readonly BranchService _branchService;
        private readonly SavingsProductService _savingsProductService;

        public CustomerAccountService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
            _companyAttachedProductService = new CompanyAttachedProductService();
            _branchService = new BranchService();
            _savingsProductService = new SavingsProductService();
        }


        public async Task<IEnumerable<dynamic>> GetAccountsByCustomerIdAsync(Guid customerId)
        {
            try
            {
                const string sql = @"
    WITH WithdrawalTypes AS (
        SELECT * FROM (VALUES 
            ('Withdrawals'),
            ('Transfer'),
            ('Withdrawal'),
            ('Cash Withdrawal'),
            ('Bank Transfer'),
            ('EFT')
        ) AS wt(Description)
    ),
    WithdrawalJournals AS (
        SELECT DISTINCT j.Id AS JournalId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Journals] j
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je 
            ON j.Id = je.JournalId
        INNER JOIN WithdrawalTypes wt ON j.PrimaryDescription = wt.Description
        WHERE je.CustomerAccountId IN (
            SELECT Id FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
            WHERE CustomerId = @CustomerId
        )
    )
    SELECT 
        a.[Id],
        a.[CustomerId],
        a.[BranchId],
        a.[CustomerAccountType_ProductCode],
        a.[CustomerAccountType_TargetProductId],
        a.[CustomerAccountType_TargetProductCode],
        a.[ScoredLoanDisbursementProductCode],
        a.[ScoredLoanLimit],
        a.[ScoredLoanLimitRemarks],
        a.[ScoredLoanLimitDate],
        a.[Status],
        a.[Remarks],
        a.[RecordStatus],
        a.[ModifiedBy],
        a.[ModifiedDate],
        a.[SigningInstructions],
        a.[SequentialId],
        a.[CreatedBy],
        a.[CreatedDate],

        -- Product description
        CASE 
            WHEN lp.[Id] IS NOT NULL THEN lp.[Description]
            WHEN sp.[Id] IS NOT NULL THEN sp.[Description]
            ELSE 'Unknown Product Type'
        END AS ProductDescription,

        -- Product type
        CASE 
            WHEN lp.[Id] IS NOT NULL THEN 'Loan'
            WHEN sp.[Id] IS NOT NULL THEN 'Savings'
            ELSE 'Unknown'
        END AS ProductType,

        -- Balance: Loan accounts use TotalLoansBalance from LoanCases
        --          Savings accounts use journal entries with withdrawal logic
        CASE
            WHEN lp.[Id] IS NOT NULL THEN
                -- LOAN: pull live balance directly from LoanCases (same source as SP)
                ISNULL((
                    SELECT TOP 1 ROUND(lc.TotalLoansBalance, 0)
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanCases] lc
                    WHERE lc.CustomerId = a.CustomerId
                      AND lc.LoanProductId = a.CustomerAccountType_TargetProductId
                      AND lc.TotalLoansBalance > 0
                    ORDER BY lc.CreatedDate DESC
                ), 0)

            WHEN sp.[Id] IS NOT NULL THEN
                -- SAVINGS/SHARES: credits minus withdrawals from journal entries
                ISNULL((
                    SELECT SUM(
                        CASE 
                            WHEN je.Amount < 0 AND wj.JournalId IS NULL THEN ABS(je.Amount)
                            WHEN je.Amount > 0 AND wj.JournalId IS NOT NULL THEN -je.Amount
                            ELSE 0
                        END
                    )
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je
                    LEFT JOIN WithdrawalJournals wj ON je.JournalId = wj.JournalId
                    WHERE je.CustomerAccountId = a.[Id]
                ), 0)

            ELSE 0
        END AS AccountBalance,

        -- Last transaction date
        (
            SELECT MAX(je.[ValueDate]) 
            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je
            WHERE je.[CustomerAccountId] = a.[Id]
        ) AS LastTransactionDate

    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] a
    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts] lp 
        ON a.[CustomerAccountType_TargetProductId] = lp.[Id]
    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts] sp 
        ON a.[CustomerAccountType_TargetProductId] = sp.[Id]
    WHERE a.[CustomerId] = @CustomerId
    ORDER BY 
        CASE WHEN lp.[Id] IS NOT NULL THEN 1 ELSE 0 END,
        sp.[Description], 
        lp.[Description]";

                using (var connection = new SqlConnection(_connectionString))
                {
                    var accounts = await connection.QueryAsync(sql, new { CustomerId = customerId });
                    return accounts;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error in GetAccountsByCustomerIdAsync: {ex.Message}");
                throw;
            }
        }


        public IEnumerable<CustomerAccountDTO> GetAll()
        {
            var list = new List<CustomerAccountDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ca.*, 
                                c.SerialNumber as CustomerSerialNumber,
                                c.Type as CustomerType,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription,
                                c.Reference1 as CustomerReference1,
                                c.Reference2 as CustomerReference2,
                                c.Reference3 as CustomerReference3,
                                c.Address_MobileLine as CustomerAddressMobileLine,
                                c.Address_Email as CustomerAddressEmail,
                                sp.Description as CustomerAccountTypeTargetProductDescription,
                                sp.Code as CustomerAccountTypeTargetProductCode,
                                b.Code as BranchCode,
                                b.Description as BranchDescription,
                                comp.Description as BranchCompanyDescription
                                FROM [swiftFin_CustomerAccounts] ca
                                LEFT JOIN [swiftFin_Customers] c ON ca.CustomerId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON ca.CustomerAccountType_TargetProductId = sp.Id
                                LEFT JOIN [swiftFin_Branches] b ON ca.BranchId = b.Id
                                LEFT JOIN [swiftFin_Companies] comp ON b.CompanyId = comp.Id
                                ORDER BY ca.CreatedDate DESC";
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

        public CustomerAccountDTO GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // NOTE: Explicit column list (no "ca.*") to avoid any ambiguous/duplicate
                // column name collisions between swiftFin_CustomerAccounts and the
                // aliased columns below — this was the root cause of BranchCode
                // and CustomerAddressMobileLine returning wrong/default values.
                string query = @"SELECT 
                        ca.Id,
                        ca.CustomerId,
                        ca.BranchId,
                        ca.CustomerAccountType_ProductCode,
                        ca.CustomerAccountType_TargetProductId,
                        ca.CustomerAccountType_TargetProductCode,
                        ca.ScoredLoanDisbursementProductCode,
                        ca.ScoredLoanLimit,
                        ca.ScoredLoanLimitRemarks,
                        ca.ScoredLoanLimitDate,
                        ca.Status,
                        ca.Remarks,
                        ca.RecordStatus,
                        ca.ModifiedBy,
                        ca.ModifiedDate,
                        ca.SigningInstructions,
                        ca.SequentialId,
                        ca.CreatedBy,
                        ca.CreatedDate,
                                c.SerialNumber as CustomerSerialNumber,
                                c.Type as CustomerType,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription,
                                c.Reference1 as CustomerReference1,
                                c.Reference2 as CustomerReference2,
                                c.Reference3 as CustomerReference3,
                        c.Address_MobileLine as CustomerAddressMobileLine,
                        c.Address_Email as CustomerAddressEmail,
                                sp.Description as CustomerAccountTypeTargetProductDescription,
                                sp.Code as CustomerAccountTypeTargetProductCode,
                                b.Code as BranchCode,
                                b.Description as BranchDescription,
                        b.CompanyId as BranchCompanyId,
                                comp.Description as BranchCompanyDescription
                                FROM [swiftFin_CustomerAccounts] ca
                                LEFT JOIN [swiftFin_Customers] c ON ca.CustomerId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON ca.CustomerAccountType_TargetProductId = sp.Id
                                LEFT JOIN [swiftFin_Branches] b ON ca.BranchId = b.Id
                                LEFT JOIN [swiftFin_Companies] comp ON b.CompanyId = comp.Id
                                WHERE ca.Id = @Id";
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

        public IEnumerable<CustomerAccountDTO> GetByCustomerId(Guid customerId)
        {
            var list = new List<CustomerAccountDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ca.*, 
                                c.SerialNumber as CustomerSerialNumber,
                                c.Type as CustomerType,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription,
                                c.Reference1 as CustomerReference1,
                                c.Reference2 as CustomerReference2,
                                c.Reference3 as CustomerReference3,
                                c.Address_MobileLine as CustomerAddressMobileLine,
                                c.Address_Email as CustomerAddressEmail,
                                sp.Description as CustomerAccountTypeTargetProductDescription,
                                sp.Code as CustomerAccountTypeTargetProductCode,
                                b.Code as BranchCode,
                                b.Description as BranchDescription,
                                comp.Description as BranchCompanyDescription
                                FROM [swiftFin_CustomerAccounts] ca
                                LEFT JOIN [swiftFin_Customers] c ON ca.CustomerId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON ca.CustomerAccountType_TargetProductId = sp.Id
                                LEFT JOIN [swiftFin_Branches] b ON ca.BranchId = b.Id
                                LEFT JOIN [swiftFin_Companies] comp ON b.CompanyId = comp.Id
                                WHERE ca.CustomerId = @CustomerId
                                ORDER BY ca.CreatedDate";
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

        public CustomerAccountDTO GetByCustomerAndProduct(Guid customerId, Guid targetProductId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ca.*, 
                                c.SerialNumber as CustomerSerialNumber,
                                c.Type as CustomerType,
                                c.Individual_FirstName as CustomerIndividualFirstName,
                                c.Individual_LastName as CustomerIndividualLastName,
                                c.NonIndividual_Description as CustomerNonIndividualDescription,
                                c.Reference1 as CustomerReference1,
                                c.Reference2 as CustomerReference2,
                                c.Reference3 as CustomerReference3,
                                c.Address_MobileLine as CustomerAddressMobileLine,
                                c.Address_Email as CustomerAddressEmail,
                                sp.Description as CustomerAccountTypeTargetProductDescription
                                FROM [swiftFin_CustomerAccounts] ca
                                LEFT JOIN [swiftFin_Customers] c ON ca.CustomerId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON ca.CustomerAccountType_TargetProductId = sp.Id
                                WHERE ca.CustomerId = @CustomerId AND ca.CustomerAccountType_TargetProductId = @TargetProductId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@TargetProductId", targetProductId);
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

        public Dictionary<Guid, List<CustomerAccountDTO>> GetAccountsByCustomerIds(List<Guid> customerIds)
        {
            var result = new Dictionary<Guid, List<CustomerAccountDTO>>();

            if (customerIds == null || customerIds.Count == 0)
                return result;

            // Initialize dictionary
            foreach (var customerId in customerIds)
            {
                result[customerId] = new List<CustomerAccountDTO>();
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

                string query = $@"SELECT ca.*, 
                         c.SerialNumber as CustomerSerialNumber,
                         c.Type as CustomerType,
                         c.Individual_FirstName as CustomerIndividualFirstName,
                         c.Individual_LastName as CustomerIndividualLastName,
                         c.NonIndividual_Description as CustomerNonIndividualDescription,
                         c.Reference1 as CustomerReference1,
                         c.Reference2 as CustomerReference2,
                         c.Reference3 as CustomerReference3,
                         c.Address_MobileLine as CustomerAddressMobileLine,
                         c.Address_Email as CustomerAddressEmail,
                         sp.Description as CustomerAccountTypeTargetProductDescription,
                         sp.Code as CustomerAccountTypeTargetProductCode,
                         b.Code as BranchCode,
                         b.Description as BranchDescription,
                         comp.Description as BranchCompanyDescription
                         FROM [swiftFin_CustomerAccounts] ca
                         LEFT JOIN [swiftFin_Customers] c ON ca.CustomerId = c.Id
                         LEFT JOIN [swiftFin_SavingsProducts] sp ON ca.CustomerAccountType_TargetProductId = sp.Id
                         LEFT JOIN [swiftFin_Branches] b ON ca.BranchId = b.Id
                         LEFT JOIN [swiftFin_Companies] comp ON b.CompanyId = comp.Id
                         WHERE ca.CustomerId IN ({string.Join(",", paramNames)})
                         ORDER BY ca.CustomerId, ca.CreatedDate";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            var account = Map(reader);
                            if (result.ContainsKey(account.CustomerId))
                            {
                                result[account.CustomerId].Add(account);
                            }
                        }
                }
            }
            return result;
        }

        public CustomerAccountDTO Create(CustomerAccountDTO account)
        {
            // Validate required fields
            if (account.CustomerId == Guid.Empty)
                throw new ArgumentException("Customer ID is required");

            if (account.BranchId == Guid.Empty)
                throw new ArgumentException("Branch ID is required");

            if (account.CustomerAccountTypeTargetProductId == Guid.Empty)
                throw new ArgumentException("Target Product ID is required");

            // Check if account already exists for this customer and product
            var existingAccount = GetByCustomerAndProduct(account.CustomerId, account.CustomerAccountTypeTargetProductId);
            if (existingAccount != null)
                throw new InvalidOperationException("Account already exists for this customer and product");

            using (var conn = new SqlConnection(_connectionString))
            {
                // Generate new Guid if not provided
                if (account.Id == Guid.Empty)
                    account.Id = Guid.NewGuid();

                account.CreatedDate = DateTime.Now;

                // Get default status values
                account.Status = (int)CustomerAccountStatus.Normal;
                account.RecordStatus = (int)RecordStatus.Approved; // This now returns 2 (Approved)

                // Get default values for required NOT NULL columns
                account.ScoredLoanDisbursementProductCode = GetDefaultScoredLoanDisbursementProductCode();
                account.ScoredLoanLimit = GetDefaultScoredLoanLimit();
                account.ScoredLoanLimitRemarks = GetDefaultScoredLoanLimitRemarks();

                // Get customer details to populate fields
                var customerService = new CustomerService();
                var customer = customerService.GetById(account.CustomerId);
                if (customer == null)
                    throw new ArgumentException("Customer does not exist");

                // Populate customer details
                account.CustomerSerialNumber = customer.SerialNumber;
                account.CustomerType = customer.Type;
                account.CustomerIndividualFirstName = customer.IndividualFirstName ?? "";
                account.CustomerIndividualLastName = customer.IndividualLastName ?? "";
                account.CustomerNonIndividualDescription = customer.NonIndividualDescription ?? "";
                account.CustomerReference1 = customer.Reference1 ?? "";
                account.CustomerReference2 = customer.Reference2 ?? "";
                account.CustomerReference3 = customer.Reference3 ?? "";
                account.CustomerAddressMobileLine = customer.AddressMobileLine ?? "";
                account.CustomerAddressEmail = customer.AddressEmail ?? "";

                // ── Branch enrichment is informational only — never let a bug here
                //    block account creation. Wrapped so any exception inside
                //    _branchService.GetById() (e.g. a bad int cast on Code) cannot
                //    surface as a confusing "BranchCode" error during account creation.
                try
                {
                    var branch = _branchService.GetById(account.BranchId);
                    if (branch != null)
                    {
                        account.BranchCode = branch.Code;
                        account.BranchDescription = branch.Description ?? "";
                        account.BranchCompanyId = branch.CompanyId;
                        account.BranchCompanyDescription = branch.CompanyDescription ?? "";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"WARNING: Could not load branch details for BranchId={account.BranchId}: {ex.Message}");
                }

                string query = @"INSERT INTO [swiftFin_CustomerAccounts] 
                                ([Id], [CustomerId], [BranchId], 
                                 [CustomerAccountType_ProductCode], [CustomerAccountType_TargetProductId],
                                 [CustomerAccountType_TargetProductCode], [ScoredLoanDisbursementProductCode],
                                 [ScoredLoanLimit], [ScoredLoanLimitRemarks], [Status], [RecordStatus],
                                 [CreatedBy], [CreatedDate])
                                VALUES 
                                (@Id, @CustomerId, @BranchId, 
                                 @CustomerAccountTypeProductCode, @CustomerAccountTypeTargetProductId,
                                 @CustomerAccountTypeTargetProductCode, @ScoredLoanDisbursementProductCode,
                                 @ScoredLoanLimit, @ScoredLoanLimitRemarks, @Status, @RecordStatus,
                                 @CreatedBy, @CreatedDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, account);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return GetById(account.Id);
        }

        public IEnumerable<CustomerAccountDTO> CreateAccountsForCustomer(Guid customerId, Guid branchId)
        {
            var createdAccounts = new List<CustomerAccountDTO>();
            System.Diagnostics.Debug.WriteLine($"DEBUG: Starting CreateAccountsForCustomer for CustomerId: {customerId}, BranchId: {branchId}");

            // Get the customer
            var customerService = new CustomerService();
            var customer = customerService.GetById(customerId);

            if (customer == null)
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG ERROR: Customer with ID {customerId} does not exist");
                throw new ArgumentException("Customer does not exist");
            }

            // Get branch to find company
            var branch = _branchService.GetById(branchId);
            if (branch == null)
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG ERROR: Branch with ID {branchId} does not exist");
                throw new ArgumentException("Branch does not exist");
            }

            Guid companyId = branch.CompanyId;
            System.Diagnostics.Debug.WriteLine($"DEBUG: Branch {branchId} belongs to CompanyId: {companyId}");

            // Get all attached products for the company
            var attachedProducts = _companyAttachedProductService.GetByCompanyId(companyId);
            System.Diagnostics.Debug.WriteLine($"DEBUG: Found {attachedProducts.Count()} attached products for company {companyId}");

            if (!attachedProducts.Any())
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG WARNING: No attached products found for company {companyId}");
                return createdAccounts; // Return empty list
            }

            foreach (var attachedProduct in attachedProducts)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Processing product {attachedProduct.TargetProductId}, Code: {attachedProduct.ProductCode}");

                    // Check if account already exists for this product
                    var existingAccount = GetByCustomerAndProduct(customerId, attachedProduct.TargetProductId);
                    if (existingAccount == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG: Creating new account for product {attachedProduct.TargetProductId}");

                        // Get the savings product details
                        var savingsProduct = _savingsProductService.GetById(attachedProduct.TargetProductId);
                        if (savingsProduct == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"DEBUG ERROR: Savings product with ID {attachedProduct.TargetProductId} does not exist");
                            continue; // Skip this product
                        }

                        System.Diagnostics.Debug.WriteLine($"DEBUG: Found savings product with code: {savingsProduct.Code}");

                        // Create new account with proper values
                        var newAccount = new CustomerAccountDTO
                        {
                            CustomerId = customerId,
                            BranchId = branchId,
                            // Use the actual ProductCode from CompanyAttachedProducts
                            CustomerAccountTypeProductCode = attachedProduct.ProductCode,
                            CustomerAccountTypeTargetProductId = attachedProduct.TargetProductId,
                            // Use the actual Code from SavingsProducts
                            CustomerAccountTypeTargetProductCode = savingsProduct.Code,
                            Status = (int)CustomerAccountStatus.Normal,
                            RecordStatus = (int)RecordStatus.Approved, // This now returns 2 (Approved)
                            CreatedBy = customer.CreatedBy ?? GetDefaultCreatedBy(),
                            CreatedDate = DateTime.Now,
                            // Add required fields for NOT NULL columns
                            ScoredLoanDisbursementProductCode = GetDefaultScoredLoanDisbursementProductCode(),
                            ScoredLoanLimit = GetDefaultScoredLoanLimit(),
                            ScoredLoanLimitRemarks = GetDefaultScoredLoanLimitRemarks()
                        };

                        var createdAccount = Create(newAccount);
                        createdAccounts.Add(createdAccount);
                        System.Diagnostics.Debug.WriteLine($"DEBUG: Successfully created account {createdAccount.Id}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG: Account already exists for product {attachedProduct.TargetProductId}");
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with other products
                    System.Diagnostics.Debug.WriteLine($"DEBUG ERROR: Error creating account for product {attachedProduct.TargetProductId}: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"DEBUG ERROR Stack Trace: {ex.StackTrace}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"DEBUG: Created {createdAccounts.Count} accounts for customer {customerId}");
            return createdAccounts;
        }

        // Helper methods to get default values
        private int GetDefaultStatus()
        {
            try
            {
                var defaultStatus = ConfigurationManager.AppSettings["DefaultCustomerAccountStatus"];
                if (!string.IsNullOrEmpty(defaultStatus) && int.TryParse(defaultStatus, out int status))
                {
                    return status;
                }
            }
            catch
            {
                // If config fails, use default
            }

            return 1; // Default to Active
        }

        private int GetDefaultRecordStatus()
        {
            try
            {
                var defaultRecordStatus = ConfigurationManager.AppSettings["DefaultCustomerAccountRecordStatus"];
                if (!string.IsNullOrEmpty(defaultRecordStatus) && int.TryParse(defaultRecordStatus, out int recordStatus))
                {
                    return recordStatus;
                }
            }
            catch
            {
                // If config fails, use default
            }

            // CHANGED FROM 1 TO 2 - Now defaults to "Approved" status
            return 2; // Default to Approved
        }

        private int GetDefaultScoredLoanDisbursementProductCode()
        {
            try
            {
                var defaultCode = ConfigurationManager.AppSettings["DefaultScoredLoanDisbursementProductCode"];
                if (!string.IsNullOrEmpty(defaultCode) && int.TryParse(defaultCode, out int code))
                {
                    return code;
                }
            }
            catch
            {
                // If config fails, use default
            }

            return 0; // Default to 0 (no scored loan product)
        }

        private decimal GetDefaultScoredLoanLimit()
        {
            try
            {
                var defaultLimit = ConfigurationManager.AppSettings["DefaultScoredLoanLimit"];
                if (!string.IsNullOrEmpty(defaultLimit) && decimal.TryParse(defaultLimit, out decimal limit))
                {
                    return limit;
                }
            }
            catch
            {
                // If config fails, use default
            }

            return 0.00m; // Default to 0.00
        }

        private string GetDefaultScoredLoanLimitRemarks()
        {
            try
            {
                var defaultRemarks = ConfigurationManager.AppSettings["DefaultScoredLoanLimitRemarks"];
                if (!string.IsNullOrEmpty(defaultRemarks))
                {
                    return defaultRemarks;
                }
            }
            catch
            {
                // If config fails, use default
            }

            return string.Empty; // Default to empty string
        }

        private string GetDefaultCreatedBy()
        {
            try
            {
                var defaultCreatedBy = ConfigurationManager.AppSettings["DefaultSystemUser"];
                if (!string.IsNullOrEmpty(defaultCreatedBy))
                {
                    return defaultCreatedBy;
                }
            }
            catch
            {
                // If config fails, use default
            }

            return "SYSTEM";
        }

        private void AddParams(SqlCommand cmd, CustomerAccountDTO account)
        {
            cmd.Parameters.AddWithValue("@Id", account.Id);
            cmd.Parameters.AddWithValue("@CustomerId", account.CustomerId);
            cmd.Parameters.AddWithValue("@BranchId", account.BranchId);
            cmd.Parameters.AddWithValue("@CustomerAccountTypeProductCode", account.CustomerAccountTypeProductCode);
            cmd.Parameters.AddWithValue("@CustomerAccountTypeTargetProductId", account.CustomerAccountTypeTargetProductId);
            cmd.Parameters.AddWithValue("@CustomerAccountTypeTargetProductCode", account.CustomerAccountTypeTargetProductCode);
            cmd.Parameters.AddWithValue("@ScoredLoanDisbursementProductCode", account.ScoredLoanDisbursementProductCode);
            cmd.Parameters.AddWithValue("@ScoredLoanLimit", account.ScoredLoanLimit);
            cmd.Parameters.AddWithValue("@ScoredLoanLimitRemarks", account.ScoredLoanLimitRemarks ?? "");
            cmd.Parameters.AddWithValue("@Status", account.Status);
            cmd.Parameters.AddWithValue("@RecordStatus", account.RecordStatus); // This will now be 2 (Approved)
            cmd.Parameters.AddWithValue("@CreatedBy", account.CreatedBy ?? GetDefaultCreatedBy());
            cmd.Parameters.AddWithValue("@CreatedDate", account.CreatedDate);
        }

        private CustomerAccountDTO Map(IDataReader reader)
        {
            string SafeString(string col)
            {
                try
                {
                    //int ord = reader.GetOrdinal(col);
                    int ord = reader.SafeGetInt32(col);
                    return reader.IsDBNull(ord) ? null : reader[ord].ToString();
                }
                catch
                {
                    return null;
                }
            }

            int SafeInt(string col)
            {
                try
                {
                    int ord = reader.GetOrdinal(col);
                    if (reader.IsDBNull(ord)) return 0;

                    var val = reader[ord];
                    if (val is int i) return i;
                    if (int.TryParse(val.ToString(), out int parsed)) return parsed;
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }

            Guid SafeGuidLocal(string col)
            {
                try
                {
                    int ord = reader.GetOrdinal(col);
                    return reader.IsDBNull(ord) ? Guid.Empty : (Guid)reader[ord];
                }
                catch
                {
                    return Guid.Empty;
                }
            }

            return new CustomerAccountDTO
            {
                Id = (Guid)reader["Id"],
                CustomerId = (Guid)reader["CustomerId"],
                BranchId = (Guid)reader["BranchId"],
                CustomerSerialNumber = reader["CustomerSerialNumber"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CustomerSerialNumber"]),
                CustomerType = reader["CustomerType"] == DBNull.Value ? (byte)0 : Convert.ToByte(reader["CustomerType"]),
                CustomerIndividualFirstName = SafeString("CustomerIndividualFirstName"),
                CustomerIndividualLastName = SafeString("CustomerIndividualLastName"),
                CustomerNonIndividualDescription = SafeString("CustomerNonIndividualDescription"),
                CustomerAccountTypeProductCode = reader["CustomerAccountType_ProductCode"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CustomerAccountType_ProductCode"]),
                CustomerAccountTypeTargetProductId = reader["CustomerAccountType_TargetProductId"] == DBNull.Value ? Guid.Empty : (Guid)reader["CustomerAccountType_TargetProductId"],
                CustomerAccountTypeTargetProductCode = reader["CustomerAccountType_TargetProductCode"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CustomerAccountType_TargetProductCode"]),
                CustomerAccountTypeTargetProductDescription = SafeString("CustomerAccountTypeTargetProductDescription"),
                ScoredLoanDisbursementProductCode = reader["ScoredLoanDisbursementProductCode"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ScoredLoanDisbursementProductCode"]),
                ScoredLoanLimit = reader["ScoredLoanLimit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ScoredLoanLimit"]),
                ScoredLoanLimitRemarks = SafeString("ScoredLoanLimitRemarks"),
                Status = reader["Status"] == DBNull.Value ? GetDefaultStatus() : Convert.ToInt32(reader["Status"]),
                RecordStatus = reader["RecordStatus"] == DBNull.Value ? GetDefaultRecordStatus() : Convert.ToInt32(reader["RecordStatus"]),
                CreatedBy = SafeString("CreatedBy"),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedDate"]),
                CustomerReference1 = SafeString("CustomerReference1"),
                CustomerReference2 = SafeString("CustomerReference2"),
                CustomerReference3 = SafeString("CustomerReference3"),
                CustomerAddressMobileLine = SafeString("CustomerAddressMobileLine"),
                CustomerAddressEmail = SafeString("CustomerAddressEmail"),
                BranchCode = SafeInt("BranchCode"),
                BranchDescription = SafeString("BranchDescription"),
                BranchCompanyId = SafeGuidLocal("BranchCompanyId"),
                BranchCompanyDescription = SafeString("BranchCompanyDescription")
            };
        }

        // ── safe guid reader — handles missing/NULL BranchCompanyId column gracefully ──
        private Guid SafeGuid(IDataReader reader, string col)
        {
            try
            {
                int ord = reader.GetOrdinal(col);
                return reader.IsDBNull(ord) ? Guid.Empty : (Guid)reader[ord];
            }
            catch
            {
                return Guid.Empty;
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Creates a customer account for ANY savings product (M-WALLET, Deposits,
        // Share Capital, Entrance Fee, Holiday Savings, etc.) identified by its Code.
        // Use the existing SavingsProductService/get-products endpoint to list
        // available products and their codes for the frontend to choose from.
        // ─────────────────────────────────────────────────────────────────────────────
        public CustomerAccountDTO CreateAccountForCustomer(Guid customerId, int productCode, Guid? branchId = null)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID is required");

            // ── Single branch system — always use the default branch ──────────────
            Guid resolvedBranchId = branchId ?? Guid.Parse("5C84C824-0CE3-455B-A5C5-994E2BFBA380");

            // ── 1. Resolve the savings product by Code ─────────────────────────────
            Guid productId = Guid.Empty;
            string productDescription = "";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
            SELECT TOP 1 Id, Description
            FROM [swiftFin_SavingsProducts]
            WHERE Code = @Code", conn))
                {
                    cmd.Parameters.AddWithValue("@Code", productCode);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException(
                                $"Savings product with Code={productCode} is not configured.");

                        productId = (Guid)reader["Id"];
                        productDescription = reader["Description"]?.ToString();
                    }
                }
            }

            // ── 2. Check member doesn't already have an active account for this product ──
            var existingAccount = GetByCustomerAndProduct(customerId, productId);
            if (existingAccount != null)
                throw new InvalidOperationException(
                    $"Member already has an active {productDescription} account.");

            // ── 3. Build the account DTO and reuse the existing Create() pipeline ────
            var newAccount = new CustomerAccountDTO
            {
                CustomerId = customerId,
                BranchId = resolvedBranchId,
                CustomerAccountTypeProductCode = 4,            // Savings
                CustomerAccountTypeTargetProductId = productId,
                CustomerAccountTypeTargetProductCode = productCode,
                CreatedBy = GetDefaultCreatedBy()
            };

            return Create(newAccount);
        }
    }
}