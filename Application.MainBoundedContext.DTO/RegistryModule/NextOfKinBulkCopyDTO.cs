using System;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class NextOfKinBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public int Salutation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IdentityCardNumber { get; set; }

        public int Gender { get; set; }

        public int Relationship { get; set; }

        public string Address_AddressLine1 { get; set; }

        public string Address_AddressLine2 { get; set; }

        public string Address_Street { get; set; }

        public string Address_PostalCode { get; set; }

        public string Address_City { get; set; }

        public string Address_Email { get; set; }

        public string Address_LandLine { get; set; }

        public string Address_MobileLine { get; set; }

        public double NominatedPercentage { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
