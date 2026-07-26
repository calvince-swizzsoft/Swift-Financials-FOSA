using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.NumberSeriesAgg
{
    public static class NumberSeriesFactory
    {

        public static NumberSeries CreateNumberSeries(string code, string prefix, int lastUsedNumber, int padding)
        {

            var numberSeries = new NumberSeries()
            {
                Code = code
            };

            numberSeries.GenerateNewIdentity();

            numberSeries.Prefix = prefix;
            numberSeries.LastUsedNumber = lastUsedNumber;
            numberSeries.LastUsedNumber = lastUsedNumber;
                    numberSeries.Padding = padding;
       

            

            return numberSeries;
        }
    }
}
