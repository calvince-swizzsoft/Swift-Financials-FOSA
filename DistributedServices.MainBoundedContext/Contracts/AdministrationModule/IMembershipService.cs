using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.MainBoundedContext.Identity;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IMembershipService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<UserDTO> FindMembershipByFilterInPage(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<RoleDTO> FindMembershipRolesByFilterInPage(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<UserDTO> AddNewMembershipAsync(UserDTO userDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateMembershipAsync(UserDTO userDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        int GetApplicationUsersCountByCompanyId(Guid companyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        int GetApplicationUsersCount();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<UserDTO> FindMembershipAsync(string id);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<UserDTO> FindMemberships();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ResetMembershipPasswordAsync(UserDTO userDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> VerifyMembershipAsync(UserDTO userDTO);
    }
}