using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.EmployeeInBranchAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemInRoleAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInBranchAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInRoleAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class AuthorizationAppService : IAuthorizationAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ModuleNavigationItem> _moduleNavigationItemRepository;
        private readonly IRepository<ModuleNavigationItemInRole> _moduleNavigationItemInRoleRepository;
        private readonly IRepository<SystemPermissionTypeInRole> _systemPermissionTypeInRoleRepository;
        private readonly IRepository<SystemPermissionTypeInBranch> _systemPermissionTypeInBranchRepository;
        private readonly IRepository<EmployeeInBranch> _employeeInBranchRepository;

        public AuthorizationAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<ModuleNavigationItem> moduleNavigationItemRepository,
            IRepository<ModuleNavigationItemInRole> moduleNavigationItemInRoleRepository,
            IRepository<SystemPermissionTypeInRole> systemPermissionTypeInRoleRepository,
            IRepository<SystemPermissionTypeInBranch> systemPermissionTypeInBranchRepository,
            IRepository<EmployeeInBranch> employeeInBranchRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (moduleNavigationItemRepository == null)
                throw new ArgumentNullException(nameof(moduleNavigationItemRepository));

            if (moduleNavigationItemInRoleRepository == null)
                throw new ArgumentNullException(nameof(moduleNavigationItemInRoleRepository));

            if (systemPermissionTypeInRoleRepository == null)
                throw new ArgumentNullException(nameof(systemPermissionTypeInRoleRepository));

            if (systemPermissionTypeInBranchRepository == null)
                throw new ArgumentNullException(nameof(systemPermissionTypeInBranchRepository));

            if (employeeInBranchRepository == null)
                throw new ArgumentNullException(nameof(employeeInBranchRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _moduleNavigationItemRepository = moduleNavigationItemRepository;
            _moduleNavigationItemInRoleRepository = moduleNavigationItemInRoleRepository;
            _systemPermissionTypeInRoleRepository = systemPermissionTypeInRoleRepository;
            _systemPermissionTypeInBranchRepository = systemPermissionTypeInBranchRepository;
            _employeeInBranchRepository = employeeInBranchRepository;
        }

        public bool AddNewModuleNavigationItems(List<ModuleNavigationItemDTO> moduleNavigationItems, ServiceHeader serviceHeader)
        {
            if (moduleNavigationItems == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                moduleNavigationItems.ToList().ForEach(item =>
                {
                    if (item == null) return;

                    // Get the specification
                    ISpecification<ModuleNavigationItem> spec = ModuleNavigationItemSpecifications.ModuleNavigationItemCode(item.Code);

                    // Query this criteria
                    var matchedNavigationItems = _moduleNavigationItemRepository.AllMatching(spec, serviceHeader);

                    if (matchedNavigationItems != null && matchedNavigationItems.Any())
                    {
                        foreach (var matchedNavigationItem in matchedNavigationItems)
                        {
                            // nb: need to retrieve from db with change tracking! izmoto 13.07.2013
                            var persisted = _moduleNavigationItemRepository.Get(matchedNavigationItem.Id, serviceHeader);

                            // manufacture entity via factory
                            var current = ModuleNavigationItemFactory.CreateModuleNavigationItem(item.ParentId, item.Description, item.Icon, item.Code,item.ControllerName, item.ActionName, item.AreaCode,item.AreaName);

                            // reset identity
                            current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                            current.CreatedBy = serviceHeader.ApplicationUserName;

                            // merge changes
                            _moduleNavigationItemRepository.Merge(persisted, current, serviceHeader);
                        }
                    }
                    else
                    {
                        // Create account type from factory and set persistent id
                        var moduleNavigationItem = ModuleNavigationItemFactory.CreateModuleNavigationItem(item.ParentId, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, item.AreaCode, item.AreaName);

                        moduleNavigationItem.CreatedBy = serviceHeader.ApplicationUserName;

                        //add the moduleNavigationItem into the repository
                        _moduleNavigationItemRepository.Add(moduleNavigationItem, serviceHeader);
                    }
                });

                // commit changes
                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public string[] GetRolesForModuleNavigationItemCode(int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<ModuleNavigationItemInRole> spec = ModuleNavigationItemInRoleSpecifications.ModuleNavigationItemCode(moduleNavigationItemCode);

                var moduleNavigationItemsInRoles = _moduleNavigationItemInRoleRepository.AllMatching(spec, serviceHeader);

                if (moduleNavigationItemsInRoles != null && moduleNavigationItemsInRoles.Any())
                {
                    return (from m in moduleNavigationItemsInRoles
                            select m.RoleName).ToArray();
                }
                else return null;
            }
        }

        public List<ModuleNavigationItemInRoleDTO> GetModuleNavigationItemsInRole(string roleName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ModuleNavigationItemInRoleSpecifications.ModuleNavigationRoleName(roleName);

                ISpecification<ModuleNavigationItemInRole> spec = filter;

                var moduleNavigationItemInRoles = _moduleNavigationItemInRoleRepository.AllMatching(spec, serviceHeader);

                if (moduleNavigationItemInRoles != null && moduleNavigationItemInRoles.Any())
                {
                    return moduleNavigationItemInRoles.ProjectedAsCollection<ModuleNavigationItemInRoleDTO>();
                }
                else return null;
            }
        }

        public List<ModuleNavigationItemInRoleDTO> GetModuleNavigationItemsWithItemIdInRole(Guid moduleNavigationItemId, string roleName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ModuleNavigationItemInRoleSpecifications.ModuleNavigationItemAndRoleName(moduleNavigationItemId, roleName);

                ISpecification<ModuleNavigationItemInRole> spec = filter;

                var moduleNavigationItemInRoles = _moduleNavigationItemInRoleRepository.AllMatching(spec, serviceHeader);

                if (moduleNavigationItemInRoles != null && moduleNavigationItemInRoles.Any())
                {
                    return moduleNavigationItemInRoles.ProjectedAsCollection<ModuleNavigationItemInRoleDTO>();
                }
                else return null;
            }
        }

        public bool AddModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, ServiceHeader serviceHeader)
        {
            if (moduleNavigationItem == null) return false;

            if (roleNames != null && roleNames.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allNavItems = _moduleNavigationItemInRoleRepository.GetAll(serviceHeader);

                    Array.ForEach(roleNames, (item) =>
                    {
                        var matches = allNavItems.Where(x => x.ModuleNavigationItem.Code == moduleNavigationItem.Code && x.RoleName.Trim().ToUpper() == item.Trim().ToUpper());

                        if (matches.Any()) return;

                        // get the specification
                        ISpecification<ModuleNavigationItem> spec = ModuleNavigationItemSpecifications.ModuleNavigationItemCode(moduleNavigationItem.Code);

                        // Query this criteria
                        var navItem = _moduleNavigationItemRepository.AllMatching(spec, serviceHeader).FirstOrDefault();

                        if (navItem == null) return;

                        var moduleNavigationItemInRole = ModuleNavigationItemInRoleFactory.CreateModuleNavigationItemInRole(navItem.Id, item);

                        moduleNavigationItemInRole.CreatedBy = serviceHeader.ApplicationUserName;

                        _moduleNavigationItemInRoleRepository.Add(moduleNavigationItemInRole, serviceHeader);
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool RemoveModuleNavigationItemFromRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, ServiceHeader serviceHeader)
        {
            if (moduleNavigationItem == null) return false;

            if (roleNames != null && roleNames.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allNavItems = _moduleNavigationItemInRoleRepository.GetAll(serviceHeader);

                    Array.ForEach(roleNames, (item) =>
                    {
                        var matches = allNavItems.Where(x =>
                        {
                            var navItem = _moduleNavigationItemRepository.Get(x.ModuleNavigationItemId, serviceHeader);

                            if (navItem != null)
                                return navItem.Code == moduleNavigationItem.Code && x.RoleName.Trim().ToUpper() == item.Trim().ToUpper();
                            else return false;
                        });

                        if (matches.Any())
                        {
                            matches.ToList().ForEach(x => _moduleNavigationItemInRoleRepository.Remove(x, serviceHeader));
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool IsModuleNavigationItemInRole(ModuleNavigationItemDTO moduleNavigationItem, string roleName, ServiceHeader serviceHeader)
        {
            if (moduleNavigationItem == null) return false;

            if (roleName == null)
                return false;
            else
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var matches =
                        _moduleNavigationItemInRoleRepository.GetAll(serviceHeader).Where(
                            x =>
                            x.ModuleNavigationItem.Code == moduleNavigationItem.Code &&
                            x.RoleName.Trim().ToUpper() == roleName.Trim().ToUpper());

                    return matches.Any();
                }
            }
        }

        public List<ModuleNavigationItemDTO> GetModuleNavigationItemsWithItemCode(int itemCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ModuleNavigationItemSpecifications.ModuleNavigationItemCode(itemCode);

                ISpecification<ModuleNavigationItem> spec = filter;

                var matches = _moduleNavigationItemRepository.AllMatching(spec, serviceHeader);

                if (matches != null && matches.Any())
                {
                    return matches.ProjectedAsCollection<ModuleNavigationItemDTO>();
                }
                else return null;
            }
        }

        public string[] GetRolesForSystemPermissionType(int systemPermissionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SystemPermissionTypeInRole> spec = SystemPermissionTypeInRoleSpecifications.SystemPermissionType(systemPermissionType);

                var systemPermissionTypesInRoles = _systemPermissionTypeInRoleRepository.AllMatching(spec, serviceHeader);

                if (systemPermissionTypesInRoles != null && systemPermissionTypesInRoles.Any())
                {
                    return (from m in systemPermissionTypesInRoles
                            select m.RoleName).ToArray();
                }
                else return null;
            }
        }

        public List<SystemPermissionTypeInRoleDTO> GetRolesListForSystemPermissionType(int systemPermissionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SystemPermissionTypeInRole> spec = SystemPermissionTypeInRoleSpecifications.SystemPermissionType(systemPermissionType);

                var systemPermissionTypesInRoles = _systemPermissionTypeInRoleRepository.AllMatching(spec, serviceHeader);

                if (systemPermissionTypesInRoles != null && systemPermissionTypesInRoles.Any())
                {
                    return systemPermissionTypesInRoles.ProjectedAsCollection<SystemPermissionTypeInRoleDTO>();
                }
                else return null;
            }
        }

        public bool AddSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader)
        {
            if (rolesInSystemPermissionType != null && rolesInSystemPermissionType.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _systemPermissionTypeInRoleRepository.GetAll(serviceHeader);

                    foreach (var item in rolesInSystemPermissionType)
                    {
                        var matches = allAuthTypes.Where(x => x.SystemPermissionType == systemPermissionType && x.RoleName.Trim().ToUpper() == item.RoleName.Trim().ToUpper());

                        if (matches.Any())
                        {
                            // If a match is found, return false to indicate that the entry already exists
                            matches.ToList().ForEach(x => _systemPermissionTypeInRoleRepository.Add(x, serviceHeader));
                            return true;
                        }

                        var systemPermissionTypeInRole = SystemPermissionTypeInRoleFactory.CreateSystemPermissionTypeInRole(systemPermissionType, item.RoleName, item.RequiredApprovers, item.ApprovalPriority);

                        systemPermissionTypeInRole.CreatedBy = serviceHeader.ApplicationUserName;

                        _systemPermissionTypeInRoleRepository.Add(systemPermissionTypeInRole, serviceHeader);
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else
            {
                return false;
            }
        }

        public bool RemoveSystemPermissionTypeFromRoles(int systemPermissionType, string[] roleNames, ServiceHeader serviceHeader)
        {
            if (roleNames != null && roleNames.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _systemPermissionTypeInRoleRepository.GetAll(serviceHeader);

                    Array.ForEach(roleNames, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.SystemPermissionType == systemPermissionType && x.RoleName.Trim().ToUpper() == item.Trim().ToUpper());

                        if (matches.Any())
                        {
                            matches.ToList().ForEach(x => _systemPermissionTypeInRoleRepository.Remove(x, serviceHeader));
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool IsSystemPermissionTypeInRole(int systemPermissionType, string roleName, ServiceHeader serviceHeader)
        {
            if (roleName == null)
                return false;
            else
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SystemPermissionTypeInRoleSpecifications.SystemPermissionTypeAndRoleName(systemPermissionType, roleName);

                    // get the specification
                    ISpecification<SystemPermissionTypeInRole> spec = filter;

                    // Query this criteria
                    var matches = _systemPermissionTypeInRoleRepository.AllMatching(spec, serviceHeader);

                    return matches != null && matches.Any();
                }
            }
        }

        public bool MapModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var matches = GetModuleNavigationItemsWithItemCode(moduleNavigationItem.Code, serviceHeader);

                if (matches != null && matches.Any())
                {
                    var existingRoles = GetRolesForModuleNavigationItemCode(moduleNavigationItem.Code, serviceHeader);

                    var oldSet = from c in existingRoles ?? new string[] { string.Empty } select c;

                    var newSet = from c in roleNames ?? new string[] { string.Empty } select c;

                    var commonSet = oldSet.Intersect(newSet);

                    var insertSet = newSet.Except(commonSet);

                    var deleteSet = oldSet.Except(commonSet);

                    foreach (var mapping in matches)
                    {
                        foreach (var roleName in insertSet)
                        {
                            var moduleNavigationItemInRole = ModuleNavigationItemInRoleFactory.CreateModuleNavigationItemInRole(mapping.Id, roleName);

                            moduleNavigationItemInRole.CreatedBy = serviceHeader.ApplicationUserName;

                            _moduleNavigationItemInRoleRepository.Add(moduleNavigationItemInRole, serviceHeader);
                        }

                        foreach (var roleName in deleteSet)
                        {
                            var inRoleMatches = GetModuleNavigationItemsWithItemIdInRole(mapping.Id, roleName, serviceHeader);

                            if (inRoleMatches != null && inRoleMatches.Any())
                            {
                                foreach (var inRoleMapping in inRoleMatches)
                                {
                                    var rolePersisted = _moduleNavigationItemInRoleRepository.Get(inRoleMapping.Id, serviceHeader);

                                    _moduleNavigationItemInRoleRepository.Remove(rolePersisted, serviceHeader);
                                }
                            }
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool MapSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingRoles = GetRolesListForSystemPermissionType(systemPermissionType, serviceHeader);

                if (existingRoles != null && existingRoles.Any())
                {
                    var oldSet = from c in existingRoles ?? new List<SystemPermissionTypeInRoleDTO> { } select c;

                    var newSet = from c in rolesInSystemPermissionType ?? new List<SystemPermissionTypeInRoleDTO> { } select c;

                    var commonSet = oldSet.Intersect(newSet, new SystemPermissionTypeInRoleDTOEqualityComparer());

                    var insertSet = newSet.Except(commonSet, new SystemPermissionTypeInRoleDTOEqualityComparer());

                    var deleteSet = oldSet.Except(commonSet, new SystemPermissionTypeInRoleDTOEqualityComparer());

                    foreach (var role in insertSet)
                    {
                        var systemPermissionTypeInRole = SystemPermissionTypeInRoleFactory.CreateSystemPermissionTypeInRole(systemPermissionType, role.RoleName, role.RequiredApprovers, role.ApprovalPriority);

                        systemPermissionTypeInRole.CreatedBy = serviceHeader.ApplicationUserName;

                        _systemPermissionTypeInRoleRepository.Add(systemPermissionTypeInRole, serviceHeader);
                    }

                    foreach (var role in deleteSet)
                    {
                        var filter = SystemPermissionTypeInRoleSpecifications.SystemPermissionTypeAndRoleName(systemPermissionType, role.RoleName);

                        ISpecification<SystemPermissionTypeInRole> spec = filter;

                        var matches = _systemPermissionTypeInRoleRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _systemPermissionTypeInRoleRepository.Get(mapping.Id, serviceHeader);

                                _systemPermissionTypeInRoleRepository.Remove(persisted, serviceHeader);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var role in rolesInSystemPermissionType)
                    {
                        var systemPermissionTypeInRole = SystemPermissionTypeInRoleFactory.CreateSystemPermissionTypeInRole(systemPermissionType, role.RoleName, role.RequiredApprovers, role.ApprovalPriority);

                        systemPermissionTypeInRole.CreatedBy = serviceHeader.ApplicationUserName;

                        _systemPermissionTypeInRoleRepository.Add(systemPermissionTypeInRole, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool MapSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingBranches = GetBranchesForSystemPermissionType(systemPermissionType, serviceHeader);

                if (existingBranches != null && existingBranches.Any())
                {
                    var oldSet = from c in existingBranches ?? new List<BranchDTO> { } select c;

                    var newSet = from c in branches ?? new BranchDTO[] { } select c;

                    var commonSet = oldSet.Intersect(newSet, new BranchDTOEqualityComparer());

                    var insertSet = newSet.Except(commonSet, new BranchDTOEqualityComparer());

                    var deleteSet = oldSet.Except(commonSet, new BranchDTOEqualityComparer());

                    foreach (var branch in insertSet)
                    {
                        var systemPermissionTypeInBranch = SystemPermissionTypeInBranchFactory.CreateSystemPermissionTypeInBranch(systemPermissionType, branch.Id);

                        systemPermissionTypeInBranch.CreatedBy = serviceHeader.ApplicationUserName;

                        _systemPermissionTypeInBranchRepository.Add(systemPermissionTypeInBranch, serviceHeader);
                    }

                    foreach (var branch in deleteSet)
                    {
                        var filter = SystemPermissionTypeInBranchSpecifications.SystemPermissionTypeAndBranchId(systemPermissionType, branch.Id);

                        ISpecification<SystemPermissionTypeInBranch> spec = filter;

                        var matches = _systemPermissionTypeInBranchRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _systemPermissionTypeInBranchRepository.Get(mapping.Id, serviceHeader);

                                _systemPermissionTypeInBranchRepository.Remove(persisted, serviceHeader);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var branch in branches)
                    {
                        var systemPermissionTypeInBranch = SystemPermissionTypeInBranchFactory.CreateSystemPermissionTypeInBranch(systemPermissionType, branch.Id);

                        systemPermissionTypeInBranch.CreatedBy = serviceHeader.ApplicationUserName;

                        _systemPermissionTypeInBranchRepository.Add(systemPermissionTypeInBranch, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<SystemPermissionTypeInRoleDTO> GetSystemPermissionTypesInRole(string roleName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemPermissionTypeInRoleSpecifications.SystemPermissionTypeInRole(roleName);

                ISpecification<SystemPermissionTypeInRole> spec = filter;

                var systemPermissionTypesInRole = _systemPermissionTypeInRoleRepository.AllMatching(spec, serviceHeader);

                if (systemPermissionTypesInRole != null && systemPermissionTypesInRole.Any())
                {
                    return systemPermissionTypesInRole.ProjectedAsCollection<SystemPermissionTypeInRoleDTO>();
                }
                else return null;
            }
        }

        public List<SystemPermissionTypeInRoleDTO> GetRolesAndApprovalPriorityByPermissionType(int systemPermissionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemPermissionTypeInRoleSpecifications.SystemPermissionType(systemPermissionType);

                ISpecification<SystemPermissionTypeInRole> spec = filter;

                var systemPermissionTypesInRole = _systemPermissionTypeInRoleRepository.AllMatching(spec, serviceHeader);

                if (systemPermissionTypesInRole != null && systemPermissionTypesInRole.Any())
                {
                    return systemPermissionTypesInRole.ProjectedAsCollection<SystemPermissionTypeInRoleDTO>();
                }
                else return null;
            }
        }

        public List<SystemPermissionTypeInBranchDTO> GetSystemPermissionTypesInBranch(Guid branchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemPermissionTypeInBranchSpecifications.SystemPermissionTypeInBranch(branchId);

                ISpecification<SystemPermissionTypeInBranch> spec = filter;

                var systemPermissionTypesInBranch = _systemPermissionTypeInBranchRepository.AllMatching(spec, serviceHeader);

                if (systemPermissionTypesInBranch != null && systemPermissionTypesInBranch.Any())
                {
                    return systemPermissionTypesInBranch.ProjectedAsCollection<SystemPermissionTypeInBranchDTO>();
                }
                else return null;
            }
        }

        public List<BranchDTO> GetBranchesForSystemPermissionType(int systemPermissionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SystemPermissionTypeInBranch> spec = SystemPermissionTypeInBranchSpecifications.SystemPermissionType(systemPermissionType);

                var systemPermissionTypesInBranches = _systemPermissionTypeInBranchRepository.AllMatching(spec, serviceHeader);

                if (systemPermissionTypesInBranches != null && systemPermissionTypesInBranches.Any())
                {
                    return (from m in systemPermissionTypesInBranches
                            select m.Branch).ProjectedAsCollection<BranchDTO>();
                }
                else return null;
            }
        }

        public bool AddSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches, ServiceHeader serviceHeader)
        {
            if (branches != null && branches.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _systemPermissionTypeInBranchRepository.GetAll(serviceHeader);

                    Array.ForEach(branches, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.SystemPermissionType == systemPermissionType && x.BranchId == item.Id);

                        if (matches.Any()) return;

                        var systemPermissionTypeInBranch = SystemPermissionTypeInBranchFactory.CreateSystemPermissionTypeInBranch(systemPermissionType, item.Id);

                        systemPermissionTypeInBranch.CreatedBy = serviceHeader.ApplicationUserName;

                        _systemPermissionTypeInBranchRepository.Add(systemPermissionTypeInBranch, serviceHeader);
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool RemoveSystemPermissionTypeFromBranches(int systemPermissionType, BranchDTO[] branches, ServiceHeader serviceHeader)
        {
            if (branches != null && branches.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _systemPermissionTypeInBranchRepository.GetAll(serviceHeader);

                    Array.ForEach(branches, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.SystemPermissionType == systemPermissionType && x.BranchId == item.Id);

                        if (matches.Any())
                        {
                            matches.ToList().ForEach(x => _systemPermissionTypeInBranchRepository.Remove(x, serviceHeader));
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool IsSystemPermissionTypeInBranch(int systemPermissionType, Guid branchId, ServiceHeader serviceHeader)
        {
            if (branchId == null)
                return false;
            else
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SystemPermissionTypeInBranchSpecifications.SystemPermissionTypeAndBranchId(systemPermissionType, branchId);

                    // get the specification
                    ISpecification<SystemPermissionTypeInBranch> spec = filter;

                    // Query this criteria
                    var matches = _systemPermissionTypeInBranchRepository.AllMatching(spec, serviceHeader);

                    return matches != null && matches.Any();
                }
            }
        }

        public bool MapEmployeeToBranches(Guid employeeId, BranchDTO[] branches, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingBranches = GetBranchesForEmployee(employeeId, serviceHeader);

                if (existingBranches != null && existingBranches.Any())
                {
                    var oldSet = from c in existingBranches ?? new List<BranchDTO> { } select c;

                    var newSet = from c in branches ?? new BranchDTO[] { } select c;

                    var commonSet = oldSet.Intersect(newSet);

                    var insertSet = newSet.Except(commonSet);

                    var deleteSet = oldSet.Except(commonSet);

                    foreach (var branch in insertSet)
                    {
                        var employeeInBranch = EmployeeInBranchFactory.CreateEmployeeInBranch(employeeId, branch.Id);

                        employeeInBranch.CreatedBy = serviceHeader.ApplicationUserName;

                        _employeeInBranchRepository.Add(employeeInBranch, serviceHeader);
                    }

                    foreach (var branch in deleteSet)
                    {
                        var filter = EmployeeInBranchSpecifications.EmployeeAndBranch(employeeId, branch.Id);

                        ISpecification<EmployeeInBranch> spec = filter;

                        var matches = _employeeInBranchRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _employeeInBranchRepository.Get(mapping.Id, serviceHeader);

                                _employeeInBranchRepository.Remove(persisted, serviceHeader);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var branch in branches)
                    {
                        var employeeInBranch = EmployeeInBranchFactory.CreateEmployeeInBranch(employeeId, branch.Id);

                        employeeInBranch.CreatedBy = serviceHeader.ApplicationUserName;

                        _employeeInBranchRepository.Add(employeeInBranch, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<BranchDTO> GetBranchesForEmployee(Guid employeeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<EmployeeInBranch> spec = EmployeeInBranchSpecifications.Employee(employeeId);

                var employeesInBranches = _employeeInBranchRepository.AllMatching(spec, serviceHeader);

                if (employeesInBranches != null && employeesInBranches.Any())
                {
                    return (from m in employeesInBranches
                            select m.Branch).ProjectedAsCollection<BranchDTO>();
                }
                else return null;
            }
        }

        public bool AddEmployeeToBranches(Guid employeeId, BranchDTO[] branches, ServiceHeader serviceHeader)
        {
            if (branches != null && branches.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _employeeInBranchRepository.GetAll(serviceHeader);

                    Array.ForEach(branches, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.EmployeeId == employeeId && x.BranchId == item.Id);

                        if (matches.Any()) return;

                        var employeeInBranch = EmployeeInBranchFactory.CreateEmployeeInBranch(employeeId, item.Id);

                        employeeInBranch.CreatedBy = serviceHeader.ApplicationUserName;

                        _employeeInBranchRepository.Add(employeeInBranch, serviceHeader);
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool RemoveEmployeeFromBranches(Guid employeeId, BranchDTO[] branches, ServiceHeader serviceHeader)
        {
            if (branches != null && branches.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _employeeInBranchRepository.GetAll(serviceHeader);

                    Array.ForEach(branches, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.EmployeeId == employeeId && x.BranchId == item.Id);

                        if (matches.Any())
                        {
                            matches.ToList().ForEach(x => _employeeInBranchRepository.Remove(x, serviceHeader));
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool IsEmployeeInBranch(Guid employeeId, Guid branchId, ServiceHeader serviceHeader)
        {
            if (branchId == null)
                return false;
            else
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EmployeeInBranchSpecifications.EmployeeAndBranch(employeeId, branchId);

                    // get the specification
                    ISpecification<EmployeeInBranch> spec = filter;

                    // Query this criteria
                    var matches = _employeeInBranchRepository.AllMatching(spec, serviceHeader);

                    return matches != null && matches.Any();
                }
            }
        }
    }
}
