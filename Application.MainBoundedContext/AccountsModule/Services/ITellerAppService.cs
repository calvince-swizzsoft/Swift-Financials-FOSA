using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ITellerAppService
    {
        TellerDTO AddNewTeller(TellerDTO tellerDTO, ServiceHeader serviceHeader);

        bool UpdateTeller(TellerDTO tellerDTO, ServiceHeader serviceHeader);

        List<TellerDTO> FindTellers(ServiceHeader serviceHeader);

        PageCollectionInfo<TellerDTO> FindTellers(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<TellerDTO> FindTellers(int tellerType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<TellerDTO> FindTellers(int tellerType, string reference, ServiceHeader serviceHeader);

        List<TellerDTO> FindTellers(string reference, ServiceHeader serviceHeader);

        TellerDTO FindTeller(Guid tellerId, ServiceHeader serviceHeader);

        TellerDTO FindTellerByEmployeeId(Guid employeeId, ServiceHeader serviceHeader);
        
        void FetchTellerBalances(List<TellerDTO> tellers, ServiceHeader serviceHeader);

        List<TariffWrapper> ComputeCashTariffs(CustomerAccountDTO customerAccountDTO, decimal totalValue, int frontOfficeTransactionType, ServiceHeader serviceHeader);
    }
}
