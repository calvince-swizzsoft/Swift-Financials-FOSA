using Application.MainBoundedContext.DTO;
using Infrastructure.Crosscutting.Framework.Utils;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.Services
{
    public class FinancialsService : IFinancialsService
    {
        public double FV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV = 0, int Due = 0)
        {
            double Rate = (APR / 100) / paymentFrequencyPerYear;

            double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

            return Financial.FV(Rate, NPer, Pmt, PV, (DueDate)Due);
        }

        public double PV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV = 0, int Due = 0)
        {
            double Rate = (APR / 100) / paymentFrequencyPerYear;

            double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

            return Financial.PV(Rate, NPer, Pmt, FV, (DueDate)Due);
        }

        public double Pmt(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV = 0, int Due = 0)
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
                    case InterestCalculationMode.StraightLineAmortization:
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

        public double PPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0)
        {
            double Rate = (APR / 100) / paymentFrequencyPerYear;

            double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

            return Financial.PPmt(Rate, Per, NPer, PV, FV, (DueDate)Due);
        }

        public double IPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0)
        {
            double Rate = (APR / 100) / paymentFrequencyPerYear;

            double NPer = (termInMonths / 12d) * paymentFrequencyPerYear;

            return Financial.IPmt(Rate, Per, NPer, PV, FV, (DueDate)Due);
        }

        public double NPer(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV = 0, int Due = 0)
        {
            double Rate = (APR / 100) / paymentFrequencyPerYear;

            return Financial.NPer(Rate, Pmt, PV, FV, (DueDate)Due);
        }

        public double SLN(double initialCost, double residualValue, double usefulLife)
        {
            return Financial.SLN(initialCost, residualValue, usefulLife);
        }

        public double SYD(double initialCost, double residualValue, double usefulLife, double period)
        {
            return Financial.SYD(initialCost, residualValue, usefulLife, period);
        }

        /// <summary>
        /// Returns the depreciation of an asset for a specified period. 
        /// This is a method of accelerated depreciation which is faster than straight line depreciation early in the life of the asset.
        /// </summary>
        /// <param name="initialCost">is the initial cost of the asset</param>
        /// <param name="residualValue">is the value of the asset at the end of the depreciation period</param>
        /// <param name="usefulLife">is the number of periods over which the asset is depreciated</param>
        /// <param name="period">is the period of time over which depreciation is calculated</param>
        /// <param name="month">is an integer indicating the number of months in the first year</param>
        /// <returns>depreciation for the period specified</returns>
        public double DB(double initialCost, double residualValue, double usefulLife, double period, int month)
        {
            double rate = 1 - Math.Pow((residualValue / initialCost), (1 / usefulLife));

            double depreciation = 0;

            double amountLeft = initialCost;

            double depreciationFirstPeriod = initialCost * rate * (month / 12);

            amountLeft -= depreciationFirstPeriod;

            for (int i = 1; i < period; i++)
            {
                if (period == usefulLife)
                {
                    depreciation = ((amountLeft) * rate * (12 - month)) / 12;

                    amountLeft -= depreciation;

                    return depreciation;
                }

                depreciation = amountLeft * rate;

                amountLeft -= depreciation;
            }

            return depreciation;
        }

        public double DDB(double initialCost, double residualValue, double usefulLife, double period)
        {
            return Financial.DDB(initialCost, residualValue, usefulLife, period);
        }

        public double VDB(double initialCost, double residualValue, double usefulLife, double period, double factor)
        {
            return Financial.DDB(initialCost, residualValue, usefulLife, period, factor);
        }

        public List<AmortizationTableEntry> RepaymentSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV = 0, int Due = 0)
        {
            var repaymentScheduleList = new List<AmortizationTableEntry>();

            if (Enum.IsDefined(typeof(InterestCalculationMode), interestCalculationMode))
            {
                switch ((InterestCalculationMode)interestCalculationMode)
                {
                    case InterestCalculationMode.DiminishingBalanceAmortization:
                        repaymentScheduleList = DiminishingBalanceSchedule(termInMonths, paymentFrequencyPerYear, gracePeriod, APR, PV, FV, Due);
                        break;
                    case InterestCalculationMode.ReducingBalance:
                    case InterestCalculationMode.StraightLineAmortization:
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

        public static List<AmortizationTableEntry> DiminishingBalanceSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
        {
            List<AmortizationTableEntry> result = new List<AmortizationTableEntry>();

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
                    new AmortizationTableEntry
                    {
                        Period = period,
                        DueDate = dueDate,
                        StartingBalance = (decimal)startingBalance,
                        Payment = (decimal)payment,
                        InterestPayment = (decimal)interestPayment,
                        PrincipalPayment = (decimal)principalPayment,
                        EndingBalance = (decimal)endingBalance,
                    });

                startingBalance = endingBalance;
            }

            return result;
        }

        public static List<AmortizationTableEntry> ReducingBalanceSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
        {
            List<AmortizationTableEntry> result = new List<AmortizationTableEntry>();

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
                    new AmortizationTableEntry
                    {
                        Period = period,
                        DueDate = dueDate,
                        StartingBalance = (decimal)startingBalance,
                        Payment = (decimal)(principalPayment + interestPayment),
                        InterestPayment = (decimal)interestPayment,
                        PrincipalPayment = (decimal)principalPayment,
                        EndingBalance = (decimal)endingBalance,
                    });

                startingBalance = endingBalance;
            }

            return result;
        }

        public static List<AmortizationTableEntry> StraightLineAmortizationSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
        {
            List<AmortizationTableEntry> result = new List<AmortizationTableEntry>();

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

                var entry = new AmortizationTableEntry
                {
                    Period = period,
                    DueDate = dueDate,
                    StartingBalance = (decimal)startingBalance,
                    Payment = (decimal)payment,
                    InterestPayment = (decimal)interestPayment,
                    PrincipalPayment = (decimal)principalPayment,
                    EndingBalance = (decimal)endingBalance,
                };

                result.Add(entry);
            }

            return result;
        }

        public static List<AmortizationTableEntry> StraightLineSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
        {
            List<AmortizationTableEntry> result = new List<AmortizationTableEntry>();

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
                    new AmortizationTableEntry
                    {
                        Period = period,
                        DueDate = dueDate,
                        StartingBalance = (decimal)startingBalance,
                        Payment = (decimal)(principalPayment + interestPayment),
                        InterestPayment = (decimal)interestPayment,
                        PrincipalPayment = (decimal)principalPayment,
                        EndingBalance = (decimal)endingBalance,
                    });

                startingBalance = endingBalance;
            }

            return result;
        }

        public static List<AmortizationTableEntry> FixedInterestSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, double APR, double PV, double FV = 0, int Due = 0)
        {
            List<AmortizationTableEntry> result = new List<AmortizationTableEntry>();

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
                   new AmortizationTableEntry
                   {
                       Period = period,
                       DueDate = dueDate,
                       StartingBalance = (decimal)startingBalance,
                       Payment = (decimal)(principalPayment + interestPayment),
                       InterestPayment = (decimal)interestPayment,
                       PrincipalPayment = (decimal)principalPayment,
                       EndingBalance = (decimal)endingBalance,
                   });

                startingBalance = endingBalance;
            }

            return result;
        }
    }
}