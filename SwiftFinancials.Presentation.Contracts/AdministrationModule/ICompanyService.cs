using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "ICompanyService")]
    public interface ICompanyService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCompany(CompanyDTO companyDTO, AsyncCallback callback, Object state);
        CompanyDTO EndAddCompany(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCompany(CompanyDTO companyDTO, AsyncCallback callback, Object state);
        bool EndUpdateCompany(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCompanies(AsyncCallback callback, Object state);
        List<CompanyDTO> EndFindCompanies(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCompaniesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CompanyDTO> EndFindCompaniesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCompaniesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CompanyDTO> EndFindCompaniesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCompany(Guid companyId, AsyncCallback callback, Object state);
        CompanyDTO EndFindCompany(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitTypesByCompanyId(Guid companyId, AsyncCallback callback, Object state);
        List<DebitTypeDTO> EndFindDebitTypesByCompanyId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDebitTypesByCompanyId(Guid companyId, List<DebitTypeDTO> debitTypes, AsyncCallback callback, Object state);
        bool EndUpdateDebitTypesByCompanyId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAttachedProductsByCompanyId(Guid companyId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindAttachedProductsByCompanyId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAttachedProductsByCompanyId(Guid companyId, ProductCollectionInfo attachedProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateAttachedProductsByCompanyId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetCompaniesCount(AsyncCallback callback, Object state);
        int EndGetCompaniesCount(IAsyncResult result);
    }
}
