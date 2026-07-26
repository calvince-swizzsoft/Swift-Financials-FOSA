using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Range : ValueObject<Range>
    {
        public decimal LowerLimit { get; private set; }

        public decimal UpperLimit { get; private set; }

        public Range(decimal lowerLimit, decimal upperLimit)
        {
            this.LowerLimit = lowerLimit;
            this.UpperLimit = upperLimit;
        }

        private Range()
        { }
    }
}
