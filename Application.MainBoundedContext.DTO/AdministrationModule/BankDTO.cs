using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{

    public class BankDTO : BindingModelBase<BankDTO>
    {
        public BankDTO()
        {
            AddAllAttributeValidators();
            BankBranche = new ObservableCollection<BankBranchDTO>();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [DataMember]
        [Display(Name = "Padded Code")]
        public string PaddedCode
        {
            get
            {
                return Code.ToString().PadLeft(2, '0');
            }
        }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Address")]
        [Required]
        public string Address { get; set; }

        [DataMember]
        [Display(Name = "City")]
        [Required]
        public string City { get; set; }


        [DataMember]
        [Display(Name = "IBAN")]
        [Required]
        public string IbanNo { get; set; }


        [DataMember]
        [Display(Name = "SwiftCode")]
        [Required]
        public string SwiftCode { get; set; }


        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        // Using ObservableCollection for BankBranches
        private ObservableCollection<BankBranchDTO> _bankBranches;
        public BankBranchDTO BankBranches { get; set; }
        [DataMember]
        [Display(Name = "Bank Branches")]
        public ObservableCollection<BankBranchDTO> BankBranche
        {
            get
            {
                if (_bankBranches == null)
                {
                    _bankBranches = new ObservableCollection<BankBranchDTO>();
                }
                return _bankBranches;
            }
            set
            {
                _bankBranches = value;
            }
        }



        [DataMember]
        public ObservableCollection<BankBranchDTO> BankBranchesDTO { get; set; }
        public string ErrorMessageResult { get; set; }

        //BankLinkageDTO
       
        [DataMember]
        [Display(Name = "Bank")]
        [Required]
        public string BankName { get; set; }

        [DataMember]
        [Display(Name = "Bank Branch")]
        [Required]
        public string BankBranchName { get; set; }

        [DataMember]
        [Display(Name = "Bank Account Number")]
        [Required]
        public string BankAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

    
        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Name")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }


        [DataMember]
        [Display(Name = "No")]
        public int No { get; set; }


    }
}
