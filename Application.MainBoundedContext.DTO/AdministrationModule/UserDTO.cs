using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class UserDTO
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Other Names")]
        public string OtherNames { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Branch")]
        public Guid? BranchId { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchDescription { get; set; }

        [Display(Name = "2FA Enabled")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Lock User ?")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Role")]
        public Guid? RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Customer")]
        public Guid? CustomerId { get; set; }
        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Callback Url")]
        public string CallbackUrl { get; set; }

        [Display(Name = "Token")]
        public string Token { get; set; }

        [Display(Name = "Provider")]
        public int Provider { get; set; }

        public string Actions { get; set; }
    }
}
