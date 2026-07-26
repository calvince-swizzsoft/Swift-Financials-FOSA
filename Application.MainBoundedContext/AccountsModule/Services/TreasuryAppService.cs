using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.TreasuryAgg;
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
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public TreasuryAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Treasury> treasuryRepository,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (treasuryRepository == null)
                throw new ArgumentNullException(nameof(treasuryRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _treasuryRepository = treasuryRepository;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public TreasuryDTO AddNewTreasury(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader)
        {
            if (treasuryDTO != null && treasuryDTO.BranchId != Guid.Empty && treasuryDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var filter = TreasurySpecifications.TreasuryWithBranchId(treasuryDTO.BranchId);

                    ISpecification<Treasury> spec = filter;

                    var treasuries = _treasuryRepository.AllMatching(spec, serviceHeader);

                    if (treasuries != null && treasuries.Any())
                    {
                        treasuryDTO.ErrorMessageResult = string.Format("Sorry, but another treasury has already been linked to branch {0}", treasuryDTO.BranchDescription);

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

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _treasuryRepository.Get(treasuryDTO.Id, serviceHeader);

                if (persisted != null)
                {
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
    }
}
