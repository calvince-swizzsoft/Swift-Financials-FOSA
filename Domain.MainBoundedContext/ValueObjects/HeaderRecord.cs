using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class HeaderRecord : ValueObject<HeaderRecord>
    {
        public string RecordType { get; private set; }

        public string FileType { get; private set; }

        public DateTime DateOfFileExchange { get; private set; }

        public string PresentingOrganisationClearingCentre { get; private set; }

        public string PresentingOrganisationBank { get; private set; }

        public string PresentingOrganisation { get; private set; }

        public string ReceivingOrganisationClearingCentre { get; private set; }

        public string ReceivingOrganisationBank { get; private set; }

        public string ReceivingOrganisation { get; private set; }

        public string FileSerialNumber { get; private set; }

        public string LastFileIndicator { get; private set; }

        public string Filler { get; private set; }

        public HeaderRecord(string recordType, string fileType, DateTime dateOfFileExchange, string presentingOrganisationClearingCentre, string presentingOrganisationBank, string presentingOrganisation, string receivingOrganisationClearingCentre, string receivingOrganisationBank, string receivingOrganisation, string fileSerialNumber, string lastFileIndicator, string filler)
        {
            this.RecordType = recordType;
            this.FileType = fileType;
            this.DateOfFileExchange = dateOfFileExchange;
            this.PresentingOrganisationClearingCentre = presentingOrganisationClearingCentre;
            this.PresentingOrganisationBank = presentingOrganisationBank;
            this.PresentingOrganisation = presentingOrganisation;
            this.ReceivingOrganisationClearingCentre = receivingOrganisationClearingCentre;
            this.ReceivingOrganisationBank = receivingOrganisationBank;
            this.ReceivingOrganisation = receivingOrganisation;
            this.FileSerialNumber = fileSerialNumber;
            this.LastFileIndicator = lastFileIndicator;
            this.Filler = filler;
        }

        private HeaderRecord()
        {

        }
    }
}
