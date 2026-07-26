using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ITellerService
    {
        #region Teller

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TellerDTO AddTeller(TellerDTO tellerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateTeller(TellerDTO tellerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TellerDTO> FindTellers(bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TellerDTO FindTeller(Guid tellerId, bool includeBalance);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TellerDTO> FindTellersInPage(int pageIndex, int pageSize, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TellerDTO> FindTellersByFilterInPage(int tellerType, string text, int pageIndex, int pageSize, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TellerDTO> FindTellersByType(int tellerType, string reference, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TellerDTO> FindTellersByReference(string reference, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TellerDTO FindTellerByEmployeeId(Guid employeeId, bool includeBalance);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTellerCashTariffs(CustomerAccountDTO customerAccountDTO, decimal totalValue, int frontOfficeTransactionType);

        #endregion
    }
}
