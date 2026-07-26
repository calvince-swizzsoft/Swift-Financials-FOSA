using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ARCustomerAgg
{
    public static class ARCustomerFactory
    {
        public static ARCustomer CreateARCustomer(string no, string name, string address, string mobilePhoneNumber, string town, string city, string country, string contactPersonName, string contactPersonPhoneNo)
        {
            var arCustomer = new ARCustomer();
            arCustomer.GenerateNewIdentity();
            arCustomer.No = no;
            arCustomer.Name = name;
            arCustomer.Address = address;
            arCustomer.MobilePhoneNumber = mobilePhoneNumber;
            arCustomer.Town = town;
            arCustomer.City = city;
            arCustomer.Country = country;
            arCustomer.ContactPersonName = contactPersonName;
            arCustomer.ContactPersonPhoneNo = contactPersonPhoneNo;
            return arCustomer;
        }
    }
}
