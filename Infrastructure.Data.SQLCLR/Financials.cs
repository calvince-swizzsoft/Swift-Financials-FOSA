using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

public partial class UserDefinedFunctions
{
    public class AmortizationTableResult
    {
        public SqlInt32 Period { get; set; }

        public SqlDateTime DueDate { get; set; }

        public SqlDecimal StartingBalance { get; set; }

        public SqlDecimal Payment { get; set; }

        public SqlDecimal InterestPayment { get; set; }

        public SqlDecimal PrincipalPayment { get; set; }

        public SqlDecimal EndingBalance { get; set; }

        public AmortizationTableResult(SqlInt32 period, SqlDateTime dueDate, SqlDecimal startingBalance, SqlDecimal payment, SqlDecimal interestPayment, SqlDecimal principalPayment, SqlDecimal endingBalance)
        {
            Period = period;
            DueDate = dueDate;
            StartingBalance = startingBalance;
            Payment = payment;
            InterestPayment = interestPayment;
            PrincipalPayment = principalPayment;
            EndingBalance = endingBalance;
        }
    }

    public enum PaymentFrequencyPerYear
    {
        [Description("Annual")]
        Annual = 1,
        /// <summary>
        /// Semi-Annual (every 6 months)
        /// </summary>
        [Description("Semi-Annual (every 6 months)")]
        SemiAnnual = 2,
        /// <summary>
        /// Quarterly (every 3 months)
        /// </summary>
        [Description("Quarterly (every 3 months)")]
        Quarterly = 3,
        /// <summary>
        /// Tri-Annual (every 4 months)
        /// </summary>
        [Description("Tri-Annual (every 4 months)")]
        TriAnnual = 4,
        /// <summary>
        /// Bi-Monthly (every 2 months)
        /// </summary>
        [Description("Bi-Monthly (every 2 months)")]
        BiMonthly = 6,
        [Description("Monthly")]
        Monthly = 12,
        /// <summary>
        /// Semi-Monthly (twice a month)
        /// </summary>
        [Description("Semi-Monthly (twice a month)")]
        SemiMonthly = 24,
        /// <summary>
        /// Bi-Weekly (every 2 weeks)
        /// </summary>
        [Description("Bi-Weekly (every 2 weeks)")]
        BiWeekly = 26,
        [Description("Weekly")]
        Weekly = 52,
        [Description("Daily")]
        Daily = 365
    }

    public enum InterestCalculationMode
    {
        [Description("Reducing Balance")]
        ReducingBalance = 0x200,
        [Description("Straight Line")]
        StraightLine = 0x200 + 1,
        [Description("Amortization (Straight Line)")]
        Amortization = 0x200 + 2,
        [Description("Amortization (Diminishing Balance)")]
        DiminishingBalanceAmortization = 0x200 + 3,
        [Description("Fixed Interest")]
        FixedInterest = 0x200 + 4,
    }

