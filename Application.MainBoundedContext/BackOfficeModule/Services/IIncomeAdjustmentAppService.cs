using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface IIncomeAdjustmentAppService
    {
        IncomeAdjustmentDTO AddNewIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO, ServiceHeader serviceHeader);

        bool UpdateIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO, ServiceHeader serviceHeader);

        List<IncomeAdjustmentDTO> FindIncomeAdjustments(ServiceHeader serviceHeader);

        PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustments(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        IncomeAdjustmentDTO FindIncomeAdjustment(Guid incomeAdjustmentId, ServiceHeader serviceHeader);
    }
}
