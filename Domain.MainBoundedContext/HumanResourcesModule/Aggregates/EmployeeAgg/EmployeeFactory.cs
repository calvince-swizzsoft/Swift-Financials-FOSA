using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg
{
    public static class EmployeeFactory
    {
        public static Employee CreateEmployee(Guid customerId, Guid branchId, Guid designationId, Guid departmentId, Guid employeeTypeId, string nationalSocialSecurityFundNumber, string nationalHospitalInsuranceFundNumber, int bloodGroup, string remarks, bool onlineNotificationsEnabled, bool enforceBiometricsForLogin)
        {
            var employee = new Employee();

            employee.GenerateNewIdentity();

            employee.CustomerId = customerId;

            employee.BranchId = branchId;

            employee.DesignationId = designationId;

            employee.DepartmentId = departmentId;

            employee.EmployeeTypeId = employeeTypeId;

            employee.NationalSocialSecurityFundNumber = nationalSocialSecurityFundNumber;

            employee.NationalHospitalInsuranceFundNumber = nationalHospitalInsuranceFundNumber;

            employee.BloodGroup = (byte)bloodGroup;

            employee.Remarks = remarks;

            employee.OnlineNotificationsEnabled = onlineNotificationsEnabled;

            employee.EnforceBiometricsForLogin = enforceBiometricsForLogin;

            employee.CreatedDate = DateTime.Now;

            return employee;
        }
    }
}
