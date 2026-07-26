using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditOfficerAgg
{
    public static class MicroCreditOfficerFactory
    {
        public static MicroCreditOfficer CreateMicroCreditOfficer(Guid employeeId, string remarks)
        {
            var microCreditOfficer = new MicroCreditOfficer()
            {
                EmployeeId = employeeId,
                Remarks = remarks,
            };

            microCreditOfficer.GenerateNewIdentity();

            microCreditOfficer.CreatedDate = DateTime.Now;

            return microCreditOfficer;
        }
    }
}
