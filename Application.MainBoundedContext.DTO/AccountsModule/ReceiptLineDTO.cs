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
    public class ReceiptLineDTO
    {


        //[DataMember]
        //[Display(Name = "Id")]
        //public Guid Id { get; set; }



        [DataMember]
        [Display(Name = "CustomerAccountNo")]
        public string CustomerAccountNo { get; set; }



        [DataMember]
        [Display(Name = "CustomerAccountId")]
        public Guid CustomerAccountId { get; set; }



        //[DataMember]

        //[Display(Name = "ReceiptId")]

        //public Guid ReceiptId { get; set; }

        //[Display(Name = "InvoiceId")]

        //public Guid InvoiceId { get; set; }

        //[Display(Name = "InvoiceNo")]

        //public int InvoiceNo { get; set; }

        //[Display(Name = "Type")]
        //public int Type { get; set; }

        //[Display(Name = "No")]
        //public int No { get; set; }

        [Display(Name = "CreditChartOfAccountId")]
        public Guid CreditChartOfAccountId { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Amount")]

        public Decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "AccountType")]
        public int AccountType { get; set; }

        [DataMember]
        [Display(Name = "AccountType")]
        public string AccountTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ReceiptAccountType), AccountType) ? EnumHelper.GetDescription((ReceiptAccountType)AccountType) : string.Empty;

            }
        }


        [DataMember]
        [Display(Name = "DocumentType")]
        public int DocumentType { get; set; }

        [DataMember]
        [Display(Name = "DocumentType")]
        public string DocumentTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ReceiptDocumentType), DocumentType) ? EnumHelper.GetDescription((ReceiptDocumentType)DocumentType) : string.Empty;

            }
        }

    }
}