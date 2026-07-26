using Domain.Seedwork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class CustomerAccountType : ValueObject<CustomerAccountType>
    {
        [Index("IX_CustomerAccountType_ProductCode")]
        public byte ProductCode { get; private set; }

        [Index("IX_CustomerAccountType_TargetProductId")]
        public Guid TargetProductId { get; private set; }

        public short TargetProductCode { get; private set; }

        public CustomerAccountType(int productCode, Guid targetProductId, int targetProductCode)
        {
            this.ProductCode = (byte)productCode;
            this.TargetProductId = targetProductId;
            this.TargetProductCode = (short)targetProductCode;
        }

        private CustomerAccountType()
        { }
    }
}
