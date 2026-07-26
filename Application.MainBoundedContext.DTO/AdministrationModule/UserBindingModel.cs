using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class UserBindingModel : BindingModelBase<UserBindingModel>
    {
        public UserBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        [Required]
        public string OtherNames { get; set; }

        [DataMember]
        [Display(Name = "UserName")]
        [Required]
        public string UserName { get; set; }

        [DataMember]
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataMember]
        [Display(Name = "Phone Number")]
        [Required]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "Entered phone length is not valid.")]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid? BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Name")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "2FA Enabled")]
        public bool TwoFactorEnabled { get; set; }

        [DataMember]
        [Display(Name = "Lock User ?")]
        public bool LockoutEnabled { get; set; }

        [DataMember]
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Role")]
        public Guid? RoleId { get; set; }

        [DataMember]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid? CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "CallbackUrl")]
        public string CallbackUrl { get; set; }

        [Display(Name = "FullName")]
        public string EmployeeCustomerFullName { get; set; }


        [Display(Name = "Token")]
        public string Token { get; set; }

        [Display(Name = "Provider")]
        public int Provider { get; set; }

        [Display(Name = "Designation")]
        public string EmployeeDesignationDescription { get; set; }
        public string Actions { get; set; }
    }
}
