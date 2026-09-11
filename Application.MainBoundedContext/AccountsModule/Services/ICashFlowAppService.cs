using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ICashFlowAppService
    {
        Task<List<CashFlowMappingDTO>> GetMappings(ServiceHeader header);
        Task SaveMapping(CashFlowMappingDTO mapping, ServiceHeader header);
        Task RemoveMapping(Guid accountId, ServiceHeader header);
        Task<CashFlowReportDTO> Generate(DateTime startDate, DateTime endDate, Guid? branchId, ServiceHeader header);
        Task<List<CashFlowDetailDTO>> GetDetails(DateTime startDate, DateTime endDate, Guid? branchId, string section, string line, int pageIndex, int pageSize, ServiceHeader header);
    }

    public class CashFlowValidationException : Exception
    {
        public CashFlowValidationException(string message) : base(message) { }
    }
}
