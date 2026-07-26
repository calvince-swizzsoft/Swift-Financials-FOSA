using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ICreditTypeAppService
    {
        CreditTypeDTO AddNewCreditType(CreditTypeDTO creditTypeDTO, ServiceHeader serviceHeader);

        bool UpdateCreditType(CreditTypeDTO creditTypeDTO, ServiceHeader serviceHeader);

        List<CreditTypeDTO> FindCreditTypes(ServiceHeader serviceHeader);

        List<CreditTypeDTO> FindCreditTypesByAttachedProductId(Guid attachedProductId, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditTypeDTO> FindCreditTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditTypeDTO> FindCreditTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CreditTypeDTO FindCreditType(Guid creditTypeId, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid creditTypeId, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid creditTypeId, List<CommissionDTO> commissions, ServiceHeader serviceHeader);

        List<DirectDebitDTO> FindDirectDebits(Guid creditTypeId, ServiceHeader serviceHeader);

        List<DirectDebitDTO> FindCachedDirectDebits(Guid creditTypeId, ServiceHeader serviceHeader);

        bool UpdateDirectDebits(Guid creditTypeId, List<DirectDebitDTO> directDebitDTOs, ServiceHeader serviceHeader);

        ProductCollectionInfo FindAttachedProducts(Guid creditTypeId, ServiceHeader serviceHeader, bool useCache = true);

        ProductCollectionInfo FindCachedAttachedProducts(Guid creditTypeId, ServiceHeader serviceHeader);

        ProductCollectionInfo FindConcessionExemptProducts(Guid creditTypeId, ServiceHeader serviceHeader, bool useCache = true);

        ProductCollectionInfo FindCachedConcessionExemptProducts(Guid creditTypeId, ServiceHeader serviceHeader);

        bool UpdateAttachedProducts(Guid creditTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader);

        bool UpdateConcessionExemptProducts(Guid creditTypeId, ProductCollectionInfo concessionExemptProductsTuple, ServiceHeader serviceHeader);
    }
}
