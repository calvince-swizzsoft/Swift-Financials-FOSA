using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class TransactionThresholdDTOEqualityComparer : EqualityComparer<TransactionThresholdDTO>
    {
        public override bool Equals(TransactionThresholdDTO x, TransactionThresholdDTO y)
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

        public override int GetHashCode(TransactionThresholdDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
