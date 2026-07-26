using Domain.MainBoundedContext.AdministrationModule.Aggregates.BankBranchAgg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BankAgg
{
    public class Bank : Domain.Seedwork.Entity
    {
        public short Code { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string SwiftCode { get; set; }

        public string IbanNo { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int No { get; set; }


        HashSet<BankBranch> _bankBranches;
        public virtual ICollection<BankBranch> BankBranches
        {
            get
            {
                if (_bankBranches == null)
                {
                    _bankBranches = new HashSet<BankBranch>();
                }
                return _bankBranches;
            }
            private set
            {
                _bankBranches = new HashSet<BankBranch>(value);
            }
        }
    }
}
