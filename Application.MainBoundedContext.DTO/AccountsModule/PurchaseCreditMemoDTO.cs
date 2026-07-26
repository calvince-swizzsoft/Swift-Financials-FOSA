using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class PurchaseCreditMemoDTO : BindingModelBase<PurchaseCreditMemoDTO>
    {

        public PurchaseCreditMemoDTO()
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
        [Display(Name = "VendorNo")]
        public string ApprovalStatus { get; set; }


        [DataMember]
        [Display(Name = "PurchaseCreditMemoLines")]
        public HashSet<PurchaseCreditMemoLineDTO> PurchaseCreditMemoLines { get; set; }

        [Display(Name = "BankId")]
        public Guid BankId { get; set; }

        [Display(Name = "BrahnchId")]
        public Guid BranchId { get; set; }

        [Display(Name = "BankBranchName")]
        public string BankBranchName { get; set; }


        //[Display(Name = "InvoiceNo")]
        //public int InvoiceNo { get; set; }

        [Display(Name = "PurchaseInvoiceId")]
        public Guid PurchaseInvoiceId { get; set; }

        [Display(Name = ("Posted"))]
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

        //AppliesToDocNo
        [DataMember]
        [Display(Name = ("PurchaseInvoiceNo"))]
        public string PurchaseInvoiceNo { get; set; }


        //[Display(Name = "ChartOfAccountId")]
        //public string ChartOfAccountId { get; set; }

    }
}
