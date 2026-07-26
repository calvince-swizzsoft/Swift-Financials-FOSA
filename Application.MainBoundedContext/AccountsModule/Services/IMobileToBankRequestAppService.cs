using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IMobileToBankRequestAppService
    {
        MobileToBankRequestDTO AddNewMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader);

        bool ReconcileMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader);

        bool AuditMobileToBankRequestReconciliation(MobileToBankRequestDTO mobileToBankRequestDTO, int requestAuthOption, ServiceHeader serviceHeader);

        List<MobileToBankRequestDTO> FindMobileToBankRequests(ServiceHeader serviceHeader);

        PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequests(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequests(int status, int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequests(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        MobileToBankRequestDTO FindMobileToBankRequest(Guid mobileToBankRequestId, ServiceHeader serviceHeader);
    }
}
