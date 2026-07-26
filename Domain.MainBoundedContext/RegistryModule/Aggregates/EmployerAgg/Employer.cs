using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg
{
    public class Employer : Entity
    {
        public string Description { get; set; }
        
        public virtual Address Address { get; set; }

        public byte RetirementAge { get; set; }

        public bool EnforceRetirementAge { get; set; }

        public bool IsLocked { get; private set; }

        HashSet<Division> _divisions;
        public virtual ICollection<Division> Divisions
        {
            get
            {
                if (_divisions == null)
                {
                    _divisions = new HashSet<Division>();
                }
                return _divisions;
            }
            private set
            {
                _divisions = new HashSet<Division>(value);
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
