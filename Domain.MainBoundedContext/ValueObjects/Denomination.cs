using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Denomination : ValueObject<Denomination>
    {
        public decimal OneThousandValue { get; private set; }

        public decimal FiveHundredValue { get; private set; }

        public decimal TwoHundredValue { get; private set; }

        public decimal OneHundredValue { get; private set; }

        public decimal FiftyValue { get; private set; }

        public decimal FourtyValue { get; private set; }

        public decimal TwentyValue { get; private set; }

        public decimal TenValue { get; private set; }

        public decimal FiveValue { get; private set; }

        public decimal OneValue { get; private set; }

        public decimal FiftyCentValue { get; private set; }

        public Denomination(decimal oneThousandValue = 0.00m, decimal fiveHundredValue = 0.00m, decimal twoHundredValue = 0.00m, decimal oneHundredValue = 0.00m, decimal fiftyValue = 0.00m, decimal fourtyValue = 0.00m, decimal twentyValue = 0.00m, decimal tenValue = 0.00m, decimal fiveValue = 0.00m, decimal oneValue = 0.00m, decimal fiftyCentValue = 0.00m)
        {
            this.OneThousandValue = oneThousandValue;
            this.FiveHundredValue = fiveHundredValue;
            this.TwoHundredValue = twoHundredValue;
            this.OneHundredValue = oneHundredValue;
            this.FiftyValue = fiftyValue;
            this.FourtyValue = fourtyValue;
            this.TwentyValue = twentyValue;
            this.TenValue = tenValue;
            this.FiveValue = fiveValue;
            this.OneValue = oneValue;
            this.FiftyCentValue = fiftyCentValue;
        }

        private Denomination()
        { }
    }
}
