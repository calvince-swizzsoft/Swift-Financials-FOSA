using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICompanyService
    {
        #region Company

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CompanyDTO AddCompany(CompanyDTO companyDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCompany(CompanyDTO companyDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CompanyDTO> FindCompanies();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CompanyDTO> FindCompaniesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CompanyDTO> FindCompaniesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CompanyDTO FindCompany(Guid companyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DebitTypeDTO> FindDebitTypesByCompanyId(Guid companyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDebitTypesByCompanyId(Guid companyId, List<DebitTypeDTO> debitTypes);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ProductCollectionInfo FindAttachedProductsByCompanyId(Guid companyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAttachedProductsByCompanyId(Guid companyId, ProductCollectionInfo attachedProductsTuple);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<int> GetCompaniesCountAsync();

        #endregion
    }
}
