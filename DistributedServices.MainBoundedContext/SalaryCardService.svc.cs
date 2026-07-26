using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
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
    public class SalaryCardService : ISalaryCardService
    {
        private readonly ISalaryCardAppService _salaryCardAppService;

        public SalaryCardService(
            ISalaryCardAppService salaryCardAppService)
        {
            Guard.ArgumentNotNull(salaryCardAppService, nameof(salaryCardAppService));

            _salaryCardAppService = salaryCardAppService;
        }

        #region Salary Card

        public SalaryCardDTO AddSalaryCard(SalaryCardDTO salaryCardDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.AddNewSalaryCard(salaryCardDTO, serviceHeader);
        }

        public bool UpdateSalaryCard(SalaryCardDTO salaryCardDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.UpdateSalaryCard(salaryCardDTO, serviceHeader);
        }

        public bool ResetSalaryCardEntries(SalaryCardDTO salaryCardDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.ResetSalaryCardEntries(salaryCardDTO, serviceHeader);
        }

        public bool UpdateSalaryCardEntry(SalaryCardEntryDTO salaryCardEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.UpdateSalaryCardEntry(salaryCardEntryDTO, serviceHeader);
        }

        public List<SalaryCardDTO> FindSalaryCards()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.FindSalaryCards(serviceHeader);
        }

        public PageCollectionInfo<SalaryCardDTO> FindSalaryCardsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.FindSalaryCards(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<SalaryCardDTO> FindSalaryCardsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.FindSalaryCards(text, pageIndex, pageSize, serviceHeader);
        }

        public SalaryCardDTO FindSalaryCard(Guid salaryCardId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.FindSalaryCard(salaryCardId, serviceHeader);
        }

        public List<SalaryCardEntryDTO> FindSalaryCardEntriesBySalaryCardId(Guid salaryCardId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryCardAppService.FindSalaryCardEntriesBySalaryCardId(salaryCardId, serviceHeader);
        }

        #endregion
    }
}
