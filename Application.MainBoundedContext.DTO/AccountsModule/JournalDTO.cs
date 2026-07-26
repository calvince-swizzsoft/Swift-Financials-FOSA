using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalDTO : BindingModelBase<JournalDTO>
    {
        public JournalDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Sequential Id")]
        public Guid SequentialId { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string BranchAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid BranchCompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }
        
        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Log")]
        public Guid? AlternateChannelLogId { get; set; }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        public string PrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        public string SecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "App. User Name")]
        public string ApplicationUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. User Name")]
        public string EnvironmentUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. Machine Name")]
        public string EnvironmentMachineName { get; set; }

        [DataMember]
        [Display(Name = "Env. Domain Name")]
        public string EnvironmentDomainName { get; set; }

        [DataMember]
        [Display(Name = "Env. Operating System Version")]
        public string EnvironmentOSVersion { get; set; }

        [DataMember]
        [Display(Name = "Env. MAC Address")]
        public string EnvironmentMACAddress { get; set; }

        [DataMember]
        [Display(Name = "Env. Motherboard Serial Number")]
        public string EnvironmentMotherboardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Env. Processor Id")]
        public string EnvironmentProcessorId { get; set; }

        [DataMember]
        [Display(Name = "Env. IP Address")]
        public string EnvironmentIPAddress { get; set; }

        [DataMember]
        [Display(Name = "Module Navigation Item Code")]
        public int ModuleNavigationItemCode { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public int TransactionCode { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public string TransactionCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemTransactionCode), TransactionCode) ? EnumHelper.GetDescription((SystemTransactionCode)TransactionCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Value Date")]
        public DateTime? ValueDate { get; set; }

        [DataMember]
        [Display(Name = "Suppress Account Alert")] /*account alerts are generated when ParentId is null*/
        public bool SuppressAccountAlert { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")] /*locked when reversed*/
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Integrity Hash")]
        public string IntegrityHash { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Transaction Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public byte Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherType), (int)Type) ? EnumHelper.GetDescription((JournalVoucherType)Type) : string.Empty;
            }
        }
    }
}
