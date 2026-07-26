using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg
{
    public class ChequeBook : Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public byte Type { get; set; }

        public int SerialNumber { get; set; }

        public int NumberOfVouchers { get; set; }

        public int InitialVoucherNumber { get; set; }

        public string Reference { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; private set; }

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

        public void Activate()
        {
            if (!IsActive)
                this.IsActive = true;
        }

        public void DeActivate()
        {
            if (IsActive)
                this.IsActive = false;
        }
    }
}
