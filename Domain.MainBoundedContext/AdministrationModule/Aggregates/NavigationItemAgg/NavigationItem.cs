using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg
{
    public class NavigationItem : Entity
    {
        public Guid? ParentId { get; set; }

        public virtual NavigationItem Parent { get; private set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public int Code { get; set; }

        public bool IsArea { get; set; }

        public int AreaCode { get; set; }

        public string AreaName { get; set; }

        HashSet<NavigationItem> _children;
        public virtual ICollection<NavigationItem> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<NavigationItem>();
                return _children;
            }
            private set
            {
                _children = new HashSet<NavigationItem>(value);
            }
        }
    }
}