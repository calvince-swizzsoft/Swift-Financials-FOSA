using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeTypeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg
{
    public class Employee : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid DesignationId { get; set; }

        public virtual Designation Designation { get; private set; }

        public Guid DepartmentId { get; set; }

        public virtual Department Department { get; private set; }

        public Guid? EmployeeTypeId { get; set; }

        public virtual EmployeeType EmployeeType { get; private set; }
        
        public string NationalSocialSecurityFundNumber { get; set; }

        public string NationalHospitalInsuranceFundNumber { get; set; }

        public byte BloodGroup { get; set; }

        public string Remarks { get; set; }

        public bool OnlineNotificationsEnabled { get; set; }

        public bool EnforceBiometricsForLogin { get; set; }

        public bool IsLocked { get; private set; }
        
        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
