using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IDelegateService
    {
        #region Delegate

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DelegateDTO AddDelegate(DelegateDTO delegateDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDelegate(DelegateDTO delegateDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DelegateDTO> FindDelegates();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DelegateDTO> FindDelegatesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DelegateDTO> FindDelegatesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DelegateDTO FindDelegate(Guid delegateId);

        #endregion
    }
}
