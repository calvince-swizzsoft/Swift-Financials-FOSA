using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodEntryAgg
{
    public class TrainingPeriodEntrySpecifications
    {
        public static Specification<TrainingPeriodEntry> DefaultSpec()
        {
            Specification<TrainingPeriodEntry> specification = new TrueSpecification<TrainingPeriodEntry>();

            return specification;
        }

        public static ISpecification<TrainingPeriodEntry> TrainingPeriodEntriesByTrainingPeriodIdWithText(Guid trainingPeriodId, string text)
        {
            Specification<TrainingPeriodEntry> specification = DefaultSpec();

            if (trainingPeriodId != null && trainingPeriodId != Guid.Empty)
            {
                specification = new DirectSpecification<TrainingPeriodEntry>(x => x.TrainingPeriodId == trainingPeriodId);

                if (!string.IsNullOrWhiteSpace(text))
                {
                    var trainingSpec = new DirectSpecification<TrainingPeriodEntry>(x => x.TrainingPeriodId == trainingPeriodId && x.TrainingPeriod.Description.Contains(text));

                    specification &= trainingSpec;
                }
            }
            return specification;
        }

        public static ISpecification<TrainingPeriodEntry> TrainingPeriodEntriesByEmployeeIdWithText(Guid employeeId, string text)
        {
            Specification<TrainingPeriodEntry> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty)
            {
                specification = new DirectSpecification<TrainingPeriodEntry>(x => x.EmployeeId == employeeId);

                if (!string.IsNullOrWhiteSpace(text))
                {
                    var employeeFirstNameSpec = new DirectSpecification<TrainingPeriodEntry>(x => x.EmployeeId == employeeId && (x.Employee.Customer.Individual.FirstName.Contains(text)));

                    var employeeLasNameSpec = new DirectSpecification<TrainingPeriodEntry>(x => x.EmployeeId == employeeId && x.Employee.Customer.Individual.LastName.Contains(text));

                    specification &= employeeFirstNameSpec || employeeLasNameSpec;
                }
            }
            return specification;
        }
         
        public static Specification<TrainingPeriodEntry> TrainingPeriodEntryWithEmployeeIdAndTrainingPeriodId(Guid employeeId, Guid trainingPeriodId)
        {
            Specification<TrainingPeriodEntry> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty && trainingPeriodId != null && trainingPeriodId != Guid.Empty)
            {
                var spec = new DirectSpecification<TrainingPeriodEntry>(c => c.EmployeeId == employeeId && c.TrainingPeriodId == trainingPeriodId);

                specification &= spec;
            }

            return specification;
        }
    }
}
