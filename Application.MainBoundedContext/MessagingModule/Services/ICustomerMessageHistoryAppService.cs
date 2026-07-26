using Application.MainBoundedContext.DTO.MessagingModule;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public interface ICustomerMessageHistoryAppService
    {
        CustomerMessageHistoryDTO AddNewCustomerMessageHistory(CustomerMessageHistoryDTO customerMessageHistoryDTO, ServiceHeader serviceHeader);

        List<CustomerMessageHistoryDTO> FindCustomerMessageHistories(ServiceHeader serviceHeader);
    }
}
