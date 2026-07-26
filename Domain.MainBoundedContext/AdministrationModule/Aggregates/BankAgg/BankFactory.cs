using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BankAgg
{
    public static class BankFactory
    {
        public static Bank CreateBank(int code, string description, string address, string swiftcode, string ibanNo, string city)
        {
            var bank = new Bank()
            {
                Code = (short)code,
                Description = description,
                Address = address,
                City = city,
                IbanNo = ibanNo,
                SwiftCode = swiftcode
            };

            bank.GenerateNewIdentity();

            bank.CreatedDate = DateTime.Now;

            return bank;
        }
    }
}
