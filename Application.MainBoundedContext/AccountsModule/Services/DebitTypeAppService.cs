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
        private readonly ICommissionAppService _commissionAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;

        public DebitTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DebitType> debitTypeRepository,
           IRepository<DebitTypeCommission> debitTypeCommissionRepository,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ILoanProductAppService loanProductAppService,
           ICommissionAppService commissionAppService)
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

            _commissionAppService = commissionAppService ?? throw new ArgumentNullException(nameof(commissionAppService));
            _dbContextScopeFactory = dbContextScopeFactory;
            _debitTypeRepository = debitTypeRepository;
            _debitTypeCommissionRepository = debitTypeCommissionRepository;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
        }

        // One scope owns header and commission replacement: validation failures
        // cannot leave a partially configured debit type behind.
        public DebitTypeDTO SaveConfiguredDebitType(DebitTypeDTO dto, List<CommissionDTO> commissions, ServiceHeader header)
        {
            if (dto == null) throw new ArgumentException("Debit type data is required.");
            dto.Description = (dto.Description ?? string.Empty).Trim();
            dto.ValidateAll();
            if (dto.HasErrors) throw new ArgumentException(string.Join("; ", dto.ErrorMessages));
            if (dto.CustomerAccountTypeTargetProductId == Guid.Empty)
                throw new ArgumentException("Select a target product.");
            switch ((ProductCode)dto.CustomerAccountTypeProductCode)
            {
                case ProductCode.Savings:
                    var savings = _savingsProductAppService.FindSavingsProduct(dto.CustomerAccountTypeTargetProductId, Guid.Empty, header);
                    if (savings == null) throw new ArgumentException("Savings product not found.");
                    dto.CustomerAccountTypeTargetProductCode = savings.Code;
                    break;
                case ProductCode.Loan:
                    var loan = _loanProductAppService.FindLoanProduct(dto.CustomerAccountTypeTargetProductId, header);
                    if (loan == null) throw new ArgumentException("Loan product not found.");
                    dto.CustomerAccountTypeTargetProductCode = loan.Code;
                    break;
                case ProductCode.Investment:
                    var investment = _investmentProductAppService.FindInvestmentProduct(dto.CustomerAccountTypeTargetProductId, header);
                    if (investment == null) throw new ArgumentException("Investment product not found.");
                    dto.CustomerAccountTypeTargetProductCode = investment.Code;
                    break;
                default: throw new ArgumentException("Select Savings, Loan or Investment as the product type.");
            }
            if (commissions == null || commissions.Any(item => item == null || item.Id == Guid.Empty))
                throw new ArgumentException("Supply the complete commissions array; every commission must have an Id.");
            if (commissions.Select(item => item.Id).Distinct().Count() != commissions.Count)
                throw new ArgumentException("A commission may be selected only once.");
            foreach (var commission in commissions)
                if (_commissionAppService.FindCommission(commission.Id, header) == null)
                    throw new ArgumentException("Selected commission not found.");
            using (var scope = _dbContextScopeFactory.Create())
            {
                var duplicates = _debitTypeRepository.AllMatching(DebitTypeSpecifications.DebitTypeDescription(dto.Description), header);
                if (duplicates != null && duplicates.Any(item => item.Id != dto.Id))
                    throw new ArgumentException("A debit type with this name already exists.");
                var id = dto.Id;
                if (id == Guid.Empty)
                {
                    var created = AddNewDebitType(dto, header);
                    if (created == null || created.Id == Guid.Empty || !string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                        throw new InvalidOperationException("Debit type could not be created.");
                    id = created.Id;
                }
                else if (!UpdateDebitType(dto, header))
                    throw new InvalidOperationException("Debit type could not be updated.");
                if (!UpdateCommissions(id, commissions, header))
                    throw new InvalidOperationException("Debit type commissions could not be saved.");
                scope.SaveChanges(header);
                return FindDebitType(id, header);
            }
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
                var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader) ?? new List<LoanProductDTO>();
                var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader) ?? new List<InvestmentProductDTO>();
                var savingsProducts = _savingsProductAppService.FindSavingsProducts(serviceHeader) ?? new List<SavingsProductDTO>();

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
