using System;

namespace Application.MainBoundedContext.BackOfficeModule.Calculations
{
    // ================= INPUT =================
    public class LoanRepaymentCalculation
    {
        public decimal OpeningBalance { get; set; }
        public decimal InterestDue { get; set; }
        public decimal PrincipalDue { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    // ================= OUTPUT =================
    public class LoanRepaymentResult
    {
        public decimal InterestPaid { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal ExcessAmount { get; set; }
        public decimal NewOutstandingBalance { get; set; }
        public bool IsLoanCleared { get; set; }
    }

    // ================= CALCULATOR =================
    public static class LoanRepaymentCalculator
    {
        /// <summary>
        /// Waterfall order:
        /// 1. Interest
        /// 2. Principal
        /// 3. Excess (prepayment / suspense)
        /// </summary>
        /// 
        public static decimal CalculateMonthlyInterest(decimal principal, decimal annualRatePercent)
        {
            return principal * (annualRatePercent / 12m / 100m);
        }

        public static decimal CalculateMonthlyPrincipal(decimal monthlyPayment, decimal interestDue)
        {
            var principalDue = monthlyPayment - interestDue;
            return principalDue > 0 ? principalDue : 0;
        }
        public static LoanRepaymentResult Calculate(LoanRepaymentCalculation input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.PaymentAmount < 0)
                throw new ArgumentException("Payment amount cannot be negative");

            var remaining = input.PaymentAmount;

            // ---- INTEREST ----
            var interestPaid = Math.Min(remaining, Safe(input.InterestDue));
            remaining -= interestPaid;

            // ---- PRINCIPAL ----
            var principalDue = Safe(input.PrincipalDue);
            var principalPaid = Math.Min(remaining, principalDue);
            remaining -= principalPaid;

            // ---- NEW BALANCE ----
            var newOutstanding = Safe(input.OpeningBalance) - principalPaid;
            if (newOutstanding < 0) newOutstanding = 0;

            return new LoanRepaymentResult
            {
                InterestPaid = interestPaid,
                PrincipalPaid = principalPaid,
                ExcessAmount = remaining,
                NewOutstandingBalance = newOutstanding,
                IsLoanCleared = newOutstanding == 0
            };
        }

        private static decimal Safe(decimal value)
        {
            return value < 0 ? 0 : value;
        }
    }
}
