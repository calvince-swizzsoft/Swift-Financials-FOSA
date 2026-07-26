using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "ITrainingPeriodService")]
    public interface ITrainingPeriodService
    {
        #region TrainingPeriodDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, AsyncCallback callback, Object state);
        TrainingPeriodDTO EndAddTrainingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))] 
        IAsyncResult BeginAddNewTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, AsyncCallback callback, Object state);
        TrainingPeriodDTO EndAddNewTrainingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, AsyncCallback callback, Object state); 
        bool EndUpdateTrainingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTrainingPeriod(Guid trainingPeriodId, AsyncCallback callback, Object state);
        TrainingPeriodDTO EndFindTrainingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTrainingPeriodsFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TrainingPeriodDTO> EndFindTrainingPeriodsFilterInPage(IAsyncResult result);

        #endregion

        #region TrainingPeriodEntryDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTrainingPeriodEntry(TrainingPeriodEntryDTO trainingPeriodEntryDTO, AsyncCallback callback, Object state);
        TrainingPeriodEntryDTO EndAddTrainingPeriodEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateTrainingPeriodEntriesByTrainingPeriodId(Guid trainingPeriodId, List<TrainingPeriodEntryDTO> trainingPeriodEntries, AsyncCallback callback, Object state);
        bool EndUpdateTrainingPeriodEntriesByTrainingPeriodId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTrainingPeriodEntriesByTrainingPeriodIdFilterInPage(Guid trainingPeriodId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TrainingPeriodEntryDTO> EndFindTrainingPeriodEntriesByTrainingPeriodIdFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTrainingPeriodEntriesByEmployeeIdFilterInPage(Guid employeeId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TrainingPeriodEntryDTO> EndFindTrainingPeriodEntriesByEmployeeIdFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveTrainingPeriodEntries(List<TrainingPeriodEntryDTO> trainingPeriodEntries, AsyncCallback callback, Object state);
        bool EndRemoveTrainingPeriodEntries(IAsyncResult result);

        #endregion
    }
}
