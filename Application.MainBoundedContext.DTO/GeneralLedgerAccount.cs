using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO
{
    public class GeneralLedgerAccount : IComparable<GeneralLedgerAccount>
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

        [Display(Name = "Category Code")]
        public int Category { get; set; }

        [Display(Name = "Category")]
        public string CategoryDescription { get; set; }

        [Display(Name = "Type Code")]
        public int Type { get; set; }

        [Display(Name = "Type")]
        public string TypeDescription { get; set; }

        [Display(Name = "Code")]
        public int Code { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "G/L Account Name")]
        public string IndentedName { get; set; }

        [Display(Name = "Tree Depth")]
        public int Depth { get; set; }

        [Display(Name = "Linked")]
        public bool Linked { get; set; }

        [Display(Name = "Book Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Cost Center")]
        public Guid? CostCenterId { get; set; }

        [Display(Name = "Cost Center")]
        public string CostCenterDescription { get; set; }

        [Display(Name = "Is Control Account?")]
        public bool IsControlAccount { get; set; }

        [Display(Name = "Is Control Account?")]
        public string IsControlAccountDescription
        {
            get
            {
                return IsControlAccount ? "Yes" : "No";
            }
        }

        [Display(Name = "Is Reconciliation Account?")]
        public bool IsReconciliationAccount { get; set; }

        [Display(Name = "Is Reconciliation Account?")]
        public string IsReconciliationAccountDescription
        {
            get
            {
                return IsReconciliationAccount ? "Yes" : "No";
            }
        }

        [Display(Name = "Post Automatically Only?")]
        public bool PostAutomaticallyOnly { get; set; }

        [Display(Name = "Post Automatically Only?")]
        public string PostAutomaticallyOnlyDescription
        {
            get
            {
                return PostAutomaticallyOnly ? "Yes" : "No";
            }
        }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Is Locked?")]
        public string IsLockedDescription
        {
            get
            {
                return IsLocked ? "Yes" : "No";
            }
        }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public int CompareTo(GeneralLedgerAccount other)
        {
            return this.CompareTo(other);
        }
    }
}
