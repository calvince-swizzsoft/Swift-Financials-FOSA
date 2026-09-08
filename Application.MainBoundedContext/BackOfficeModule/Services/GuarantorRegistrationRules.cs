using System;
using System.Collections.Generic;
using System.Linq;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public static class GuarantorRegistrationRules
    {
        public static string ValidateEditableCase(LoanCaseDTO loan)
        {
            if (loan == null) return "Loan case not found.";
            return loan.Status == (int)LoanCaseStatus.Registered ? null : "Guarantors can only be edited while the loan case is Registered.";
        }

        public static decimal EligibleShares(LoanProductDTO product, IEnumerable<CustomerAccountDTO> accounts, IEnumerable<Guid> designatedProductIds)
        {
            if (product.LoanRegistrationLoanProductSection == (int)LoanProductSection.BOSA)
            {
                var ids = new HashSet<Guid>(designatedProductIds.Where(id => id != Guid.Empty));
                if (ids.Count == 0)
                    throw new InvalidOperationException("Configure the BOSA loan product's investment qualification products to designate deposits eligible for guarantees.");
                return accounts.Where(account => account.CustomerAccountTypeProductCode == (int)ProductCode.Investment
                    && ids.Contains(account.CustomerAccountTypeTargetProductId)).Sum(account => account.BookBalance);
            }
            return accounts.Where(account => account.CustomerAccountTypeProductCode == (int)ProductCode.Savings
                || account.CustomerAccountTypeProductCode == (int)ProductCode.Investment).Sum(account => account.BookBalance);
        }

        public static decimal CommittedShares(IEnumerable<LoanGuarantorDTO> guarantees, Guid? excludedLoanCaseId)
        {
            // Released records retain AmountGuaranteed for history, but no longer reserve capacity.
            return guarantees.Where(item => item.Status == (int)LoanGuarantorStatus.Attached
                && (!excludedLoanCaseId.HasValue || item.LoanCaseId != excludedLoanCaseId))
                .Sum(item => item.AmountGuaranteed);
        }

        public static string ValidateCustomer(CustomerDTO customer)
        {
            if (customer == null) return "Guarantor not found.";
            if (customer.IsLocked) return "The selected guarantor is locked.";
            if (customer.RecordStatus != (int)RecordStatus.Approved) return "The selected guarantor has not been approved.";
            if (customer.InhibitGuaranteeing) return "The selected member is not permitted to guarantee loans.";
            return null;
        }

        public static string ValidateCount(LoanProductDTO product, IList<LoanGuarantorDTO> guarantors)
        {
            if (product == null) return "Loan product not found.";
            if (product.LoanRegistrationMinimumGuarantors < 0 || product.LoanRegistrationMaximumGuarantees < product.LoanRegistrationMinimumGuarantors)
                return "The loan product has inconsistent guarantor limits. Correct its configuration before registration.";
            if (!Enum.IsDefined(typeof(GuarantorSecurityMode), product.LoanRegistrationGuarantorSecurityMode))
                return "The loan product has an invalid guarantor security mode.";
            if (guarantors == null || guarantors.Any(item => item == null || item.GuarantorId == Guid.Empty))
                return "Every guarantor entry must identify a member.";
            if (guarantors.Select(item => item.GuarantorId).Distinct().Count() != guarantors.Count)
                return "The same guarantor cannot be submitted more than once.";
            // Explicit counts apply even to microcredit and when Security Required is off.
            if (guarantors.Count < product.LoanRegistrationMinimumGuarantors)
                return string.Format("This loan product requires at least {0} guarantor(s).", product.LoanRegistrationMinimumGuarantors);
            if (guarantors.Count > product.LoanRegistrationMaximumGuarantees)
                return string.Format("This loan product allows at most {0} guarantor(s).", product.LoanRegistrationMaximumGuarantees);
            return null;
        }

        public static string ValidateAmount(LoanProductDTO product, LoanCaseDTO loan, LoanGuarantorDTO guarantor)
        {
            if (guarantor.AmountGuaranteed <= 0m || decimal.Round(guarantor.AmountGuaranteed, 2) != guarantor.AmountGuaranteed)
                return "Each guarantee must be a positive amount with at most two decimal places.";
            if (guarantor.GuarantorId == loan.CustomerId)
            {
                if (!product.LoanRegistrationAllowSelfGuarantee) return "The selected loan product does not allow self-guarantee.";
                var percentage = product.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage;
                if (double.IsNaN(percentage) || double.IsInfinity(percentage) || percentage < 0 || percentage > 100)
                    return "The loan product has an invalid self-guarantee percentage.";
                if (guarantor.AmountGuaranteed > loan.AmountApplied * Convert.ToDecimal(percentage) / 100m)
                    return "The self-guarantee exceeds the product's allowed percentage of the loan amount.";
            }
            // Income mode has no shares-based ceiling. Income assessment stays in appraisal.
            if (product.LoanRegistrationGuarantorSecurityMode == (int)GuarantorSecurityMode.Investments)
            {
                if (double.IsNaN(guarantor.AppraisalFactor) || double.IsInfinity(guarantor.AppraisalFactor) || guarantor.AppraisalFactor < 0)
                    return "The guarantor appraisal factor is invalid.";
                var available = Math.Max(0m, guarantor.TotalShares * Convert.ToDecimal(guarantor.AppraisalFactor) - guarantor.CommittedShares);
                if (guarantor.AmountGuaranteed > available) return "Amount guaranteed exceeds the member's available guarantee capacity.";
            }
            return null;
        }

        public static string ValidateCoverage(LoanProductDTO product, LoanCaseDTO loan, IList<LoanGuarantorDTO> guarantors)
        {
            if (product.LoanRegistrationSecurityRequired
                && product.LoanRegistrationGuarantorSecurityMode == (int)GuarantorSecurityMode.Investments
                && guarantors.Sum(item => item.AmountGuaranteed) + loan.TotalCollateralAmount < loan.AmountApplied)
                return "Guaranteed shares and collateral must fully secure the amount applied.";
            return null;
        }
    }
}
