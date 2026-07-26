using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MembershipModule
{
    [ServiceContract(Name = "IMembershipManagerService")]
    public interface IMembershipManagerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCreateUser(EmployeeDTO employeeDTO, AsyncCallback callback, Object state);
        int EndCreateUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateUser(EmployeeDTO employeeDTO, AsyncCallback callback, Object state);
        bool EndUpdateUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetUserInfo(string userName, AsyncCallback callback, Object state);
        EmployeeDTO EndGetUserInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetUserInfoCollection(AsyncCallback callback, Object state);
        List<EmployeeDTO> EndGetUserInfoCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetUserInfoCollectionInPage(int pageIndex, int pageSize, string filter, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDTO> EndGetUserInfoCollectionInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetUserInfoCollectionInRole(string roleName, AsyncCallback callback, Object state);
        List<EmployeeDTO> EndGetUserInfoCollectionInRole(IAsyncResult result);
    }
}
