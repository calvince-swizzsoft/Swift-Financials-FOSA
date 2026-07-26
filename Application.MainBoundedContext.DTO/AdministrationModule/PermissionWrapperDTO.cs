using Application.Seedwork;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class PermissionWrapperDTO : BindingModelBase<PermissionWrapperDTO>
    {
        public PermissionWrapperDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        public int ItemCode { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }
    }
}
