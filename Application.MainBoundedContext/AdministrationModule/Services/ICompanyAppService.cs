using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface ICompanyAppService
    {
        CompanyDTO AddNewCompany(CompanyDTO companyDTO, ServiceHeader serviceHeader);

        bool UpdateCompany(CompanyDTO companyDTO, ServiceHeader serviceHeader);

        List<CompanyDTO> FindCompanies(ServiceHeader serviceHeader);

        PageCollectionInfo<CompanyDTO> FindCompanies(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CompanyDTO> FindCompanies(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CompanyDTO FindCompany(Guid companyId, ServiceHeader serviceHeader);

        List<DebitTypeDTO> FindDebitTypes(Guid companyId, ServiceHeader serviceHeader);

        List<DebitTypeDTO> FindCachedDebitTypes(Guid companyId, ServiceHeader serviceHeader);

        bool UpdateDebitTypes(Guid companyId, List<DebitTypeDTO> debitTypeDTOs, ServiceHeader serviceHeader);

        ProductCollectionInfo FindAttachedProducts(Guid companyId, ServiceHeader serviceHeader, bool useCache = true);

        ProductCollectionInfo FindCachedAttachedProducts(Guid companyId, ServiceHeader serviceHeader);
        
        bool UpdateAttachedProducts(Guid companyId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader);

        Task<int> GetCompaniesCountAsync(ServiceHeader serviceHeader);
    }
}
