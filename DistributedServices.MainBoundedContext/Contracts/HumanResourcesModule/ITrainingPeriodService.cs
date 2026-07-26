using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITrainingService" in both code and config file together.
    [ServiceContract]
    public interface ITrainingPeriodService
    {

        #region TrainingPeriodDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TrainingPeriodDTO AddNewTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TrainingPeriodDTO FindTrainingPeriod(Guid trainingPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TrainingPeriodDTO> FindTrainingPeriodsFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        #endregion

        #region TrainingPeriodEntryDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TrainingPeriodEntryDTO AddTrainingPeriodEntry(TrainingPeriodEntryDTO trainingPeriodEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateTrainingPeriodEntriesByTrainingPeriodId(Guid trainingPeriodId, List<TrainingPeriodEntryDTO> trainingPeriodEntries); 

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByTrainingPeriodIdFilterInPage(Guid trainingPeriodId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByEmployeeIdFilterInPage(Guid employeeId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveTrainingPeriodEntries(List<TrainingPeriodEntryDTO> trainingPeriodEntries);

        #endregion
    }
}
