using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class BranchAppService : IBranchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IAppCache _appCache;

        public BranchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Branch> branchRepository,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (branchRepository == null)
                throw new ArgumentNullException(nameof(branchRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _branchRepository = branchRepository;
            _appCache = appCache;
        }

        public BranchDTO AddNewBranch(BranchDTO branchDTO, ServiceHeader serviceHeader)
        {
            if (branchDTO == null)
                return null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var address = new Address(branchDTO.AddressAddressLine1, branchDTO.AddressAddressLine2, branchDTO.AddressStreet, branchDTO.AddressPostalCode, branchDTO.AddressCity, branchDTO.AddressEmail, branchDTO.AddressLandLine, branchDTO.AddressMobileLine);

                var branch = BranchFactory.CreateBranch(branchDTO.CompanyId, branchDTO.Description, address);

                branch.Code = (short)_branchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Code),0) + 1 AS Expr1 FROM {0}Branches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                if (branchDTO.IsLocked)
                    branch.Lock();
                else branch.UnLock();

                _branchRepository.Add(branch, serviceHeader);

                dbContextScope.SaveChanges(serviceHeader);

                return branch.ProjectedAs<BranchDTO>();
            }
        }

        public bool UpdateBranch(BranchDTO branchDTO, ServiceHeader serviceHeader)
        {
            if (branchDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _branchRepository.Get(branchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var address = new Address(branchDTO.AddressAddressLine1, branchDTO.AddressAddressLine2, branchDTO.AddressStreet, branchDTO.AddressPostalCode, branchDTO.AddressCity, branchDTO.AddressEmail, branchDTO.AddressLandLine, branchDTO.AddressMobileLine);

                    var current = BranchFactory.CreateBranch(branchDTO.CompanyId, branchDTO.Description, address);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Code = persisted.Code;


                    if (branchDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _branchRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }


        public async Task<bool> UpdateBranchAsync(BranchDTO branchDTO, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var result = default(bool);

                if (branchDTO != null)
                {
                    var persisted = _branchRepository.Get(branchDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var address = new Address(branchDTO.AddressAddressLine1, branchDTO.AddressAddressLine2, branchDTO.AddressStreet, branchDTO.AddressPostalCode, branchDTO.AddressCity, branchDTO.AddressEmail, branchDTO.AddressLandLine, branchDTO.AddressMobileLine);

                        var current = BranchFactory.CreateBranch(branchDTO.CompanyId, branchDTO.Description, address);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                        current.Code = persisted.Code;

                        if (branchDTO.IsLocked)
                            current.Lock();
                        else current.UnLock();

                        _branchRepository.Merge(persisted, current, serviceHeader);

                        result = await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                    }
                }

                return result;
            }
        }

        public List<BranchDTO> FindBranches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<Branch> spec = BranchSpecifications.DefaultSpec();

                var branches = _branchRepository.AllMatching(spec, serviceHeader);

                if (branches != null && branches.Any())
                {
                    return branches.ProjectedAsCollection<BranchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<BranchDTO> FindBranches(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BranchSpecifications.DefaultSpec();

                ISpecification<Branch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var branchPagedCollection = _branchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (branchPagedCollection != null)
                {
                    var pageCollection = branchPagedCollection.PageCollection.ProjectedAsCollection<BranchDTO>();

                    var itemsCount = branchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BranchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<BranchDTO> FindBranches(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BranchSpecifications.BranchFullText(text);

                ISpecification<Branch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var branchCollection = _branchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (branchCollection != null)
                {
                    var pageCollection = branchCollection.PageCollection.ProjectedAsCollection<BranchDTO>();

                    var itemsCount = branchCollection.ItemsCount;

                    return new PageCollectionInfo<BranchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public BranchDTO FindBranch(Guid branchId, ServiceHeader serviceHeader)
        {
            if (branchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var branch = _branchRepository.Get(branchId, serviceHeader);

                    if (branch != null)
                    {
                        return branch.ProjectedAs<BranchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public BranchDTO FindBranch(int branchCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BranchSpecifications.BranchCode(branchCode);

                ISpecification<Branch> spec = filter;

                var branches = _branchRepository.AllMatching(spec, serviceHeader);

                if (branches != null && branches.Any())
                {
                    return branches.ProjectedAsCollection<BranchDTO>()[0];
                }
                else return null;
            }
        }

        public BranchDTO FindCachedBranch(Guid branchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<BranchDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, branchId.ToString("D")), () =>
            {
                return FindBranch(branchId, serviceHeader);
            });
        }
    }
}
