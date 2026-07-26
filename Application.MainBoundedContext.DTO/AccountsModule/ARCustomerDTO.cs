using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ARCustomerDTO : BindingModelBase<ARCustomerDTO>
    { 

        public ARCustomerDTO ()
        {

            AddAllAttributeValidators();
        }


        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }


        [DataMember]
        [Display(Name = "No")]
        public string No { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [DataMember]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [DataMember]
        [Display(Name = "MobilePhoneNumber")]
        public string MobilePhoneNumber { get; set; }

        [DataMember]
        [Display(Name = "Town")]
        public string Town { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string City { get; set; }

        [DataMember]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [DataMember]
        [Display(Name = "ContactPersonName")]
        public string ContactPersonName { get; set; }

        [DataMember]
        [Display(Name = "ContactPersonPhoneNumber")]
        public string ContactPersonPhoneNo { get; set; }

        [DataMember]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
            
    }
}
