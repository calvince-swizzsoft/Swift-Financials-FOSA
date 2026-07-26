using Application.MainBoundedContext.DTO.RegistryModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace WebApplication.Services
{
    public class CustomerService
    {
        private readonly string _connectionString;

        public CustomerService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        }

        public IEnumerable<CustomerDTO> GetAll()
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                        FROM [swiftFin_Customers] c
                        ORDER BY c.SerialNumber DESC";
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

        public CustomerDTO GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE c.Id = @Id";
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

        public IEnumerable<CustomerDTO> GetByType(int type)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE c.Type = @Type
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Type", type);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public IEnumerable<CustomerDTO> GetByName(string name)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE (c.Individual_FirstName LIKE @Name OR 
                                       c.Individual_LastName LIKE @Name OR
                                       c.NonIndividual_Description LIKE @Name)
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public IEnumerable<CustomerDTO> GetByIdentificationNumber(string identificationNumber)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE (c.Individual_IdentityCardNumber LIKE @IdentificationNumber OR
                                       c.NonIndividual_RegistrationNumber LIKE @IdentificationNumber)
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdentificationNumber", "%" + identificationNumber + "%");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public IEnumerable<CustomerDTO> GetByStationId(Guid stationId)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE c.StationId = @StationId
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StationId", stationId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public IEnumerable<CustomerDTO> Search(string searchQuery)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE (c.Individual_FirstName LIKE @SearchQuery OR 
                                       c.Individual_LastName LIKE @SearchQuery OR
                                       c.NonIndividual_Description LIKE @SearchQuery OR
                                       c.Individual_IdentityCardNumber LIKE @SearchQuery OR
                                       c.NonIndividual_RegistrationNumber LIKE @SearchQuery OR
                                       c.Reference1 LIKE @SearchQuery OR
                                       c.Reference2 LIKE @SearchQuery OR
                                       c.Reference3 LIKE @SearchQuery OR
                                       c.BankName LIKE @SearchQuery OR
                                       c.BranchName LIKE @SearchQuery)
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        // Get customer by identity card number (for duplicate check)
        public CustomerDTO GetByIdentityCardNumber(string identityCardNumber)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE c.Individual_IdentityCardNumber = @IdentityCardNumber
                                OR c.NonIndividual_RegistrationNumber = @IdentityCardNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdentityCardNumber", identityCardNumber);
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

        public bool CustomerExists(string identityCardOrRegistrationNumber)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT COUNT(*) 
                        FROM [swiftFin_Customers] 
                        WHERE Individual_IdentityCardNumber = @Number
                        OR NonIndividual_RegistrationNumber = @Number";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Number", identityCardOrRegistrationNumber);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        // Get customers with pagination
        public IEnumerable<CustomerDTO> GetAllWithPagination(int page = 1, int pageSize = 20)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                ORDER BY c.SerialNumber
                                OFFSET @Offset ROWS
                                FETCH NEXT @PageSize ROWS ONLY";

                using (var cmd = new SqlCommand(query, conn))
                {
                    int offset = (page - 1) * pageSize;
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public int GetTotalCount()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM [swiftFin_Customers]";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        // Get customers by bank name
        public IEnumerable<CustomerDTO> GetByBankName(string bankName)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE c.BankName LIKE @BankName
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BankName", "%" + bankName + "%");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        // Get customers by branch name
        public IEnumerable<CustomerDTO> GetByBranchName(string branchName)
        {
            var list = new List<CustomerDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                                FROM [swiftFin_Customers] c
                                WHERE c.BranchName LIKE @BranchName
                                ORDER BY c.SerialNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchName", "%" + branchName + "%");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        // Get distinct bank names for dropdown
        public List<string> GetDistinctBankNames()
        {
            var banks = new List<string>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT DISTINCT BankName 
                                FROM [swiftFin_Customers] 
                                WHERE BankName IS NOT NULL AND BankName != ''
                                ORDER BY BankName";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            banks.Add(reader["BankName"]?.ToString());
                }
            }
            return banks;
        }

        // Get distinct branch names for a specific bank
        public List<string> GetDistinctBranchNames(string bankName)
        {
            var branches = new List<string>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT DISTINCT BranchName 
                                FROM [swiftFin_Customers] 
                                WHERE BankName = @BankName 
                                AND BranchName IS NOT NULL AND BranchName != ''
                                ORDER BY BranchName";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BankName", bankName);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            branches.Add(reader["BranchName"]?.ToString());
                }
            }
            return branches;
        }

        public CustomerDTO GetByRegistrationNumber(string registrationNumber)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT c.*
                        FROM [swiftFin_Customers] c
                        WHERE c.NonIndividual_RegistrationNumber = @RegistrationNumber";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RegistrationNumber", registrationNumber);
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
        public CustomerDTO GetByMemberNo(string memberNo)
        {
            if (string.IsNullOrWhiteSpace(memberNo))
                throw new ArgumentException("MemberNo cannot be null or empty", nameof(memberNo));

            CustomerDTO customer = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
    SELECT TOP 1 c.*
    FROM [swiftFin_Customers] c
    WHERE c.Reference2 LIKE @MemberNo
       OR c.NonIndividual_RegistrationNumber LIKE @MemberNo
    ORDER BY c.SerialNumber";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MemberNo", "%" + memberNo + "%");

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = Map(reader);
                        }
                    }
                }
            }

            return customer; // will be null if not found
        }
        public CustomerDTO Create(CustomerDTO customer)
        {
            // DUPLICATE VALIDATION: Check before creating
            if (!string.IsNullOrEmpty(customer.IndividualIdentityCardNumber))
            {
                // Check for individual duplicates
                var existingIndividual = GetByIdentityCardNumber(customer.IndividualIdentityCardNumber);
                if (existingIndividual != null)
                {
                    throw new InvalidOperationException(
                        $"Customer with ID Card Number '{customer.IndividualIdentityCardNumber}' already exists. " +
                        $"Existing customer: {existingIndividual.IndividualFirstName} {existingIndividual.IndividualLastName} (ID: {existingIndividual.Id})");
                }
            }

            if (!string.IsNullOrEmpty(customer.NonIndividualRegistrationNumber))
            {
                // Check for corporate duplicates
                var existingCorporate = GetByRegistrationNumber(customer.NonIndividualRegistrationNumber);
                if (existingCorporate != null)
                {
                    throw new InvalidOperationException(
                        $"Customer with Registration Number '{customer.NonIndividualRegistrationNumber}' already exists. " +
                        $"Existing customer: {existingCorporate.NonIndividualDescription} (ID: {existingCorporate.Id})");
                }
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                // Generate new Guid if not provided
                if (customer.Id == Guid.Empty)
                    customer.Id = Guid.NewGuid();

                customer.CreatedDate = DateTime.Now;

                // Set default values for NOT NULL columns
                if (customer.BiometricFingerprintTemplateFormat == 0)
                    customer.BiometricFingerprintTemplateFormat = 0;

                if (customer.BiometricFingerVeinTemplateFormat == 0)
                    customer.BiometricFingerVeinTemplateFormat = 0;

                // Generate system-generated serial number in format S0001, S0002, etc.
                if (customer.SerialNumber == 0)
                    customer.SerialNumber = GenerateSerialNumber();

                // DON'T generate Reference1 - user must provide it
                // if (string.IsNullOrEmpty(customer.Reference1))
                //     customer.Reference1 = GenerateReference1();

                if (string.IsNullOrEmpty(customer.Reference2))
                    customer.Reference2 = GenerateReference2();

                if (string.IsNullOrEmpty(customer.Reference3))
                    customer.Reference3 = GenerateReference3();

                // Set default bank name if not provided
                if (string.IsNullOrEmpty(customer.BankName))
                    customer.BankName = "NOT PROVIDED";

                // Branch name can be null/empty

                string query = @"INSERT INTO [swiftFin_Customers] 
                        ([Id], [StationId], [Type], [SerialNumber], 
                         [PersonalIdentificationNumber], [Individual_Type], 
                         [Individual_FirstName], [Individual_LastName], 
                         [Individual_IdentityCardType], [Individual_IdentityCardNumber], 
                         [Individual_IdentityCardSerialNumber], [Individual_PayrollNumbers], 
                         [Individual_Salutation], [Individual_Gender], 
                         [Individual_MaritalStatus], [Individual_Nationality], 
                         [Individual_BirthDate], [Individual_EmploymentDesignation], 
                         [Individual_EmploymentTermsOfService], [Individual_EmploymentDate], 
                         [Individual_Classification], [NonIndividual_Description], 
                         [NonIndividual_RegistrationNumber], [NonIndividual_RegistrationSerialNumber], 
                         [NonIndividual_DateEstablished], [Address_AddressLine1], 
                         [Address_AddressLine2], [Address_Street], 
                         [Address_PostalCode], [Address_City], 
                         [Address_Email], [Address_LandLine], 
                         [Address_MobileLine], [PassportImageId], [SignatureImageId], 
                         [IdentityCardFrontSideImageId], [IdentityCardBackSideImageId], 
                         [BiometricFingerprintImageId], [BiometricFingerprintTemplateId], 
                         [BiometricFingerprintTemplateFormat], [BiometricFingerVeinTemplateId], 
                         [BiometricFingerVeinTemplateFormat], [RegistrationDate], 
                         [Reference1], [Reference2], [Reference3], [Remarks], 
                         [IsDefaulter], [IsLocked], [InhibitGuaranteeing], 
                         [RecruitedBy], [RecordStatus], [ModifiedBy], 
                         [ModifiedDate], [AdministrativeDivisionId], 
                         [BankName], [BranchName],
                         [CreatedBy], [CreatedDate])
                        VALUES 
                        (@Id, @StationId, @Type, @SerialNumber, 
                         @PersonalIdentificationNumber, @Individual_Type, 
                         @Individual_FirstName, @Individual_LastName, 
                         @Individual_IdentityCardType, @Individual_IdentityCardNumber, 
                         @Individual_IdentityCardSerialNumber, @Individual_PayrollNumbers, 
                         @Individual_Salutation, @Individual_Gender, 
                         @Individual_MaritalStatus, @Individual_Nationality, 
                         @Individual_BirthDate, @Individual_EmploymentDesignation, 
                         @Individual_EmploymentTermsOfService, @Individual_EmploymentDate, 
                         @Individual_Classification, @NonIndividual_Description, 
                         @NonIndividual_RegistrationNumber, @NonIndividual_RegistrationSerialNumber, 
                         @NonIndividual_DateEstablished, @Address_AddressLine1, 
                         @Address_AddressLine2, @Address_Street, 
                         @Address_PostalCode, @Address_City, 
                         @Address_Email, @Address_LandLine, 
                         @Address_MobileLine, @PassportImageId, @SignatureImageId, 
                         @IdentityCardFrontSideImageId, @IdentityCardBackSideImageId, 
                         @BiometricFingerprintImageId, @BiometricFingerprintTemplateId, 
                         @BiometricFingerprintTemplateFormat, @BiometricFingerVeinTemplateId, 
                         @BiometricFingerVeinTemplateFormat, @RegistrationDate, 
                         @Reference1, @Reference2, @Reference3, @Remarks, 
                         @IsDefaulter, @IsLocked, @InhibitGuaranteeing, 
                         @RecruitedBy, @RecordStatus, @ModifiedBy, 
                         @ModifiedDate, @AdministrativeDivisionId, 
                         @BankName, @BranchName,
                         @CreatedBy, @CreatedDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, customer);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Return the created customer with all details
            return GetById(customer.Id);
        }

        public void Update(CustomerDTO customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Check if customer exists
            var existingCustomer = GetById(customer.Id);
            if (existingCustomer == null)
                throw new KeyNotFoundException($"Customer with ID {customer.Id} not found");

            // Validate duplicate identity/registration numbers (excluding current customer)
            ValidateDuplicateForUpdate(customer);

            // FIXED: Validate dates before setting ModifiedDate
            if (!customer.ModifiedDate.HasValue || customer.ModifiedDate.Value < SqlDateTime.MinValue.Value)
                customer.ModifiedDate = DateTime.Now;

            // FIXED: Validate all dates to ensure they're not too early for SQL Server
            ValidateDates(customer);

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [swiftFin_Customers]
                SET [StationId] = @StationId,
                    [Type] = @Type,
                    [PersonalIdentificationNumber] = @PersonalIdentificationNumber,
                    [Individual_Type] = @Individual_Type,
                    [Individual_FirstName] = @Individual_FirstName,
                    [Individual_LastName] = @Individual_LastName,
                    [Individual_IdentityCardType] = @Individual_IdentityCardType,
                    [Individual_IdentityCardNumber] = @Individual_IdentityCardNumber,
                    [Individual_IdentityCardSerialNumber] = @Individual_IdentityCardSerialNumber,
                    [Individual_PayrollNumbers] = @Individual_PayrollNumbers,
                    [Individual_Salutation] = @Individual_Salutation,
                    [Individual_Gender] = @Individual_Gender,
                    [Individual_MaritalStatus] = @Individual_MaritalStatus,
                    [Individual_Nationality] = @Individual_Nationality,
                    [Individual_BirthDate] = @Individual_BirthDate,
                    [Individual_EmploymentDesignation] = @Individual_EmploymentDesignation,
                    [Individual_EmploymentTermsOfService] = @Individual_EmploymentTermsOfService,
                    [Individual_EmploymentDate] = @Individual_EmploymentDate,
                    [Individual_Classification] = @Individual_Classification,
                    [NonIndividual_Description] = @NonIndividual_Description,
                    [NonIndividual_RegistrationNumber] = @NonIndividual_RegistrationNumber,
                    [NonIndividual_RegistrationSerialNumber] = @NonIndividual_RegistrationSerialNumber,
                    [NonIndividual_DateEstablished] = @NonIndividual_DateEstablished,
                    [Address_AddressLine1] = @Address_AddressLine1,
                    [Address_AddressLine2] = @Address_AddressLine2,
                    [Address_Street] = @Address_Street,
                    [Address_PostalCode] = @Address_PostalCode,
                    [Address_City] = @Address_City,
                    [Address_Email] = @Address_Email,
                    [Address_LandLine] = @Address_LandLine,
                    [Address_MobileLine] = @Address_MobileLine,
                    [PassportImageId] = @PassportImageId,
                    [SignatureImageId] = @SignatureImageId,
                    [IdentityCardFrontSideImageId] = @IdentityCardFrontSideImageId,
                    [IdentityCardBackSideImageId] = @IdentityCardBackSideImageId,
                    [BiometricFingerprintImageId] = @BiometricFingerprintImageId,
                    [BiometricFingerprintTemplateId] = @BiometricFingerprintTemplateId,
                    [BiometricFingerprintTemplateFormat] = @BiometricFingerprintTemplateFormat,
                    [BiometricFingerVeinTemplateId] = @BiometricFingerVeinTemplateId,
                    [BiometricFingerVeinTemplateFormat] = @BiometricFingerVeinTemplateFormat,
                    [RegistrationDate] = @RegistrationDate,
                    [Reference1] = @Reference1,
                    [Remarks] = @Remarks,
                    [IsDefaulter] = @IsDefaulter,
                    [IsLocked] = @IsLocked,
                    [InhibitGuaranteeing] = @InhibitGuaranteeing,
                    [RecruitedBy] = @RecruitedBy,
                    [RecordStatus] = @RecordStatus,
                    [ModifiedBy] = @ModifiedBy,
                    [ModifiedDate] = @ModifiedDate,
                    [AdministrativeDivisionId] = @AdministrativeDivisionId,
                    [BankName] = @BankName,
                    [BranchName] = @BranchName
                WHERE [Id] = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddUpdateParams(cmd, customer);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new KeyNotFoundException($"Customer with ID {customer.Id} not found");
                }
            }
        }
        // NEW: Add this helper method to validate dates
        private void ValidateDates(CustomerDTO customer)
        {
            // SQL Server datetime minimum value is 1753-01-01
            DateTime sqlMinDate = SqlDateTime.MinValue.Value;

            // Check and adjust dates that are too early
            if (customer.IndividualBirthDate.HasValue && customer.IndividualBirthDate.Value < sqlMinDate)
                customer.IndividualBirthDate = null;

            if (customer.IndividualEmploymentDate.HasValue && customer.IndividualEmploymentDate.Value < sqlMinDate)
                customer.IndividualEmploymentDate = null;

            if (customer.NonIndividualDateEstablished.HasValue && customer.NonIndividualDateEstablished.Value < sqlMinDate)
                customer.NonIndividualDateEstablished = null;

            if (customer.RegistrationDate.HasValue && customer.RegistrationDate.Value < sqlMinDate)
                customer.RegistrationDate = null;

            if (customer.ModifiedDate.HasValue && customer.ModifiedDate.Value < sqlMinDate)
                customer.ModifiedDate = DateTime.Now;
        }

        private void ValidateDuplicateForUpdate(CustomerDTO customer)
        {
            // For Individual customers - check duplicate ID card number (excluding current customer)
            if (customer.Type == 1 && !string.IsNullOrEmpty(customer.IndividualIdentityCardNumber))
            {
                var existingIndividual = GetByIdentityCardNumber(customer.IndividualIdentityCardNumber);
                if (existingIndividual != null && existingIndividual.Id != customer.Id)
                {
                    throw new InvalidOperationException(
                        $"Another individual customer already exists with ID Card Number '{customer.IndividualIdentityCardNumber}'. " +
                        $"Existing customer: {existingIndividual.IndividualFirstName} {existingIndividual.IndividualLastName} (ID: {existingIndividual.Id})");
                }
            }

            // For Corporate/Partnership customers - check duplicate registration number (excluding current customer)
            if ((customer.Type == 3 || customer.Type == 2 || customer.Type == 4) &&
                !string.IsNullOrEmpty(customer.NonIndividualRegistrationNumber))
            {
                var existingCorporate = GetByRegistrationNumber(customer.NonIndividualRegistrationNumber);
                if (existingCorporate != null && existingCorporate.Id != customer.Id)
                {
                    throw new InvalidOperationException(
                        $"Another corporate customer already exists with Registration Number '{customer.NonIndividualRegistrationNumber}'. " +
                        $"Existing customer: {existingCorporate.NonIndividualDescription} (ID: {existingCorporate.Id})");
                }
            }
        }


        private void AddUpdateParams(SqlCommand cmd, CustomerDTO customer)
        {
            // ID (required for WHERE clause)
            cmd.Parameters.AddWithValue("@Id", customer.Id);

            // Basic info
            cmd.Parameters.AddWithValue("@StationId", customer.StationId.HasValue ? (object)customer.StationId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Type", customer.Type);
            cmd.Parameters.AddWithValue("@PersonalIdentificationNumber", customer.PersonalIdentificationNumber ?? string.Empty);

            // Individual fields
            cmd.Parameters.AddWithValue("@Individual_Type", customer.IndividualType);
            cmd.Parameters.AddWithValue("@Individual_FirstName", customer.IndividualFirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@Individual_LastName", customer.IndividualLastName ?? string.Empty);
            cmd.Parameters.AddWithValue("@Individual_IdentityCardType", customer.IndividualIdentityCardType);
            cmd.Parameters.AddWithValue("@Individual_IdentityCardNumber", customer.IndividualIdentityCardNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@Individual_IdentityCardSerialNumber", customer.IndividualIdentityCardSerialNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@Individual_PayrollNumbers", customer.IndividualPayrollNumbers ?? string.Empty);
            cmd.Parameters.AddWithValue("@Individual_Salutation", customer.IndividualSalutation);
            cmd.Parameters.AddWithValue("@Individual_Gender", customer.IndividualGender);
            cmd.Parameters.AddWithValue("@Individual_MaritalStatus", customer.IndividualMaritalStatus);
            cmd.Parameters.AddWithValue("@Individual_Nationality", customer.IndividualNationality);

            // FIXED: Handle date properly with SqlDateTime.MinValue check
            cmd.Parameters.AddWithValue("@Individual_BirthDate",
                customer.IndividualBirthDate.HasValue &&
                customer.IndividualBirthDate.Value >= SqlDateTime.MinValue.Value
                    ? (object)customer.IndividualBirthDate.Value
                    : DBNull.Value);

            cmd.Parameters.AddWithValue("@Individual_EmploymentDesignation", customer.IndividualEmploymentDesignation ?? string.Empty);

            // FIXED: Handle nullable byte
            cmd.Parameters.AddWithValue("@Individual_EmploymentTermsOfService",
                customer.IndividualEmploymentTermsOfService.HasValue
                    ? (object)customer.IndividualEmploymentTermsOfService.Value
                    : DBNull.Value);

            // FIXED: Handle date properly
            cmd.Parameters.AddWithValue("@Individual_EmploymentDate",
                customer.IndividualEmploymentDate.HasValue &&
                customer.IndividualEmploymentDate.Value >= SqlDateTime.MinValue.Value
                    ? (object)customer.IndividualEmploymentDate.Value
                    : DBNull.Value);

            cmd.Parameters.AddWithValue("@Individual_Classification", customer.IndividualClassification);

            // Corporate fields
            cmd.Parameters.AddWithValue("@NonIndividual_Description", customer.NonIndividualDescription ?? string.Empty);
            cmd.Parameters.AddWithValue("@NonIndividual_RegistrationNumber", customer.NonIndividualRegistrationNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@NonIndividual_RegistrationSerialNumber", customer.NonIndividualRegistrationSerialNumber ?? string.Empty);

            // FIXED: Handle date properly
            cmd.Parameters.AddWithValue("@NonIndividual_DateEstablished",
                customer.NonIndividualDateEstablished.HasValue &&
                customer.NonIndividualDateEstablished.Value >= SqlDateTime.MinValue.Value
                    ? (object)customer.NonIndividualDateEstablished.Value
                    : DBNull.Value);

            // Address fields
            cmd.Parameters.AddWithValue("@Address_AddressLine1", customer.AddressAddressLine1 ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_AddressLine2", customer.AddressAddressLine2 ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_Street", customer.AddressStreet ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_PostalCode", customer.AddressPostalCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_City", customer.AddressCity ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_Email", customer.AddressEmail ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_LandLine", customer.AddressLandLine ?? string.Empty);
            cmd.Parameters.AddWithValue("@Address_MobileLine", customer.AddressMobileLine ?? string.Empty);

            // Image fields
            cmd.Parameters.AddWithValue("@PassportImageId", customer.PassportImageId.HasValue ? (object)customer.PassportImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@SignatureImageId", customer.SignatureImageId.HasValue ? (object)customer.SignatureImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IdentityCardFrontSideImageId", customer.IdentityCardFrontSideImageId.HasValue ? (object)customer.IdentityCardFrontSideImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IdentityCardBackSideImageId", customer.IdentityCardBackSideImageId.HasValue ? (object)customer.IdentityCardBackSideImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerprintImageId", customer.BiometricFingerprintImageId.HasValue ? (object)customer.BiometricFingerprintImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerprintTemplateId", customer.BiometricFingerprintTemplateId.HasValue ? (object)customer.BiometricFingerprintTemplateId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerprintTemplateFormat", customer.BiometricFingerprintTemplateFormat);
            cmd.Parameters.AddWithValue("@BiometricFingerVeinTemplateId", customer.BiometricFingerVeinTemplateId.HasValue ? (object)customer.BiometricFingerVeinTemplateId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerVeinTemplateFormat", customer.BiometricFingerVeinTemplateFormat);

            // FIXED: Handle RegistrationDate properly
            cmd.Parameters.AddWithValue("@RegistrationDate",
                customer.RegistrationDate.HasValue &&
                customer.RegistrationDate.Value >= SqlDateTime.MinValue.Value
                    ? (object)customer.RegistrationDate.Value
                    : DBNull.Value);

            // References (only Reference1 can be updated)
            cmd.Parameters.AddWithValue("@Reference1", customer.Reference1 ?? string.Empty);

            // Status fields
            cmd.Parameters.AddWithValue("@Remarks", customer.Remarks ?? string.Empty);
            cmd.Parameters.AddWithValue("@IsDefaulter", customer.IsDefaulter);
            cmd.Parameters.AddWithValue("@IsLocked", customer.IsLocked);
            cmd.Parameters.AddWithValue("@InhibitGuaranteeing", customer.InhibitGuaranteeing);
            cmd.Parameters.AddWithValue("@RecruitedBy", customer.RecruitedBy ?? string.Empty);
            cmd.Parameters.AddWithValue("@RecordStatus", customer.RecordStatus);
            cmd.Parameters.AddWithValue("@ModifiedBy", customer.ModifiedBy ?? string.Empty);

            // FIXED: Handle ModifiedDate properly
            cmd.Parameters.AddWithValue("@ModifiedDate",
                customer.ModifiedDate.HasValue &&
                customer.ModifiedDate.Value >= SqlDateTime.MinValue.Value
                    ? (object)customer.ModifiedDate.Value
                    : (object)DateTime.Now);

            // Administrative
            cmd.Parameters.AddWithValue("@AdministrativeDivisionId", customer.AdministrativeDivisionId.HasValue ? (object)customer.AdministrativeDivisionId.Value : DBNull.Value);

            // Bank fields
            cmd.Parameters.AddWithValue("@BankName", customer.BankName ?? "NOT PROVIDED");
            cmd.Parameters.AddWithValue("@BranchName", customer.BranchName ?? string.Empty);
        }

        public void Delete(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_Customers] WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public CustomerDTO GetByReference(string reference1, string reference2, string reference3)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT TOP 1 
                c.*,
                b.Description as BranchDescription,
                b.Address_Email as BranchAddressEmail,
                comp.Description as BranchCompanyDescription
            FROM [swiftFin_Customers] c
            LEFT JOIN [swiftFin_Branches] b ON c.BranchId = b.Id
            LEFT JOIN [swiftFin_Companies] comp ON b.CompanyId = comp.Id
            WHERE (@Reference1 IS NOT NULL AND @Reference1 != '' AND c.Reference1 = @Reference1)
               OR (@Reference2 IS NOT NULL AND @Reference2 != '' AND c.Reference2 = @Reference2)
               OR (@Reference3 IS NOT NULL AND @Reference3 != '' AND c.Reference3 = @Reference3)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Reference1", reference1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference2", reference2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference3", reference3 ?? (object)DBNull.Value);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapWithBranchDetails(reader);
                    }
                }
            }
            return null;
        }

        private CustomerDTO MapWithBranchDetails(IDataReader reader)
        {
            var customer = Map(reader); // Use your existing Map method

            // Add branch details
            customer.BranchDescription = reader["BranchDescription"]?.ToString();
            customer.BranchAddressEmail = reader["BranchAddressEmail"]?.ToString();
            customer.BranchCompanyDescription = reader["BranchCompanyDescription"]?.ToString();

            return customer;
        }



        private string GenerateReference1()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT MAX(TRY_CAST(SUBSTRING(Reference1, 4, LEN(Reference1)) AS INT)) FROM [swiftFin_Customers] WHERE Reference1 LIKE 'ACC%'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                    return $"ACC{nextNumber:D4}"; // Format as ACC0001, ACC0002, etc.
                }
            }
        }

        private string GenerateReference2()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT MAX(TRY_CAST(Reference2 AS INT)) FROM [swiftFin_Customers] WHERE ISNUMERIC(Reference2) = 1";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                    return $"{nextNumber:D4}"; // Format as 0001, 0002, etc.
                }
            }
        }

        private string GenerateReference3()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT MAX(TRY_CAST(SUBSTRING(Reference3, 3, LEN(Reference3)) AS INT)) FROM [swiftFin_Customers] WHERE Reference3 LIKE 'PF%'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                    return $"PF{nextNumber:D4}"; // Format as PF0001, PF0002, etc.
                }
            }
        }

        private int GenerateSerialNumber()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // Get the maximum serial number and convert it to the next number
                string query = "SELECT MAX(SerialNumber) FROM [swiftFin_Customers]";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                    return nextNumber;
                }
            }
        }

        private void AddParams(SqlCommand cmd, CustomerDTO customer)
        {
            cmd.Parameters.AddWithValue("@Id", customer.Id);
            cmd.Parameters.AddWithValue("@StationId", customer.StationId.HasValue ? (object)customer.StationId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Type", customer.Type);
            cmd.Parameters.AddWithValue("@SerialNumber", customer.SerialNumber);
            cmd.Parameters.AddWithValue("@PersonalIdentificationNumber", customer.PersonalIdentificationNumber ?? "");
            cmd.Parameters.AddWithValue("@Individual_Type", customer.IndividualType);
            cmd.Parameters.AddWithValue("@Individual_FirstName", customer.IndividualFirstName ?? "");
            cmd.Parameters.AddWithValue("@Individual_LastName", customer.IndividualLastName ?? "");
            cmd.Parameters.AddWithValue("@Individual_IdentityCardType", customer.IndividualIdentityCardType);
            cmd.Parameters.AddWithValue("@Individual_IdentityCardNumber", customer.IndividualIdentityCardNumber ?? "");
            cmd.Parameters.AddWithValue("@Individual_IdentityCardSerialNumber", customer.IndividualIdentityCardSerialNumber ?? "");
            cmd.Parameters.AddWithValue("@Individual_PayrollNumbers", customer.IndividualPayrollNumbers ?? "");
            cmd.Parameters.AddWithValue("@Individual_Salutation", customer.IndividualSalutation);
            cmd.Parameters.AddWithValue("@Individual_Gender", customer.IndividualGender);
            cmd.Parameters.AddWithValue("@Individual_MaritalStatus", customer.IndividualMaritalStatus);
            cmd.Parameters.AddWithValue("@Individual_Nationality", customer.IndividualNationality);
            cmd.Parameters.AddWithValue("@Individual_BirthDate",
                    customer.IndividualBirthDate.HasValue &&
                    customer.IndividualBirthDate.Value >= SqlDateTime.MinValue.Value
                        ? (object)customer.IndividualBirthDate.Value
                        : DBNull.Value);
            cmd.Parameters.AddWithValue("@Individual_EmploymentDesignation", customer.IndividualEmploymentDesignation ?? "");
            cmd.Parameters.AddWithValue("@Individual_EmploymentTermsOfService",
                    customer.IndividualEmploymentTermsOfService.HasValue
                        ? (object)customer.IndividualEmploymentTermsOfService.Value
                        : DBNull.Value);
            cmd.Parameters.AddWithValue("@Individual_EmploymentDate",
                    customer.IndividualEmploymentDate.HasValue &&
                    customer.IndividualEmploymentDate.Value >= SqlDateTime.MinValue.Value
                        ? (object)customer.IndividualEmploymentDate.Value
                        : DBNull.Value);
            cmd.Parameters.AddWithValue("@Individual_Classification", customer.IndividualClassification);
            cmd.Parameters.AddWithValue("@NonIndividual_Description", customer.NonIndividualDescription ?? "");
            cmd.Parameters.AddWithValue("@NonIndividual_RegistrationNumber", customer.NonIndividualRegistrationNumber ?? "");
            cmd.Parameters.AddWithValue("@NonIndividual_RegistrationSerialNumber", customer.NonIndividualRegistrationSerialNumber ?? "");
            cmd.Parameters.AddWithValue("@NonIndividual_DateEstablished",
                   customer.NonIndividualDateEstablished.HasValue &&
                   customer.NonIndividualDateEstablished.Value >= SqlDateTime.MinValue.Value
                       ? (object)customer.NonIndividualDateEstablished.Value
                       : DBNull.Value);
            cmd.Parameters.AddWithValue("@Address_AddressLine1", customer.AddressAddressLine1 ?? "");
            cmd.Parameters.AddWithValue("@Address_AddressLine2", customer.AddressAddressLine2 ?? "");
            cmd.Parameters.AddWithValue("@Address_Street", customer.AddressStreet ?? "");
            cmd.Parameters.AddWithValue("@Address_PostalCode", customer.AddressPostalCode ?? "");
            cmd.Parameters.AddWithValue("@Address_City", customer.AddressCity ?? "");
            cmd.Parameters.AddWithValue("@Address_Email", customer.AddressEmail ?? "");
            cmd.Parameters.AddWithValue("@Address_LandLine", customer.AddressLandLine ?? "");
            cmd.Parameters.AddWithValue("@Address_MobileLine", customer.AddressMobileLine ?? "");
            cmd.Parameters.AddWithValue("@PassportImageId", customer.PassportImageId.HasValue ? (object)customer.PassportImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@SignatureImageId", customer.SignatureImageId.HasValue ? (object)customer.SignatureImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IdentityCardFrontSideImageId", customer.IdentityCardFrontSideImageId.HasValue ? (object)customer.IdentityCardFrontSideImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IdentityCardBackSideImageId", customer.IdentityCardBackSideImageId.HasValue ? (object)customer.IdentityCardBackSideImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerprintImageId", customer.BiometricFingerprintImageId.HasValue ? (object)customer.BiometricFingerprintImageId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerprintTemplateId", customer.BiometricFingerprintTemplateId.HasValue ? (object)customer.BiometricFingerprintTemplateId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerprintTemplateFormat", customer.BiometricFingerprintTemplateFormat);
            cmd.Parameters.AddWithValue("@BiometricFingerVeinTemplateId", customer.BiometricFingerVeinTemplateId.HasValue ? (object)customer.BiometricFingerVeinTemplateId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BiometricFingerVeinTemplateFormat", customer.BiometricFingerVeinTemplateFormat);
            cmd.Parameters.AddWithValue("@RegistrationDate",
                    customer.RegistrationDate.HasValue &&
                    customer.RegistrationDate.Value >= SqlDateTime.MinValue.Value
                        ? (object)customer.RegistrationDate.Value
                        : DBNull.Value);
            cmd.Parameters.AddWithValue("@Reference1", customer.Reference1 ?? "");
            cmd.Parameters.AddWithValue("@Reference2", customer.Reference2 ?? "");
            cmd.Parameters.AddWithValue("@Reference3", customer.Reference3 ?? "");
            cmd.Parameters.AddWithValue("@Remarks", customer.Remarks ?? "");
            cmd.Parameters.AddWithValue("@IsDefaulter", customer.IsDefaulter);
            cmd.Parameters.AddWithValue("@IsLocked", customer.IsLocked);
            cmd.Parameters.AddWithValue("@InhibitGuaranteeing", customer.InhibitGuaranteeing);
            cmd.Parameters.AddWithValue("@RecruitedBy", customer.RecruitedBy ?? "");
            cmd.Parameters.AddWithValue("@RecordStatus", customer.RecordStatus);
            cmd.Parameters.AddWithValue("@ModifiedBy", customer.ModifiedBy ?? "");
            cmd.Parameters.AddWithValue("@ModifiedDate",
                   customer.ModifiedDate.HasValue &&
                   customer.ModifiedDate.Value >= SqlDateTime.MinValue.Value
                       ? (object)customer.ModifiedDate.Value
                       : DBNull.Value);
            cmd.Parameters.AddWithValue("@AdministrativeDivisionId", customer.AdministrativeDivisionId.HasValue ? (object)customer.AdministrativeDivisionId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@BankName", customer.BankName ?? "NOT PROVIDED");
            cmd.Parameters.AddWithValue("@BranchName", customer.BranchName ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", customer.CreatedBy ?? "");
            cmd.Parameters.AddWithValue("@CreatedDate", customer.CreatedDate);
        }

        private CustomerDTO Map(IDataReader reader)
        {
            return new CustomerDTO
            {
                Id = (Guid)reader["Id"],
                BranchId = Guid.Empty,
                Type = Convert.ToByte(reader["Type"]),
                SerialNumber = Convert.ToInt32(reader["SerialNumber"]),
                PersonalIdentificationNumber = reader["PersonalIdentificationNumber"]?.ToString(),
                IndividualType = Convert.ToByte(reader["Individual_Type"]),
                IndividualFirstName = reader["Individual_FirstName"]?.ToString(),
                IndividualLastName = reader["Individual_LastName"]?.ToString(),
                IndividualIdentityCardType = Convert.ToByte(reader["Individual_IdentityCardType"]),
                IndividualIdentityCardNumber = reader["Individual_IdentityCardNumber"]?.ToString(),
                IndividualIdentityCardSerialNumber = reader["Individual_IdentityCardSerialNumber"]?.ToString(),
                IndividualPayrollNumbers = reader["Individual_PayrollNumbers"]?.ToString(),
                IndividualSalutation = Convert.ToByte(reader["Individual_Salutation"]),
                IndividualGender = Convert.ToByte(reader["Individual_Gender"]),
                IndividualMaritalStatus = Convert.ToByte(reader["Individual_MaritalStatus"]),
                IndividualNationality = Convert.ToByte(reader["Individual_Nationality"]),
                IndividualBirthDate = reader["Individual_BirthDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Individual_BirthDate"]),
                IndividualEmploymentDesignation = reader["Individual_EmploymentDesignation"]?.ToString(),
                IndividualEmploymentTermsOfService = reader["Individual_EmploymentTermsOfService"] == DBNull.Value ? (byte?)null : Convert.ToByte(reader["Individual_EmploymentTermsOfService"]),
                IndividualEmploymentDate = reader["Individual_EmploymentDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Individual_EmploymentDate"]),
                IndividualClassification = Convert.ToByte(reader["Individual_Classification"]),
                NonIndividualDescription = reader["NonIndividual_Description"]?.ToString(),
                NonIndividualRegistrationNumber = reader["NonIndividual_RegistrationNumber"]?.ToString(),
                NonIndividualRegistrationSerialNumber = reader["NonIndividual_RegistrationSerialNumber"]?.ToString(),
                NonIndividualDateEstablished = reader["NonIndividual_DateEstablished"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NonIndividual_DateEstablished"]),
                AddressAddressLine1 = reader["Address_AddressLine1"]?.ToString(),
                AddressAddressLine2 = reader["Address_AddressLine2"]?.ToString(),
                AddressStreet = reader["Address_Street"]?.ToString(),
                AddressPostalCode = reader["Address_PostalCode"]?.ToString(),
                AddressCity = reader["Address_City"]?.ToString(),
                AddressEmail = reader["Address_Email"]?.ToString(),
                AddressLandLine = reader["Address_LandLine"]?.ToString(),
                AddressMobileLine = reader["Address_MobileLine"]?.ToString(),
                StationId = reader["StationId"] == DBNull.Value ? (Guid?)null : (Guid)reader["StationId"],
                PassportImageId = reader["PassportImageId"] == DBNull.Value ? (Guid?)null : (Guid)reader["PassportImageId"],
                SignatureImageId = reader["SignatureImageId"] == DBNull.Value ? (Guid?)null : (Guid)reader["SignatureImageId"],
                IdentityCardFrontSideImageId = reader["IdentityCardFrontSideImageId"] == DBNull.Value ? (Guid?)null : (Guid)reader["IdentityCardFrontSideImageId"],
                IdentityCardBackSideImageId = reader["IdentityCardBackSideImageId"] == DBNull.Value ? (Guid?)null : (Guid)reader["IdentityCardBackSideImageId"],
                BiometricFingerprintImageId = reader["BiometricFingerprintImageId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BiometricFingerprintImageId"],
                BiometricFingerprintTemplateId = reader["BiometricFingerprintTemplateId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BiometricFingerprintTemplateId"],
                BiometricFingerprintTemplateFormat = Convert.ToByte(reader["BiometricFingerprintTemplateFormat"]),
                BiometricFingerVeinTemplateId = reader["BiometricFingerVeinTemplateId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BiometricFingerVeinTemplateId"],
                BiometricFingerVeinTemplateFormat = Convert.ToByte(reader["BiometricFingerVeinTemplateFormat"]),
                RegistrationDate = reader["RegistrationDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["RegistrationDate"]),
                Reference1 = reader["Reference1"]?.ToString(),
                Reference2 = reader["Reference2"]?.ToString(),
                Reference3 = reader["Reference3"]?.ToString(),
                Remarks = reader["Remarks"]?.ToString(),
                IsDefaulter = Convert.ToBoolean(reader["IsDefaulter"]),
                IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                InhibitGuaranteeing = Convert.ToBoolean(reader["InhibitGuaranteeing"]),
                RecruitedBy = reader["RecruitedBy"]?.ToString(),
                RecordStatus = Convert.ToByte(reader["RecordStatus"]),
                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ModifiedDate"]),
                AdministrativeDivisionId = reader["AdministrativeDivisionId"] == DBNull.Value ? (Guid?)null : (Guid)reader["AdministrativeDivisionId"],
                // NEW BANK COLUMNS
                BankName = reader["BankName"]?.ToString(),
                BranchName = reader["BranchName"]?.ToString(),
                CreatedBy = reader["CreatedBy"]?.ToString(),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }


        public IEnumerable<CustomerDTO> SearchByIndividualIdentityCardNumber(
string identityCardNumber,
bool exactMatch = false)
        {
            var list = new List<CustomerDTO>();

            if (string.IsNullOrWhiteSpace(identityCardNumber))
                return list;

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = exactMatch
                    ? @"SELECT c.*
            FROM [swiftFin_Customers] c
            WHERE c.Type = 1
              AND c.Individual_IdentityCardNumber = @IdentityCardNumber
            ORDER BY c.SerialNumber DESC"
                    : @"SELECT c.*
            FROM [swiftFin_Customers] c
            WHERE c.Type = 1
              AND c.Individual_IdentityCardNumber LIKE @IdentityCardNumber
            ORDER BY c.SerialNumber DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdentityCardNumber",
                        exactMatch
                            ? identityCardNumber
                            : "%" + identityCardNumber.Trim() + "%"
                    );

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(Map(reader));
                    }
                }
            }

            return list;
        }

    }
}