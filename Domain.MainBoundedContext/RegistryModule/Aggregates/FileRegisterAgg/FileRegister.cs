using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FileMovementHistoryAgg;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FileRegisterAgg
{
    public class FileRegister : Domain.Seedwork.Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public int Status { get; set; }

        HashSet<FileMovementHistory> _history;
        public virtual ICollection<FileMovementHistory> History
        {
            get
            {
                if (_history == null)
                {
                    _history = new HashSet<FileMovementHistory>();
                }
                return _history;
            }
            private set
            {
                _history = new HashSet<FileMovementHistory>(value);
            }
        }
    }
}
