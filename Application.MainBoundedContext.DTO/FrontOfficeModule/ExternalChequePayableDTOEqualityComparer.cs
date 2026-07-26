using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class ExternalChequePayableDTOEqualityComparer : EqualityComparer<ExternalChequePayableDTO>
    {
        public override bool Equals(ExternalChequePayableDTO x, ExternalChequePayableDTO y)
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

        public override int GetHashCode(ExternalChequePayableDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
