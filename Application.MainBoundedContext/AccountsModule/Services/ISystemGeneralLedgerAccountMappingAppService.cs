using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ISystemGeneralLedgerAccountMappingAppService
    {
        SystemGeneralLedgerAccountMappingDTO AddNewSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, ServiceHeader serviceHeader);

        bool UpdateSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, ServiceHeader serviceHeader);

        List<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(ServiceHeader serviceHeader);

        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings( int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        SystemGeneralLedgerAccountMappingDTO FindSystemGeneralLedgerAccountMapping(Guid systemGeneralLedgerAccountMappingId, ServiceHeader serviceHeader);

        


        
    }
}
