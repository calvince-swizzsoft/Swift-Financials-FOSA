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
    public class SalesCreditMemoDTO : BindingModelBase<SalesCreditMemoDTO>
    {

        public SalesCreditMemoDTO()
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
        [Display(Name = "SalesCreditMemoLines")]
        public HashSet<SalesCreditMemoLineDTO> SalesCreditMemoLines { get; set; }

        [Display(Name = "BankId")]
        public Guid BankId { get; set; }

        [Display(Name = "BraNchId")]
        public Guid BranchId { get; set; }

        [Display(Name = "BankBranchName")]
        public string BankBranchName { get; set; }


        //[Display(Name = "InvoiceNo")]
        //public int InvoiceNo { get; set; }


        [DataMember]
        [Display(Name = "AppliesToDocNo")]
        public string AppliesToDocNo { get; set; }

        [DataMember]
        [Display(Name = "SalesInvoiceId")]
        public Guid SalesInvoiceId { get; set; }

        [Display(Name = ("Posted"))]
        public Boolean Posted { get; set; }

         [DataMember]
        [Display(Name = "TotalAmount")]
        public decimal TotalAmount { get; set; }




        //[Display(Name = "ChartOfAccountId")]
        //public string ChartOfAccountId { get; set; }

    }
}
