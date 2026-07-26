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
    public class TrainingPeriodService : ITrainingPeriodService
    {
        private readonly ITrainingPeriodAppService _trainingPeriodAppService;

        public TrainingPeriodService(ITrainingPeriodAppService trainingPeriodAppService)
        {
            Guard.ArgumentNotNull(trainingPeriodAppService, nameof(trainingPeriodAppService));

            _trainingPeriodAppService = trainingPeriodAppService;
        }

        #region TrainingPeriodDTO

        public TrainingPeriodDTO AddNewTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.AddNewTrainingPeriod(trainingPeriodDTO, serviceHeader);
        }

        public TrainingPeriodDTO FindTrainingPeriod(Guid trainingPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.FindTrainingPeriod(trainingPeriodId, serviceHeader);
        }

        public PageCollectionInfo<TrainingPeriodDTO> FindTrainingPeriodsFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.FindTrainingPeriods(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public bool UpdateTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.UpdateTrainingPeriod(trainingPeriodDTO, serviceHeader);
        }

        #endregion

        #region TrainingPeriodEntryDTO

        public TrainingPeriodEntryDTO AddTrainingPeriodEntry(TrainingPeriodEntryDTO trainingPeriodEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.AddNewTrainingPeriodEntry(trainingPeriodEntryDTO, serviceHeader);
        }

        public PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByEmployeeIdFilterInPage(Guid employeeId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.FindTrainingPeriodEntriesByEmployeeId(employeeId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByTrainingPeriodIdFilterInPage(Guid trainingPeriodId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.FindTrainingPeriodEntriesByTrainingPeriodId(trainingPeriodId, text, pageIndex, pageSize, serviceHeader);
        }

        public bool RemoveTrainingPeriodEntries(List<TrainingPeriodEntryDTO> trainingPeriodEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.RemoveTrainingPeriodEntries(trainingPeriodEntries, serviceHeader);
        }

        public bool UpdateTrainingPeriodEntriesByTrainingPeriodId(Guid trainingPeriodId, List<TrainingPeriodEntryDTO> trainingPeriodEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _trainingPeriodAppService.UpdateTrainingPeriodEntriesByTrainingPeriodId(trainingPeriodId, trainingPeriodEntries, serviceHeader);
        }

        #endregion
    }
}
