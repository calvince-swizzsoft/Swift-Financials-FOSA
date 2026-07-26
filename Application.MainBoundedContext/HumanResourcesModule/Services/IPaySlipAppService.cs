using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IPaySlipAppService
    {
        bool PurgePaySlips(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader);

        bool AddNewPaySlips(List<PaySlipDTO> paySlipDTOs, ServiceHeader serviceHeader);

        bool MarkPaySlipPosted(Guid paySlipId, ServiceHeader serviceHeader);

        List<PaySlipDTO> FindPaySlips( ServiceHeader serviceHeader);

        List<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, ServiceHeader serviceHeader);

        int CountPaySlipsBySalaryPeriodId(Guid salaryPeriodId, ServiceHeader serviceHeader);

        int CountPostedPaySlipsBySalaryPeriodId(Guid salaryPeriodId, ServiceHeader serviceHeader);

        PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<PaySlipDTO> FindQueablePaySlips(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PaySlipDTO FindPaySlip(Guid paySlipId, ServiceHeader serviceHeader);

        PaySlipEntryDTO FindPaySlipEntry(Guid paySlipEntryId, ServiceHeader serviceHeader);

        List<PaySlipEntryDTO> FindPaySlipEntriesByPaySlipId(Guid paySlipId, ServiceHeader serviceHeader);

        List<PaySlipDTO> FindLoanAppraisalPaySlipsByCustomerId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);
    }
}
