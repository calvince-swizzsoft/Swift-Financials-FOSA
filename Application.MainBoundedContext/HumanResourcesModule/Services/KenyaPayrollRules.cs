using System;
using System.Collections.Generic;
using System.Linq;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public sealed class PayrollSetupException : InvalidOperationException
    {
        public PayrollSetupException(string safeMessage) : base(safeMessage) { }
    }

    // Monthly cash payroll rules. See docs/api/kenya-payroll-statutory-rules.md
    // for effective dates, sources, configuration and supported scope.
    public static class KenyaPayrollRules
    {
        public static bool Applies(DateTime month) { return month >= new DateTime(2025, 1, 1); }

        public static bool IsSingleton(SalaryHeadType type)
        {
            return type == SalaryHeadType.FullTimeBasicPayEarning || type == SalaryHeadType.PartTimeBasicPayEarning ||
                type == SalaryHeadType.ContractBasicPayEarning || type == SalaryHeadType.NSSFDeduction ||
                type == SalaryHeadType.NHIFDeduction || type == SalaryHeadType.PAYEDeduction ||
                type == SalaryHeadType.StatutoryProvidentFundDeduction || type == SalaryHeadType.SHIFDeduction ||
                type == SalaryHeadType.AffordableHousingLevyDeduction;
        }

        public static void ValidateHeads(DateTime month, IEnumerable<SalaryHeadType> heads)
        {
            var types = heads.ToList();
            if (!Applies(month))
            {
                if (types.Contains(SalaryHeadType.SHIFDeduction) || types.Contains(SalaryHeadType.AffordableHousingLevyDeduction))
                    throw new PayrollSetupException("The current SHIF and Housing Levy rules support payroll from January 2025 onwards.");
                return;
            }
            Nssf(0m, month); // Reject an unverified future rate schedule before creating accounts or payslips.
            if (types.Contains(SalaryHeadType.NHIFDeduction))
                throw new PayrollSetupException("Replace NHIF with SHIF in this employee's salary group and salary card before processing payroll.");
            foreach (var type in new[] { SalaryHeadType.NSSFDeduction, SalaryHeadType.SHIFDeduction,
                SalaryHeadType.AffordableHousingLevyDeduction, SalaryHeadType.PAYEDeduction })
                if (types.Count(x => x == type) != 1)
                    throw new PayrollSetupException("The employee's salary card must contain exactly one " + type + " salary head.");
        }

        public static decimal Money(decimal value) { return Math.Round(value, 2, MidpointRounding.AwayFromZero); }

        public static decimal Nssf(decimal gross, DateTime month)
        {
            if (!Applies(month) || month >= new DateTime(2027, 2, 1))
                throw new PayrollSetupException("NSSF rates must be configured for this payroll month; the current schedule covers January 2025 to January 2027.");
            var ceiling = month >= new DateTime(2026, 2, 1) ? 108000m :
                month >= new DateTime(2025, 2, 1) ? 72000m : 36000m;
            return Money(Math.Min(Math.Max(0m, gross), ceiling) * .06m);
        }

        public static decimal Shif(decimal gross) { return gross <= 0m ? 0m : Money(Math.Max(300m, gross * .0275m)); }
        public static decimal HousingLevy(decimal regularCashPay) { return Money(Math.Max(0m, regularCashPay) * .015m); }

        public static decimal GrossTax(decimal taxablePay)
        {
            var remaining = Math.Max(0m, taxablePay);
            var widths = new[] { 24000m, 8333m, 467667m, 300000m };
            var rates = new[] { .10m, .25m, .30m, .325m };
            var tax = 0m;
            for (var i = 0; i < widths.Length && remaining > 0m; i++)
            {
                var band = Math.Min(remaining, widths[i]);
                tax += band * rates[i];
                remaining -= band;
            }
            return tax + remaining * .35m;
        }

        public static void Apply(DateTime month, ICollection<PaySlipEntryDTO> entries, decimal regularCashPay,
            SalaryProcessingDTO period, SalaryCardDTO card)
        {
            ValidateHeads(month, entries.Select(x => (SalaryHeadType)x.SalaryHeadType));
            if (!Applies(month)) throw new PayrollSetupException("Use historical tariff rules for this period.");
            if (period.TaxReliefAmount < 0m || period.TaxReliefAmount > 2400m ||
                period.MaximumProvidentFundReliefAmount < 0m || period.MaximumProvidentFundReliefAmount > 30000m ||
                period.MaximumInsuranceReliefAmount < 0m || period.MaximumInsuranceReliefAmount > 5000m ||
                card.InsuranceReliefAmount < 0m || card.TaxExemption < 0m)
                throw new PayrollSetupException("Review salary-period relief limits and salary-card relief amounts before processing payroll.");
            if (entries.Any(x => x.Principal < 0m)) throw new PayrollSetupException("Salary amounts cannot be negative.");
            var gross = entries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Earning).Sum(x => x.Principal);
            if (regularCashPay < 0m || regularCashPay > gross) throw new PayrollSetupException("Regular cash pay must be between zero and gross pay.");
            var nssf = Nssf(gross, month);
            var shif = Shif(gross);
            var housing = HousingLevy(regularCashPay);
            entries.Single(x => x.SalaryHeadType == (int)SalaryHeadType.NSSFDeduction).Principal = nssf;
            entries.Single(x => x.SalaryHeadType == (int)SalaryHeadType.SHIFDeduction).Principal = shif;
            entries.Single(x => x.SalaryHeadType == (int)SalaryHeadType.AffordableHousingLevyDeduction).Principal = housing;
            var pension = nssf + entries.Where(x => x.SalaryHeadType == (int)SalaryHeadType.StatutoryProvidentFundDeduction ||
                x.SalaryHeadType == (int)SalaryHeadType.VoluntaryProvidentFundDeduction).Sum(x => x.Principal);
            var allowablePension = Math.Min(pension, Math.Min(gross * .30m, period.MaximumProvidentFundReliefAmount));
            var taxablePay = Math.Max(0m, gross - allowablePension - shif - housing - (card.IsTaxExempt ? card.TaxExemption : 0m));
            var relief = period.TaxReliefAmount + Math.Min(card.InsuranceReliefAmount, period.MaximumInsuranceReliefAmount);
            entries.Single(x => x.SalaryHeadType == (int)SalaryHeadType.PAYEDeduction).Principal = Money(Math.Max(0m, GrossTax(taxablePay) - relief));
        }
    }
}
