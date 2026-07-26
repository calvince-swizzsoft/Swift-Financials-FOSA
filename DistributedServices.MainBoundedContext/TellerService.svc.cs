using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TellerService : ITellerService
    {
        private readonly ITellerAppService _tellerAppService;

        public TellerService(
           ITellerAppService tellerAppService)
        {
            Guard.ArgumentNotNull(tellerAppService, nameof(tellerAppService));

            _tellerAppService = tellerAppService;
        }

        #region Teller

        public TellerDTO AddTeller(TellerDTO tellerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _tellerAppService.AddNewTeller(tellerDTO, serviceHeader);
        }

        public bool UpdateTeller(TellerDTO tellerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _tellerAppService.UpdateTeller(tellerDTO, serviceHeader);
        }

        public List<TellerDTO> FindTellers(bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var tellers = _tellerAppService.FindTellers(serviceHeader);

            if (tellers != null)
            {
                //if (includeBalances)
                    _tellerAppService.FetchTellerBalances(tellers, serviceHeader);
            }

            return tellers;
        }

        public PageCollectionInfo<TellerDTO> FindTellersInPage(int pageIndex, int pageSize, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var tellers = _tellerAppService.FindTellers(pageIndex, pageSize, serviceHeader);

            if (tellers != null)
            {
                if (includeBalances)
                    _tellerAppService.FetchTellerBalances(tellers.PageCollection, serviceHeader);
            }

            return tellers;
        }

        public PageCollectionInfo<TellerDTO> FindTellersByFilterInPage(int tellerType, string text, int pageIndex, int pageSize, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var tellers = _tellerAppService.FindTellers(tellerType, text, pageIndex, pageSize, serviceHeader);

            if (tellers != null)
            {
                if (includeBalances)
                    _tellerAppService.FetchTellerBalances(tellers.PageCollection, serviceHeader);
            }

            return tellers;
        }

        public List<TellerDTO> FindTellersByType(int tellerType, string reference, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var tellers = _tellerAppService.FindTellers(tellerType, reference, serviceHeader);

            if (tellers != null)
            {
                if (includeBalances)
                    _tellerAppService.FetchTellerBalances(tellers, serviceHeader);
            }

            return tellers;
        }

        public List<TellerDTO> FindTellersByReference(string reference, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var tellers = _tellerAppService.FindTellers(reference, serviceHeader);

            if (tellers != null)
            {
                if (includeBalances)
                    _tellerAppService.FetchTellerBalances(tellers, serviceHeader);
            }

            return tellers;
        }

        public TellerDTO FindTeller(Guid tellerId, bool includeBalance)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var teller = _tellerAppService.FindTeller(tellerId, serviceHeader);

            if (teller != null)
            {
                if (includeBalance)
                    _tellerAppService.FetchTellerBalances(new List<TellerDTO> { teller }, serviceHeader);
            }

            return teller;
        }

        public TellerDTO FindTellerByEmployeeId(Guid employeeId, bool includeBalance)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var teller = _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);

            if (teller != null)
            {
                if (includeBalance)
                    _tellerAppService.FetchTellerBalances(new List<TellerDTO> { teller }, serviceHeader);
            }

            return teller;
        }
               
        public List<TariffWrapper> ComputeTellerCashTariffs(CustomerAccountDTO customerAccountDTO, decimal totalValue, int frontOfficeTransactionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _tellerAppService.ComputeCashTariffs(customerAccountDTO, totalValue, frontOfficeTransactionType, serviceHeader);
        }

        #endregion
    }
}
