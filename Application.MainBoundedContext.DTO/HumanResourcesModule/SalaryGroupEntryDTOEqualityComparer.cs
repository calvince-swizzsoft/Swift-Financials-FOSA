using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryGroupEntryDTOEqualityComparer : EqualityComparer<SalaryGroupEntryDTO>
    {
        public override bool Equals(SalaryGroupEntryDTO x, SalaryGroupEntryDTO y)
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

        public override int GetHashCode(SalaryGroupEntryDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
