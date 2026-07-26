using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeCommissionAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class DebitTypeAppService : IDebitTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<DebitType> _debitTypeRepository;
        private readonly IRepository<DebitTypeCommission> _debitTypeCommissionRepository;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;

        public DebitTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DebitType> debitTypeRepository,
           IRepository<DebitTypeCommission> debitTypeCommissionRepository,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ILoanProductAppService loanProductAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (debitTypeRepository == null)
                throw new ArgumentNullException(nameof(debitTypeRepository));

            if (debitTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(debitTypeCommissionRepository));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _debitTypeRepository = debitTypeRepository;
            _debitTypeCommissionRepository = debitTypeCommissionRepository;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
        }

        public DebitTypeDTO AddNewDebitType(DebitTypeDTO debitTypeDTO, ServiceHeader serviceHeader)
        {
            if (debitTypeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<DebitType> spec = DebitTypeSpecifications.DebitTypeDescription(debitTypeDTO.Description);

                    var matcheddebitType = _debitTypeRepository.AllMatching(spec, serviceHeader);

                    if (matcheddebitType != null && matcheddebitType.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        debitTypeDTO.ErrorMessageResult = string.Format("Sorry, but Debit Type {0} already exists!", debitTypeDTO.Description.ToUpper());
                        return debitTypeDTO;
                    }
                    else
                    {
                        var customerAccountType = new CustomerAccountType(debitTypeDTO.CustomerAccountTypeProductCode, debitTypeDTO.CustomerAccountTypeTargetProductId, debitTypeDTO.CustomerAccountTypeTargetProductCode);

                        var debitType = DebitTypeFactory.CreateDebitType(debitTypeDTO.Description, customerAccountType);

                        if (debitTypeDTO.IsLocked)
                            debitType.Lock();
                        else debitType.UnLock();

                        if (debitTypeDTO.IsMandatory)
                            debitType.SetAsMandatory();
                        else debitType.ResetAsMandatory();

                        _debitTypeRepository.Add(debitType, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return debitType.ProjectedAs<DebitTypeDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateDebitType(DebitTypeDTO debitTypeDTO, ServiceHeader serviceHeader)
        {
            if (debitTypeDTO == null || debitTypeDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _debitTypeRepository.Get(debitTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var customerAccountType = new CustomerAccountType(debitTypeDTO.CustomerAccountTypeProductCode, debitTypeDTO.CustomerAccountTypeTargetProductId, debitTypeDTO.CustomerAccountTypeTargetProductCode);

                    var current = DebitTypeFactory.CreateDebitType(debitTypeDTO.Description, customerAccountType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (debitTypeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    if (debitTypeDTO.IsMandatory)
                        current.SetAsMandatory();
                    else current.ResetAsMandatory();

                    _debitTypeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DebitTypeDTO> FindDebitTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<DebitType> spec = DebitTypeSpecifications.DefaultSpec();

                var debitTypes = _debitTypeRepository.AllMatching(spec, serviceHeader);

                if (debitTypes != null && debitTypes.Any())
                {
                    return debitTypes.ProjectedAsCollection<DebitTypeDTO>();
                }
                else return null;
            }
        }

        public List<DebitTypeDTO> FindMandatoryDebitTypes(bool isMandatory, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<DebitType> spec = DebitTypeSpecifications.MandatoryDebitTypes(isMandatory);

                var debitTypes = _debitTypeRepository.AllMatching(spec, serviceHeader);

                if (debitTypes != null && debitTypes.Any())
                {
                    return debitTypes.ProjectedAsCollection<DebitTypeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DebitTypeDTO> FindDebitTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DebitTypeSpecifications.DefaultSpec();

                ISpecification<DebitType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var debitTypeCollection = _debitTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (debitTypeCollection != null)
                {
                    var pageCollection = debitTypeCollection.PageCollection.ProjectedAsCollection<DebitTypeDTO>();

                    var itemsCount = debitTypeCollection.ItemsCount;

                    return new PageCollectionInfo<DebitTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DebitTypeDTO> FindDebitTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DebitTypeSpecifications.DebitTypeFullText(text);

                ISpecification<DebitType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var debitTypeCollection = _debitTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (debitTypeCollection != null)
                {
                    var pageCollection = debitTypeCollection.PageCollection.ProjectedAsCollection<DebitTypeDTO>();

                    var itemsCount = debitTypeCollection.ItemsCount;

                    return new PageCollectionInfo<DebitTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DebitTypeDTO FindDebitType(Guid debitTypeId, ServiceHeader serviceHeader)
        {
            if (debitTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var debitType = _debitTypeRepository.Get(debitTypeId, serviceHeader);

                    if (debitType != null)
                    {
                        return debitType.ProjectedAs<DebitTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCommissions(Guid debitTypeId, ServiceHeader serviceHeader)
        {
            if (debitTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DebitTypeCommissionSpecifications.DebitTypeCommissionWithDebitTypeId(debitTypeId);

                    ISpecification<DebitTypeCommission> spec = filter;

                    var debitTypeCommissions = _debitTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (debitTypeCommissions != null)
                    {
                        var projection = debitTypeCommissions.ProjectedAsCollection<DebitTypeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateCommissions(Guid debitTypeId, List<CommissionDTO> dynamicCharges, ServiceHeader serviceHeader)
        {
            if (debitTypeId != null && dynamicCharges != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _debitTypeRepository.Get(debitTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = DebitTypeCommissionSpecifications.DebitTypeCommissionWithDebitTypeId(debitTypeId);

                        ISpecification<DebitTypeCommission> spec = filter;

                        var debitTypeCommissions = _debitTypeCommissionRepository.AllMatching(spec, serviceHeader);

                        if (debitTypeCommissions != null)
                        {
                            debitTypeCommissions.ToList().ForEach(x => _debitTypeCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (dynamicCharges.Any())
                        {
                            foreach (var item in dynamicCharges)
                            {
                                var debitTypeCommission = DebitTypeCommissionFactory.CreateDebitTypeCommission(persisted.Id, item.Id);

                                _debitTypeCommissionRepository.Add(debitTypeCommission, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public void FetchDebitTypesProductDescription(List<DebitTypeDTO> debitTypes, ServiceHeader serviceHeader)
        {
            if (debitTypes != null && debitTypes.Any())
            {
                var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);
                var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader);
                var savingsProducts = _savingsProductAppService.FindSavingsProducts(serviceHeader);

                debitTypes.ForEach(item =>
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
