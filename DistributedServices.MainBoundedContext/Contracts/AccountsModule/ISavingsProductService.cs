using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISavingsProductService
    {
        #region Savings Product

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SavingsProductDTO AddSavingsProduct(SavingsProductDTO savingsProductDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSavingsProduct(SavingsProductDTO savingsProductDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SavingsProductDTO> FindSavingsProducts();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SavingsProductDTO> FindMandatorySavingsProducts(bool isMandatory);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SavingsProductDTO> FindSavingsProductsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SavingsProductDTO> FindSavingsProductsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SavingsProductDTO FindSavingsProduct(Guid savingsProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SavingsProductDTO FindDefaultSavingsProduct();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsBySavingsProductId(Guid savingsProductId, int savingsProductKnownChargeType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsBySavingsProductId(Guid savingsProductId, List<CommissionDTO> commissions, int savingsProductKnownChargeType, int savingsProductChargeBenefactor);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SavingsProductExemptionDTO> FindSavingsProductExemptionsBySavingsProductId(Guid savingsProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSavingsProductExemptionsBySavingsProductId(Guid savingsProductId, List<SavingsProductExemptionDTO> savingsProductExemptions);

        #endregion
    }
}
