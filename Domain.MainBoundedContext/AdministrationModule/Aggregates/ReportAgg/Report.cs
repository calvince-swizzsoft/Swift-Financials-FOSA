using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ReportAgg
{
    public class Report : Domain.Seedwork.Entity
    {
        public Guid? ParentId { get; set; }

        public virtual Report Parent { get; private set; }

        public string ReportName { get; set; }

        public string ReportPath { get; set; }

        public string ReportQuery { get; set; }

        [Index("IX_Report_Category")]
        public int Category { get; set; }

        public int Depth { get; set; }

        

        HashSet<Report> _children;
        public virtual ICollection<Report> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<Report>();
                return _children;
            }
            private set
            {
                _children = new HashSet<Report>(value);
            }
        }
    }
}
