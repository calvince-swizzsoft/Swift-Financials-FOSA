using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg
{
    public class EmployeeAppraisalTarget: Entity
    {
        public Guid? ParentId { get; set; }

        public virtual EmployeeAppraisalTarget Parent { get; private set; }

        public string Description { get; set; }

        public byte Type { get; set; }

        public byte AnswerType { get; set; }

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

        HashSet<EmployeeAppraisalTarget> _children;
        public virtual ICollection<EmployeeAppraisalTarget> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<EmployeeAppraisalTarget>();
                return _children;
            }
            private set
            {
                _children = new HashSet<EmployeeAppraisalTarget>(value);
            }
        }
    }
}