    [SqlFunction(
        DataAccess = DataAccessKind.Read,
        FillRowMethodName = "RepaymentSchedule_FillRow",
        TableDefinition = "Period int, DueDate datetime, StartingBalance decimal(18,2), Payment decimal(18,2), InterestPayment decimal(18,2), PrincipalPayment decimal(18,2), EndingBalance decimal(18,2)")]
    public static IEnumerable RepaymentSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV = 0, int Due = 0)
    {
        var repaymentScheduleList = new List<AmortizationTableResult>();

        if (Enum.IsDefined(typeof(InterestCalculationMode), interestCalculationMode))
        {
            switch ((InterestCalculationMode)interestCalculationMode)
            {
                case InterestCalculationMode.DiminishingBalanceAmortization:
                    repaymentScheduleList = DiminishingBalanceSchedule(termInMonths, paymentFrequencyPerYear, gracePeriod, APR, PV, FV, Due);
                    break;
                case InterestCalculationMode.ReducingBalance:
                case InterestCalculationMode.Amortization:
                    repaymentScheduleList = ReducingBalanceSchedule(termInMonths, paymentFrequencyPerYear, gracePeriod, APR, PV, FV, Due);
                    break;
                case InterestCalculationMode.StraightLine:
                    repaymentScheduleList = StraightLineSchedule(termInMonths, paymentFrequencyPerYear, gracePeriod, APR, PV, FV, Due);
                    break;
                case InterestCalculationMode.FixedInterest:
                    repaymentScheduleList = FixedInterestSchedule(termInMonths, paymentFrequencyPerYear, gracePeriod, APR, PV, FV, Due);
                    break;
                default:
                    break;
            }
        }

        return repaymentScheduleList;
    }

    public static void RepaymentSchedule_FillRow(object auditLogResultObj, out SqlInt32 period, out SqlDateTime dueDate, out SqlDecimal startingBalance, out SqlDecimal payment, out SqlDecimal interestPayment, out SqlDecimal principalPayment, out SqlDecimal endingBalance)
    {
        AmortizationTableResult amortizationTableResult = (AmortizationTableResult)auditLogResultObj;

        period = amortizationTableResult.Period;
        dueDate = amortizationTableResult.DueDate;
        startingBalance = amortizationTableResult.StartingBalance;
        payment = amortizationTableResult.Payment;
        interestPayment = amortizationTableResult.InterestPayment;
        principalPayment = amortizationTableResult.PrincipalPayment;
        endingBalance = amortizationTableResult.EndingBalance;
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble FV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV = 0, int Due = 0)
    {
        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        return Financial.FV(Rate, NPer, Pmt, PV, (DueDate)Due);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble PV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV = 0, int Due = 0)
    {
        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        return Financial.PV(Rate, NPer, Pmt, FV, (DueDate)Due);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble Pmt(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV = 0, int Due = 0)
    {
        double paymentPerPeriod = default(double);

        if (Enum.IsDefined(typeof(InterestCalculationMode), interestCalculationMode))
        {
            double Rate = (APR / 100) / paymentFrequencyPerYear;

            double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

            switch ((InterestCalculationMode)interestCalculationMode)
            {
                case InterestCalculationMode.DiminishingBalanceAmortization:
                    paymentPerPeriod = Financial.Pmt(Rate, NPer, PV, FV, (DueDate)Due);
                    break;
                case InterestCalculationMode.Amortization:
                case InterestCalculationMode.ReducingBalance:
                case InterestCalculationMode.StraightLine:

                    double startingBalance = PV * -1 > 0d ? PV * -1 : PV;

                    double principalPayment = startingBalance / NPer;

                    double interestPayment = (startingBalance * Rate);

                    paymentPerPeriod = principalPayment + interestPayment;

                    break;
                case InterestCalculationMode.FixedInterest:

                    double fixedInterestStartingBalance = PV * -1 > 0d ? PV * -1 : PV;

                    double fixedInterestInterestPayment = (fixedInterestStartingBalance * (APR / 100));

                    paymentPerPeriod = (fixedInterestStartingBalance + fixedInterestInterestPayment) / termInMonths;

                    break;
                default:
                    break;
            }
        }

        return paymentPerPeriod;
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble PPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0)
    {
        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        return Financial.PPmt(Rate, Per, NPer, PV, FV, (DueDate)Due);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble IPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0)
    {
        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        return Financial.IPmt(Rate, Per, NPer, PV, FV, (DueDate)Due);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble NPer(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV = 0, int Due = 0)
    {
        double Rate = (APR / 100) / paymentFrequencyPerYear;

        return Financial.NPer(Rate, Pmt, PV, FV, (DueDate)Due);
    }

    private static List<AmortizationTableResult> DiminishingBalanceSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
    {
        List<AmortizationTableResult> result = new List<AmortizationTableResult>();

        DateTime dueDate = DateTime.Today.AddDays(gracePeriod);

        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        double startingBalance = PV * -1 > 0d ? PV * -1 : PV;

        double payment = Financial.Pmt(Rate, NPer, PV, FV, (DueDate)Due);

        for (int i = 0; i < NPer; i++)
        {
            int period = i + 1;

            if (i != 0)
            {
                switch ((PaymentFrequencyPerYear)paymentFrequencyPerYear)
                {
                    case PaymentFrequencyPerYear.Annual:
                        dueDate = dueDate.AddYears(1);
                        break;
                    case PaymentFrequencyPerYear.SemiAnnual:
                        dueDate = dueDate.AddMonths(6);
                        break;
                    case PaymentFrequencyPerYear.TriAnnual:
                        dueDate = dueDate.AddMonths(4);
                        break;
                    case PaymentFrequencyPerYear.Quarterly:
                        dueDate = dueDate.AddMonths(3);
                        break;
                    case PaymentFrequencyPerYear.BiMonthly:
                        dueDate = dueDate.AddMonths(2);
                        break;
                    case PaymentFrequencyPerYear.Monthly:
                        dueDate = dueDate.AddMonths(1);
                        break;
                    case PaymentFrequencyPerYear.SemiMonthly:
                        dueDate = dueDate.AddDays(15);
                        break;
                    case PaymentFrequencyPerYear.BiWeekly:
                        dueDate = dueDate.AddDays(14);
                        break;
                    case PaymentFrequencyPerYear.Weekly:
                        dueDate = dueDate.AddDays(7);
                        break;
                    case PaymentFrequencyPerYear.Daily:
                        dueDate = dueDate.AddDays(1);
                        break;
                    default:
                        break;
                }
            }

            double interestPayment = (startingBalance * Rate);

            double principalPayment = payment - interestPayment;

            double endingBalance = startingBalance - principalPayment;

            result.Add(
                new AmortizationTableResult
                (
                    period, dueDate, (decimal)startingBalance, (decimal)payment, (decimal)interestPayment, (decimal)principalPayment, (decimal)endingBalance
                ));

            startingBalance = endingBalance;
        }

        return result;
    }

    private static List<AmortizationTableResult> ReducingBalanceSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
    {
        List<AmortizationTableResult> result = new List<AmortizationTableResult>();

        DateTime dueDate = DateTime.Today.AddDays(gracePeriod);

        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        double startingBalance = PV * -1 > 0d ? PV * -1 : PV;

        double principalPayment = startingBalance / NPer;

        for (int i = 0; i < NPer; i++)
        {
            int period = i + 1;

            if (i != 0)
            {
                switch ((PaymentFrequencyPerYear)paymentFrequencyPerYear)
                {
                    case PaymentFrequencyPerYear.Annual:
                        dueDate = dueDate.AddYears(1);
                        break;
                    case PaymentFrequencyPerYear.SemiAnnual:
                        dueDate = dueDate.AddMonths(6);
                        break;
                    case PaymentFrequencyPerYear.TriAnnual:
                        dueDate = dueDate.AddMonths(4);
                        break;
                    case PaymentFrequencyPerYear.Quarterly:
                        dueDate = dueDate.AddMonths(3);
                        break;
                    case PaymentFrequencyPerYear.BiMonthly:
                        dueDate = dueDate.AddMonths(2);
                        break;
                    case PaymentFrequencyPerYear.Monthly:
                        dueDate = dueDate.AddMonths(1);
                        break;
                    case PaymentFrequencyPerYear.SemiMonthly:
                        dueDate = dueDate.AddDays(15);
                        break;
                    case PaymentFrequencyPerYear.BiWeekly:
                        dueDate = dueDate.AddDays(14);
                        break;
                    case PaymentFrequencyPerYear.Weekly:
                        dueDate = dueDate.AddDays(7);
                        break;
                    case PaymentFrequencyPerYear.Daily:
                        dueDate = dueDate.AddDays(1);
                        break;
                    default:
                        break;
                }
            }

            double interestPayment = (startingBalance * Rate);

            double endingBalance = startingBalance - principalPayment;

            result.Add(
                new AmortizationTableResult
                (
                    period, dueDate, (decimal)startingBalance, (decimal)(principalPayment + interestPayment), (decimal)interestPayment, (decimal)principalPayment, (decimal)endingBalance
                ));

            startingBalance = endingBalance;
        }

        return result;
    }

    private static List<AmortizationTableResult> StraightLineAmortizationSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
    {
        List<AmortizationTableResult> result = new List<AmortizationTableResult>();

        DateTime dueDate = DateTime.Today.AddDays(gracePeriod);

        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        for (int i = 0; i < NPer; i++)
        {
            int period = i + 1;

            if (i != 0)
            {
                switch ((PaymentFrequencyPerYear)paymentFrequencyPerYear)
                {
                    case PaymentFrequencyPerYear.Annual:
                        dueDate = dueDate.AddYears(1);
                        break;
                    case PaymentFrequencyPerYear.SemiAnnual:
                        dueDate = dueDate.AddMonths(6);
                        break;
                    case PaymentFrequencyPerYear.TriAnnual:
                        dueDate = dueDate.AddMonths(4);
                        break;
                    case PaymentFrequencyPerYear.Quarterly:
                        dueDate = dueDate.AddMonths(3);
                        break;
                    case PaymentFrequencyPerYear.BiMonthly:
                        dueDate = dueDate.AddMonths(2);
                        break;
                    case PaymentFrequencyPerYear.Monthly:
                        dueDate = dueDate.AddMonths(1);
                        break;
                    case PaymentFrequencyPerYear.SemiMonthly:
                        dueDate = dueDate.AddDays(15);
                        break;
                    case PaymentFrequencyPerYear.BiWeekly:
                        dueDate = dueDate.AddDays(14);
                        break;
                    case PaymentFrequencyPerYear.Weekly:
                        dueDate = dueDate.AddDays(7);
                        break;
                    case PaymentFrequencyPerYear.Daily:
                        dueDate = dueDate.AddDays(1);
                        break;
                    default:
                        break;
                }
            }

            double startingBalance = -Financial.PV(Rate, NPer - (period - 1), Financial.Pmt(Rate, NPer, PV, FV, (DueDate)Due));

            double payment = Financial.Pmt(Rate, NPer, PV, FV, (DueDate)Due);

            double interestPayment = Financial.IPmt(Rate, period, NPer, PV, FV, (DueDate)Due);

            double principalPayment = Financial.PPmt(Rate, period, NPer, PV, FV, (DueDate)Due);

            double endingBalance = -Financial.PV(Rate, NPer - period, Financial.Pmt(Rate, NPer, PV, FV, (DueDate)Due), FV, (DueDate)Due);

            result.Add(
                new AmortizationTableResult
                (
                    period, dueDate, (decimal)startingBalance, (decimal)payment, (decimal)interestPayment, (decimal)principalPayment, (decimal)endingBalance
                ));
        }

        return result;
    }

    private static List<AmortizationTableResult> StraightLineSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
    {
        List<AmortizationTableResult> result = new List<AmortizationTableResult>();

        DateTime dueDate = DateTime.Today.AddDays(gracePeriod);

        double Rate = (APR / 100) / paymentFrequencyPerYear;

        double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

        double startingBalance = PV * -1 > 0d ? PV * -1 : PV;

        double principalPayment = startingBalance / NPer;

        double interestPayment = (startingBalance * Rate);

        for (int i = 0; i < NPer; i++)
        {
            int period = i + 1;

            if (i != 0)
            {
                switch ((PaymentFrequencyPerYear)paymentFrequencyPerYear)
                {
                    case PaymentFrequencyPerYear.Annual:
                        dueDate = dueDate.AddYears(1);
                        break;
                    case PaymentFrequencyPerYear.SemiAnnual:
                        dueDate = dueDate.AddMonths(6);
                        break;
                    case PaymentFrequencyPerYear.TriAnnual:
                        dueDate = dueDate.AddMonths(4);
                        break;
                    case PaymentFrequencyPerYear.Quarterly:
                        dueDate = dueDate.AddMonths(3);
                        break;
                    case PaymentFrequencyPerYear.BiMonthly:
                        dueDate = dueDate.AddMonths(2);
                        break;
                    case PaymentFrequencyPerYear.Monthly:
                        dueDate = dueDate.AddMonths(1);
                        break;
                    case PaymentFrequencyPerYear.SemiMonthly:
                        dueDate = dueDate.AddDays(15);
                        break;
                    case PaymentFrequencyPerYear.BiWeekly:
                        dueDate = dueDate.AddDays(14);
                        break;
                    case PaymentFrequencyPerYear.Weekly:
                        dueDate = dueDate.AddDays(7);
                        break;
                    case PaymentFrequencyPerYear.Daily:
                        dueDate = dueDate.AddDays(1);
                        break;
                    default:
                        break;
                }
            }

            double endingBalance = startingBalance - principalPayment;

            result.Add(
                new AmortizationTableResult
                (
                    period, dueDate, (decimal)startingBalance, (decimal)(principalPayment + interestPayment), (decimal)interestPayment, (decimal)principalPayment, (decimal)endingBalance
                ));

            startingBalance = endingBalance;
        }

        return result;
    }

    private static List<AmortizationTableResult> FixedInterestSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
    {
        List<AmortizationTableResult> result = new List<AmortizationTableResult>();

        DateTime dueDate = DateTime.Today.AddDays(gracePeriod);

        double Rate = (APR / 100);

        double startingBalance = PV * -1 > 0d ? PV * -1 : PV;

        double principalPayment = startingBalance / termInMonths;

        double interestPayment = (startingBalance * Rate) / termInMonths;

        for (int i = 0; i < termInMonths; i++)
        {
            int period = i + 1;

            if (i != 0)
            {
                switch ((PaymentFrequencyPerYear)paymentFrequencyPerYear)
                {
                    case PaymentFrequencyPerYear.Annual:
                        dueDate = dueDate.AddYears(1);
                        break;
                    case PaymentFrequencyPerYear.SemiAnnual:
                        dueDate = dueDate.AddMonths(6);
                        break;
                    case PaymentFrequencyPerYear.TriAnnual:
                        dueDate = dueDate.AddMonths(4);
                        break;
                    case PaymentFrequencyPerYear.Quarterly:
                        dueDate = dueDate.AddMonths(3);
                        break;
                    case PaymentFrequencyPerYear.BiMonthly:
                        dueDate = dueDate.AddMonths(2);
                        break;
                    case PaymentFrequencyPerYear.Monthly:
                        dueDate = dueDate.AddMonths(1);
                        break;
                    case PaymentFrequencyPerYear.SemiMonthly:
                        dueDate = dueDate.AddDays(15);
                        break;
                    case PaymentFrequencyPerYear.BiWeekly:
                        dueDate = dueDate.AddDays(14);
                        break;
                    case PaymentFrequencyPerYear.Weekly:
                        dueDate = dueDate.AddDays(7);
                        break;
                    case PaymentFrequencyPerYear.Daily:
                        dueDate = dueDate.AddDays(1);
                        break;
                    default:
                        break;
                }
            }

            double endingBalance = startingBalance - principalPayment;

            result.Add(
               new AmortizationTableResult
               (
                   period, dueDate, (decimal)startingBalance, (decimal)(principalPayment + interestPayment), (decimal)interestPayment, (decimal)principalPayment, (decimal)endingBalance
               ));

            startingBalance = endingBalance;
        }

        return result;
    }
}
