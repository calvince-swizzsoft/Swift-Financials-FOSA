using AutoMapper;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodRecommendationAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDisciplinaryCaseAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDocumentAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeePasswordHistoryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeTypeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewAnswerAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewQuestionAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryHeadAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TransactionThresholdAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class HumanResourcesModuleProfile : Profile
    {
        public HumanResourcesModuleProfile()
        {
            //Department => DepartmentDTO
            CreateMap<Department, DepartmentDTO>();

            //Designation => DesignationDTO
            CreateMap<Designation, DesignationDTO>();

            //Employee => EmployeeDTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.BloodGroupDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeTypeCategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //SalaryHead => SalaryHeadDTO
            CreateMap<SalaryHead, SalaryHeadDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountTypeProductCodeDescription, opt => opt.Ignore());

            //SalaryGroup => SalaryGroupDTO
            CreateMap<SalaryGroup, SalaryGroupDTO>()
                .ForMember(dest => dest.SalaryGroupEntries, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryGroupEntry, opt => opt.Ignore());

            //SalaryGroupEntry => SalaryGroupEntryDTO
            CreateMap<SalaryGroupEntry, SalaryGroupEntryDTO>()
                .ForMember(dest => dest.SalaryHeadTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryHeadCategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.RoundingTypeDescription, opt => opt.Ignore());

            //SalaryCard => SalaryCardDTO
            CreateMap<SalaryCard, SalaryCardDTO>()
                .ForMember(dest => dest.PaddedCardNumber, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeBloodGroupDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeEmployeeTypeCategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedEmployeeCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerFullName, opt => opt.Ignore());

            //SalaryCardEntry => SalaryCardEntryDTO
            CreateMap<SalaryCardEntry, SalaryCardEntryDTO>();

            //SalaryPeriod => SalaryPeriodDTO
            CreateMap<SalaryPeriod, SalaryProcessingDTO>()
                .ForMember(dest => dest.MonthDescription, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TotalNetPay, opt => opt.MapFrom(src => ComputeTotalNetPay(src)));

            //PaySlip => PaySlipDTO
            CreateMap<PaySlip, PaySlipDTO>()
                .ForMember(dest => dest.PaddedSalaryCardCardNumber, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryPeriodMonthDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryPeriodStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryCardEmployeeBloodGroupDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryCardEmployeeEmployeeTypeCategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedSalaryCardEmployeeCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryCardEmployeeCustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryCardEmployeeCustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryCardEmployeeCustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryCardEmployeeCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.NetPay, opt => opt.MapFrom(src => src.PaySlipEntries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Earning).Sum(x => x.Principal + x.Interest) - src.PaySlipEntries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Deduction).Sum(x => x.Principal + x.Interest)));

            //PaySlipEntry => PaySlipEntryDTO
            CreateMap<PaySlipEntry, PaySlipEntryDTO>()
                .ForMember(dest => dest.RoundingTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryHeadTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.SalaryHeadCategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore());

            //EmployeeDocument => EmployeeDocumentDTO
            CreateMap<EmployeeDocument, EmployeeDocumentDTO>()
                .ForMember(dest => dest.EmployeeBloodGroupDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeEmployeeTypeCategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedEmployeeCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerFullName, opt => opt.Ignore());

            //Holiday => HolidayDTO
            CreateMap<Holiday, HolidayDTO>();

            //LeaveType => LeaveTypeDTO
            CreateMap<LeaveType, LeaveTypeDTO>();

            //LeaveTypeDTO => LeaveTypeBindingModel
            CreateMap<LeaveTypeDTO, LeaveTypeBindingModel>();

            //LeaveApplication => LeaveApplicationDTO
            CreateMap<LeaveApplication, LeaveApplicationDTO>();

            //LeaveApplicationDTO => LeaveApplicationBindingModel
            CreateMap<LeaveApplicationDTO, LeaveApplicationBindingModel>();

            //EmployeePasswordHistory => EmployeePasswordHistoryDTO
            CreateMap<EmployeePasswordHistory, EmployeePasswordHistoryDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            //TransactionThreshold => TransactionThresholdDTO
            CreateMap<TransactionThreshold, TransactionThresholdDTO>();

            //EmployeeType => EmployeeTypeDTO
            CreateMap<EmployeeType, EmployeeTypeDTO>();

            //EmployeeDisciplinaryCase => EmployeeDisciplinaryCaseDTO
            CreateMap<EmployeeDisciplinaryCase, EmployeeDisciplinaryCaseDTO>();

            //TrainingPeriod => TrainingPeriodDTO
            CreateMap<TrainingPeriod, TrainingPeriodDTO>();

            //TrainingPeriodEntry => TrainingPeriodEntryDTO
            CreateMap<TrainingPeriodEntry, TrainingPeriodEntryDTO>();

            //EmployeeAppraisalTarget => EmployeeAppraisalTargetDTO
            CreateMap<EmployeeAppraisalTarget, EmployeeAppraisalTargetDTO>();

            //EmployeeAppraisalPeriod => EmployeeAppraisalPeriodDTO
            CreateMap<EmployeeAppraisalPeriod, EmployeeAppraisalPeriodDTO>();

            //EmployeeAppraisalPeriod => EmployeeAppraisalPeriodDTO
            CreateMap<EmployeeAppraisal, EmployeeAppraisalDTO>();

            //EmployeeAppraisalPeriodRecommendation => EmployeeAppraisalPeriodRecommendationDTO
            CreateMap<EmployeeAppraisalPeriodRecommendation, EmployeeAppraisalPeriodRecommendationDTO>();

            //EmployeeAppraisalPeriodRecommendationDTO => EmployeeAppraisalPeriodRecommendationBindingModel
            CreateMap<EmployeeAppraisalPeriodRecommendationDTO, EmployeeAppraisalPeriodRecommendationBindingModel>();

            //ExitInterviewQuestion => ExitInterviewQuestionDTO
            CreateMap<ExitInterviewQuestion, ExitInterviewQuestionDTO>();

            //ExitInterviewQuestionDTO => ExitInterviewQuestionBindingModel
            CreateMap<ExitInterviewQuestionDTO, ExitInterviewQuestionBindingModel>();

            //EmployeeExit => EmployeeExitQueryableDTO
            CreateMap<EmployeeExit, EmployeeExitQueryableDTO>()
            .ForMember(dest => dest.EmployeeCustomerIndividualSalutation, opt => opt.MapFrom(src => (int)(src.Employee.Customer.Individual.Salutation)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>(int)(src.Status)))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)(src.Type)));

            //EmployeeExitQueryableDTO => EmployeeExitDTO
            CreateMap<EmployeeExitQueryableDTO, EmployeeExitDTO>();

            //EmployeeExitDTO => EmployeeExitBindingModel
            CreateMap<EmployeeExitDTO, EmployeeExitBindingModel>();

            // ExitInterviewAnswer => ExitInterviewAnswerDTO
            CreateMap<ExitInterviewAnswer, ExitInterviewAnswerDTO>();

            //ExitInterviewAnswerDTO => ExitInterviewAnswerBindingModel
            CreateMap<ExitInterviewAnswerDTO, ExitInterviewAnswerBindingModel>();
        }

        static decimal ComputeTotalNetPay(SalaryPeriod salaryPeriod)
        {
            var totalNetPay = 0m;

            foreach (var item in salaryPeriod.PaySlips)
            {
                var netPay = item.PaySlipEntries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Earning).Sum(x => x.Principal + x.Interest) - item.PaySlipEntries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Deduction).Sum(x => x.Principal + x.Interest);

                totalNetPay += netPay;
            }

            return totalNetPay;
        }
    }
}
