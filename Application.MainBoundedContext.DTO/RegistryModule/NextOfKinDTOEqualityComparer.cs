using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class NextOfKinDTOEqualityComparer : EqualityComparer<NextOfKinDTO>
    {
        public override bool Equals(NextOfKinDTO x, NextOfKinDTO y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.IdentityCardNumber == y.IdentityCardNumber && x.FirstName.ToLower() == y.FirstName.ToLower() && x.LastName.ToLower() == y.LastName.ToLower())
            {
                return true;
            }
            else if (x.AddressMobileLine == y.AddressMobileLine && x.FirstName.ToLower() == y.FirstName.ToLower() && x.LastName.ToLower() == y.LastName.ToLower())
            {
                return true;
            }
            else if (x.IdentityCardNumber == y.IdentityCardNumber && x.AddressMobileLine == y.AddressMobileLine)
            {
                return true;
            }
            else
                return false;
        }

        public override int GetHashCode(NextOfKinDTO obj)
        {
            int hashCode = 29;
            hashCode = (hashCode * 31) + obj.IdentityCardNumber.GetHashCode();
            hashCode = (hashCode * 31) + obj.FirstName.ToLower().GetHashCode();
            hashCode = (hashCode * 31) + obj.LastName.GetHashCode();
            hashCode = (hashCode * 31) + obj.AddressMobileLine.GetHashCode();
            return hashCode;
        }
    }
}
