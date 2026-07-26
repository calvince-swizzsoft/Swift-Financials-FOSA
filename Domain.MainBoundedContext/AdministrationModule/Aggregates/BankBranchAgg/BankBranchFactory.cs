using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BankBranchAgg
{
    public static class BankBranchFactory
    {
        public static BankBranch CreateBankBranch(Guid bankId, int code, string description, Address address)
        {
            var bankBranch = new BankBranch()
            {
                Code = (short)code,
                Description = description,
                Address = address
            };

            bankBranch.GenerateNewIdentity();

            bankBranch.BankId = bankId;

            bankBranch.CreatedDate = DateTime.Now;

            return bankBranch;
        }
    }
}
