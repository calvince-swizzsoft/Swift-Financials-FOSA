using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IDynamicChargeAppService
    {
        DynamicChargeDTO AddNewDynamicCharge(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader);

        bool UpdateDynamicCharge(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader);

        List<DynamicChargeDTO> FindDynamicCharges(ServiceHeader serviceHeader);

        PageCollectionInfo<DynamicChargeDTO> FindDynamicCharges(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DynamicChargeDTO> FindDynamicCharges(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DynamicChargeDTO FindDynamicCharge(Guid dynamicChargeId, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid dynamicChargeId, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid dynamicChargeId, List<CommissionDTO> Commissions, ServiceHeader serviceHeader);
    }
}
