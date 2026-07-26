using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ImprestDTO : BindingModelBase<ImprestDTO>
    {

        public ImprestDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [ValidGuid]
        [Display(Name = "BankId")]
        public Guid BankId { get; set; }

        [DataMember]
        [Display(Name = "BranchId")]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "BankBranchName")]
        public string BankBranchName { get; set; }



        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "No")]
        public string No { get; set; }

        [DataMember]
        [Display(Name = "EmployeeNo")]
        public string EmployeeNo { get; set; }

        [DataMember]
        //[ValidGuid]
        [Display(Name = "EmployeeId")]
        public Guid EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "EmployeeName")]
        public string EmployeeName { get; set; }

        [DataMember]
        [Display(Name = "RequestDate")]
        public DateTime RequestDate { get; set; }

        [DataMember]
        [Display(Name = "Purpose")]
        public string Purpose { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [DataMember]
        [Display(Name = "AmountRequested")]
        public decimal AmountRequested { get; set; }

        [DataMember]
        [Display(Name = "AmountApproved")]
        public decimal AmountApproved { get; set; }

        [DataMember]
        [Display(Name = "PayingBankAccount")]
        public Guid BankChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "ImprestLines")]
        public HashSet<ImprestLineDTO> ImprestLines { get; set; }

       

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Posted")]
        public bool Posted { get; set; }


        [DataMember]
        [Display(Name = "Surrendered")]
        public bool Surrendered { get; set; }


        [DataMember]
        [Display(Name = "SurrenderDate")]
        public DateTime SurrenderDate { get; set; }


        [DataMember]
        [Display(Name = "AmountSpent")]
        public Decimal AmountSpent { get; set; }



        [DataMember]
        [Display(Name = "AmountReturned")]
        public Decimal AmountReturned { get; set; }


        [DataMember]
        [Display(Name = "ReimbursementAmount")]
        public Decimal ReimbursementAmount { get; set; } 
    }
}
