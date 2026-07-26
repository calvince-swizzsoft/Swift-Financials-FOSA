using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Application.MainBoundedContext.Services
{
    public class SqlCommandAppService : ISqlCommandAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Holiday> _repository;
        private readonly IRepository<EmailAlert> _emailAlertRepository;
        private readonly IRepository<TextAlert> _textAlertRepository;

        public SqlCommandAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Holiday> repository,
            IRepository<EmailAlert> emailAlertRepository, IRepository<TextAlert> textAlertRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _repository = repository;
            _emailAlertRepository = emailAlertRepository ?? throw new ArgumentNullException(nameof(emailAlertRepository));
            _textAlertRepository = textAlertRepository ?? throw new ArgumentNullException(nameof(textAlertRepository));
        }

        public List<CustomerDTO> FindCustomersByPayrollNumber(string payrollNumber, ServiceHeader serviceHeader)
        {
            var customers = new List<CustomerDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerBulkCopyDTO>("EXEC sp_FindCustomerByPayrollNumber @pIndividual_PayrollNumber", serviceHeader,
                    new SqlParameter("pIndividual_PayrollNumber", payrollNumber));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customer = new CustomerDTO
                        {
                            Id = item.Id,
                            Type = item.Type,
                            SerialNumber = item.SerialNumber,
                            PersonalIdentificationNumber = item.PersonalIdentificationNumber,
                            IndividualFirstName = item.Individual_FirstName,
                            IndividualLastName = item.Individual_LastName,
                            IndividualIdentityCardNumber = item.Individual_IdentityCardNumber,
                            IndividualPayrollNumbers = item.Individual_PayrollNumbers,
                            IndividualSalutation = item.Individual_Salutation,
                            IndividualGender = item.Individual_Gender,
                            IndividualMaritalStatus = item.Individual_MaritalStatus,
                            IndividualNationality = item.Individual_Nationality,
                            IndividualBirthDate = item.Individual_BirthDate,
                            IndividualEmploymentDesignation = item.Individual_EmploymentDesignation,
                            IndividualEmploymentTermsOfService = item.Individual_EmploymentTermsOfService.HasValue ? item.Individual_EmploymentTermsOfService : 0,
                            IndividualEmploymentDate = item.Individual_EmploymentDate,
                            NonIndividualDescription = item.NonIndividual_Description,
                            NonIndividualRegistrationNumber = item.NonIndividual_RegistrationNumber,
                            NonIndividualDateEstablished = item.NonIndividual_DateEstablished,
                            AddressAddressLine1 = item.Address_AddressLine1,
                            AddressAddressLine2 = item.Address_AddressLine2,
                            AddressStreet = item.Address_Street,
                            AddressPostalCode = item.Address_PostalCode,
                            AddressCity = item.Address_City,
                            AddressEmail = item.Address_Email,
                            AddressLandLine = item.Address_LandLine,
                            AddressMobileLine = item.Address_MobileLine,
                            PassportImageId = item.PassportImageId,
                            SignatureImageId = item.SignatureImageId,
                            IdentityCardFrontSideImageId = item.IdentityCardFrontSideImageId,
                            IdentityCardBackSideImageId = item.IdentityCardBackSideImageId,
                            BiometricFingerprintImageId = item.BiometricFingerprintImageId,
                            BiometricFingerprintTemplateId = item.BiometricFingerprintTemplateId,
                            StationId = item.StationId,
                            Reference1 = item.Reference1,
                            Reference2 = item.Reference2,
                            Reference3 = item.Reference3,
                            Remarks = item.Remarks,
                            IsLocked = item.IsLocked,
                            RegistrationDate = item.RegistrationDate,
                            RecruitedBy = item.RecruitedBy,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate
                        };

                        customers.Add(customer);
                    }
                }
            }

            return customers;
        }

        public CustomerAccountDTO FindCustomerAccountById(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            CustomerAccountDTO customerAccount = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerAccountById @pCustomerAccountId", serviceHeader,
                    new SqlParameter("pCustomerAccountId", customerAccountId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        break;
                    }
                }
            }

            return customerAccount;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByTargetProductIdAndReference3(Guid targetProductId, string reference3, ServiceHeader serviceHeader)
        {
            serviceHeader = new ServiceHeader
            {
                ApplicationUserName = "User",
                ApplicationDomainName = "Domain",
                EnvironmentUserName = "System",
                EnvironmentProcessorId = "System",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMACAddress = "000000",
                EnvironmentMotherboardSerialNumber = "000000",
                EnvironmentDomainName = Environment.UserDomainName,
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentIPAddress = HttpContext.Current?.Request?.UserHostAddress ?? "127.0.0.1"
            };
            var customerAccounts = new List<CustomerAccountDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerAccountByTargetProductIdAndReference3 @pCustomerAccountType_TargetProductId, @pReference3", serviceHeader,
                    new SqlParameter("pCustomerAccountType_TargetProductId", targetProductId),
                    new SqlParameter("pReference3", reference3));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        customerAccounts.Add(customerAccount);
                    }
                }
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByTargetProductIdAndPayrollNumber(Guid targetProductId, string payrollNumber, ServiceHeader serviceHeader)
        {
            var customerAccounts = new List<CustomerAccountDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerAccountByTargetProductIdAndPayrollNumber @pCustomerAccountType_TargetProductId, @pIndividual_PayrollNumber", serviceHeader,
                    new SqlParameter("pCustomerAccountType_TargetProductId", targetProductId),
                    new SqlParameter("pIndividual_PayrollNumber", payrollNumber));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        customerAccounts.Add(customerAccount);
                    }
                }
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByTargetProductId(Guid targetProductId, ServiceHeader serviceHeader)
        {
            List<CustomerAccountDTO> customerAccounts = new List<CustomerAccountDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerAccountByTargetProductId @pCustomerAccountType_TargetProductId", serviceHeader,
                    new SqlParameter("pCustomerAccountType_TargetProductId", targetProductId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        customerAccounts.Add(customerAccount);
                    }
                }
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsGivenEmployerAndLoanProduct(Guid employerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            List<CustomerAccountDTO> customerAccounts = new List<CustomerAccountDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerLoanAccountsGivenEmployerAndLoanProduct @EmployerID, @LoanProductID", serviceHeader,
                    new SqlParameter("EmployerID", employerId),
                    new SqlParameter("LoanProductID", loanProductId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        customerAccounts.Add(customerAccount);
                    }
                }
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsGivenCreditTypeAndLoanProduct(Guid creditTypeId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            List<CustomerAccountDTO> customerAccounts = new List<CustomerAccountDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerLoanAccountsGivenCreditTypeAndLoanProduct @CreditTypeID, @LoanProductID", serviceHeader,
                    new SqlParameter("CreditTypeID", creditTypeId),
                    new SqlParameter("LoanProductID", loanProductId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        customerAccounts.Add(customerAccount);
                    }
                }
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByTargetProductIdAndCustomerId(Guid targetProductId, Guid customerId, ServiceHeader serviceHeader)
        {
            List<CustomerAccountDTO> customerAccounts = new List<CustomerAccountDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CustomerAccountBulkCopyDTO>("EXEC sp_FindCustomerAccountByTargetProductIdAndCustomerId @pCustomerAccountType_TargetProductId, @pCustomerId", serviceHeader,
                    new SqlParameter("pCustomerAccountType_TargetProductId", targetProductId),
                    new SqlParameter("pCustomerId", customerId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var customerAccount = new CustomerAccountDTO
                        {
                            Id = item.Id,
                            CustomerId = item.CustomerId,
                            BranchId = item.BranchId,
                            CustomerAccountTypeProductCode = item.CustomerAccountType_ProductCode,
                            CustomerAccountTypeTargetProductId = item.CustomerAccountType_TargetProductId,
                            CustomerAccountTypeTargetProductCode = item.CustomerAccountType_ProductCode,
                            Remarks = item.Remarks,
                            Status = item.Status,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ScoredLoanDisbursementProductCode = item.ScoredLoanDisbursementProductCode,
                            ScoredLoanLimit = item.ScoredLoanLimit,
                            ScoredLoanLimitRemarks = item.ScoredLoanLimitRemarks,
                            ScoredLoanLimitDate = item.ScoredLoanLimitDate,
                        };

                        customerAccounts.Add(customerAccount);
                    }
                }
            }

            return customerAccounts;
        }

        public int DeleteDebitBatchEntries(Guid debitBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteDebitBatchEntries @DebitBatchId", serviceHeader,
                    new SqlParameter("DebitBatchId", debitBatchId));
            }
        }

        public decimal FindCreditBatchEntriesTotal(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            decimal result = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_SumCreditBatchEntries @CreditBatchId", serviceHeader,
                    new SqlParameter("CreditBatchId", creditBatchId));

                if (query != null)
                {
                    result = query.Sum();
                }
            }

            return result;
        }

        public int DeleteCreditBatchEntries(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteCreditBatchEntries @CreditBatchId", serviceHeader,
                    new SqlParameter("CreditBatchId", creditBatchId));
            }
        }

        public int DeleteOverDeductionBatchEntries(Guid overDeductionBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteOverDeductionBatchEntries @OverDeductionBatchId", serviceHeader,
                    new SqlParameter("OverDeductionBatchId", overDeductionBatchId));
            }
        }

        public decimal FindWireTransferBatchEntriesTotal(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            decimal result = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_SumWireTransferBatchEntries @WireTransferBatchId", serviceHeader,
                    new SqlParameter("WireTransferBatchId", wireTransferBatchId));

                if (query != null)
                {
                    result = query.Sum();
                }
            }

            return result;
        }

        public int DeleteWireTransferBatchEntries(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteWireTransferBatchEntries @WireTransferBatchId", serviceHeader,
                    new SqlParameter("WireTransferBatchId", wireTransferBatchId));
            }
        }

        public int DeleteAlternateChannelReconciliationEntries(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteAlternateChannelReconciliationEntries @AlternateChannelReconciliationPeriodId", serviceHeader,
                    new SqlParameter("AlternateChannelReconciliationPeriodId", alternateChannelReconciliationPeriodId));
            }
        }

        public int DeleteCreditBatchDiscrepancies(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteCreditBatchDiscrepancies @CreditBatchId", serviceHeader,
                    new SqlParameter("CreditBatchId", creditBatchId));
            }
        }

        public int DeleteOverDeductionBatchDiscrepancies(Guid overDeductionBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteOverDeductionBatchDiscrepancies @OverDeductionBatchId", serviceHeader,
                    new SqlParameter("OverDeductionBatchId", overDeductionBatchId));
            }
        }

        public int DeleteJournalReversalBatchEntries(Guid journalReversalBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteJournalReversalBatchEntries @JournalReversalBatchId", serviceHeader,
                    new SqlParameter("JournalReversalBatchId", journalReversalBatchId));
            }
        }

        public int DeleteLoanDisbursementBatchEntries(Guid loanDisbursementBatchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DeleteLoanDisbursementBatchEntries @LoanDisbursementBatchID", serviceHeader,
                    new SqlParameter("LoanDisbursementBatchID", loanDisbursementBatchId));
            }
        }

        public int CheckCapitalization(Guid customerAccountId, int month, int interestCapitalizationMonths, ServiceHeader serviceHeader)
        {
            var result = default(int);

            if (interestCapitalizationMonths > 1)/*ignore check*/
                return result;
            else
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var query = _repository.DatabaseSqlQuery<int>("EXEC sp_ValidateGracePeriodComputation @CustomerAccountID, @Month", serviceHeader,
                        new SqlParameter("CustomerAccountID", customerAccountId),
                        new SqlParameter("Month", month));

                    if (query != null)
                    {
                        result = query.Sum();
                    }
                }

                return result;
            }
        }

        public decimal FindCustomerAccountBookBalance(CustomerAccountDTO customerAccountDTO, int type, DateTime cutOffDate, ServiceHeader serviceHeader, bool considerMaturityPeriodForInvestmentAccounts)
        {
            decimal balance = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_CustomerAccountBalance @CustomerAccountID, @Type, @considerMaturityPeriodForInvestmentAccounts, @CutoffDate, @CustomerAccountType_TargetProductId, @CustomerAccountType_ProductCode", serviceHeader,
                    new SqlParameter("CustomerAccountID", customerAccountDTO.Id),
                    new SqlParameter("Type", type),
                    new SqlParameter("considerMaturityPeriodForInvestmentAccounts", considerMaturityPeriodForInvestmentAccounts),
                    new SqlParameter("CutoffDate", cutOffDate),
                    new SqlParameter("CustomerAccountType_TargetProductId", customerAccountDTO.CustomerAccountTypeTargetProductId),
                    new SqlParameter("CustomerAccountType_ProductCode", customerAccountDTO.CustomerAccountTypeProductCode));

                if (query != null)
                {
                    balance = query.Sum();
                }
            }

            return Math.Abs(balance);
            //return balance
        }

        public decimal FindCustomerAccountAvailableBalance(CustomerAccountDTO customerAccountDTO, DateTime cutOffDate, ServiceHeader serviceHeader)
        {
            decimal balance = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_CustomerAccountBalanceAvailable @CustomerAccountID, @CutoffDate, @CustomerAccountType_TargetProductId, @CustomerAccountType_ProductCode, @CustomerAccountBranchId", serviceHeader,
                    new SqlParameter("CustomerAccountID", customerAccountDTO.Id),
                    new SqlParameter("CutoffDate", cutOffDate),
                    new SqlParameter("CustomerAccountType_TargetProductId", customerAccountDTO.CustomerAccountTypeTargetProductId),
                    new SqlParameter("CustomerAccountType_ProductCode", customerAccountDTO.CustomerAccountTypeProductCode),
                    new SqlParameter("CustomerAccountBranchId", customerAccountDTO.BranchId));

                if (query != null)
                {
                    balance = query.Sum();
                }
            }

            return Math.Abs(balance);
        }

        public decimal FindGlAccountBalance(Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            decimal balance = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_GetGlAccountBalance @ChartOfAccountID, @EndDate, @TransactionDateFilter", serviceHeader,
                    new SqlParameter("ChartOfAccountID", chartOfAccountId),
                    new SqlParameter("EndDate", cutOffDate),
                    new SqlParameter("TransactionDateFilter", transactionDateFilter));

                if (query != null)
                {
                    balance = query.Sum();
                }
            }

            return balance;
        }

        public decimal FindGlAccountBalance(Guid branchId, Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            decimal balance = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_GetGlAccountBalanceByBranch @BranchID, @ChartOfAccountID, @EndDate, @TransactionDateFilter", serviceHeader,
                    new SqlParameter("BranchID", branchId),
                    new SqlParameter("ChartOfAccountID", chartOfAccountId),
                    new SqlParameter("EndDate", cutOffDate),
                    new SqlParameter("TransactionDateFilter", transactionDateFilter));

                if (query != null)
                {
                    balance = query.Sum();
                }
            }

            return balance;
        }

        public decimal FindGlAccountBalance(Guid branchId, Guid chartOfAccountId, Guid postingPeriodId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            decimal balance = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_GetGlAccountBalanceByBranchAndPostingPeriod @BranchID, @ChartOfAccountID,@PostingPeriodId, @EndDate, @TransactionDateFilter", serviceHeader,
                    new SqlParameter("BranchID", branchId),
                    new SqlParameter("ChartOfAccountID", chartOfAccountId),
                    new SqlParameter("PostingPeriodId", postingPeriodId),
                    new SqlParameter("EndDate", cutOffDate),
                    new SqlParameter("TransactionDateFilter", transactionDateFilter));

                if (query != null)
                {
                    balance = query.Sum();
                }
            }

            return balance;
        }

        public decimal FindDisbursedLoanCasesValue(Guid loanProductId, Guid branchId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader)
        {
            decimal balance = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_GetDisbursedLoanCasesValue @LoanProductId, @BranchId, @StartDate, @EndDate", serviceHeader,
                    new SqlParameter("LoanProductId", loanProductId),
                    new SqlParameter("BranchId", branchId),
                    new SqlParameter("StartDate", startDate),
                    new SqlParameter("EndDate", endDate));

                if (query != null)
                {
                    balance = query.Sum();
                }
            }

            return balance;
        }

        public decimal ComputeDividendsPayable(Guid customerId, ServiceHeader serviceHeader)
        {
            decimal dividendsPayable = 0m;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<decimal>("EXEC sp_ComputeSingleDividend @CustomerID", serviceHeader,
                    new SqlParameter("CustomerID", customerId));

                if (query != null)
                {
                    dividendsPayable = query.Sum();
                }
            }

            return dividendsPayable;
        }

        public Task<decimal> ComputeDividendsPayableAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Tuple<decimal, decimal, decimal, decimal, int> FindGlAccountStatistics(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            var totalCredits = 0m;
            var totalDebits = 0m;
            var openingBalance = 0m;
            var closingBalance = 0m;
            var itemsCount = 0;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<GLAccountStatisticsBag>("EXEC sp_GetGlTotalDebitsCredits @ChartOfAccountID, @StartDate, @EndDate, @TransactionDateFilter", serviceHeader,
                    new SqlParameter("ChartOfAccountID", chartOfAccountId),
                    new SqlParameter("StartDate", endDate),
                    new SqlParameter("EndDate", endDate),
                    new SqlParameter("TransactionDateFilter", transactionDateFilter));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        totalCredits = item.Credit;
                        totalDebits = item.Debit;
                        itemsCount = item.Count;
                    }
                }
            }


            openingBalance = FindGlAccountBalance(chartOfAccountId, startDate.AddDays(-1), transactionDateFilter, serviceHeader);

            closingBalance = FindGlAccountBalance(chartOfAccountId, UberUtil.AdjustTimeSpan(endDate), transactionDateFilter, serviceHeader);


            return new Tuple<decimal, decimal, decimal, decimal, int>(totalCredits, totalDebits, openingBalance, closingBalance, itemsCount);
        }

        //public Tuple<decimal, decimal, decimal, decimal, int> FindGlAccountStatistics(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionDateFilter, ServiceHeader serviceHeader)
        //{
        //    var totalCredits = 0m;
        //    var totalDebits = 0m;
        //    var openingBalance = 0m;
        //    var closingBalance = 0m;
        //    var itemsCount = 0;

        //    using (_dbContextScopeFactory.CreateReadOnly())
        //    {
        //        var query = _repository.DatabaseSqlQuery<GLAccountStatisticsBag>("EXEC sp_GetGlStatistics @ChartOfAccountID, @StartDate, @EndDate, @TransactionDateFilter", serviceHeader,
        //            new SqlParameter("ChartOfAccountID", chartOfAccountId),
        //            new SqlParameter("StartDate", endDate),
        //            new SqlParameter("EndDate", endDate),
        //            new SqlParameter("TransactionDateFilter", transactionDateFilter));

        //        if (query != null)
        //        {
        //            foreach (var item in query)
        //            {
        //                totalCredits = item.TotalCredits;
        //                totalDebits = item.TotalDebits;
        //                openingBalance = item.OpeningBalance;
        //                closingBalance = item.ClosingBalance;
        //                itemsCount = item.Count;
        //            }
        //        }
        //    }

        //    return new Tuple<decimal, decimal, decimal, decimal, int>(totalCredits, totalDebits, openingBalance, closingBalance, itemsCount);
        //}

        public List<StandingOrderDTO> FindStandingOrdersByEmployerAndTrigger(Guid employerId, int trigger, ServiceHeader serviceHeader)
        {
            List<StandingOrderDTO> standingOrders = new List<StandingOrderDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<StandingOrderBulkCopyDTO>("EXEC sp_GetStandingOrdersByEmployerAndTrigger @EmployerID, @Trigger", serviceHeader,
                    new SqlParameter("EmployerID", employerId),
                    new SqlParameter("Trigger", trigger));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var standingOrder = new StandingOrderDTO
                        {
                            Id = item.Id,
                            BenefactorCustomerAccountId = item.BenefactorCustomerAccountId,
                            BeneficiaryCustomerAccountId = item.BeneficiaryCustomerAccountId,
                            DurationStartDate = item.Duration_StartDate,
                            DurationEndDate = item.Duration_EndDate,
                            ScheduleActualRunDate = item.Schedule_ActualRunDate,
                            ScheduleExpectedRunDate = item.Schedule_ExpectedRunDate,
                            ScheduleFrequency = item.Schedule_Frequency,
                            ScheduleExecuteAttemptCount = item.Schedule_ExecuteAttemptCount,
                            ChargeType = item.Charge_Type,
                            ChargeFixedAmount = item.Charge_FixedAmount,
                            ChargePercentage = item.Charge_Percentage,
                            Trigger = item.Trigger,
                            Principal = item.Principal,
                            Interest = item.Interest,
                            LoanAmount = item.LoanAmount,
                            PaymentPerPeriod = item.PaymentPerPeriod,
                            CapitalizedInterest = item.CapitalizedInterest,
                            IsLocked = item.IsLocked,
                            Remarks = item.Remarks,
                            ScheduleForceExecute = item.ForceExecute,
                            Chargeable = item.Chargeable,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                        };

                        standingOrders.Add(standingOrder);
                    }
                }
            }

            return standingOrders;
        }

        public List<StandingOrderDTO> FindStandingOrdersByEmployerAndProductAndTrigger(Guid employerId, Guid productId, int trigger, ServiceHeader serviceHeader)
        {
            List<StandingOrderDTO> standingOrders = new List<StandingOrderDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<StandingOrderBulkCopyDTO>("EXEC sp_GetStandingOrdersByEmployerProductTrigger @EmployerID, @ProductID, @Trigger", serviceHeader,
                    new SqlParameter("EmployerID", employerId),
                    new SqlParameter("ProductID", productId),
                    new SqlParameter("Trigger", trigger));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var standingOrder = new StandingOrderDTO
                        {
                            Id = item.Id,
                            BenefactorCustomerAccountId = item.BenefactorCustomerAccountId,
                            BeneficiaryCustomerAccountId = item.BeneficiaryCustomerAccountId,
                            DurationStartDate = item.Duration_StartDate,
                            DurationEndDate = item.Duration_EndDate,
                            ScheduleActualRunDate = item.Schedule_ActualRunDate,
                            ScheduleExpectedRunDate = item.Schedule_ExpectedRunDate,
                            ScheduleFrequency = item.Schedule_Frequency,
                            ScheduleExecuteAttemptCount = item.Schedule_ExecuteAttemptCount,
                            ChargeType = item.Charge_Type,
                            ChargeFixedAmount = item.Charge_FixedAmount,
                            ChargePercentage = item.Charge_Percentage,
                            Trigger = item.Trigger,
                            Principal = item.Principal,
                            Interest = item.Interest,
                            LoanAmount = item.LoanAmount,
                            PaymentPerPeriod = item.PaymentPerPeriod,
                            CapitalizedInterest = item.CapitalizedInterest,
                            IsLocked = item.IsLocked,
                            Remarks = item.Remarks,
                            ScheduleForceExecute = item.ForceExecute,
                            Chargeable = item.Chargeable,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                        };

                        standingOrders.Add(standingOrder);
                    }
                }
            }

            return standingOrders;
        }

        public List<StandingOrderDTO> FindStandingOrdersByCreditTypeAndProductAndTrigger(Guid creditTypeId, Guid productId, int trigger, ServiceHeader serviceHeader)
        {
            List<StandingOrderDTO> standingOrders = new List<StandingOrderDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<StandingOrderBulkCopyDTO>("EXEC sp_GetStandingOrdersByCreditTypeProductTrigger @CreditTypeID, @ProductID, @Trigger", serviceHeader,
                    new SqlParameter("CreditTypeID", creditTypeId),
                    new SqlParameter("ProductID", productId),
                    new SqlParameter("Trigger", trigger));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var standingOrder = new StandingOrderDTO
                        {
                            Id = item.Id,
                            BenefactorCustomerAccountId = item.BenefactorCustomerAccountId,
                            BeneficiaryCustomerAccountId = item.BeneficiaryCustomerAccountId,
                            DurationStartDate = item.Duration_StartDate,
                            DurationEndDate = item.Duration_EndDate,
                            ScheduleActualRunDate = item.Schedule_ActualRunDate,
                            ScheduleExpectedRunDate = item.Schedule_ExpectedRunDate,
                            ScheduleFrequency = item.Schedule_Frequency,
                            ScheduleExecuteAttemptCount = item.Schedule_ExecuteAttemptCount,
                            ChargeType = item.Charge_Type,
                            ChargeFixedAmount = item.Charge_FixedAmount,
                            ChargePercentage = item.Charge_Percentage,
                            Trigger = item.Trigger,
                            Principal = item.Principal,
                            Interest = item.Interest,
                            LoanAmount = item.LoanAmount,
                            PaymentPerPeriod = item.PaymentPerPeriod,
                            CapitalizedInterest = item.CapitalizedInterest,
                            IsLocked = item.IsLocked,
                            Remarks = item.Remarks,
                            ScheduleForceExecute = item.ForceExecute,
                            Chargeable = item.Chargeable,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                        };

                        standingOrders.Add(standingOrder);
                    }
                }
            }

            return standingOrders;
        }

        public List<StandingOrderDTO> FindStandingOrdersByCustomerAndProductAndTrigger(Guid customerId, Guid productId, int trigger, ServiceHeader serviceHeader)
        {
            List<StandingOrderDTO> standingOrders = new List<StandingOrderDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<StandingOrderBulkCopyDTO>("EXEC sp_GetStandingOrdersByCustomerProductTrigger @CustomerID, @ProductID, @Trigger", serviceHeader,
                    new SqlParameter("CustomerID", customerId),
                    new SqlParameter("ProductID", productId),
                    new SqlParameter("Trigger", trigger));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var standingOrder = new StandingOrderDTO
                        {
                            Id = item.Id,
                            BenefactorCustomerAccountId = item.BenefactorCustomerAccountId,
                            BeneficiaryCustomerAccountId = item.BeneficiaryCustomerAccountId,
                            DurationStartDate = item.Duration_StartDate,
                            DurationEndDate = item.Duration_EndDate,
                            ScheduleActualRunDate = item.Schedule_ActualRunDate,
                            ScheduleExpectedRunDate = item.Schedule_ExpectedRunDate,
                            ScheduleFrequency = item.Schedule_Frequency,
                            ScheduleExecuteAttemptCount = item.Schedule_ExecuteAttemptCount,
                            ChargeType = item.Charge_Type,
                            ChargeFixedAmount = item.Charge_FixedAmount,
                            ChargePercentage = item.Charge_Percentage,
                            Trigger = item.Trigger,
                            Principal = item.Principal,
                            Interest = item.Interest,
                            LoanAmount = item.LoanAmount,
                            PaymentPerPeriod = item.PaymentPerPeriod,
                            CapitalizedInterest = item.CapitalizedInterest,
                            IsLocked = item.IsLocked,
                            Remarks = item.Remarks,
                            ScheduleForceExecute = item.ForceExecute,
                            Chargeable = item.Chargeable,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                        };

                        standingOrders.Add(standingOrder);
                    }
                }
            }

            return standingOrders;
        }

        public List<StandingOrderDTO> FindStandingOrdersByCustomerRerence3AndTrigger(string customerReference3, int trigger, ServiceHeader serviceHeader)
        {
            List<StandingOrderDTO> standingOrders = new List<StandingOrderDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<StandingOrderBulkCopyDTO>("EXEC sp_GetStandingOrdersByCustomerReference3 @pReference3, @Trigger", serviceHeader,
                    new SqlParameter("pReference3", customerReference3),
                    new SqlParameter("Trigger", trigger));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var standingOrder = new StandingOrderDTO
                        {
                            Id = item.Id,
                            BenefactorCustomerAccountId = item.BenefactorCustomerAccountId,
                            BeneficiaryCustomerAccountId = item.BeneficiaryCustomerAccountId,
                            DurationStartDate = item.Duration_StartDate,
                            DurationEndDate = item.Duration_EndDate,
                            ScheduleActualRunDate = item.Schedule_ActualRunDate,
                            ScheduleExpectedRunDate = item.Schedule_ExpectedRunDate,
                            ScheduleFrequency = item.Schedule_Frequency,
                            ScheduleExecuteAttemptCount = item.Schedule_ExecuteAttemptCount,
                            ChargeType = item.Charge_Type,
                            ChargeFixedAmount = item.Charge_FixedAmount,
                            ChargePercentage = item.Charge_Percentage,
                            Trigger = item.Trigger,
                            Principal = item.Principal,
                            Interest = item.Interest,
                            LoanAmount = item.LoanAmount,
                            PaymentPerPeriod = item.PaymentPerPeriod,
                            CapitalizedInterest = item.CapitalizedInterest,
                            IsLocked = item.IsLocked,
                            Remarks = item.Remarks,
                            ScheduleForceExecute = item.ForceExecute,
                            Chargeable = item.Chargeable,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                        };

                        standingOrders.Add(standingOrder);
                    }
                }
            }

            return standingOrders;
        }

        public StandingOrderDTO FindStandingOrder(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            StandingOrderDTO standingOrder = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<StandingOrderBulkCopyDTO>("EXEC sp_FindStandingOrder @pBenefactorCustomerAccountId, @pBeneficiaryCustomerAccountId, @pTrigger", serviceHeader,
                    new SqlParameter("pBenefactorCustomerAccountId", benefactorCustomerAccountId),
                    new SqlParameter("pBeneficiaryCustomerAccountId", beneficiaryCustomerAccountId),
                    new SqlParameter("pTrigger", trigger));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        standingOrder = new StandingOrderDTO
                        {
                            Id = item.Id,
                            BenefactorCustomerAccountId = item.BenefactorCustomerAccountId,
                            BeneficiaryCustomerAccountId = item.BeneficiaryCustomerAccountId,
                            DurationStartDate = item.Duration_StartDate,
                            DurationEndDate = item.Duration_EndDate,
                            ScheduleActualRunDate = item.Schedule_ActualRunDate,
                            ScheduleExpectedRunDate = item.Schedule_ExpectedRunDate,
                            ScheduleFrequency = item.Schedule_Frequency,
                            ScheduleExecuteAttemptCount = item.Schedule_ExecuteAttemptCount,
                            ChargeType = item.Charge_Type,
                            ChargeFixedAmount = item.Charge_FixedAmount,
                            ChargePercentage = item.Charge_Percentage,
                            Trigger = item.Trigger,
                            Principal = item.Principal,
                            Interest = item.Interest,
                            LoanAmount = item.LoanAmount,
                            PaymentPerPeriod = item.PaymentPerPeriod,
                            CapitalizedInterest = item.CapitalizedInterest,
                            IsLocked = item.IsLocked,
                            Remarks = item.Remarks,
                            ScheduleForceExecute = item.ForceExecute,
                            Chargeable = item.Chargeable,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                        };

                        break;
                    }
                }
            }

            return standingOrder;
        }

        public CreditBatchEntryDTO FindLastCreditBatchEntryByCustomerAccountId(Guid customerAccountId, int creditBatchType, ServiceHeader serviceHeader)
        {
            CreditBatchEntryDTO creditBatchEntry = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<CreditBatchEntryDTO>("EXEC sp_GetLastCreditBatchEntry @CustomerAccountID, @Type", serviceHeader,
                    new SqlParameter("CustomerAccountID", customerAccountId),
                    new SqlParameter("Type", creditBatchType));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        creditBatchEntry = item;

                        break;
                    }
                }
            }

            return creditBatchEntry;
        }

        public LoanCaseDTO FindLastLoanCaseByCustomerId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            LoanCaseDTO loanCase = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<LoanCaseDTO>("EXEC sp_GetMonthlyAbility @CustomerID, @LoanProductID", serviceHeader,
                    new SqlParameter("CustomerID", customerId),
                    new SqlParameter("LoanProductID", loanProductId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        loanCase = item;

                        break;
                    }
                }
            }

            return loanCase;
        }

        public List<Guid> FindCustomerIds(ServiceHeader serviceHeader)
        {
            List<Guid> customerIds = new List<Guid>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<Guid>("EXEC sp_GetCustomerIds", serviceHeader);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        customerIds.Add(item);
                    }
                }
            }

            return customerIds;
        }

        public int UpdateStandingOrderCapitalizedInterest(Guid beneficiaryCustomerAccountId, decimal interestAmount, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_UpdateStandingOrderCapitalizedInterest @BeneficiaryAccount, @InterestAMount", serviceHeader,
                    new SqlParameter("BeneficiaryAccount", beneficiaryCustomerAccountId),
                    new SqlParameter("InterestAMount", interestAmount));
            }
        }

        public bool BulkInsert<T>(string tableName, IList<T> list, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            //using (var bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings[serviceHeader.ApplicationDomainName].ConnectionString, SqlBulkCopyOptions.FireTriggers))
            //{
            //    bulkCopy.BulkCopyTimeout = 300;
            //    bulkCopy.BatchSize = 5000;
            //    bulkCopy.DestinationTableName = tableName;

            //    var table = new DataTable();

            //    var props = TypeDescriptor.GetProperties(typeof(T))
            //                               //Dirty hack to make sure we only have system data types
            //                               //i.e. filter out the relationships/collections
            //                               .Cast<PropertyDescriptor>()
            //                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
            //                               .ToArray();

            //    foreach (var propertyInfo in props)
            //    {
            //        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);

            //        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            //    }

            //    var values = new object[props.Length];

            //    foreach (var item in list)
            //    {
            //        for (var i = 0; i < values.Length; i++)
            //        {
            //            values[i] = props[i].GetValue(item);
            //        }

            //        table.Rows.Add(values);
            //    }

            //    bulkCopy.WriteToServer(table);

            //    result = true;
            //}


            using (var bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString, SqlBulkCopyOptions.FireTriggers))
            {
                bulkCopy.BulkCopyTimeout = 300;
                bulkCopy.BatchSize = 5000;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();

                var props = TypeDescriptor.GetProperties(typeof(T))
                                           //Dirty hack to make sure we only have system data types
                                           //i.e. filter out the relationships/collections
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);

                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];

                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);

                result = true;
            }

            return result;
        }
        public DateTime? CheckCustomerAccountLastWithdrawalDate(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            DateTime? result = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<DateTime?>("EXEC sp_CheckCustomerAccountLastWithdrawalDate @CustomerAccountID", serviceHeader,
                    new SqlParameter("CustomerAccountID", customerAccountId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        result = item;

                        break;
                    }
                }
            }

            return result;
        }

        public int DelinkStation(Guid stationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DatabaseExecuteSqlCommand("EXEC sp_DelinkStation @StationId", serviceHeader,
                    new SqlParameter("StationId", stationId));
            }
        }

        public Task<List<CustomerDTO>> FindCustomersByPayrollNumberAsync(string payrollNumber, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerAccountDTO> FindCustomerAccountByIdAsync(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAndReference3Async(Guid targetProductId, string reference3, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAndPayrollNumberAsync(Guid targetProductId, string payrollNumber, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAsync(Guid targetProductId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerAccountDTO>> FindCustomerAccountsGivenEmployerAndLoanProductAsync(Guid employerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerAccountDTO>> FindCustomerAccountsGivenCreditTypeAndLoanProductAsync(Guid creditTypeId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAndCustomerIdAsync(Guid targetProductId, Guid customerId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteDebitBatchEntriesAsync(Guid debitBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> FindCreditBatchEntriesTotalAsync(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteCreditBatchEntriesAsync(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAlternateChannelReconciliationEntriesAsync(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteCreditBatchDiscrepanciesAsync(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteLoanDisbursementBatchEntriesAsync(Guid loanDisbursementBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteJournalReversalBatchEntriesAsync(Guid journalReversalBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> CheckCapitalizationAsync(Guid customerAccountId, int month, int interestCapitalizationMonths, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> FindCustomerAccountBookBalanceAsync(CustomerAccountDTO customerAccountDTO, int type, DateTime cutOffDate, ServiceHeader serviceHeader, bool considerMaturityPeriodForInvestmentAccounts = false)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var result = 0m;

                var query = await _repository.DatabaseSqlQueryAsync<decimal>("EXEC sp_CustomerAccountBalance @CustomerAccountID, @Type, @considerMaturityPeriodForInvestmentAccounts, @CutoffDate, @CustomerAccountType_TargetProductId, @CustomerAccountType_ProductCode", serviceHeader,
                    new SqlParameter("CustomerAccountID", customerAccountDTO.Id),
                    new SqlParameter("Type", type),
                    new SqlParameter("considerMaturityPeriodForInvestmentAccounts", considerMaturityPeriodForInvestmentAccounts),
                    new SqlParameter("CutoffDate", cutOffDate),
                    new SqlParameter("CustomerAccountType_TargetProductId", customerAccountDTO.CustomerAccountTypeTargetProductId),
                    new SqlParameter("CustomerAccountType_ProductCode", customerAccountDTO.CustomerAccountTypeProductCode));

                if (query != null)
                {
                    result = query.Sum();
                }

                return result;
            }
        }

        public async Task<decimal> FindCustomerAccountAvailableBalanceAsync(CustomerAccountDTO customerAccountDTO, DateTime cutOffDate, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var result = 0m;

                var query = await _repository.DatabaseSqlQueryAsync<decimal>("EXEC sp_CustomerAccountBalanceAvailable @CustomerAccountID, @CutoffDate, @CustomerAccountType_TargetProductId, @CustomerAccountType_ProductCode, @CustomerAccountBranchId", serviceHeader,
                   new SqlParameter("CustomerAccountID", customerAccountDTO.Id),
                   new SqlParameter("CutoffDate", cutOffDate),
                   new SqlParameter("CustomerAccountType_TargetProductId", customerAccountDTO.CustomerAccountTypeTargetProductId),
                   new SqlParameter("CustomerAccountType_ProductCode", customerAccountDTO.CustomerAccountTypeProductCode),
                   new SqlParameter("CustomerAccountBranchId", customerAccountDTO.BranchId));

                if (query != null)
                {
                    result = query.Sum();
                }

                return result;
            }
        }

        public async Task<decimal> FindGlAccountBalanceAsync(Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var result = 0m;

                var query = await _repository.DatabaseSqlQueryAsync<decimal>("EXEC sp_GetGlAccountBalance @ChartOfAccountID, @EndDate, @TransactionDateFilter", serviceHeader,
                    new SqlParameter("ChartOfAccountID", chartOfAccountId),
                    new SqlParameter("EndDate", cutOffDate),
                    new SqlParameter("TransactionDateFilter", transactionDateFilter));

                if (query != null)
                {
                    result = query.Sum();
                }

                return result;
            }
        }

        public async Task<decimal> FindGlAccountBalanceAsync(Guid branchId, Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var result = 0m;

                var query = await _repository.DatabaseSqlQueryAsync<decimal>("EXEC sp_GetGlAccountBalanceByBranch @BranchID, @ChartOfAccountID, @EndDate, @TransactionDateFilter", serviceHeader,
                   new SqlParameter("BranchID", branchId),
                   new SqlParameter("ChartOfAccountID", chartOfAccountId),
                   new SqlParameter("EndDate", cutOffDate),
                   new SqlParameter("TransactionDateFilter", transactionDateFilter));

                if (query != null)
                {
                    result = query.Sum();
                }

                return result;
            }
        }

        public Task<Tuple<decimal, decimal, int>> FindGlAccountStatisticsAsync(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<StandingOrderDTO>> FindStandingOrdersByEmployerAndTriggerAsync(Guid employerId, int trigger, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<StandingOrderDTO>> FindStandingOrdersByEmployerAndProductAndTriggerAsync(Guid employerId, Guid productId, int trigger, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<StandingOrderDTO>> FindStandingOrdersByCreditTypeAndProductAndTriggerAsync(Guid creditTypeId, Guid productId, int trigger, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<StandingOrderDTO>> FindStandingOrdersByCustomerAndProductAndTriggerAsync(Guid customerId, Guid productId, int trigger, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<StandingOrderDTO>> FindStandingOrdersByCustomerRerence3AndTriggerAsync(string customerReference3, int trigger, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<StandingOrderDTO> FindStandingOrderAsync(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<CreditBatchEntryDTO> FindLastCreditBatchEntryByCustomerAccountIdAsyn(Guid customerAccountId, int creditBatchType, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<LoanCaseDTO> FindLastLoanCaseByCustomerIdAsyn(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<List<Guid>> FindCustomerIdsAsync(ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateStandingOrderCapitalizedInterestAsync(Guid beneficiaryCustomerAccountId, decimal interestAmount, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkInsertAsync<T>(string tableName, IList<T> list, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime?> CheckCustomerAccountLastWithdrawalDateAsync(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DelinkStationAsync(Guid stationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var result = await _repository.DatabaseExecuteSqlCommandAsync("EXEC sp_DelinkStation @StationId", serviceHeader,
                       new SqlParameter("StationId", stationId));

                return result;
            }
        }

        public Task<decimal> FindWireTransferBatchEntriesTotalAsync(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteWireTransferBatchEntriesAsync(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public SuperSaverInterestDTO FindCustomerSuperSaverPayable(Guid customerId, ServiceHeader serviceHeader)
        {
            SuperSaverInterestDTO result = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _repository.DatabaseSqlQuery<SuperSaverInterestDTO>("exec sp_SuperSaver @CustomerId", serviceHeader, new SqlParameter("CustomerId", customerId));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        result = item;

                        break;
                    }
                }
            }

            return result;
        }

        public List<MonthlySummaryValuesDTO> FindEmailAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader)
        {
            var result = new List<MonthlySummaryValuesDTO>();

            using (_dbContextScopeFactory.CreateReadOnlyWithTransaction(IsolationLevel.ReadUncommitted))
            {
                List<MonthlySummaryValuesDTO> query = null;

                if (companyId != null && companyId != Guid.Empty)
                {
                    query = _emailAlertRepository.DatabaseSqlQuery<MonthlySummaryValuesDTO>("exec sp_MonthlyEmailAlertsSummaryByCompany @CompanyId, @StartDate, @EndDate", serviceHeader,
                        new object[] { new SqlParameter("CompanyId", companyId), new SqlParameter("StartDate", startDate), new SqlParameter("EndDate", endDate) }).ToList();
                }
                else
                {
                    query = _emailAlertRepository.DatabaseSqlQuery<MonthlySummaryValuesDTO>("exec sp_MonthlyEmailAlertsSummary @StartDate, @EndDate", serviceHeader,
                        new object[] { new SqlParameter("StartDate", startDate), new SqlParameter("EndDate", endDate) }).ToList();
                }

                if (query != null)
                {
                    result = query;
                }
            }

            return result;
        }

        public List<MonthlySummaryValuesDTO> FindTextAlertsMonthlyStatatistics(Guid companyId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader)
        {
            var result = new List<MonthlySummaryValuesDTO>();

            using (_dbContextScopeFactory.CreateReadOnlyWithTransaction(IsolationLevel.ReadUncommitted))
            {
                List<MonthlySummaryValuesDTO> query = null;

                if (companyId != null && companyId != Guid.Empty)
                {
                    query = _textAlertRepository.DatabaseSqlQuery<MonthlySummaryValuesDTO>("exec sp_MonthlyTextAlertsSummaryByCompany @CompanyId, @StartDate, @EndDate", serviceHeader,
                        new object[] { new SqlParameter("CompanyId", companyId), new SqlParameter("StartDate", startDate), new SqlParameter("EndDate", endDate) }).ToList();
                }
                else
                {
                    query = _textAlertRepository.DatabaseSqlQuery<MonthlySummaryValuesDTO>("exec sp_MonthlyTextAlertsSummary @StartDate, @EndDate", serviceHeader,
                        new object[] { new SqlParameter("StartDate", startDate), new SqlParameter("EndDate", endDate) }).ToList();
                }

                if (query != null)
                {
                    result = query;
                }
            }

            return result;
        }


        // Implementation

        public JournalDTO FindLastPrincipalRepaymentByLoanReference(
    string loanCaseNumber,
    Guid customerAccountId,
    ServiceHeader serviceHeader)
        {
            JournalDTO result = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var sql = @"
            SELECT TOP 1
                j.Id,
                j.Reference,
                j.ValueDate,
                j.CreatedDate,
                j.PrimaryDescription,
                j.SecondaryDescription
            FROM swiftFin_JournalEntries je
            INNER JOIN swiftFin_Journals j ON j.Id = je.JournalId
            WHERE
                je.CustomerAccountId = @pCustomerAccountId
                AND j.Reference      = @pLoanCaseNumber
                AND je.Amount        < 0
                AND (
                    (j.PrimaryDescription LIKE '%Principal%Repayment%'
                        AND j.SecondaryDescription LIKE '%Normal%')
                    OR
                    (j.PrimaryDescription LIKE '%Principal%Repayment%'
                        AND j.SecondaryDescription LIKE '%Check%Off%')
                )
            ORDER BY j.ValueDate DESC, j.CreatedDate DESC";

                var query = _repository.DatabaseSqlQuery<JournalLastPaymentDTO>(
                    sql,
                    serviceHeader,
                    new SqlParameter("pCustomerAccountId", customerAccountId),
                    new SqlParameter("pLoanCaseNumber", loanCaseNumber));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        result = new JournalDTO
                        {
                            Id = item.Id,
                            Reference = item.Reference,
                            ValueDate = item.ValueDate,
                            CreatedDate = item.CreatedDate,
                            PrimaryDescription = item.PrimaryDescription,
                            SecondaryDescription = item.SecondaryDescription
                        };

                        break; // TOP 1, but being defensive
                    }
                }
            }

            return result;
        }

        public async Task<JournalDTO> FindLastPrincipalRepaymentByLoanReferenceAsync(
            string loanCaseNumber,
            Guid customerAccountId,
            ServiceHeader serviceHeader)
        {
            JournalDTO result = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var sql = @"
            SELECT TOP 1
                j.Id,
                j.Reference,
                j.ValueDate,
                j.CreatedDate,
                j.PrimaryDescription,
                j.SecondaryDescription
            FROM swiftFin_JournalEntries je
            INNER JOIN swiftFin_Journals j ON j.Id = je.JournalId
            WHERE
                je.CustomerAccountId = @pCustomerAccountId
                AND j.Reference      = @pLoanCaseNumber
                AND je.Amount        < 0
                AND (
                    (j.PrimaryDescription LIKE '%Primary%Repayment%'
                        AND j.SecondaryDescription LIKE '%Normal%')
                    OR
                    (j.PrimaryDescription LIKE '%Principal%Repayment%'
                        AND j.SecondaryDescription LIKE '%Check%Off%')
                )
            ORDER BY j.ValueDate DESC, j.CreatedDate DESC";

                var query = await Task.Run(() =>
                    _repository.DatabaseSqlQuery<JournalLastPaymentDTO>(
                        sql,
                        serviceHeader,
                        new SqlParameter("pCustomerAccountId", customerAccountId),
                        new SqlParameter("pLoanCaseNumber", loanCaseNumber)));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        result = new JournalDTO
                        {
                            Id = item.Id,
                            Reference = item.Reference,
                            ValueDate = item.ValueDate,
                            CreatedDate = item.CreatedDate,
                            PrimaryDescription = item.PrimaryDescription,
                            SecondaryDescription = item.SecondaryDescription
                        };

                        break;
                    }
                }
            }

            return result;
        }


    }

    public class GLAccountStatisticsBag
    {
        public decimal Credit { get; set; }

        public decimal Debit { get; set; }

        public decimal OpeningBalance { get; set; }

        public decimal ClosingBalance { get; set; }

        public int Count { get; set; }

    }

    public class JournalLastPaymentDTO
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PrimaryDescription { get; set; }
        public string SecondaryDescription { get; set; }
    }




}
