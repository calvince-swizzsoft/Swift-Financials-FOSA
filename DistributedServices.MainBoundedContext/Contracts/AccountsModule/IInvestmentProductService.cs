using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IInvestmentProductService
    {
        #region Investment Product

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InvestmentProductDTO AddInvestmentProduct(InvestmentProductDTO investmentProductDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateInvestmentProduct(InvestmentProductDTO investmentProductDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InvestmentProductDTO> FindInvestmentProducts();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InvestmentProductDTO> FindMandatoryInvestmentProducts(bool isMandatory);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InvestmentProductDTO> FindInvestmentProductsByCode(int code);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InvestmentProductDTO> FindInvestmentProductsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InvestmentProductDTO> FindInvestmentProductsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InvestmentProductDTO FindInvestmentProduct(Guid investmentProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InvestmentProductDTO FindSuperSaverInvestmentProduct();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InvestmentProductExemptionDTO> FindInvestmentProductExemptionsByInvestmentProductId(Guid investmentProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateInvestmentProductExemptionsByInvestmentProductId(Guid investmentProductId, List<InvestmentProductExemptionDTO> investmentProductExemptions);

        #endregion
    }
}
