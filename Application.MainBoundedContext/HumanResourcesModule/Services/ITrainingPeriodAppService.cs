using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ITrainingPeriodAppService
    {

        #region TrainingPeriodDTO

        TrainingPeriodDTO AddNewTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader);

        bool UpdateTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader);
        
        TrainingPeriodDTO FindTrainingPeriod(Guid trainingPeriodId, ServiceHeader serviceHeader);
        
        PageCollectionInfo<TrainingPeriodDTO> FindTrainingPeriods( DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        #endregion

        #region TrainingPeriodEntryDTO

        TrainingPeriodEntryDTO AddNewTrainingPeriodEntry(TrainingPeriodEntryDTO trainingPeriodEntryDTO, ServiceHeader serviceHeader);

        bool UpdateTrainingPeriodEntriesByTrainingPeriodId(Guid trainingPeriodId, List<TrainingPeriodEntryDTO> trainingPeriodEntries, ServiceHeader serviceHeader);
         
        PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByTrainingPeriodId(Guid trainingPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByEmployeeId(Guid employeeId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
         
        bool RemoveTrainingPeriodEntries(List<TrainingPeriodEntryDTO> trainingPeriodEntries, ServiceHeader serviceHeader);
        
        #endregion
    }
}
