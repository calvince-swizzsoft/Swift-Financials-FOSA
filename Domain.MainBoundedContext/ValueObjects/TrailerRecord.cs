using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class TrailerRecord : ValueObject<TrailerRecord>
    {
        public string RecordType { get; private set; }

        public string ClearingCentre { get; private set; }

        public string Bank { get; private set; }

        public string Organisation { get; private set; }

        public int TransactionCount { get; private set; }

        public decimal TotalValueCredits { get; private set; }

        public decimal TotalValueDebits { get; private set; }

        public string Filler { get; private set; }

        public TrailerRecord(string recordType, string clearingCentre, string bank, string organisation, int transactionCount, decimal totalValueCredits, decimal totalValueDebits, string filler)
        {
            this.RecordType = recordType;
            this.ClearingCentre = clearingCentre;
            this.Bank = bank;
            this.Organisation = organisation;
            this.TransactionCount = transactionCount;
            this.TotalValueCredits = totalValueCredits;
            this.TotalValueDebits = totalValueDebits;
            this.Filler = filler;
        }

        private TrailerRecord()
        {

        }
    }
}
