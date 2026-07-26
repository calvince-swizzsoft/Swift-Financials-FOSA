using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IFixedDepositTypeService")]
    public interface IFixedDepositTypeService
    {
        #region FixedDepositTypeDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, AsyncCallback callback, Object state);
        FixedDepositTypeDTO EndAddFixedDepositType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, AsyncCallback callback, Object state);
        bool EndUpdateFixedDepositType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositTypes(AsyncCallback callback, Object state);
        List<FixedDepositTypeDTO> EndFindFixedDepositTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositType(Guid fixedDepositTypeId, AsyncCallback callback, Object state);
        FixedDepositTypeDTO EndFindFixedDepositType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositTypesByMonths(int months, AsyncCallback callback, Object state);
        List<FixedDepositTypeDTO> EndFindFixedDepositTypesByMonths(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositTypesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositTypeDTO> EndFindFixedDepositTypesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositTypesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositTypeDTO> EndFindFixedDepositTypesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeviesByFixedDepositTypeId(Guid fixedDepositTypeId, AsyncCallback callback, Object state);
        List<LevyDTO> EndFindLeviesByFixedDepositTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLeviesByFixedDepositTypeId(Guid fixedDepositTypeId, List<LevyDTO> levies, AsyncCallback callback, Object state);
        bool EndUpdateLeviesByFixedDepositTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAttachedProductsByFixedDepositTypeId(Guid fixedDepositTypeId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindAttachedProductsByFixedDepositTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAttachedProductsByFixedDepositTypeId(Guid fixedDepositTypeId, ProductCollectionInfo attachedProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateAttachedProductsByFixedDepositTypeId(IAsyncResult result);

        #endregion

        #region FixedDepositTypeGraduatedScaleDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGraduatedScalesByFixedDepositTypeId(Guid fixedDepositTypeId, AsyncCallback callback, Object state);
        List<FixedDepositTypeGraduatedScaleDTO> EndFindGraduatedScalesByFixedDepositTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateGraduatedScalesByFixedDepositTypeId(Guid fixedDepositTypeId, List<FixedDepositTypeGraduatedScaleDTO> graduatedScales, AsyncCallback callback, Object state);
        bool EndUpdateGraduatedScalesByFixedDepositTypeId(IAsyncResult result);
        #endregion
    }
}
