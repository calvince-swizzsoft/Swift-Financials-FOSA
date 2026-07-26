using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg
{
    public class PaymentVoucher : Domain.Seedwork.Entity
    {
        public Guid ChequeBookId { get; set; }

        public virtual ChequeBook ChequeBook { get; private set; }

        public int VoucherNumber { get; set; }

        public string Payee { get; set; }

        public decimal Amount { get; set; }

        public DateTime? WriteDate { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        public string PaidBy { get; set; }

        public DateTime? PaidDate { get; set; }

        public string PaymentMethod { get; set; }




    }
}
