using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface INavigationItemInRoleService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<NavigationItemInRoleDTO>> GetNavigationItemsInRoleAsync(string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<string[]> GetRolesForNavigationItemCodeAsync(int navigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> IsNavigationItemInRoleAsync(int navigationItemCode, string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AddNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveNavigationItemRoleAsync(Guid navigationItemId, string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ValidateModuleAccessAsync(string controllerName, string roleName);
    }
}