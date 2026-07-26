using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class RecurringBatchDTO : BindingModelBase<RecurringBatchDTO>
    {
        public RecurringBatchDTO()
        {
            AddAllAttributeValidators();
        }

      
        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        Guid? _branchId;

        [DataMember]
        [Display(Name = "Branch")]
        public Guid? BranchId {

            get { return _branchId; }
            set
            {
                if (_branchId != value)
                {
                    _branchId = value;
                    OnPropertyChanged(() => BranchId);
                }
            }
        }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }


        Guid? _postingPeriodId;
        [DataMember]
        [Display(Name = "Posting Period")]
        public Guid? PostingPeriodId {

            get { return _postingPeriodId; }
            set
            {
                if (_postingPeriodId != value)
                {
                    _postingPeriodId = value;
                    OnPropertyChanged(() => PostingPeriodId);
                }
            }
        }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int BatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedBatchNumber
        {
            get
            {
                return string.Format("{0}", BatchNumber).PadLeft(7, '0');
            }
        }


        int _type;
        
        [DataMember]
        [Display(Name = "Type")]
        public int Type {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(() => Type);
                }
            }

        }

        [DataMember]
        [Display(Name = "Batch Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecurringBatchType), Type) ? EnumHelper.GetDescription((RecurringBatchType)Type) : string.Empty;
            }
        }


        int _month;
        [DataMember]
        [Display(Name = "Month")]
        public int Month {

            get { return _month;  }

            set 
            {
                if (_month != value) {
                    _month = value;
                    OnPropertyChanged(() => Month);
                } 
            
            }
        }


        [DataMember]
        [Display(Name = "Month")]
        public string MonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), Month) ? EnumHelper.GetDescription((Month)Month) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Interest Capitalization Months")]
       
        public int InterestCapitalizationMonths { get; set; }
        
        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }


        int _priority;
        [DataMember]
        [Display(Name = "Priority")]
        public int Priority { 
        
         get { return _priority; }

         set { if (_priority != value)
                {
                    _priority = value;

                    OnPropertyChanged(() => Priority);
                }
                        
          }
        }

        [DataMember]
        [Display(Name = "Priority")]
        public string PriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), Priority) ? EnumHelper.GetDescription((QueuePriority)Priority) : string.Empty;
            }
        }

        int _status;
        [DataMember]
        [Display(Name = "Status")]
        public int Status {
        
         get { return _status; }

            set { if (_status != value)
                {

                    _status = value;
                    OnPropertyChanged(() => Status);
                }          
                        
               }
        }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), Status) ? EnumHelper.GetDescription((BatchStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Enforce Month Value Date?")]
        public bool EnforceMonthValueDate { get; set; }

        [DataMember]
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public List<RecurringBatchDTO> SelectedRows { get; set; }
        [DataMember]
        [Display(Name = "CheckedCheckboxIds?")]
        public bool CheckedCheckboxIds { get; set; }
        [DataMember]
        [Display(Name = "CheckedRowsData?")]
        public bool CheckedRowsData { get; set; }
        public List<RecurringBatchEntryDTO> RecouringBatchEntries { get; set; }

       
    }
   
}
