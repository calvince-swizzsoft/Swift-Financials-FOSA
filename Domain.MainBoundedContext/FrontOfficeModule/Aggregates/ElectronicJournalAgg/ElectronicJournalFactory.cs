using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg
{
    public static class ElectronicJournalFactory
    {
        public static ElectronicJournal CreateElectronicJournal(string fileName, HeaderRecord headerRecord, TrailerRecord trailerRecord)
        {
            var electronicJournal = new ElectronicJournal();

            electronicJournal.GenerateNewIdentity();

            electronicJournal.FileName = fileName;

            electronicJournal.HeaderRecord = headerRecord;

            electronicJournal.TrailerRecord = trailerRecord;

            electronicJournal.CreatedDate = DateTime.Now;

            return electronicJournal;
        }
    }
}
