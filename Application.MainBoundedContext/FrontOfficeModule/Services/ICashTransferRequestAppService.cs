using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface ICashTransferRequestAppService
    {
        Task<CashTransferRequestDTO> AddNewCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, ServiceHeader serviceHeader);

        Task<bool> AcknowledgeCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption, ServiceHeader serviceHeader);

        Task<List<CashTransferRequestDTO>> FindCashTransferRequestsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsAsync(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<CashTransferRequestDTO> FindCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader);

        Task<List<CashTransferRequestDTO>> FindMatureCashTransferRequestsAsync(Guid employeeId, ServiceHeader serviceHeader);

        Task<bool> UtilizeCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader);
        Task<PageCollectionInfo<CashTransferRequestDTO>> FindAllCashTransferRequestsAsync(DateTime startDate, DateTime endDate, string text, int status, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}