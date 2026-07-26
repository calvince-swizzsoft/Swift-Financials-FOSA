using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FileMovementHistoryAgg
{
    public static class FileMovementHistorySpecifications
    {
        public static Specification<FileMovementHistory> DefaultSpec()
        {
            Specification<FileMovementHistory> specification = new TrueSpecification<FileMovementHistory>();

            return specification;
        }

        public static Specification<FileMovementHistory> FileMovementHistoryWithFileRegisterId(Guid fileRegisterId)
        {
            Specification<FileMovementHistory> specification = new TrueSpecification<FileMovementHistory>();

            if (fileRegisterId != null && fileRegisterId != Guid.Empty)
            {
                var fileRegisterIdSpec = new DirectSpecification<FileMovementHistory>(h => h.FileRegisterId == fileRegisterId);

                specification &= fileRegisterIdSpec;
            }

            return specification;
        }

        public static Specification<FileMovementHistory> FileMovementHistoryWithSourceDepartmentIdAndCustomerId(Guid sourceDepartmentId, Guid customerId)
        {
            Specification<FileMovementHistory> specification = new TrueSpecification<FileMovementHistory>();

            if (sourceDepartmentId != null && sourceDepartmentId != Guid.Empty && customerId != null && customerId != Guid.Empty)
            {
                var fileRegisterIdSpec = new DirectSpecification<FileMovementHistory>(h => h.SourceDepartmentId == sourceDepartmentId && h.FileRegister.CustomerId == customerId);

                specification &= fileRegisterIdSpec;
            }

            return specification;
        }

        public static Specification<FileMovementHistory> FileMovementHistoryWithSourceDepartmentIdDestinationDepartmentIdAndCustomerId(Guid sourceDepartmentId, Guid destinationDepartmentId, Guid customerId)
        {
            Specification<FileMovementHistory> specification = new TrueSpecification<FileMovementHistory>();

            if (sourceDepartmentId != null && sourceDepartmentId != Guid.Empty && destinationDepartmentId != null && destinationDepartmentId != Guid.Empty && customerId != null && customerId != Guid.Empty)
            {
                var fileRegisterIdSpec = new DirectSpecification<FileMovementHistory>(h => h.SourceDepartmentId == sourceDepartmentId && h.DestinationDepartmentId == destinationDepartmentId && h.FileRegister.CustomerId == customerId);

                specification &= fileRegisterIdSpec;
            }

            return specification;
        }

    }
}
