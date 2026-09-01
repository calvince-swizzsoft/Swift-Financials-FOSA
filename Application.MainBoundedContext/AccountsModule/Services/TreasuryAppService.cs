using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.TreasuryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class TreasuryAppService : ITreasuryAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Treasury> _treasuryRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public TreasuryAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Treasury> treasuryRepository,
           IRepository<Branch> branchRepository,
           IRepository<ChartOfAccount> chartOfAccountRepository,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (treasuryRepository == null)
                throw new ArgumentNullException(nameof(treasuryRepository));

            if (branchRepository == null)
                throw new ArgumentNullException(nameof(branchRepository));

            if (chartOfAccountRepository == null)
                throw new ArgumentNullException(nameof(chartOfAccountRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _treasuryRepository = treasuryRepository;
            _branchRepository = branchRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public TreasuryDTO AddNewTreasury(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader)
        {
            ValidateTreasury(treasuryDTO, serviceHeader);
            treasuryDTO.Description = treasuryDTO.Description.Trim();

            if (treasuryDTO != null && treasuryDTO.BranchId != Guid.Empty && treasuryDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var filter = TreasurySpecifications.TreasuryWithBranchId(treasuryDTO.BranchId);

                    ISpecification<Treasury> spec = filter;

                    var treasuries = _treasuryRepository.AllMatching(spec, serviceHeader);

                    if (treasuries != null && treasuries.Any())
                    {
                        var branch = _branchRepository.Get(treasuryDTO.BranchId, serviceHeader);
                        treasuryDTO.ErrorMessageResult = string.Format("Another treasury has already been linked to branch '{0}'.", branch.Description);

                        return treasuryDTO;
                    }
                    else
                    {

                        var filterDescription = TreasurySpecifications.DescriptionWithDescription(treasuryDTO.Description);

                        ISpecification<Treasury> descSpec = filterDescription;

                        var treasuryDesc = _treasuryRepository.AllMatching(descSpec, serviceHeader);

                        if (treasuryDesc != null && treasuryDesc.Any())
                        {
                            treasuryDTO.ErrorMessageResult = string.Format("Sorry, but Treasury \"{0}\" already exists!", treasuryDTO.Description.ToUpper());

                            return treasuryDTO;
                        }
                        else
                        {
                            var range = new Range(treasuryDTO.RangeLowerLimit, treasuryDTO.RangeUpperLimit);

                            var treasury = TreasuryFactory.CreateTreasury(treasuryDTO.BranchId, treasuryDTO.ChartOfAccountId, treasuryDTO.Description, range);

                            treasury.Code = (short)_treasuryRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Code),0) + 1 AS Expr1 FROM {0}Treasuries", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                            if (treasuryDTO.IsLocked)
                                treasury.Lock();
                            else treasury.UnLock();

                            _treasuryRepository.Add(treasury, serviceHeader);

                            dbContextScope.SaveChanges(serviceHeader);

                            return treasury.ProjectedAs<TreasuryDTO>();
                        }
                    }
                }
            }
            else return null;
        }

        public bool UpdateTreasury(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader)
        {
            if (treasuryDTO == null || treasuryDTO.Id == Guid.Empty || treasuryDTO.BranchId == Guid.Empty || treasuryDTO.ChartOfAccountId == Guid.Empty)
                return false;

            ValidateTreasury(treasuryDTO, serviceHeader);
            treasuryDTO.Description = treasuryDTO.Description.Trim();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _treasuryRepository.Get(treasuryDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var duplicateDescription = _treasuryRepository.AllMatching(
                        TreasurySpecifications.DescriptionWithDescription(treasuryDTO.Description), serviceHeader);
                    if (duplicateDescription != null && duplicateDescription.Any(x => x.Id != treasuryDTO.Id))
                        throw new InvalidOperationException(string.Format("Treasury '{0}' already exists.", treasuryDTO.Description));

                    var range = new Range(treasuryDTO.RangeLowerLimit, treasuryDTO.RangeUpperLimit);

                    var current = TreasuryFactory.CreateTreasury(persisted.BranchId, treasuryDTO.ChartOfAccountId, treasuryDTO.Description, range);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Code = persisted.Code;


                    if (treasuryDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _treasuryRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        private void ValidateTreasury(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader)
        {
            if (treasuryDTO == null)
                throw new ArgumentNullException(nameof(treasuryDTO), "Treasury details are required.");
            if (string.IsNullOrWhiteSpace(treasuryDTO.Description))
                throw new InvalidOperationException("Treasury name is required.");
            if (treasuryDTO.Description.Trim().Length > 256)
                throw new InvalidOperationException("Treasury name cannot exceed 256 characters.");
            if (treasuryDTO.BranchId == Guid.Empty)
                throw new InvalidOperationException("A branch is required.");
            if (treasuryDTO.ChartOfAccountId == Guid.Empty)
                throw new InvalidOperationException("A G/L account is required.");
            if (treasuryDTO.RangeLowerLimit < 0m || treasuryDTO.RangeUpperLimit < 0m)
                throw new InvalidOperationException("Treasury limits cannot be negative.");
            if (treasuryDTO.RangeUpperLimit <= treasuryDTO.RangeLowerLimit)
                throw new InvalidOperationException("The upper limit must be greater than the lower limit.");

            var branch = _branchRepository.Get(treasuryDTO.BranchId, serviceHeader);
            if (branch == null)
                throw new InvalidOperationException("The selected branch could not be found.");
            if (branch.IsLocked)
                throw new InvalidOperationException("The selected branch is locked and cannot be assigned to a treasury.");

            var chartOfAccount = _chartOfAccountRepository.Get(treasuryDTO.ChartOfAccountId, serviceHeader);
            if (chartOfAccount == null)
                throw new InvalidOperationException("The selected G/L account could not be found.");
            if (chartOfAccount.IsLocked)
                throw new InvalidOperationException("The selected G/L account is locked and cannot be assigned to a treasury.");
            if (chartOfAccount.IsControlAccount)
                throw new InvalidOperationException("A control G/L account cannot be assigned directly to a treasury.");
        }

        public List<TreasuryDTO> FindTreasuries(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var treasuries = _treasuryRepository.GetAll(serviceHeader);

                if (treasuries != null && treasuries.Any())
                {
                    return treasuries.ProjectedAsCollection<TreasuryDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<TreasuryDTO> FindTreasuries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TreasurySpecifications.DefaultSpec();

                ISpecification<Treasury> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var treasuryPagedCollection = _treasuryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (treasuryPagedCollection != null)
                {
                    var pageCollection = treasuryPagedCollection.PageCollection.ProjectedAsCollection<TreasuryDTO>();

                    var itemsCount = treasuryPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TreasuryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<TreasuryDTO> FindTreasuries(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TreasurySpecifications.TreasuryFullText(text);

                ISpecification<Treasury> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var treasuryCollection = _treasuryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (treasuryCollection != null)
                {
                    var pageCollection = treasuryCollection.PageCollection.ProjectedAsCollection<TreasuryDTO>();

                    var itemsCount = treasuryCollection.ItemsCount;

                    return new PageCollectionInfo<TreasuryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public TreasuryDTO FindTreasury(Guid treasuryId, ServiceHeader serviceHeader)
        {
            if (treasuryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var treasury = _treasuryRepository.Get(treasuryId, serviceHeader);

                    if (treasury != null)
                    {
                        return treasury.ProjectedAs<TreasuryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public TreasuryDTO FindTreasuryByBranchId(Guid branchId, ServiceHeader serviceHeader)
        {
            if (branchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = TreasurySpecifications.TreasuryWithBranchId(branchId);

                    ISpecification<Treasury> spec = filter;

                    var treasuries = _treasuryRepository.AllMatching(spec, serviceHeader);

                    if (treasuries != null && treasuries.Any())
                    {
                        var treasuryDTOs = treasuries.ProjectedAsCollection<TreasuryDTO>();

                        if (treasuryDTOs != null && treasuryDTOs.Any())
                        {
                            return (treasuryDTOs.Count == 1) ? treasuryDTOs[0] : null;
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public void FetchTreasuryBalances(List<TreasuryDTO> treasuries, ServiceHeader serviceHeader)
        {
            if (treasuries != null && treasuries.Any())
            {
                treasuries.ForEach(treasury =>
                {
                    treasury.BookBalance = _sqlCommandAppService.FindGlAccountBalance(treasury.ChartOfAccountId, DateTime.Now, (int)TransactionDateFilter.CreatedDate, serviceHeader);
                });
            }
        }

        public string ValidateCashMovement(Guid activeTreasuryId, Guid? destinationTreasuryId, decimal amount, int transactionType, ServiceHeader serviceHeader)
        {
            if (activeTreasuryId == Guid.Empty)
                return "The active treasury could not be identified.";

            if (amount <= 0m)
                return "The transaction amount must be greater than zero.";

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var activeTreasury = _treasuryRepository.Get(activeTreasuryId, serviceHeader);
                if (activeTreasury == null)
                    return "The active treasury could not be found.";

                var activeBalance = _sqlCommandAppService.FindGlAccountBalance(
                    activeTreasury.ChartOfAccountId,
                    DateTime.Now,
                    (int)TransactionDateFilter.CreatedDate,
                    serviceHeader);

                switch ((TreasuryTransactionType)transactionType)
                {
                    case TreasuryTransactionType.BankToTreasury:
                    case TreasuryTransactionType.TellerToTreasury:
                        if (activeBalance + amount > activeTreasury.Range.UpperLimit)
                            return string.Format(
                                "The transaction would increase treasury '{0}' above its upper limit of {1:N2}.",
                                activeTreasury.Description,
                                activeTreasury.Range.UpperLimit);
                        break;

                    case TreasuryTransactionType.TreasuryToBank:
                    case TreasuryTransactionType.TreasuryToTeller:
                        if (activeBalance - amount < activeTreasury.Range.LowerLimit)
                            return string.Format(
                                "The transaction would reduce treasury '{0}' below its lower limit of {1:N2}.",
                                activeTreasury.Description,
                                activeTreasury.Range.LowerLimit);
                        break;

                    case TreasuryTransactionType.TreasuryToTreasury:
                        if (activeBalance - amount < activeTreasury.Range.LowerLimit)
                            return string.Format(
                                "The transaction would reduce treasury '{0}' below its lower limit of {1:N2}.",
                                activeTreasury.Description,
                                activeTreasury.Range.LowerLimit);

                        if (!destinationTreasuryId.HasValue || destinationTreasuryId.Value == Guid.Empty)
                            return "The receiving treasury could not be identified.";

                        var destinationTreasury = _treasuryRepository.Get(destinationTreasuryId.Value, serviceHeader);
                        if (destinationTreasury == null)
                            return "The receiving treasury could not be found.";

                        if (destinationTreasury.Id == activeTreasury.Id)
                            return "The source and destination treasury must be different.";

                        var destinationBalance = _sqlCommandAppService.FindGlAccountBalance(
                            destinationTreasury.ChartOfAccountId,
                            DateTime.Now,
                            (int)TransactionDateFilter.CreatedDate,
                            serviceHeader);

                        if (destinationBalance + amount > destinationTreasury.Range.UpperLimit)
                            return string.Format(
                                "The transaction would increase treasury '{0}' above its upper limit of {1:N2}.",
                                destinationTreasury.Description,
                                destinationTreasury.Range.UpperLimit);
                        break;

                    default:
                        return "The treasury transaction type is not supported.";
                }

                return null;
            }
        }
    }
}
