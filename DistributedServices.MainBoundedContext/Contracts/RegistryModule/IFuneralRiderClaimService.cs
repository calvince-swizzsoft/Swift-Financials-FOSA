using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IFuneralRiderClaimService
    {
        #region FuneralRiderClaimDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FuneralRiderClaimDTO AddNewFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaimsByFilterAndDateInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaimsByStatusAndFilterInPage(int status, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FuneralRiderClaimDTO> FindFuneralRiderClaimsByCustomerId(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FuneralRiderClaimDTO FindFuneralRiderClaim(Guid funeralRiderClaimId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaimsByFilterInPage(string filter, int pageIndex, int pageSize);
        #endregion

        #region FuneralRiderClaimPayableDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FuneralRiderClaimPayableDTO AddNewFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int verificationOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int authorizationOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostFuneralRiderClaimPayable(Guid funeralRiderClaimPayableId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayablesByRecordStatusFilterAndDateInPage(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayablesByRecordStatusPaymentStatusFilterAndDateInPage(int recordStatus, int paymentStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayablesByRecordStatusPaymentStatusAndFilterInPage(int recordStatus, int paymentStatus, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayableAsync(Guid funeralRiderClaimPayableId);

        #endregion
    }
}
