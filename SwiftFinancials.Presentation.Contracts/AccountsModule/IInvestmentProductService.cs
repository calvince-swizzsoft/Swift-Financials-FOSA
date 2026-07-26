using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IInvestmentProductService")]
    public interface IInvestmentProductService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInvestmentProduct(InvestmentProductDTO investmentProductDTO, AsyncCallback callback, Object state);
        InvestmentProductDTO EndAddInvestmentProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateInvestmentProduct(InvestmentProductDTO investmentProductDTO, AsyncCallback callback, Object state);
        bool EndUpdateInvestmentProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInvestmentProducts(AsyncCallback callback, Object state);
        List<InvestmentProductDTO> EndFindInvestmentProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMandatoryInvestmentProducts(bool isMandatory, AsyncCallback callback, Object state);
        List<InvestmentProductDTO> EndFindMandatoryInvestmentProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInvestmentProductsByCode(int code, AsyncCallback callback, Object state);
        List<InvestmentProductDTO> EndFindInvestmentProductsByCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInvestmentProductsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InvestmentProductDTO> EndFindInvestmentProductsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInvestmentProductsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InvestmentProductDTO> EndFindInvestmentProductsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInvestmentProduct(Guid investmentProductId, AsyncCallback callback, Object state);
        InvestmentProductDTO EndFindInvestmentProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSuperSaverInvestmentProduct(AsyncCallback callback, Object state);
        InvestmentProductDTO EndFindSuperSaverInvestmentProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInvestmentProductExemptionsByInvestmentProductId(Guid investmentProductId, AsyncCallback callback, Object state);
        List<InvestmentProductExemptionDTO> EndFindInvestmentProductExemptionsByInvestmentProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateInvestmentProductExemptionsByInvestmentProductId(Guid investmentProductId, List<InvestmentProductExemptionDTO> investmentProductExemptions, AsyncCallback callback, Object state);
        bool EndUpdateInvestmentProductExemptionsByInvestmentProductId(IAsyncResult result);
    }
}
