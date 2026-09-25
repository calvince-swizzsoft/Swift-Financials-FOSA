using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Newtonsoft.Json;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public sealed class LoanIncomeAssessmentException : InvalidOperationException
    {
        public LoanIncomeAssessmentException(string message) : base(message) { }
    }

    public static class LoanIncomeAssessmentRules
    {
        public static decimal RequiredTakeHome(decimal gross, int type, double percentage, decimal fixedAmount)
        {
            if (gross <= 0m) throw new LoanIncomeAssessmentException("Verified monthly gross income must be greater than zero.");
            decimal minimum;
            if (type == (int)ChargeType.Percentage)
            {
                if (double.IsNaN(percentage) || double.IsInfinity(percentage) || percentage <= 0d || percentage > 100d)
                    throw new LoanIncomeAssessmentException("Configure a positive minimum take-home percentage before income assessment.");
                minimum = gross * Convert.ToDecimal(percentage) / 100m;
            }
            else if (type == (int)ChargeType.FixedAmount && fixedAmount > 0m) minimum = fixedAmount;
            else throw new LoanIncomeAssessmentException("Configure a positive minimum take-home amount before income assessment.");
            return Math.Ceiling(minimum * 100m) / 100m;
        }

        public static decimal Validate(decimal gross, decimal net, decimal instalment, int type, double percentage, decimal fixedAmount)
        {
            var required = RequiredTakeHome(gross, type, percentage, fixedAmount);
            if (net < 0m || net > gross) throw new LoanIncomeAssessmentException("Verified deductions must leave net income between zero and gross income.");
            if (instalment <= 0m) throw new LoanIncomeAssessmentException("A valid server-generated monthly repayment schedule is required.");
            if (net - instalment < required)
                throw new LoanIncomeAssessmentException("The scheduled instalment would reduce take-home below the product minimum. Reduce the loan amount or defer it for reassessment.");
            return net - required;
        }

        // Decimal scale is storage formatting: 40000 and SQL decimal 40000.00
        // must sign the same assessment. G29 removes only insignificant zeroes.
        private static decimal CanonicalAmount(decimal value)
        {
            return decimal.Parse(value.ToString("G29", System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string Signature(LoanCaseDTO loan, decimal principal, decimal gross, decimal net, string reference)
        {
            return SignaturePayload(loan, CanonicalAmount(principal), CanonicalAmount(gross), CanonicalAmount(net), reference, true);
        }

        public static bool MatchesSignature(LoanCaseDTO loan, decimal principal, decimal gross, decimal net, string reference, string saved)
        {
            if (string.IsNullOrEmpty(saved)) return false;
            if (saved == Signature(loan, principal, gross, net, reference)) return true;
            // Read compatibility for signatures already written before canonicalization.
            // Try only numerically identical persisted/canonical scales, never changed values.
            foreach (var p in new[] { principal, CanonicalAmount(principal) })
            foreach (var g in new[] { gross, CanonicalAmount(gross) })
            foreach (var n in new[] { net, CanonicalAmount(net) })
                if (saved == SignaturePayload(loan, p, g, n, reference, false)) return true;
            return false;
        }

        private static string SignaturePayload(LoanCaseDTO loan, decimal principal, decimal gross, decimal net, string reference, bool canonical)
        {
            var payload = JsonConvert.SerializeObject(new {
                loan.Id, loan.CustomerId, loan.LoanProductId, principal, gross, net, reference,
                loan.RequireIncomeAssessment, loan.LoanInterestAnnualPercentageRate, loan.LoanInterestCalculationMode,
                loan.LoanInterestChargeMode, loan.LoanInterestRecoveryMode, loan.LoanRegistrationTermInMonths,
                loan.LoanRegistrationPaymentFrequencyPerYear, loan.LoanRegistrationGracePeriod,
                loan.LoanRegistrationPaymentDueDate, loan.TakeHomeType, loan.TakeHomePercentage,
                TakeHomeFixedAmount = canonical ? CanonicalAmount(loan.TakeHomeFixedAmount) : loan.TakeHomeFixedAmount
            });
            using(var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(payload))).Replace("-", "");
        }
    }

    public partial class LoanCaseAppService
    {
        private void ApplyIncomeAssessment(LoanCase persisted, LoanCaseDTO input, ServiceHeader header)
        {
            if (persisted.RequireIncomeAssessment != true) return;
            if (persisted.LoanRegistration.PaymentFrequencyPerYear != (int)PaymentFrequencyPerYear.Monthly)
                throw new LoanIncomeAssessmentException("Income assessment currently requires a monthly repayment schedule.");
            if (string.IsNullOrWhiteSpace(input.IncomeAssessmentReference) || input.IncomeAssessmentReference.Length > 512)
                throw new LoanIncomeAssessmentException("Provide the verified payslip or income evidence reference (up to 512 characters).");
            var factors = input.IncomeAssessmentAdjustments ?? new List<LoanAppraisalFactorDTO>();
            if (factors.Select(f => f.IncomeAdjustmentId).Distinct().Count() != factors.Count)
                throw new LoanIncomeAssessmentException("An income deduction cannot be listed more than once.");
            decimal deductions = 0m;
            foreach (var factor in factors.Where(f => f.IsEnabled))
            {
                var definition = _incomeAdjustmentAppService.FindIncomeAdjustment(factor.IncomeAdjustmentId, header);
                if (definition == null || definition.Type != (int)IncomeAdjustmentType.Deduction || factor.Amount <= 0m)
                    throw new LoanIncomeAssessmentException("List positive verified deductions and existing commitments. Include allowances in gross income, not as separate adjustments.");
                deductions += factor.Amount;
            }
            var gross = input.LoanProductLatestIncome;
            var net = gross - deductions;
            var schedule = BuildRepaymentSchedule(persisted.Id, input.AppraisedAmount, header);
            var instalment = schedule.Any() ? schedule.Max(p => p.Payment) : 0m;
            input.AppraisedAbility = LoanIncomeAssessmentRules.Validate(gross, net, instalment,
                persisted.TakeHome.Type, persisted.TakeHome.Percentage, persisted.TakeHome.FixedAmount);
            input.AppraisedNetIncome = net;
            input.MonthlyPaybackAmount = instalment;
            input.TotalPaybackAmount = schedule.Sum(p => p.Payment);
            persisted.IncomeAssessmentReference = input.IncomeAssessmentReference.Trim();
            persisted.IncomeAssessmentSignature = LoanIncomeAssessmentRules.Signature(persisted.ProjectedAs<LoanCaseDTO>(),
                input.AppraisedAmount, gross, net, persisted.IncomeAssessmentReference);
        }

        private void ValidateSavedIncomeAssessment(LoanCase persisted, decimal principal, ServiceHeader header)
        {
            ValidateDepositSecurity(persisted, principal, header);
            if (persisted.RequireIncomeAssessment != true) return;
            if (!LoanIncomeAssessmentRules.MatchesSignature(persisted.ProjectedAs<LoanCaseDTO>(), principal,
                persisted.LoanProductLatestIncome, persisted.AppraisedNetIncome, persisted.IncomeAssessmentReference, persisted.IncomeAssessmentSignature))
                throw new LoanIncomeAssessmentException("The income assessment is missing or its amount, terms or income have changed. Defer the loan for reassessment.");
            var schedule = BuildRepaymentSchedule(persisted.Id, principal, header);
            LoanIncomeAssessmentRules.Validate(persisted.LoanProductLatestIncome, persisted.AppraisedNetIncome,
                schedule.Any() ? schedule.Max(p => p.Payment) : 0m,
                persisted.TakeHome.Type, persisted.TakeHome.Percentage, persisted.TakeHome.FixedAmount);
        }
    }
}
