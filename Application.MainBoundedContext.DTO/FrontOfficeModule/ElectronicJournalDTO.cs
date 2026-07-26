
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class ElectronicJournalDTO : BindingModelBase<ElectronicJournalDTO>
    {
        public ElectronicJournalDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [DataMember]
        [Display(Name = "Header Record Type")]
        public string HeaderRecordRecordType { get; set; }

        [DataMember]
        [Display(Name = "Header File Type")]
        public string HeaderRecordFileType { get; set; }

        [DataMember]
        [Display(Name = "Date of File Exchange")]
        public DateTime HeaderRecordDateOfFileExchange { get; set; }

        [DataMember]
        [Display(Name = "Presenting Organisation Clearing Centre")]
        public string HeaderRecordPresentingOrganisationClearingCentre { get; set; }

        [DataMember]
        [Display(Name = "Presenting Organisation Bank")]
        public string HeaderRecordPresentingOrganisationBank { get; set; }

        [DataMember]
        [Display(Name = "Presenting Organisation")]
        public string HeaderRecordPresentingOrganisation { get; set; }

        [DataMember]
        [Display(Name = "Receiving Organisation Clearing Centre")]
        public string HeaderRecordReceivingOrganisationClearingCentre { get; set; }

        [DataMember]
        [Display(Name = "Receiving Organisation Bank")]
        public string HeaderRecordReceivingOrganisationBank { get; set; }

        [DataMember]
        [Display(Name = "Receiving Organisation")]
        public string HeaderRecordReceivingOrganisation { get; set; }

        [DataMember]
        [Display(Name = "File Serial Number")]
        public string HeaderRecordFileSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Last File Indicator")]
        public string HeaderRecordLastFileIndicator { get; set; }

        [DataMember]
        [Display(Name = "Filler")]
        public string HeaderRecordFiller { get; set; }

        [DataMember]
        [Display(Name = "Trailer Record Type")]
        public string TrailerRecordRecordType { get; set; }

        [DataMember]
        [Display(Name = "Trailer Clearing Centre")]
        public string TrailerRecordClearingCentre { get; set; }

        [DataMember]
        [Display(Name = "Trailer Bank")]
        public string TrailerRecordBank { get; set; }

        [DataMember]
        [Display(Name = "Trailer Organisation")]
        public string TrailerRecordOrganisation { get; set; }

        [DataMember]
        [Display(Name = "Trailer Transaction Count")]
        public int TrailerRecordTransactionCount { get; set; }

        [DataMember]
        [Display(Name = "Total Value Credits")]
        public decimal TrailerRecordTotalValueCredits { get; set; }

        [DataMember]
        [Display(Name = "Total Value Debits")]
        public decimal TrailerRecordTotalValueDebits { get; set; }

        [DataMember]
        [Display(Name = "Trailer Filler")]
        public string TrailerRecordFiller { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ElectronicJournalStatus), Status) ? EnumHelper.GetDescription((ElectronicJournalStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Closed By")]
        public string ClosedBy { get; set; }
        
        [DataMember]
        [Display(Name = "Closed Date")]
        public DateTime? ClosedDate { get; set; }

        [DataMember]
        [Display(Name = "Items Count")]
        public int ItemsCount { get; set; }

        [DataMember]
        [Display(Name = "Processed Items Count")]
        public int ProcessedItemsCount { get; set; }

        [DataMember]
        [Display(Name = "Processed Entries")]
        public string ProcessedEntries { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
