using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class BranchDTOEqualityComparer : EqualityComparer<BranchDTO>
    {
        public override bool Equals(BranchDTO x, BranchDTO y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            if (x.Id == y.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode(BranchDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
