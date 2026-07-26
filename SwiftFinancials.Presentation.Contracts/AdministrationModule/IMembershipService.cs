using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "IMembershipService")]
    public interface IMembershipService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMembershipByFilterInPage(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending, AsyncCallback callback, object state);
        PageCollectionInfo<UserDTO> EndFindMembershipByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMembershipRolesByFilterInPage(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending, AsyncCallback callback, object state);
        PageCollectionInfo<RoleDTO> EndFindMembershipRolesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewMembership(UserDTO userDTO, AsyncCallback callback, object state);
        UserDTO EndAddNewMembership(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateMembership(UserDTO userDTO, AsyncCallback callback, object state);
        bool EndUpdateMembership(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetApplicationUsersCountByCompanyId(Guid companyId, AsyncCallback callback, object state);
        int EndGetApplicationUsersCountByCompanyId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetApplicationUsersCount(AsyncCallback callback, object state);
        int EndGetApplicationUsersCount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMembership(string id, AsyncCallback callback, object state);
        UserDTO EndFindMembership(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginResetMembershipPassword(UserDTO userDTO, AsyncCallback callback, object state);
        bool EndResetMembershipPassword(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginVerifyMembership(UserDTO userDTO, AsyncCallback callback, object state);
        bool EndVerifyMembership(IAsyncResult result);
    }
}