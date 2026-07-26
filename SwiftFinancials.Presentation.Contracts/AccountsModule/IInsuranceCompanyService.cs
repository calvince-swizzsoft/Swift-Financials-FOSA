using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IInsuranceCompanyService")]
    public interface IInsuranceCompanyService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO, AsyncCallback callback, Object state);
        InsuranceCompanyDTO EndAddInsuranceCompany(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO, AsyncCallback callback, Object state);
        bool EndUpdateInsuranceCompany(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInsuranceCompanies(AsyncCallback callback, Object state);
        List<InsuranceCompanyDTO> EndFindInsuranceCompanies(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInsuranceCompany(Guid insuranceCompanyId, AsyncCallback callback, Object state);
        InsuranceCompanyDTO EndFindInsuranceCompany(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInsuranceCompaniesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InsuranceCompanyDTO> EndFindInsuranceCompaniesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInsuranceCompaniesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InsuranceCompanyDTO> EndFindInsuranceCompaniesByFilterInPage(IAsyncResult result);
    }
}
