using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class SingleDestinationDispatchModel : BindingModelBase<SingleDestinationDispatchModel>
    {
        public SingleDestinationDispatchModel()
        {
            AddAllAttributeValidators();
        }
        [DataMember]
        [Display(Name = "Dept Description")]
        public string SourceDepartment { get; set; }

        Guid _sourceDepartmentId;
        [Display(Name = "Source Department")]
        [ValidGuid]
        public Guid SourceDepartmentId
        {
            get { return _sourceDepartmentId; }
            set
            {
                if (_sourceDepartmentId != value)
                {
                    _sourceDepartmentId = value;
                    OnPropertyChanged(() => SourceDepartmentId);
                }
            }
        }

        Guid _destinationDepartmentId;
        [Display(Name = "Destination Department")]
        [ValidGuid]
        public Guid DestinationDepartmentId
        {
            get { return _destinationDepartmentId; }
            set
            {
                if (_destinationDepartmentId != value)
                {
                    _destinationDepartmentId = value;
                    OnPropertyChanged(() => DestinationDepartmentId);
                }
            }
        }

        string _remarks;
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks
        {
            get { return _remarks; }
            set
            {
                if (_remarks != value)
                {
                    _remarks = value;
                    OnPropertyChanged(() => Remarks);
                }
            }
        }

        string _carrier;
        [Display(Name = "Carrier")]
        [Required]
        public string Carrier
        {
            get { return _carrier; }
            set
            {
                if (_carrier != value)
                {
                    _carrier = value;
                    OnPropertyChanged(() => Carrier);
                }
            }
        }
    }
}
