using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IChequeTypeAppService
    {
        ChequeTypeDTO AddNewChequeType(ChequeTypeDTO chequeTypeDTO, ServiceHeader serviceHeader);

        bool UpdateChequeType(ChequeTypeDTO chequeTypeDTO, ServiceHeader serviceHeader);

        List<ChequeTypeDTO> FindChequeTypes(ServiceHeader serviceHeader);

        PageCollectionInfo<ChequeTypeDTO> FindChequeTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ChequeTypeDTO> FindChequeTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ChequeTypeDTO FindChequeType(Guid chequeTypeId, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid chequeTypeId, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid chequeTypeId, List<CommissionDTO> commissions, ServiceHeader serviceHeader);

        ProductCollectionInfo FindAttachedProducts(Guid chequeTypeId, ServiceHeader serviceHeader, bool useCache = true);

        ProductCollectionInfo FindCachedAttachedProducts(Guid chequeTypeId, ServiceHeader serviceHeader);

        bool UpdateAttachedProducts(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader);
    }
}
