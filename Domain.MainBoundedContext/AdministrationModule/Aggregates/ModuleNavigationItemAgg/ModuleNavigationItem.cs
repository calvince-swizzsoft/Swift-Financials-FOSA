using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg
{
    public class ModuleNavigationItem : Domain.Seedwork.Entity
    {
        public Guid? ParentId { get; set; }

        public virtual ModuleNavigationItem Parent { get; private set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        [Index("IX_ModuleNavigationItem_Code")]
        public int Code { get; set; }

        public bool IsArea { get; set; }

        public int AreaCode { get; set; }

        public string AreaName { get; set; }

        HashSet<ModuleNavigationItem> _children;
        public virtual ICollection<ModuleNavigationItem> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<ModuleNavigationItem>();
                return _children;
            }
            private set
            {
                _children = new HashSet<ModuleNavigationItem>(value);
            }
        }
    }
}
