using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class AttachedLoanDTOEqualityComparer : EqualityComparer<AttachedLoanDTO>
    {
        public override bool Equals(AttachedLoanDTO x, AttachedLoanDTO y)
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

        public override int GetHashCode(AttachedLoanDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
