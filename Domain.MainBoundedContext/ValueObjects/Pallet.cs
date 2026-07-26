using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Pallet : ValueObject<Pallet>
    {
        public short TI { get; private set; }

        public short HI { get; private set; }

        public Pallet(int ti, int hi)
        {
            this.TI = (short)ti;
            this.HI = (short)hi;
        }

        private Pallet()
        { }
    }
}
