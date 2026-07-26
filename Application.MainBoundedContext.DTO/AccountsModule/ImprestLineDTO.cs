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
    public class ImprestLineDTO : BindingModelBase<ImprestLineDTO>
    {

        public ImprestLineDTO()
        {

            AddAllAttributeValidators();

        }

        [DataMember]
        [Display(Name = "ExpenseCategory")]
        public string ExpenseCategory { get; set; }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [ValidGuid]
        [Display(Name = "ImprestId")]
        public Guid ImprestId { get; set; }

        [DataMember]
        [Display(Name = "LineNo")]
        public int LineNo { get; set; }


        [DataMember]
        [ValidGuid]
        [Display(Name = "ExpenseChartOfAccountId")]
        public Guid ExpenseChartOfAccountId { get; set; }

        [DataMember]
        [ValidGuid]
        [Display(Name = "ImprestDebtorsChartOfAccountId")]
        public Guid ImprestDebtorsChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "ImprestDebtors")]
        public string ImprestDebtors { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }


        [DataMember]
        [Display(Name = "AmountSpent")]
        public Decimal AmountSpent { get; set; }



        [DataMember]
        [Display(Name = "AmountReturned")]
        public Decimal AmountReturned { get; set; }


        [DataMember]
        [Display(Name = "ReimbursementAmount")]
        public Decimal ReimbursementAmount { get; set; }



        [DataMember]
        [Display(Name = "BankChartOfAccountId")]
        public Guid BankChartOfAccountId { get; set; }
    }
}
