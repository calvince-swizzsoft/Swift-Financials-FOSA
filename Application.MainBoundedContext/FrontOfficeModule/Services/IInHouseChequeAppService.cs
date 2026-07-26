using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface IInHouseChequeAppService
    {
        InHouseChequeDTO AddNewInHouseCheque(InHouseChequeDTO inHouseChequeDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool AddNewInHouseCheques(List<InHouseChequeDTO> inHouseChequeDTOs, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<InHouseChequeDTO> FindInHouseCheques(ServiceHeader serviceHeader);

        PageCollectionInfo<InHouseChequeDTO> FindInHouseCheques(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<InHouseChequeDTO> FindInHouseCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<InHouseChequeDTO> FindInHouseCheques(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        InHouseChequeDTO FindInHouseCheque(Guid inHouseChequeId, ServiceHeader serviceHeader);

        List<InHouseChequeDTO> FindUnPrintedInHouseChequesByBranchId(Guid branchId, string text, ServiceHeader serviceHeader);

        PageCollectionInfo<InHouseChequeDTO> FindUnPrintedInHouseChequesByBranchId(Guid branchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool PrintInHouseCheque(InHouseChequeDTO inHouseChequeDTO, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);
    }
}
