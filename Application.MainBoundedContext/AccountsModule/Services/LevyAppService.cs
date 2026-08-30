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

        public LevyDTO AddNewLevyConfiguration(LevyDTO levyDTO, List<LevySplitDTO> levySplits, ServiceHeader serviceHeader)
        {
            if (levySplits == null || !levySplits.Any()) throw new InvalidOperationException("At least one G/L split is required to create a levy.");
            ValidateLevyConfiguration(levyDTO, levySplits);
            using (var scope = _dbContextScopeFactory.Create())
            {
                var created = AddNewLevy(levyDTO, serviceHeader);
                if (created == null || !string.IsNullOrWhiteSpace(created.ErrorMessageResult)) return created;
                if (levySplits != null && !UpdateLevySplits(created.Id, levySplits, serviceHeader)) throw new InvalidOperationException("Levy G/L splits could not be saved.");
                scope.SaveChanges(serviceHeader);
                return FindLevy(created.Id, serviceHeader);
            }
        }

        public void ValidateLevyConfiguration(LevyDTO levyDTO, List<LevySplitDTO> levySplits)
        {
            if (levyDTO == null) throw new InvalidOperationException("Levy data is required.");
            if (string.IsNullOrWhiteSpace(levyDTO.Description)) throw new InvalidOperationException("Levy description is required.");
            if (!Enum.IsDefined(typeof(ChargeType), levyDTO.ChargeType)) throw new InvalidOperationException("Select a valid levy charge type.");
            if (double.IsNaN(levyDTO.ChargePercentage) || double.IsInfinity(levyDTO.ChargePercentage) || levyDTO.ChargePercentage < 0d || levyDTO.ChargePercentage > 100d || levyDTO.ChargeFixedAmount < 0m)
                throw new InvalidOperationException("Levy charge values must be valid and non-negative.");
            if (levyDTO.ChargeType == (int)ChargeType.Percentage && levyDTO.ChargePercentage <= 0d)
                throw new InvalidOperationException("A percentage levy requires a rate greater than 0% and no more than 100%.");
            if (levyDTO.ChargeType == (int)ChargeType.FixedAmount && levyDTO.ChargeFixedAmount <= 0m)
                throw new InvalidOperationException("A fixed-amount levy requires an amount greater than zero.");
            ValidateLevySplitRows(levySplits);
        }

        private static void ValidateLevySplitRows(IEnumerable<LevySplitDTO> levySplits)
        {
            if (levySplits == null) return;
            var items = levySplits.ToList();
            if (items.Any(item => item == null || item.ChartOfAccountId == Guid.Empty || string.IsNullOrWhiteSpace(item.Description) ||
                double.IsNaN(item.Percentage) || double.IsInfinity(item.Percentage) || item.Percentage <= 0d || item.Percentage > 100d))
                throw new InvalidOperationException("Every levy G/L split requires an account, description, and percentage greater than 0% and no more than 100%.");
            var total = items.Sum(item => item.Percentage);
            if (items.Any() && Math.Abs(total - 100d) > 0.01d)
                throw new InvalidOperationException(string.Format("Total levy split percentage must equal 100% (got {0}%).", total));
        }

        public LevyDTO AddNewLevy(LevyDTO levyDTO, ServiceHeader serviceHeader)
        {
            ValidateLevyConfiguration(levyDTO, null);
            if (levyDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var matched = _levyRepository.AllMatching(LevySpecifications.LevyWithDescription(levyDTO.Description.Trim()), serviceHeader);
                    if (matched != null && matched.Any())
                    {
                        levyDTO.ErrorMessageResult = string.Format("A levy named \"{0}\" already exists.", levyDTO.Description.Trim());
                        return levyDTO;
                    }
                    var charge = new Charge(levyDTO.ChargeType, levyDTO.ChargePercentage, levyDTO.ChargeFixedAmount);

                    var levy = LevyFactory.CreateLevy(levyDTO.Description.Trim(), charge);

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
            ValidateLevyConfiguration(levyDTO, null);
            if (levyDTO == null || levyDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _levyRepository.Get(levyDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var matched = _levyRepository.AllMatching(LevySpecifications.LevyWithDescription(levyDTO.Description.Trim()), serviceHeader);
                    if (matched != null && matched.Any(item => item.Id != persisted.Id))
                        throw new InvalidOperationException(string.Format("A levy named \"{0}\" already exists.", levyDTO.Description.Trim()));
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
            ValidateLevySplitRows(levySplits);
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
