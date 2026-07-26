using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ReceiptDTO
    {


        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }


        [DataMember]
        [Display(Name = "InvoiceNo")]
        public string InvoiceNo { get; set; }

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
        [ValidGuid]
        [Display(Name = "InvoiceId")]
        public Guid InvoiceId { get; set; }


        [DataMember]
        [ValidGuid]
        [Display(Name = "CustomerId")]
        public Guid CustomerId { get; set; }


        [DataMember]
        [Display(Name = "BankLinkageChartOfAccountId")]
        public Guid BankLinkageChartOfAccountId { get; set; }


        [DataMember]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }


        [DataMember]
        [Display(Name = "ReceiptLines")]
        public HashSet<ReceiptLineDTO> ReceiptLines { get; set; }


        [DataMember]
        [Display(Name = "ReceiptNumber")]
        public string ReceiptNumber { get; set; }

        [DataMember]
        [Display(Name = "TotalAmount")]
        public decimal TotalAmount { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }



        [Display(Name = "BankId")]
        public Guid BankId { get; set; }

        [Display(Name = "BranchId")]
        public Guid BranchId { get; set; }

        [Display(Name = "BankBranchName")]
        public string BankBranchName { get; set; }


        [DataMember]
        [Display(Name = "Posted")]
        public Boolean Posted { get; set; }

    }
}
