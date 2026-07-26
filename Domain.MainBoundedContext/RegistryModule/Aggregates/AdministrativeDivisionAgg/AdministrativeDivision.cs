using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg
{
    public class AdministrativeDivision : Entity
    {
        public Guid? ParentId { get; set; }

        public virtual AdministrativeDivision Parent { get; private set; }

        public string Description { get; set; }

        public int Depth { get; set; }

        public byte Type { get; set; } 

        public string Remarks { get; set; }

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                IsLocked = false;
        }

        HashSet<AdministrativeDivision> _children;
        public virtual ICollection<AdministrativeDivision> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<AdministrativeDivision>();
                return _children;
            }
            private set
            {
                _children = new HashSet<AdministrativeDivision>(value);
            }
        }
    }
}
