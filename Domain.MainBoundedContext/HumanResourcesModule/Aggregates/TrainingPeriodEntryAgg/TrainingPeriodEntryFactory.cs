using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodEntryAgg
{
    public static class TrainingPeriodEntryFactory
    {
        public static TrainingPeriodEntry CreateTrainingPeriodEntry(Guid employeeId, Guid trainingPeriodId)
        {
            var trainingPeriodEntry = new TrainingPeriodEntry();

            trainingPeriodEntry.GenerateNewIdentity();

            trainingPeriodEntry.EmployeeId = employeeId;

            trainingPeriodEntry.TrainingPeriodId = trainingPeriodId;

            trainingPeriodEntry.CreatedDate = DateTime.Now;

            return trainingPeriodEntry;
        }
    }
}
