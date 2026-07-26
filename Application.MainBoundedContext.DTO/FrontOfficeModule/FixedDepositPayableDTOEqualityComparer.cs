using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class FixedDepositPayableDTOEqualityComparer : EqualityComparer<FixedDepositPayableDTO>
    {
        public override bool Equals(FixedDepositPayableDTO x, FixedDepositPayableDTO y)
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

        public override int GetHashCode(FixedDepositPayableDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
