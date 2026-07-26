using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IStandingOrderAppService
    {
        StandingOrderDTO AddNewStandingOrder(StandingOrderDTO standingOrderDTO, ServiceHeader serviceHeader);

        bool UpdateStandingOrder(StandingOrderDTO standingOrderDTO, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrders(ServiceHeader serviceHeader);

        PageCollectionInfo<StandingOrderDTO> FindStandingOrders(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<StandingOrderDTO> FindStandingOrders(string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<StandingOrderDTO> FindStandingOrders(int trigger, string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<StandingOrderHistoryDTO> FindStandingOrderHistory(Guid standingOrderId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        StandingOrderDTO FindStandingOrder(Guid standingOrderId, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrders(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrders(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerId(Guid benefactorCustomerId, int benefactorCustomerAccountTypeProductCode, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindDueStandingOrders(DateTime targetDate, int targetDateOption, string searchString, int customerAccountFilter, int customerFilter, ServiceHeader serviceHeader);

        PageCollectionInfo<StandingOrderDTO> FindSkippedStandingOrders(DateTime targetDate, string searchString, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool AutoCreateStandindOrders(Guid benefactorProductId, int benefactorProductCode, Guid beneficiaryProductId, ServiceHeader serviceHeader);

        bool FixSkippedStandingOrders(DateTime targetDate, int pageSize, ServiceHeader serviceHeader);
    }
}
