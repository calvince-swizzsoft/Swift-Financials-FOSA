using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IInvestmentProductAppService
    {
        InvestmentProductDTO AddNewInvestmentProduct(InvestmentProductDTO investmentProductDTO, ServiceHeader serviceHeader);

        bool UpdateInvestmentProduct(InvestmentProductDTO investmentProductDTO, ServiceHeader serviceHeader);

        List<InvestmentProductDTO> FindInvestmentProducts(ServiceHeader serviceHeader);

        List<InvestmentProductDTO> FindCachedInvestmentProducts(ServiceHeader serviceHeader);

        List<InvestmentProductDTO> FindInvestmentProducts(int code, ServiceHeader serviceHeader);

        List<InvestmentProductDTO> FindPooledInvestmentProducts(ServiceHeader serviceHeader);

        InvestmentProductDTO FindSuperSaverInvestmentProduct(ServiceHeader serviceHeader);

        PageCollectionInfo<InvestmentProductDTO> FindInvestmentProducts(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<InvestmentProductDTO> FindInvestmentProducts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        InvestmentProductDTO FindInvestmentProduct(Guid investmentProductId, ServiceHeader serviceHeader);

        InvestmentProductDTO FindCachedInvestmentProduct(Guid investmentProductId, ServiceHeader serviceHeader);

        List<InvestmentProductExemptionDTO> FindInvestmentProductExemptions(Guid investmentProductId, ServiceHeader serviceHeader);

        InvestmentProductExemptionDTO FindInvestmentProductExemption(Guid investmentProductId, int customerClassification, ServiceHeader serviceHeader);

        bool UpdateInvestmentProductExemptions(Guid investmentProductId, List<InvestmentProductExemptionDTO> investmentProductExemptions, ServiceHeader serviceHeader);

        List<InvestmentProductDTO> FindMandatoryInvestmentProducts(bool isMandatory, ServiceHeader serviceHeader);
    }
}
