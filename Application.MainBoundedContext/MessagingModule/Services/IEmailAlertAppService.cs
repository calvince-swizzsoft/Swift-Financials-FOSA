using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public interface IEmailAlertAppService
    {
        EmailAlertDTO AddNewEmailAlert(EmailAlertDTO emailAlertDTO, ServiceHeader serviceHeader);

        bool AddNewEmailAlerts(List<EmailAlertDTO> emailAlertDTOs, ServiceHeader serviceHeader);

        bool UpdateEmailAlert(EmailAlertDTO emailAlertDTO, ServiceHeader serviceHeader);

        List<EmailAlertDTO> FindEmailAlerts(ServiceHeader serviceHeader);

        List<EmailAlertDTO> FindEmailAlertsByDLRStatus(int dlrStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<EmailAlertDTO> FindEmailAlerts(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmailAlertDTO> FindEmailAlerts(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader);

        PageCollectionInfo<EmailAlertDTO> FindEmailAlerts(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<EmailAlertDTO> FindEmailAlertsByDLRStatusAndOrigin(int dlrStatus, int origin, ServiceHeader serviceHeader);

        EmailAlertDTO FindEmailAlert(Guid emailAlertId, ServiceHeader serviceHeader);

        bool AddEmailAlertsWithHistory(GroupEmailAlertDTO groupEmailAlertDTO, ServiceHeader serviceHeader);

        bool AddQuickEmailAlert(QuickEmailAlertDTO quickEmailAlertDTO, ServiceHeader serviceHeader);
    }
}
