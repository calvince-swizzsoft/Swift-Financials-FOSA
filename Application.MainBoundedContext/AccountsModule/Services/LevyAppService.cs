using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
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
    public class LevyAppService : ILevyAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Levy> _levyRepository;
        private readonly IRepository<LevySplit> _levySplitRepository;

        public LevyAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Levy> levyRepository,
           IRepository<LevySplit> levySplitRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (levyRepository == null)
                throw new ArgumentNullException(nameof(levyRepository));

            if (levySplitRepository == null)
                throw new ArgumentNullException(nameof(levySplitRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _levyRepository = levyRepository;
            _levySplitRepository = levySplitRepository;
        }

        public LevyDTO AddNewLevy(LevyDTO levyDTO, ServiceHeader serviceHeader)
        {
            if (levyDTO != null)
            {
                levyDTO.ErrorMessageResult = string.Format("Charge Percentage cant be more than "+ levyDTO.ChargePercentage);
                
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var charge = new Charge(levyDTO.ChargeType, levyDTO.ChargePercentage, levyDTO.ChargeFixedAmount);

                    var levy = LevyFactory.CreateLevy(levyDTO.Description, charge);

                    if (levyDTO.IsLocked)
                        levy.Lock();
                    else levy.UnLock();

                    _levyRepository.Add(levy, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return levy.ProjectedAs<LevyDTO>();
                }
            }
            else return null;
        }

        public bool UpdateLevy(LevyDTO levyDTO, ServiceHeader serviceHeader)
        {
            if (levyDTO == null || levyDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _levyRepository.Get(levyDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var charge = new Charge(levyDTO.ChargeType, levyDTO.ChargePercentage, levyDTO.ChargeFixedAmount);

                    var current = LevyFactory.CreateLevy(levyDTO.Description, charge);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    
                    if (levyDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _levyRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<LevyDTO> FindLevies(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var levies = _levyRepository.GetAll(serviceHeader);

                if (levies != null && levies.Any())
                {
                    return levies.ProjectedAsCollection<LevyDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LevyDTO> FindLevies(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LevySpecifications.DefaultSpec();

                ISpecification<Levy> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var levyPagedCollection = _levyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (levyPagedCollection != null)
                {
                    var pageCollection = levyPagedCollection.PageCollection.ProjectedAsCollection<LevyDTO>();

                    var itemsCount = levyPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LevyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LevyDTO> FindLevies(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LevySpecifications.LevyFullText(text);

                ISpecification<Levy> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var levyCollection = _levyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (levyCollection != null)
                {
                    var pageCollection = levyCollection.PageCollection.ProjectedAsCollection<LevyDTO>();

                    var itemsCount = levyCollection.ItemsCount;

                    return new PageCollectionInfo<LevyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LevyDTO FindLevy(Guid levyId, ServiceHeader serviceHeader)
        {
            if (levyId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var levy = _levyRepository.Get(levyId, serviceHeader);

                    if (levy != null)
                    {
                        return levy.ProjectedAs<LevyDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LevySplitDTO> FindLevySplits(Guid levyId, ServiceHeader serviceHeader)
        {
            if (levyId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LevySplitSpecifications.LevySplitWithLevyId(levyId);

                    ISpecification<LevySplit> spec = filter;

                    var levySplits = _levySplitRepository.AllMatching(spec, serviceHeader);

                    if (levySplits != null)
                    {
                        return levySplits.ProjectedAsCollection<LevySplitDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateLevySplits(Guid levyId, List<LevySplitDTO> levySplits, ServiceHeader serviceHeader)
        {
            if (levyId != null && levySplits != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _levyRepository.Get(levyId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLevySplits(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var levySplit = _levySplitRepository.Get(item.Id, serviceHeader);

                                if (levySplit != null)
                                {
                                    _levySplitRepository.Remove(levySplit, serviceHeader);
                                }
                            }
                        }

                        if (levySplits.Any())
                        {
                            foreach (var item in levySplits)
                            {
                                var levySplit = LevySplitFactory.CreateLevySplit(persisted.Id, item.ChartOfAccountId, item.Description, item.Percentage);

                                _levySplitRepository.Add(levySplit, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }
    }
}
