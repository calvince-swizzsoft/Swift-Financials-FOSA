using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;
using System.Globalization;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg
{
    public static class LoanCaseSpecifications
    {
        public static Specification<LoanCase> DefaultSpec(bool includeBatchStatus)
        {
            if (includeBatchStatus)
            {
                Specification<LoanCase> specification = new TrueSpecification<LoanCase>();

                specification &= new DirectSpecification<LoanCase>(x => !x.IsBatched);

                return specification;
            }
            else return new TrueSpecification<LoanCase>();
        }

        public static Specification<LoanCase> LoanCaseWithCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId)
        {
            Specification<LoanCase> specification = DefaultSpec(false);

            if (customerId != null && customerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var customerIdAndLoanProductIdSpec = new DirectSpecification<LoanCase>(c => c.CustomerId == customerId && c.LoanProductId == loanProductId);

                specification &= customerIdAndLoanProductIdSpec;
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition)
        {
            Specification<LoanCase> specification = null;

            switch ((AuxiliaryLoanCondition)auxiliaryLoanCondition)
            {
                case AuxiliaryLoanCondition.SubjectToHavingLoanInProcessApproved:
                    specification = new DirectSpecification<LoanCase>(c => c.CustomerId == customerId && c.LoanProductId == loanProductId && (c.Status == (int)LoanCaseStatus.Approved));
                    break;
                case AuxiliaryLoanCondition.SubjectToHavingLoanInProcessAudited:
                    specification = new DirectSpecification<LoanCase>(c => c.CustomerId == customerId && c.LoanProductId == loanProductId && (c.Status == (int)LoanCaseStatus.Audited));
                    break;
                case AuxiliaryLoanCondition.SubjectToHavingLoanInProcessAppraised:
                    specification = new DirectSpecification<LoanCase>(c => c.CustomerId == customerId && c.LoanProductId == loanProductId && (c.Status == (int)LoanCaseStatus.Appraised));
                    break;
                default:
                    specification = LoanCaseWithCustomerIdAndLoanProductId(customerId, loanProductId);
                    break;
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithStatus(int status)
        {
            Specification<LoanCase> specification = DefaultSpec(false);

            specification &= new DirectSpecification<LoanCase>(x => x.Status == status);

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithLoanCaseNumber(int caseNumber)
        {
            Specification<LoanCase> specification = DefaultSpec(false);

            var loanCaseSpec = new DirectSpecification<LoanCase>(c => c.CaseNumber == caseNumber);

            specification &= loanCaseSpec;


            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithStatusAndFullText(int status, string text, int loanCaseFilter, bool includeBatchStatus)
        {
            Specification<LoanCase> specification = DefaultSpec(includeBatchStatus);

            specification &= new DirectSpecification<LoanCase>(x => x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanCaseFilter)loanCaseFilter)
                {
                    case LoanCaseFilter.CaseNumber:

                        var caseNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out caseNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.CaseNumber == caseNumber);
                        }

                        break;
                    case LoanCaseFilter.Reference:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanCaseFilter.Branch:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case LoanCaseFilter.Purpose:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanCaseFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanCaseFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanCaseFilter.BatchNumber:

                        var batchNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out batchNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.BatchNumber == batchNumber);
                        }

                        break;
                    case LoanCaseFilter.Remarks:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    case LoanCaseFilter.CustomerSerialNumber:
                        break;
                    case LoanCaseFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanCaseFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanCaseFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanCaseFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanCaseFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanCaseFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanCaseFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanCaseFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithLoanProductSectionAndStatusAndFullText(DateTime startDate, DateTime endDate, int loanProductSection, int status, string text, int loanCaseFilter, bool includeBatchStatus)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanCase> specification = DefaultSpec(includeBatchStatus);

            specification &= new DirectSpecification<LoanCase>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.LoanRegistration.LoanProductSection == loanProductSection && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanCaseFilter)loanCaseFilter)
                {
                    case LoanCaseFilter.CaseNumber:

                        var caseNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out caseNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.CaseNumber == caseNumber);
                        }

                        break;
                    case LoanCaseFilter.Reference:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanCaseFilter.Branch:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case LoanCaseFilter.Purpose:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanCaseFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanCaseFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanCaseFilter.BatchNumber:

                        var batchNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out batchNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.BatchNumber == batchNumber);
                        }

                        break;
                    case LoanCaseFilter.Remarks:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    case LoanCaseFilter.CustomerSerialNumber:
                        break;
                    case LoanCaseFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanCaseFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanCaseFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanCaseFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanCaseFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanCaseFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanCaseFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanCaseFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithLoanProductCategoryAndStatusAndFullText(DateTime startDate, DateTime endDate, int loanProductCategory, int status, string text, int loanCaseFilter, decimal approvedAmountThreshold, bool includeBatchStatus)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanCase> specification = DefaultSpec(includeBatchStatus);

            specification &= new DirectSpecification<LoanCase>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.LoanRegistration.LoanProductCategory == loanProductCategory && x.Status == status && x.ApprovedAmount <= approvedAmountThreshold);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanCaseFilter)loanCaseFilter)
                {
                    case LoanCaseFilter.CaseNumber:

                        var caseNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out caseNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.CaseNumber == caseNumber);
                        }

                        break;
                    case LoanCaseFilter.Reference:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanCaseFilter.Branch:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case LoanCaseFilter.Purpose:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanCaseFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanCaseFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanCaseFilter.BatchNumber:

                        var batchNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out batchNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.BatchNumber == batchNumber);
                        }

                        break;
                    case LoanCaseFilter.Remarks:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    case LoanCaseFilter.CustomerSerialNumber:
                        break;
                    case LoanCaseFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanCaseFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanCaseFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanCaseFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanCaseFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanCaseFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanCaseFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanCaseFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseFullText(string text, int loanCaseFilter, bool includeBatchStatus)
        {
            Specification<LoanCase> specification = DefaultSpec(includeBatchStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanCaseFilter)loanCaseFilter)
                {
                    case LoanCaseFilter.CaseNumber:

                        var caseNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out caseNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.CaseNumber == caseNumber);
                        }

                        break;
                    case LoanCaseFilter.Reference:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanCaseFilter.Branch:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case LoanCaseFilter.Purpose:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanCaseFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanCaseFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanCaseFilter.BatchNumber:

                        var batchNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out batchNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.BatchNumber == batchNumber);
                        }

                        break;
                    case LoanCaseFilter.Remarks:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    case LoanCaseFilter.CustomerSerialNumber:
                        break;
                    case LoanCaseFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanCaseFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanCaseFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanCaseFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanCaseFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanCaseFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanCaseFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanCaseFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithDateRangeAndLoanProductSectionAndFullText(DateTime startDate, DateTime endDate, int loanProductSection, string text, int loanCaseFilter, bool includeBatchStatus)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanCase> specification = DefaultSpec(includeBatchStatus);

            specification &= new DirectSpecification<LoanCase>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.LoanRegistration.LoanProductSection == loanProductSection);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanCaseFilter)loanCaseFilter)
                {
                    case LoanCaseFilter.CaseNumber:

                        var caseNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out caseNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.CaseNumber == caseNumber);
                        }

                        break;
                    case LoanCaseFilter.Reference:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanCaseFilter.Branch:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case LoanCaseFilter.Purpose:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanCaseFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanCaseFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanCaseFilter.BatchNumber:

                        var batchNumber = default(int);

                        if (int.TryParse(text.StripPunctuation(), out batchNumber))
                        {
                            specification &= new DirectSpecification<LoanCase>(x => x.BatchNumber == batchNumber);
                        }

                        break;
                    case LoanCaseFilter.Remarks:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    case LoanCaseFilter.CustomerSerialNumber:
                        break;
                    case LoanCaseFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanCaseFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanCaseFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanCaseFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanCaseFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanCaseFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanCaseFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanCaseFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanCaseFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanCaseFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanCaseFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanCase>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanCase> LoanCaseWithCustomerIdInProcess(Guid customerId)
        {
            Specification<LoanCase> specification = new TrueSpecification<LoanCase>();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<LoanCase>(c =>
                    c.CustomerId == customerId &&
                    (c.Status == (int)LoanCaseStatus.Registered ||
                    c.Status == (int)LoanCaseStatus.Appraised ||
                    c.Status == (int)LoanCaseStatus.Approved ||
                    c.Status == (int)LoanCaseStatus.Audited ||
                    c.Status == (int)LoanCaseStatus.Deferred));

                specification &= customerIdSpec;
            }

            return specification;
        }
    }
}
