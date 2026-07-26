using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ISavingsProductService")]
    public interface ISavingsProductService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSavingsProduct(SavingsProductDTO savingsProductDTO, AsyncCallback callback, Object state);
        SavingsProductDTO EndAddSavingsProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSavingsProduct(SavingsProductDTO savingsProductDTO, AsyncCallback callback, Object state);
        bool EndUpdateSavingsProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSavingsProducts(AsyncCallback callback, Object state);
        List<SavingsProductDTO> EndFindSavingsProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMandatorySavingsProducts(bool isMandatory, AsyncCallback callback, Object state);
        List<SavingsProductDTO> EndFindMandatorySavingsProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSavingsProductsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SavingsProductDTO> EndFindSavingsProductsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSavingsProductsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SavingsProductDTO> EndFindSavingsProductsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSavingsProduct(Guid savingsProductId, AsyncCallback callback, Object state);
        SavingsProductDTO EndFindSavingsProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDefaultSavingsProduct(AsyncCallback callback, Object state);
        SavingsProductDTO EndFindDefaultSavingsProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsBySavingsProductId(Guid savingsProductId, int savingsProductKnownChargeType, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsBySavingsProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsBySavingsProductId(Guid savingsProductId, List<CommissionDTO> commissions, int savingsProductKnownChargeType, int savingsProductChargeBenefactor, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsBySavingsProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSavingsProductExemptionsBySavingsProductId(Guid savingsProductId, AsyncCallback callback, Object state);
        List<SavingsProductExemptionDTO> EndFindSavingsProductExemptionsBySavingsProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSavingsProductExemptionsBySavingsProductId(Guid savingsProductId, List<SavingsProductExemptionDTO> savingsProductExemptions, AsyncCallback callback, Object state);
        bool EndUpdateSavingsProductExemptionsBySavingsProductId(IAsyncResult result);
    }
}
