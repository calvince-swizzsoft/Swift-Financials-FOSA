using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg
{
    public static class TrainingPeriodFactory
    {
        public static TrainingPeriod CreateTrainingPeriod(string description, string venue, Duration duration, decimal totalValue, string documentNumber, string fileName, string title, string fileDescription, string mimeType, string remarks)
        {
            var trainingPeriod = new TrainingPeriod();

            trainingPeriod.GenerateNewIdentity();

            trainingPeriod.Description = description;

            trainingPeriod.Venue = venue;

            trainingPeriod.Duration = duration;

            trainingPeriod.TotalValue = totalValue;

            trainingPeriod.DocumentNumber = documentNumber;

            trainingPeriod.FileName = fileName;

            trainingPeriod.FileTitle = title;

            trainingPeriod.FileDescription = fileDescription;

            trainingPeriod.FileMIMEType = mimeType;

            trainingPeriod.Remarks = remarks;

            trainingPeriod.CreatedDate = DateTime.Now;

            return trainingPeriod;
        }
    }
}
