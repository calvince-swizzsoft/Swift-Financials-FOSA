using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO
{
    public class AmortizationTableEntry
    {
        [Display(Name = "Period")]
        public int Period { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Starting Balance")]
        public decimal StartingBalance { get; set; }

        [Display(Name = "Payment")]
        public decimal Payment { get; set; }

        [Display(Name = "Interest Payment")]
        public decimal InterestPayment { get; set; }

        [Display(Name = "Principal Payment")]
        public decimal PrincipalPayment { get; set; }

        [Display(Name = "Ending Balance")]
        public decimal EndingBalance { get; set; }
    }
}
