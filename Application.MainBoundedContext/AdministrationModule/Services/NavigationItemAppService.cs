using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Infrastructure.Data.MainBoundedContext.Seeders;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class NavigationItemAppService : INavigationItemAppService
    {
        private readonly IRepository<NavigationItem> _navigationItemRepository;
        private readonly NumberSeriesSeeder _numberSeriesSeeder;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public NavigationItemAppService(IRepository<NavigationItem> navigationItemRepository, IDbContextScopeFactory dbContextScopeFactory, NumberSeriesSeeder numberSeriesSeeder)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _navigationItemRepository = navigationItemRepository ?? throw new ArgumentNullException(nameof(navigationItemRepository));
            _numberSeriesSeeder = numberSeriesSeeder ?? throw new ArgumentNullException(nameof(numberSeriesSeeder));
        }

        #region NavigationItemDTO

        public async Task<bool> AddNavigationItemsAsync(List<NavigationItemDTO> navigationItems, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (navigationItems == null)
                return result;

            try
            {

                _numberSeriesSeeder.SeedAsync(serviceHeader);
            }
            catch (Exception ex)
            {
                // Log the error but continue with navigation items
                // Logger.Error("Failed to seed NumberSeries", ex);
            }

            var parentNavigationItems = navigationItems.Where(x => x.IsArea).ToList();
            if (parentNavigationItems != null && parentNavigationItems.Any())
                result = await AddParentNavigationItemsAsync(parentNavigationItems, serviceHeader);

            var subParentNavigationItemsList = navigationItems.Where(x => !x.IsArea && x.ControllerName == null && x.ActionName == null).ToList();

            List<NavigationItemDTO> subParentNavigationItems = new List<NavigationItemDTO>();

            if (subParentNavigationItemsList != null && subParentNavigationItemsList.Any())
            {
                foreach (var item in subParentNavigationItemsList)
                {
                    var parentNavigationItem = await FindNavigationItemAsync(item.AreaCode, serviceHeader);

                    item.ParentId = parentNavigationItem.Id;

                    subParentNavigationItems.Add(item);
                }

                result = await AddSubParentNavigationItemsAsync(subParentNavigationItems, serviceHeader);
            }

            var childrenNavigationItems = navigationItems.Where(x => !x.IsArea && x.ControllerName != null && x.ActionName != null).ToList();

            if (result)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var item in childrenNavigationItems)
                    {
                        var matchedNavigationItems = await FindNavigationItemByControllerNameAndActionNameAsync(item.ControllerName, item.ActionName, serviceHeader);

                        var parentNavigationItem = await FindNavigationItemAsync(item.AreaCode, serviceHeader);

                        if (matchedNavigationItems == null || !matchedNavigationItems.Any())
                        {
                            var navigationItem = NavigationItemFactory.CreateNavigationItem(parentNavigationItem?.Id, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, item.AreaCode, item.AreaName);

                            navigationItem.IsArea = item.IsArea;

                            navigationItem.CreatedBy = serviceHeader.ApplicationUserName;

                            _navigationItemRepository.Add(navigationItem, serviceHeader);
                        }
                        else
                        {
                            foreach (var matchedNavigationItem in matchedNavigationItems)
                            {
                                var persisted = _navigationItemRepository.Get(matchedNavigationItem.Id, serviceHeader);

                                var current = NavigationItemFactory.CreateNavigationItem(parentNavigationItem?.Id, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, parentNavigationItem.Code, item.AreaName);

                                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                                current.IsArea = item.IsArea;

                                current.CreatedBy = serviceHeader.ApplicationUserName;

                                _navigationItemRepository.Merge(persisted, current, serviceHeader);
                            }
                        }
                    }

                    result = await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                }
            }

            return result;
        }

        private async Task<bool> AddParentNavigationItemsAsync(List<NavigationItemDTO> navigationItems, ServiceHeader serviceHeader)
        {
            if (navigationItems == null) return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in navigationItems)
                {
                    var matchedNavigationItems = await FindModuleNavigationActionByDescription(item.Description, serviceHeader);

                    if (matchedNavigationItems == null || !matchedNavigationItems.Any())
                    {
                        var navigationItem = NavigationItemFactory.CreateNavigationItem(item.ParentId, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, item.AreaCode, item.AreaName);

                        navigationItem.IsArea = item.IsArea;

                        navigationItem.CreatedBy = serviceHeader.ApplicationUserName;

                        _navigationItemRepository.Add(navigationItem, serviceHeader);
                    }
                    else
                    {
                        foreach (var matchedNavigationItem in matchedNavigationItems)
                        {
                            var persisted = _navigationItemRepository.Get(matchedNavigationItem.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var current = NavigationItemFactory.CreateNavigationItem(item.ParentId, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, item.AreaCode, item.AreaName);

                                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                                current.IsArea = item.IsArea;

                                current.CreatedBy = serviceHeader.ApplicationUserName;

                                _navigationItemRepository.Merge(persisted, current, serviceHeader);
                            }
                        }
                    }
                }
                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        private async Task<bool> AddSubParentNavigationItemsAsync(List<NavigationItemDTO> navigationItems, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (navigationItems == null) return false;

            var orderedNavigationItems = navigationItems.OrderBy(x => x.Code).ToList();

            foreach (var item in orderedNavigationItems)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var matchedNavigationItems = await FindModuleNavigationActionByDescription(item.Description, serviceHeader);

                    var parentNavigationItem = await FindNavigationItemAsync(item.AreaCode, serviceHeader);

                    if (matchedNavigationItems == null || !matchedNavigationItems.Any())
                    {
                        var navigationItem = NavigationItemFactory.CreateNavigationItem(parentNavigationItem?.ParentId, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, item.AreaCode, item.AreaName);

                        navigationItem.IsArea = item.IsArea;

                        navigationItem.CreatedBy = serviceHeader.ApplicationUserName;

                        _navigationItemRepository.Add(navigationItem, serviceHeader);
                    }
                    else
                    {
                        foreach (var matchedNavigationItem in matchedNavigationItems)
                        {
                            var persisted = await _navigationItemRepository.GetAsync(matchedNavigationItem.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var current = NavigationItemFactory.CreateNavigationItem(parentNavigationItem?.ParentId, item.Description, item.Icon, item.Code, item.ControllerName, item.ActionName, parentNavigationItem.Code, item.AreaName);

                                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                                current.IsArea = item.IsArea;

                                current.CreatedBy = serviceHeader.ApplicationUserName;

                                _navigationItemRepository.Merge(persisted, current, serviceHeader);
                            }
                        }
                    }

                    result = await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                }
            }
            return result;
        }

        public Task<bool> BulkInsertNavigationItemAsync(List<Guid> navigationItemIds, string roleName, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public async Task<List<NavigationItemDTO>> FindModuleNavigationActionByControllerNameAsync(string controllerName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                if (!string.IsNullOrWhiteSpace(controllerName))
                {
                    var filter = NavigationItemSpecifications.NavigationItemByControllerName(controllerName);

                    ISpecification<NavigationItem> spec = filter;

                    var navigationItems = await _navigationItemRepository.AllMatchingAsync(spec, serviceHeader);

                    if (navigationItems != null)
                    {
                        return navigationItems.ProjectedAsCollection<NavigationItemDTO>();
                    }
                    else return null;
                }

                return null;
            }
        }

        public async Task<List<NavigationItemDTO>> FindNavigationItemByControllerNameAndActionNameAsync(string controllerName, string actionName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                if (!string.IsNullOrWhiteSpace(controllerName) || !string.IsNullOrWhiteSpace(actionName))
                {
                    var filter = NavigationItemSpecifications.NavigationItemByControllerAndActionName(controllerName, actionName);

                    ISpecification<NavigationItem> spec = filter;

                    var navigationItems = await _navigationItemRepository.AllMatchingAsync(spec, serviceHeader);

                    if (navigationItems != null)
                    {
                        return navigationItems.ProjectedAsCollection<NavigationItemDTO>();
                    }
                    else return null;
                }

                return null;
            }
        }

        public async Task<NavigationItemDTO> FindNavigationItemAsync(Guid navigationItemId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var navigationItem = await _navigationItemRepository.GetAsync(navigationItemId, serviceHeader);

                if (navigationItem != null)
                {
                    return navigationItem.ProjectedAs<NavigationItemDTO>();
                }
                else return null;
            }
        }

        public async Task<NavigationItemDTO> FindNavigationItemAsync(int navigationItemCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = NavigationItemSpecifications.NavigationItemByCode(navigationItemCode);

                ISpecification<NavigationItem> spec = filter;

                var navigationItems = await _navigationItemRepository.AllMatchingAsync(spec, serviceHeader);

                if (navigationItems != null)
                {
                    return navigationItems.ProjectedAsCollection<NavigationItemDTO>().FirstOrDefault();
                }
                else return null;
            }
        }

        public async Task<List<NavigationItemDTO>> FindNavigationItemsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var specification = NavigationItemSpecifications.DefaultSpecification();

                var navigationItems = await _navigationItemRepository.AllMatchingAsync(specification, serviceHeader);

                if (navigationItems != null)
                {
                    return navigationItems.ProjectedAsCollection<NavigationItemDTO>();
                }
                else return null;
            }
        }

        public async Task<PageCollectionInfo<NavigationItemDTO>> FindNavigationItemsFilterPageCollectionInfoAsync(int pageIndex, int pageSize, List<string> sortedColumns, string text, bool sortAscending, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = NavigationItemSpecifications.DefaultSpecification();

                ISpecification<NavigationItem> spec = filter;

                var sortFields = sortedColumns ?? new List<string> { "SequentialId" };

                var navigationItemPagedCollection = await _navigationItemRepository.AllMatchingPagedAsync(spec, pageIndex, pageSize, sortFields, sortAscending, serviceHeader);

                if (navigationItemPagedCollection != null)
                {
                    var pageCollection = navigationItemPagedCollection.PageCollection.ProjectedAsCollection<NavigationItemDTO>();

                    var itemsCount = navigationItemPagedCollection.ItemsCount;

                    return new PageCollectionInfo<NavigationItemDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        private async Task<List<NavigationItemDTO>> FindModuleNavigationActionByDescription(string description, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                if (!string.IsNullOrWhiteSpace(description))
                {
                    var filter = NavigationItemSpecifications.NavigationItemByDescription(description);

                    ISpecification<NavigationItem> spec = filter;

                    var navigationItems = await _navigationItemRepository.AllMatchingAsync(spec, serviceHeader);

                    if (navigationItems != null)
                    {
                        return navigationItems.ProjectedAsCollection<NavigationItemDTO>();
                    }
                    else
                        return null;
                }
                return null;
            }
        }

        #endregion
    }
}