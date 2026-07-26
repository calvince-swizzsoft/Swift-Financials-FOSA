using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FileMovementHistoryAgg
{
    public static class FileMovementHistoryFactory
    {
        public static FileMovementHistory CreateFileMovementHistory(Guid fileRegisterId, Guid sourceDepartmentId, Guid destinationDepartmentId, string remarks, string carrier)
        {
            var fileMovementHistory = new FileMovementHistory();

            fileMovementHistory.GenerateNewIdentity();

            fileMovementHistory.FileRegisterId = fileRegisterId;

            fileMovementHistory.SourceDepartmentId = sourceDepartmentId;

            fileMovementHistory.DestinationDepartmentId = destinationDepartmentId;

            fileMovementHistory.Remarks = remarks;

            fileMovementHistory.Carrier = carrier;

            fileMovementHistory.CreatedDate = DateTime.Now;

            return fileMovementHistory;
        }
    }
}
