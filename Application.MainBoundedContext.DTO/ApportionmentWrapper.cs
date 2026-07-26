
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using Application.MainBoundedContext.DTO.AccountsModule;
using Newtonsoft.Json;

namespace Application.MainBoundedContext.DTO
{
    public class ApportionmentWrapper : BindingModelBase<ApportionmentWrapper>
    {
        public ApportionmentWrapper()
        {
            CreditCustomerAccount = new CustomerAccountDTO();
            DebitCustomerAccount = new CustomerAccountDTO();
            AddAllAttributeValidators();
        }

        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Apportion To")]
        public int ApportionTo { get; set; }

        [Display(Name = "Apportion To")]
        public string ApportionToDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ApportionTo), ApportionTo) ? EnumHelper.GetDescription((ApportionTo)ApportionTo) : string.Empty;
            }
        }

        [Display(Name = "Credit G/L Account")]
        [ValidGuid]
        public Guid CreditChartOfAccountId { get; set; }

        [Display(Name = "Credit Customer Account")]
        [ValidGuid]
        public Guid CreditCustomerAccountId { get; set; }

        [Display(Name = "Credit Customer Account")]
        public CustomerAccountDTO CreditCustomerAccount { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerFullName { get; set; }
                    
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [Display(Name = "Debit Customer Account")]
        [ValidGuid]
        public Guid DebitCustomerAccountId { get; set; }

        [Display(Name = "Debit Customer Account")]
        public CustomerAccountDTO DebitCustomerAccount { get; set; }

        [Display(Name = "Full Account Number")]
        public string FullAccountNumber { get; set; }

        [Display(Name = "Product Name")]
        public string ProductDescription { get; set; }

        decimal _principal;
        [Display(Name = "Principal")]
        public decimal Principal
        {
            get { return _principal; }
            set { _principal = Math.Abs(value); }
        }

        decimal _interest;
        [Display(Name = "Interest")]
        public decimal Interest
        {
            get { return _interest; }
            set { _interest = Math.Abs(value); }
        }
        
        [Display(Name = "Carry Forwards Balance")]
        public decimal CarryForwardsBalance { get; set; }

        [Display(Name = "Primary Description")]
        [Required]
        public string PrimaryDescription { get; set; }

        [Display(Name = "Secondary Description")]
        [Required]
        public string SecondaryDescription { get; set; }

        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [Display(Name = "Recover Carry Forwards?")]
        public bool RecoverCarryForwards { get; set; }

        public string CreditCustomerAccountJson
        {

            get
            {
                return CreditCustomerAccount == null ? null : JsonConvert.SerializeObject(CreditCustomerAccount);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    CreditCustomerAccount = null;
                }
                else 
                {
                    CreditCustomerAccount = JsonConvert.DeserializeObject<CustomerAccountDTO>(value);
                }
            }
        }

        public string DebitCustomerAccountJson
        {
            get
            {
                return DebitCustomerAccount == null ? null : JsonConvert.SerializeObject(DebitCustomerAccount);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DebitCustomerAccount = null;
                }
                else
                {
                    DebitCustomerAccount = JsonConvert.DeserializeObject<CustomerAccountDTO>(value);
                }
            }
        }


    }
}
