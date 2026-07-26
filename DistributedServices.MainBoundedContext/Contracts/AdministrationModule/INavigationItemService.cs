using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface INavigationItemService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AddNavigationItemsAsync(List<NavigationItemDTO> navigationItems);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<NavigationItemDTO> FindNavigationItemByIdAsync(Guid navigationItemId);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<NavigationItemDTO> FindNavigationItemByCodeAsync(int navigationItemCode);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<NavigationItemDTO>> FindNavigationItemsAsync();

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<NavigationItemDTO>> FindNavigationItemsFilterPageCollectionInfoAsync(int pageIndex, int pageSize, List<string> sortedColumns, string text, bool sortAscending);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<NavigationItemDTO>> FindModuleNavigationActionByControllerNameAsync(string controllerName);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> BulkInsertNavigationItemAsync(List<Guid> navigationItemIds, string roleName);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> BulkInsertNavigationItemInRolesAsync(List<Guid> navigationItemInRole, string roleName);
    }
}