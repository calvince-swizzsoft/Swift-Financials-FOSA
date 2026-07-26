using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ICostCenterAppService
    {
        CostCenterDTO AddNewCostCenter(CostCenterDTO costCenterDTO, ServiceHeader serviceHeader);

        bool UpdateCostCenter(CostCenterDTO costCenterDTO, ServiceHeader serviceHeader);

        List<CostCenterDTO> FindCostCenters(ServiceHeader serviceHeader);

        PageCollectionInfo<CostCenterDTO> FindCostCenters(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CostCenterDTO> FindCostCenters(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CostCenterDTO FindCostCenter(Guid costCenterId, ServiceHeader serviceHeader);
    }
}
