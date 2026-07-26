using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class AdministrativeDivisionAppService: IAdministrativeDivisionAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AdministrativeDivision> _administrativeDivisionRepository;

        public AdministrativeDivisionAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<AdministrativeDivision> administrativeDivisionDTORepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (administrativeDivisionDTORepository == null)
                throw new ArgumentNullException(nameof(administrativeDivisionDTORepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _administrativeDivisionRepository = administrativeDivisionDTORepository;
        }

        public async Task<AdministrativeDivisionDTO> AddNewAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO, ServiceHeader serviceHeader)
        {
            var administrativeDivisionBindingModel = administrativeDivisionDTO.ProjectedAs<AdministrativeDivisionBindingModel>();

            var administrativeDivisionParentId = Guid.NewGuid();

            if (administrativeDivisionBindingModel.ParentId == Guid.Empty || administrativeDivisionBindingModel.ParentId == null)
                administrativeDivisionBindingModel.ParentId = administrativeDivisionParentId;

            administrativeDivisionBindingModel.ValidateAll();

            if (administrativeDivisionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, administrativeDivisionBindingModel.ErrorMessages));

            if (administrativeDivisionBindingModel.ParentId == administrativeDivisionParentId)
                administrativeDivisionBindingModel.ParentId = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var administrativeDivision = AdministrativeDivisionFactory.CreateAdministrativeDivision(administrativeDivisionDTO.ParentId, administrativeDivisionDTO.Description, administrativeDivisionDTO.Type, administrativeDivisionDTO.Remarks);

                administrativeDivision.CreatedBy = serviceHeader.ApplicationUserName;

                if (administrativeDivisionDTO.IsLocked)
                    administrativeDivision.Lock();
                else administrativeDivision.UnLock();

                _administrativeDivisionRepository.Add(administrativeDivision, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? administrativeDivision.ProjectedAs<AdministrativeDivisionDTO>() : null;
            }
        }

        public async Task<bool> UpdateAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO, ServiceHeader serviceHeader)
        {
            var administrativeDivisionBindingModel = administrativeDivisionDTO.ProjectedAs<AdministrativeDivisionBindingModel>();

            var administrativeDivisionParentId = Guid.NewGuid();

            if (administrativeDivisionBindingModel.ParentId == Guid.Empty || administrativeDivisionBindingModel.ParentId == null)
                administrativeDivisionBindingModel.ParentId = administrativeDivisionParentId;

            administrativeDivisionBindingModel.ValidateAll();

            if (administrativeDivisionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, administrativeDivisionBindingModel.ErrorMessages));

            if (administrativeDivisionBindingModel.ParentId == administrativeDivisionParentId)
                administrativeDivisionBindingModel.ParentId = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _administrativeDivisionRepository.GetAsync(administrativeDivisionDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = AdministrativeDivisionFactory.CreateAdministrativeDivision(administrativeDivisionDTO.ParentId, administrativeDivisionDTO.Description, administrativeDivisionDTO.Type, administrativeDivisionDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (administrativeDivisionDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _administrativeDivisionRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AdministrativeDivisionSpecifications.ParentAdministrativeDivisions();

                ISpecification<AdministrativeDivision> spec = filter;

                return await _administrativeDivisionRepository.AllMatchingAsync<AdministrativeDivisionDTO>(spec, serviceHeader, c => c.Children);
            }
        }

        public async Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AdministrativeDivisionSpecifications.DefaultSpec();

                ISpecification<AdministrativeDivision> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _administrativeDivisionRepository.AllMatchingPagedAsync<AdministrativeDivisionDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AdministrativeDivisionSpecifications.AdministrativeDivisionFullText(text);

                ISpecification<AdministrativeDivision> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _administrativeDivisionRepository.AllMatchingPagedAsync<AdministrativeDivisionDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public AdministrativeDivisionDTO FindAdministrativeDivision(Guid administrativeDivisionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    return _administrativeDivisionRepository.Get(administrativeDivisionId, serviceHeader).ProjectedAs<AdministrativeDivisionDTO>();
                }
            }
        }

        public List<AdministrativeDivisionDTO> FindAdministrativeDivisions(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AdministrativeDivisionSpecifications.ParentAdministrativeDivisions();

                ISpecification<AdministrativeDivision> spec = filter;

                var administrativeDivisions = _administrativeDivisionRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (administrativeDivisions != null && administrativeDivisions.Any())
                {
                    var administrativeDivisionDTOList = new List<AdministrativeDivisionDTO>();

                    Action<AdministrativeDivisionDTO> traverse = null;

                    /* recursive lambda */
                    traverse = (node) =>
                    {
                        int depth = 0;

                        var tempNode = node;
                        while (tempNode.Parent != null)
                        {
                            tempNode = tempNode.Parent;
                            depth++;
                        }

                        var administrativeDivisionDTO = new AdministrativeDivisionDTO();
                        administrativeDivisionDTO.Id = node.Id;
                        administrativeDivisionDTO.ParentId = node.ParentId;
                        administrativeDivisionDTO.Description = node.Description;
                        administrativeDivisionDTO.Type = node.Type;
                        administrativeDivisionDTO.Depth = depth;
                        administrativeDivisionDTO.Remarks = node.Remarks;
                        administrativeDivisionDTO.IsLocked = node.IsLocked;
                        administrativeDivisionDTO.CreatedDate = node.CreatedDate;

                        administrativeDivisionDTOList.Add(administrativeDivisionDTO);

                        if (node.Children != null)
                        {
                            foreach (var item in node.Children)
                            {
                                traverse(item);
                            }
                        }
                    };

                    if (traverseTree)
                    {
                        foreach (var c in administrativeDivisions)
                        {
                            traverse(c.ProjectedAs<AdministrativeDivisionDTO>());
                        }
                    }
                    else
                    {
                        administrativeDivisionDTOList = administrativeDivisions.ProjectedAsCollection<AdministrativeDivisionDTO>();
                    }

                    return administrativeDivisionDTOList;
                }
                else return null;
            }
        }
    }
}
