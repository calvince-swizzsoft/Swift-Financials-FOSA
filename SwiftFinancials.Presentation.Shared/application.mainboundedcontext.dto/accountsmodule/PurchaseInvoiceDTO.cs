using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class PurchaseInvoiceDTO : BindingModelBase<PurchaseInvoiceDTO>
    {

        public PurchaseInvoiceDTO()
        {

            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "No")]
        public string No { get; set; }

        [DataMember]
        [Display(Name = "VendorNo")]
        public string VendorNo { get; set; }

        [DataMember]
        [Display(Name = "VendorName")]
        public string VendorName { get; set; }

        [DataMember]
        [Display(Name = "VendorAddress")]
        public string VendorAddress { get; set; }


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
        [Display(Name = "PurchaseInvoiceLines")]
        public HashSet<PurchaseInvoiceLineDTO> PurchaseInvoiceLines { get; set; }

     

        [Display(Name = "BankId")]
        public Guid BankId { get; set; }

        [Display(Name = "BrahnchId")]
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


    }
}