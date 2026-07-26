using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IInsuranceCompanyAppService
    {
        #region Insurance

        InsuranceCompanyDTO AddInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO, ServiceHeader serviceHeader);

        bool UpdateInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO, ServiceHeader serviceHeader);

        List<InsuranceCompanyDTO> FindInsuranceCompanies(ServiceHeader serviceHeader);

        InsuranceCompanyDTO FindInsuranceCompany(Guid insuranceCompanyId, ServiceHeader serviceHeader);

        PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompanies(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompanies(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        #endregion
    }
}
