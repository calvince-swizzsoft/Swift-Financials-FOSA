using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg
{
    public static class BranchFactory
    {
        public static Branch CreateBranch(Guid companyId, string description, Address address)
        {
            var branch = new Branch()
            {
                CompanyId = companyId,
                Description = description,
            };

            branch.GenerateNewIdentity();

            branch.Address = address;

            branch.CreatedDate = DateTime.Now;

            return branch;
        }
    }
}
