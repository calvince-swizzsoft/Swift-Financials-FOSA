using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class TrainingPeriodDTO : BindingModelBase<TrainingPeriodDTO>
    {
        public TrainingPeriodDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Venue")]
        [Required]
        public string Venue { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        [Required]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        [Required]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Document #")]
        public string DocumentNumber { get; set; }

        [DataMember]
        [Display(Name = "Document")]
        public string FileName { get; set; }

        [DataMember]
        [Display(Name = "Title")]
        public string FileTitle { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string FileDescription { get; set; }

        [DataMember]
        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [DataMember]
        [Display(Name = "Buffer")]
        public byte[] FileBuffer { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public List<TrainingPeriodEntryDTO> TrainingPeriodEntries { get; set; }

    }
}
