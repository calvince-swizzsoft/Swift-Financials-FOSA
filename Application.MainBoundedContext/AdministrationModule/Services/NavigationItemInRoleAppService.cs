using Application.MainBoundedContext.DTO.AdministrationModule;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemInRoleAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class NavigationItemInRoleAppService : INavigationItemInRoleAppService
    {
        private readonly IRepository<NavigationItemInRole> _NavigationItemInRoleRepository;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public NavigationItemInRoleAppService(IRepository<NavigationItemInRole> NavigationItemInRoleRepository,
            IDbContextScopeFactory dbContextScopeFactory)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _NavigationItemInRoleRepository = NavigationItemInRoleRepository ?? throw new ArgumentNullException(nameof(NavigationItemInRoleRepository));
        }

        public async Task<bool> AddNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO, ServiceHeader serviceHeader)
        {
            if (navigationItemInRoleDTO == null) return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var navigationItemInRole = NavigationItemInRoleFactory.CreateNavigationItemInRole(navigationItemInRoleDTO.NavigationItemId, navigationItemInRoleDTO.RoleName);

                navigationItemInRole.CreatedBy = serviceHeader.ApplicationUserName;

                _NavigationItemInRoleRepository.Add(navigationItemInRole, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        #region NavigationItemInRoleDTO

        public Task<bool> AddNavigationItemToRolesAsync(NavigationItemDTO navigationItemDTO, string[] roleNames, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public async Task<List<NavigationItemInRoleDTO>> GetNavigationItemsInRoleAsync(string roleName, ServiceHeader serviceHeader)
        {
            if (roleName == null)
                return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = NavigationItemInRoleSpecifications.ModuleNavigationRoleName(roleName);

                ISpecification<NavigationItemInRole> spec = filter;

                return await _NavigationItemInRoleRepository.AllMatchingAsync<NavigationItemInRoleDTO>(spec, serviceHeader);
            }
        }

        public async Task<string[]> GetRolesForNavigationItemCodeAsync(int navigationItemCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<NavigationItemInRole> spec = NavigationItemInRoleSpecifications.NavigationItemCode(navigationItemCode);

                var NavigationItemsInRoles = await _NavigationItemInRoleRepository.AllMatchingAsync(spec, serviceHeader);

                if (NavigationItemsInRoles != null && NavigationItemsInRoles.Any())
                {
                    return (from m in NavigationItemsInRoles
                            select m.RoleName).ToArray();
                }
                else return null;
            }
        }

        public async Task<bool> IsNavigationItemInRoleAsync(int navigationItemCode, string roleName, ServiceHeader serviceHeader)
        {
            if (roleName == null)
                return false;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<NavigationItemInRole> spec = NavigationItemInRoleSpecifications.NavigationItemCodeAndRole(navigationItemCode, roleName);

                var NavigationItemsInRoles = await _NavigationItemInRoleRepository.AllMatchingAsync(spec, serviceHeader);

                return NavigationItemsInRoles.Any();
            }
        }

        public Task<bool> MapNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MapNavigationItemToRolesAsync(NavigationItemDTO navigationItemDTO, string[] roleNames, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveNavigationItemFromRolesAsync(NavigationItemDTO navigationItemDTO, string[] roleNames, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveNavigationItemRoleAsync(Guid navigationItemId, string roleName, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                ISpecification<NavigationItemInRole> spec = NavigationItemInRoleSpecifications.NavigationItemAndRoleName(navigationItemId, roleName);

                var NavigationItemInRole = await _NavigationItemInRoleRepository.AllMatchingAsync(spec, serviceHeader);

                NavigationItemInRole.ForEach(x => _NavigationItemInRoleRepository.Remove(x, serviceHeader));

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public async Task<bool> ValidateModuleAccessAsync(string controllerName, string roleName, ServiceHeader serviceHeader)
        {
            if (roleName == null || controllerName == null)
                return false;
            else
            {
                using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
                {
                    ISpecification<NavigationItemInRole> spec = NavigationItemInRoleSpecifications.ControllerNameAndRole(controllerName, roleName);

                    var NavigationItemsInRole = await _NavigationItemInRoleRepository.AllMatchingAsync(spec, serviceHeader);

                    return NavigationItemsInRole.Any();
                }
            }
        }

        #endregion
    }
}