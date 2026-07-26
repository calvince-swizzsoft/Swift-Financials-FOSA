using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IElectronicStatementOrderAppService
    {
        ElectronicStatementOrderDTO AddNewElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO, ServiceHeader serviceHeader);

        bool UpdateElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO, ServiceHeader serviceHeader);

        bool FixSkippedElectronicStatementOrders(DateTime targetDate, ServiceHeader serviceHeader);

        List<ElectronicStatementOrderDTO> FindElectronicStatementOrders(ServiceHeader serviceHeader);

        PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrders(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrders(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ElectronicStatementOrderHistoryDTO> FindElectronicStatementOrderHistory(Guid electronicStatementOrderId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ElectronicStatementOrderDTO FindElectronicStatementOrder(Guid electronicStatementOrderId, ServiceHeader serviceHeader);

        ElectronicStatementOrderHistoryDTO FindElectronicStatementOrderHistory(Guid electronicStatementOrderHistoryId, ServiceHeader serviceHeader);

        List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerId(Guid customerId, int customerAccountTypeProductCode, ServiceHeader serviceHeader);

        List<ElectronicStatementOrderDTO> FindDueElectronicStatementOrders(DateTime targetDate, int targetDateOption, string searchString, int customerFilter, ServiceHeader serviceHeader);

        List<ElectronicStatementOrderDTO> FindSkippedElectronicStatementOrders(DateTime targetDate, string searchString, int customerFilter, ServiceHeader serviceHeader);
    }
}
