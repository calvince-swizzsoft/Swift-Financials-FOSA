using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Address : ValueObject<Address>
    {
        public string AddressLine1 { get; private set; }

        public string AddressLine2 { get; private set; }

        public string Street { get; private set; }

        public string PostalCode { get; private set; }

        public string City { get; private set; }

        public string Email { get; private set; }

        public string LandLine { get; private set; }

        public string MobileLine { get; private set; }

        public Address(string addressLine1, string addressLine2, string street, string postalCode, string city, string email, string landLine, string mobileLine)
        {
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.Street = street;
            this.PostalCode = postalCode;
            this.City = city;
            this.Email = email;
            this.LandLine = landLine;
            this.MobileLine = mobileLine;
        }

        private Address()
        { }
    }
}
