using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DirectDebitAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class DirectDebitAppService : IDirectDebitAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<DirectDebit> _directDebitRepository;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;

        public DirectDebitAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DirectDebit> directDebitRepository,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ILoanProductAppService loanProductAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (directDebitRepository == null)
                throw new ArgumentNullException(nameof(directDebitRepository));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _directDebitRepository = directDebitRepository;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
        }

        public DirectDebitDTO AddNewDirectDebit(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader)
        {
            if (directDebitDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<DirectDebit> spec = DirectDebitSpecifications.DirectDebitDescription(directDebitDTO.Description);

                    var matchedDirectDebit = _directDebitRepository.AllMatching(spec, serviceHeader);

                    if (matchedDirectDebit != null && matchedDirectDebit.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        directDebitDTO.ErrorMessageResult = string.Format("Sorry, but Direct Debit {0} already exists!", directDebitDTO.Description.ToUpper());
                        return directDebitDTO;
                    }
                    else
                    {
                        var customerAccountType = new CustomerAccountType(directDebitDTO.CustomerAccountTypeProductCode, directDebitDTO.CustomerAccountTypeTargetProductId, directDebitDTO.CustomerAccountTypeTargetProductCode);

                        var charge = new Charge(directDebitDTO.ChargeType, directDebitDTO.ChargePercentage, directDebitDTO.ChargeFixedAmount);

                        var directDebit = DirectDebitFactory.CreateDirectDebit(directDebitDTO.Description, customerAccountType, charge);

                        if (directDebitDTO.IsLocked)
                            directDebit.Lock();
                        else directDebit.UnLock();

                        _directDebitRepository.Add(directDebit, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return directDebit.ProjectedAs<DirectDebitDTO>();
                    }
                }
            }
            else return null;
        }

        public List<string> ValidateDirectDebit(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader)
        {
            var errors = new List<string>();
            if (directDebitDTO == null)
            {
                errors.Add("Direct debit data is required.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(directDebitDTO.Description))
                errors.Add("Name is required.");
            if (!Enum.IsDefined(typeof(ProductCode), directDebitDTO.CustomerAccountTypeProductCode))
                errors.Add("Select a valid product code.");
            if (directDebitDTO.CustomerAccountTypeTargetProductId == Guid.Empty)
                errors.Add("Select a product.");
            if (!Enum.IsDefined(typeof(ChargeType), directDebitDTO.ChargeType))
                errors.Add("Select a valid charge type.");
            else if ((ChargeType)directDebitDTO.ChargeType == ChargeType.Percentage && (directDebitDTO.ChargePercentage <= 0 || directDebitDTO.ChargePercentage > 100))
                errors.Add("Percentage must be greater than 0 and no more than 100.");
            else if ((ChargeType)directDebitDTO.ChargeType == ChargeType.FixedAmount && directDebitDTO.ChargeFixedAmount <= 0)
                errors.Add("Fixed amount must be greater than 0.");

            if (directDebitDTO.CustomerAccountTypeTargetProductId != Guid.Empty && Enum.IsDefined(typeof(ProductCode), directDebitDTO.CustomerAccountTypeProductCode))
            {
                object product = null;
                switch ((ProductCode)directDebitDTO.CustomerAccountTypeProductCode)
                {
                    case ProductCode.Savings:
                        product = _savingsProductAppService.FindSavingsProduct(directDebitDTO.CustomerAccountTypeTargetProductId, Guid.Empty, serviceHeader);
                        break;
                    case ProductCode.Loan:
                        product = _loanProductAppService.FindLoanProduct(directDebitDTO.CustomerAccountTypeTargetProductId, serviceHeader);
                        break;
                    case ProductCode.Investment:
                        product = _investmentProductAppService.FindInvestmentProduct(directDebitDTO.CustomerAccountTypeTargetProductId, serviceHeader);
                        break;
                }
                if (product == null)
                    errors.Add("The selected product does not exist for that product code.");
            }

            return errors;
        }

        public bool UpdateDirectDebit(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader)
        {
            if (directDebitDTO == null || directDebitDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _directDebitRepository.Get(directDebitDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var customerAccountType = new CustomerAccountType(directDebitDTO.CustomerAccountTypeProductCode, directDebitDTO.CustomerAccountTypeTargetProductId, directDebitDTO.CustomerAccountTypeTargetProductCode);

                    var charge = new Charge(directDebitDTO.ChargeType, directDebitDTO.ChargePercentage, directDebitDTO.ChargeFixedAmount);

                    var current = DirectDebitFactory.CreateDirectDebit(directDebitDTO.Description, customerAccountType, charge);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (directDebitDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _directDebitRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DirectDebitDTO> FindDirectDebits(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<DirectDebit> spec = DirectDebitSpecifications.DefaultSpec();

                var directDebits = _directDebitRepository.AllMatching(spec, serviceHeader);

                if (directDebits != null && directDebits.Any())
                {
                    return directDebits.ProjectedAsCollection<DirectDebitDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DirectDebitDTO> FindDirectDebits(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DirectDebitSpecifications.DefaultSpec();

                ISpecification<DirectDebit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var directDebitCollection = _directDebitRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (directDebitCollection != null)
                {
                    var pageCollection = directDebitCollection.PageCollection.ProjectedAsCollection<DirectDebitDTO>();

                    var itemsCount = directDebitCollection.ItemsCount;

                    return new PageCollectionInfo<DirectDebitDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DirectDebitDTO> FindDirectDebits(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DirectDebitSpecifications.DirectDebitFullText(text);

                ISpecification<DirectDebit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var directDebitCollection = _directDebitRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (directDebitCollection != null)
                {
                    var pageCollection = directDebitCollection.PageCollection.ProjectedAsCollection<DirectDebitDTO>();

                    var itemsCount = directDebitCollection.ItemsCount;

                    return new PageCollectionInfo<DirectDebitDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DirectDebitDTO FindDirectDebit(Guid directDebitId, ServiceHeader serviceHeader)
        {
            if (directDebitId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var directDebit = _directDebitRepository.Get(directDebitId, serviceHeader);

                    if (directDebit != null)
                    {
                        return directDebit.ProjectedAs<DirectDebitDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public void FetchDirectDebitsProductDescription(List<DirectDebitDTO> directDebits, ServiceHeader serviceHeader)
        {
            if (directDebits != null && directDebits.Any())
            {
                var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);
                var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader);
                var savingsProducts = _savingsProductAppService.FindSavingsProducts(serviceHeader);

                directDebits.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = savingsProducts.SingleOrDefault(x => x.Id == item.CustomerAccountTypeTargetProductId);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsDefault = savingsProduct.IsDefault;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = loanProducts.SingleOrDefault(x => x.Id == item.CustomerAccountTypeTargetProductId);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId = loanProduct.InterestReceivableChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountCode = loanProduct.InterestReceivableChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountName = loanProduct.InterestReceivableChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductLoanProductSection = loanProduct.LoanRegistrationLoanProductSection;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = investmentProducts.SingleOrDefault(x => x.Id == item.CustomerAccountTypeTargetProductId);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsRefundable = investmentProduct.IsRefundable;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }
    }
}
