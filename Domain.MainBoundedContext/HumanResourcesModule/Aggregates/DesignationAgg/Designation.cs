using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg
{
    public class Designation : Entity
    {
        public Guid? ParentId { get; set; }

        public virtual Designation Parent { get; private set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public int Depth { get; set; }

        public bool IsLocked { get; private set; }

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

        HashSet<Designation> _children;
        public virtual ICollection<Designation> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<Designation>();
                return _children;
            }
            private set
            {
                _children = new HashSet<Designation>(value);
            }
        }

    }
}
