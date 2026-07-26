using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public interface ITextAlertAppService
    {
        bool AddNewBulkMessage(BulkMessageDTO bulkMessageDTO, ServiceHeader serviceHeader);

        bool AddNewUSSDMessage(USSDMessageDTO ussdMessageDTO, ServiceHeader serviceHeader);

        TextAlertDTO AddNewTextAlert(TextAlertDTO textAlertDTO, ServiceHeader serviceHeader);

        bool AddNewTextAlerts(List<TextAlertDTO> textAlertDTOs, ServiceHeader serviceHeader);

        bool UpdateTextAlert(TextAlertDTO textAlertDTO, ServiceHeader serviceHeader);

        List<TextAlertDTO> FindTextAlerts(ServiceHeader serviceHeader);

        List<TextAlertDTO> FindTextAlertsByMessageReference(string messageReference, ServiceHeader serviceHeader);

        List<TextAlertDTO> FindTextAlertsByDLRStatus(int dlrStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<TextAlertDTO> FindTextAlerts(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<TextAlertDTO> FindTextAlerts(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader);

        PageCollectionInfo<TextAlertDTO> FindTextAlerts(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        TextAlertDTO FindTextAlert(Guid textAlertId, ServiceHeader serviceHeader);

        List<TextAlertDTO> FindTextAlertsByDLRStatusAndOrigin(int dlrStatus, int origin, ServiceHeader serviceHeader);

        bool ProcessInboundMessage(InboundMessageDTO inboundMessageDTO, ServiceHeader serviceHeader);

        bool AddTextAlertsWithHistory(GroupTextAlertDTO groupTextAlertDTO, ServiceHeader serviceHeader);

        bool AddQuickTextAlert(QuickTextAlertDTO quickTextAlertDTO, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(int systemTransactionCode, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCachedCommissions(int systemTransactionCode, ServiceHeader serviceHeader);

        bool UpdateCommissions(int systemTransactionCode, CommissionDTO[] commissionDTOs, int chargeBenefactor, ServiceHeader serviceHeader);
    }
}
