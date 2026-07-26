using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IInsuranceCompanyService
    {
        #region InsuranceCompany

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InsuranceCompanyDTO AddInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InsuranceCompanyDTO> FindInsuranceCompanies();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InsuranceCompanyDTO FindInsuranceCompany(Guid insuranceCompanyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompaniesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompaniesByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
