using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "INavigationItemService")]
    public interface INavigationItemService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNavigationItems(List<NavigationItemDTO> navigationItems, AsyncCallback callback, Object state);
        bool EndAddNavigationItems(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindNavigationItemById(Guid navigationItemId, AsyncCallback callback, Object state);
        NavigationItemDTO EndFindNavigationItemById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindNavigationItemByCode(int navigationItemCode, AsyncCallback callback, Object state);
        NavigationItemDTO EndFindNavigationItemByCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindNavigationItems(AsyncCallback callback, Object state);
        List<NavigationItemDTO> EndFindNavigationItems(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindNavigationItemsFilterPageCollectionInfo(int pageIndex, int pageSize, List<string> sortedColumns, string text, bool sortAscending, AsyncCallback callback, Object state);
        PageCollectionInfo<NavigationItemDTO> EndFindNavigationItemsFilterPageCollectionInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindModuleNavigationActionByControllerName(string controllerName, AsyncCallback callback, Object state);
        List<NavigationItemDTO> EndFindModuleNavigationActionByControllerName(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginBulkInsertNavigationItem(List<Guid> modulesNavigationIds, string roleName, AsyncCallback callback, Object state);
        bool EndBulkInsertNavigationItem(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginBulkInsertNavigationItemInRoles(List<Guid> NavigationItemInRole, string roleName, AsyncCallback callback, Object state);
        bool EndBulkInsertNavigationItemInRoles(IAsyncResult result);
    }
}