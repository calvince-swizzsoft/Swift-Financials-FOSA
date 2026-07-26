using Application.MainBoundedContext.DTO;
using System.Collections.Generic;

namespace Application.MainBoundedContext.Services
{
    public interface IFinancialsService
    {
        double FV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV = 0, int Due = 0);

        double PV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV = 0, int Due = 0);

        double Pmt(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV = 0, int Due = 0);

        double PPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0);

        double IPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0);

        double NPer(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV = 0, int Due = 0);

        double SLN(double initialCost, double residualValue, double usefulLife);

        double SYD(double initialCost, double residualValue, double usefulLife, double period);

        double DDB(double initialCost, double residualValue, double usefulLife, double period);

        double VDB(double initialCost, double residualValue, double usefulLife, double period, double factor);

        double DB(double initialCost, double residualValue, double usefulLife, double period, int month);

        List<AmortizationTableEntry> RepaymentSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV = 0, int Due = 0);
    }
}
