using Application.Seedwork;
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
    public class PurchaseInvoiceLineDTO : BindingModelBase<PurchaseInvoiceLineDTO>
    {

        public PurchaseInvoiceLineDTO()
        {

            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "PurchaseInvoiceId")]

        public Guid PurchaseInvoiceId { get; set; }

        [DataMember]
        [Display(Name = "PurchaseInvoiceNo")]

        public string PurchaseInvoiceNo { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PurchaseInvoiceEntryType), Type) ? EnumHelper.GetDescription((PurchaseInvoiceEntryType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "No")]
        public int No { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "UnitCost")]
        public Decimal UnitCost { get; set; }



        [DataMember]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public Decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "DebitChartOfAccountId")]
        public Guid DebitChartOfAccountId { get; set; }



    }
}
