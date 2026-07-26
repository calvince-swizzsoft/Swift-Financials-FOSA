using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg
{
    public class ReportTemplate : Domain.Seedwork.Entity
    {
        public Guid? ParentId { get; set; }

        public virtual ReportTemplate Parent { get; private set; }

        public string Description { get; set; }

        public short Category { get; set; }

        public string SpreadsheetCellReference { get; set; }

        public int Depth { get; set; }

        public bool IsLocked { get; private set; }

        

        HashSet<ReportTemplate> _children;
        public virtual ICollection<ReportTemplate> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<ReportTemplate>();
                return _children;
            }
            private set
            {
                _children = new HashSet<ReportTemplate>(value);
            }
        }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
