using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid BranchId { get; set; }

        public Guid DesignationId { get; set; }

        public Guid DepartmentId { get; set; }

        public int Type { get; set; }

        public string NationalSocialSecurityFundNumber { get; set; }

        public string NationalHospitalInsuranceFundNumber { get; set; }

        public int BloodGroup { get; set; }

        public string Remarks { get; set; }

        public bool IsLocked { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
