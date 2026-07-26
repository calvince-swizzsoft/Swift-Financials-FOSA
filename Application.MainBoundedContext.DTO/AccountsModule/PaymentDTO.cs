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
    public class PaymentDTO
    {


        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }


        [DataMember]
        [Display(Name = "InvoiceNo")]
        public string InvoiceNo { get; set; }

        [DataMember]
        [Display(Name = "VendorNo")]
        public string VendorNo { get; set; }

        [DataMember]
        [ValidGuid]
        [Display(Name = "InvoiceId")]
        public Guid InvoiceId { get; set; }


        [DataMember]
        [ValidGuid]
        [Display(Name = "VendorId")]
        public Guid VendorId { get; set; }


        [DataMember]
        [Display(Name = "VendorAddress")]
        public string VendorAddress { get; set; }

        [DataMember]
        [Display(Name = "VendorName")]
        public string VendorName { get; set; }


        [DataMember]
        [Display(Name = "BankLinkageChartOfAccountId")]
        public Guid BankLinkageChartOfAccountId { get; set; }


        [DataMember]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }


        [DataMember]
        [Display(Name = "PaymentLines")]
        public HashSet<PaymentLineDTO> PaymentLines { get; set; }


        [DataMember]
        [Display(Name = "VoucherNumber")]
        public string VoucherNumber { get; set; }

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
