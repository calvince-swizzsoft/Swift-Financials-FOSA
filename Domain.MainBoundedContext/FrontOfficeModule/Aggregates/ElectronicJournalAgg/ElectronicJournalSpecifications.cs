using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg
{
    public static class ElectronicJournalSpecifications
    {
        public static Specification<ElectronicJournal> DefaultSpec()
        {
            Specification<ElectronicJournal> specification = new TrueSpecification<ElectronicJournal>();

            return specification;
        }

        public static ISpecification<ElectronicJournal> ElectronicJournalWithFileName(string fileName)
        {
            Specification<ElectronicJournal> specification = new TrueSpecification<ElectronicJournal>();

            if (!string.IsNullOrEmpty(fileName))
            {
                specification &= new DirectSpecification<ElectronicJournal>(x => x.FileName == fileName);
            }

            return specification;
        }

        public static Specification<ElectronicJournal> ElectronicJournalFullText(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<ElectronicJournal> specification = new DirectSpecification<ElectronicJournal>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var fileNameSpec = new DirectSpecification<ElectronicJournal>(c => c.FileName.Contains(text));
                var fileSerialNumberSpec = new DirectSpecification<ElectronicJournal>(c => c.HeaderRecord.FileSerialNumber.Contains(text));
                var createdBySpec = new DirectSpecification<ElectronicJournal>(c => c.CreatedBy.Contains(text));

                specification &= (fileNameSpec | fileSerialNumberSpec | createdBySpec);
            }

            return specification;
        }
    }
}
