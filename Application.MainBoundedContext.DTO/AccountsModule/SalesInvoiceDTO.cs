using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SalesInvoiceDTO : BindingModelBase<SalesInvoiceDTO>
    {

        public SalesInvoiceDTO()
        {

            AddAllAttributeValidators();
        }


        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "SalesInvoiceNo")]
        public string No { get; set; }

        [DataMember]
        [Display(Name = "CustomerNo")]
        public string CustomerNo { get; set; }

        [DataMember]
        [Display(Name = "CustomerName")]
        public string CustomerName { get; set; }

        [DataMember]
        [Display(Name = "CustomerAddress")]
        public string CustomerAddress { get; set; }


        [DataMember]
        [Display(Name = "DocumentDate")]
        public DateTime DocumentDate { get; set; }

        [DataMember]
        [Display(Name = "PostingDate")]
        public DateTime PostingDate { get; set; }

        [DataMember]
        [Display(Name = "DueDate")]
        public DateTime DueDate { get; set; }

        [DataMember]
        [Display(Name = "ApprovalStatus")]
        public string ApprovalStatus { get; set; }


        [DataMember]
        [Display(Name = "SalesInvoiceLines")]
        public HashSet<SalesInvoiceLineDTO> SalesInvoiceLines { get; set; }

        [Display(Name = "BankId")]
        public Guid BankId { get; set; }

        [Display(Name = "BranchId")]
        public Guid BranchId { get; set; }

        [Display(Name = "BankBranchName")]
        public string BankBranchName { get; set; }


        [DataMember]
        [Display(Name = "Posted")]
        public Boolean Posted { get; set; }
        [DataMember]
        [Display(Name = "TotalAmount")]
        public Decimal TotalAmount { get; set; }

        [DataMember]
        [Display(Name = "PaidAmount")]
        public Decimal PaidAmount { get; set; }

        [DataMember]
        [Display(Name = ("RemainingAmount"))]
        public Decimal RemainingAmount { get; set; }


        //[Display(Name = "ChartOfAccountId")]
        //public string ChartOfAccountId { get; set; }

    }
}
