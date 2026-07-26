using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IDebitTypeAppService
    {
        DebitTypeDTO AddNewDebitType(DebitTypeDTO debitTypeDTO, ServiceHeader serviceHeader);

        bool UpdateDebitType(DebitTypeDTO debitTypeDTO, ServiceHeader serviceHeader);

        List<DebitTypeDTO> FindDebitTypes(ServiceHeader serviceHeader);

        PageCollectionInfo<DebitTypeDTO> FindDebitTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DebitTypeDTO> FindDebitTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DebitTypeDTO FindDebitType(Guid debitTypeId, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid debitTypeId, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid debitTypeId, List<CommissionDTO> commissions, ServiceHeader serviceHeader);

        void FetchDebitTypesProductDescription(List<DebitTypeDTO> debitTypes, ServiceHeader serviceHeader);

        List<DebitTypeDTO> FindMandatoryDebitTypes(bool isMandatory, ServiceHeader serviceHeader);
    }
}
