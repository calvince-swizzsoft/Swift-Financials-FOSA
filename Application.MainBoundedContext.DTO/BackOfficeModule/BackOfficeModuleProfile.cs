using AutoMapper;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.AttachedLoanAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentEntryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.IncomeAdjustmentAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanAppraisalFactorAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryEntryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoaningRemarkAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRequestAgg;
using System.Linq;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class BackOfficeModuleProfile : Profile
    {
        public BackOfficeModuleProfile()
        {
            //LoanPurpose => LoanPurposeDTO
            CreateMap<LoanPurpose, LoanPurposeDTO>();

            //LoanCase => LoanCaseDTO
            CreateMap<LoanCase, LoanCaseDTO>()
                .ForMember(dest => dest.PaddedCaseNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAge, opt => opt.Ignore())
                .ForMember(dest => dest.LoanRegistrationLoanProductSectionDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanInterestChargeModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanInterestRecoveryModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanInterestCalculationModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanRegistrationPaymentFrequencyPerYearDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanRegistrationPaymentDueDateDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAttachedLoansBalance, opt => opt.MapFrom(src => ComputeTotalAttachedLoans(src)));

            //LoanGuarantor => LoanGuarantorDTO
            CreateMap<LoanGuarantor, LoanGuarantorDTO>()
                .ForMember(dest => dest.LoanCasePaddedCaseNumber, opt => opt.Ignore())
                .ForMember(dest => dest.LoanCaseStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoaneeCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoaneeCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                //.ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.LoanProductLoanRegistrationGuarantorSecurityModeDescription, opt => opt.Ignore());

            //LoanAppraisalFactor => LoanAppraisalFactorDTO
            CreateMap<LoanAppraisalFactor, LoanAppraisalFactorDTO>()
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore());

            //IncomeAdjustment => IncomeAdjustmentDTO
            CreateMap<IncomeAdjustment, IncomeAdjustmentDTO>()
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore());

            //LoaningRemark => LoaningRemarkDTO
            CreateMap<LoaningRemark, LoaningRemarkDTO>();

            //AttachedLoan => AttachedLoanDTO
            CreateMap<AttachedLoan, AttachedLoanDTO>()
                .ForMember(dest => dest.LoanCasePaddedCaseNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //DataAttachmentPeriod => DataAttachmentPeriodDTO
            CreateMap<DataAttachmentPeriod, DataAttachmentPeriodDTO>()
                .ForMember(dest => dest.MonthDescription, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //DataAttachmentEntry => DataAttachmentEntryDTO
            CreateMap<DataAttachmentEntry, DataAttachmentEntryDTO>()
                .ForMember(dest => dest.TransactionTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //LoanCollateral => LoanCollateralDTO
            CreateMap<LoanCollateral, LoanCollateralDTO>();

            //LoanDisbursementBatch => LoanDisbursementBatchDTO
            CreateMap<LoanDisbursementBatch, LoanDisbursementBatchDTO>()
                .ForMember(dest => dest.BatchTotal, opt => opt.MapFrom(src => src.LoanDisbursementBatchEntries.Sum(x => x.LoanCase.ApprovedAmount)))
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore());

            //LoanDisbursementBatchEntry => LoanDisbursementBatchEntryDTO
            CreateMap<LoanDisbursementBatchEntry, LoanDisbursementBatchEntryDTO>();

            //LoanGuarantorAttachmentHistory => LoanGuarantorAttachmentHistoryDTO
            CreateMap<LoanGuarantorAttachmentHistory, LoanGuarantorAttachmentHistoryDTO>();

            //LoanGuarantorAttachmentHistoryEntry => LoanGuarantorAttachmentHistoryEntryDTO
            CreateMap<LoanGuarantorAttachmentHistoryEntry, LoanGuarantorAttachmentHistoryEntryDTO>();

            //LoanRequest => LoanRequestDTO
            CreateMap<LoanRequest, LoanRequestDTO>();
        }

        static decimal ComputeTotalAttachedLoans(LoanCase loanCase)
        {
            var totalAttachedLoans = 0m;

            foreach (var item in loanCase.AttachedLoans)
            {
                var attachedLoansBalance =
                    (item.PrincipalBalance * -1 > 0m ? item.PrincipalBalance * -1 : 0m) +
                    (item.InterestBalance * -1 > 0m ? item.InterestBalance * -1 : 0m) +
                    (item.CarryForwardsBalance * -1 > 0m ? item.CarryForwardsBalance * -1 : 0m) +
                    (item.ClearanceCharges);

                totalAttachedLoans += attachedLoansBalance;
            }

            return totalAttachedLoans;
        }
    }
}
