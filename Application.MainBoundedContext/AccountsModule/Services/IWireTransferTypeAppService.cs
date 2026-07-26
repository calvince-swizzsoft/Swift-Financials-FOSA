using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IWireTransferTypeAppService
    {
        WireTransferTypeDTO AddNewWireTransferType(WireTransferTypeDTO wireTransferTypeDTO, ServiceHeader serviceHeader);

        bool UpdateWireTransferType(WireTransferTypeDTO wireTransferTypeDTO, ServiceHeader serviceHeader);

        List<WireTransferTypeDTO> FindWireTransferTypes(ServiceHeader serviceHeader);

        PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        WireTransferTypeDTO FindWireTransferType(Guid wireTransferTypeId, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid wireTransferTypeId, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid wireTransferTypeId, List<CommissionDTO> commissions, ServiceHeader serviceHeader);
    }
}
