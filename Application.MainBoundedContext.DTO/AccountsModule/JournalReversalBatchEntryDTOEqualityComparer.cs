using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalReversalBatchEntryDTOEqualityComparer : EqualityComparer<JournalReversalBatchEntryDTO>
    {
        public override bool Equals(JournalReversalBatchEntryDTO x, JournalReversalBatchEntryDTO y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            if (x.JournalId == y.JournalId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode(JournalReversalBatchEntryDTO obj)
        {
            return obj.JournalId.GetHashCode();
        }
    }
}
