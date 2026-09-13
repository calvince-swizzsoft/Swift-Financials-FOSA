using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public interface ILoanAgeingAppService
 {
  SasraForm4Result PreviewForm4(SasraForm4Request input,ServiceHeader h);
  LoanRiskReviewDTO SaveRiskReview(LoanRiskReviewDTO input,ServiceHeader h);
  void CaptureRestructureSchedule(LoanPlanDTO pending,IEnumerable<Application.MainBoundedContext.DTO.AmortizationTableEntry> schedule,ServiceHeader h);
  LoanAgeingCasePage GetCases(string text,int pageIndex,int pageSize,ServiceHeader h);
  LoanScheduleProposalDTO GenerateSchedule(Guid caseId,ServiceHeader h);
  List<LoanPlanDTO> ConfirmGeneratedSchedules(List<LoanScheduleConfirmationDTO> input,ServiceHeader h);
  LoanPlanDTO GetPlan(Guid caseId,ServiceHeader h);
  List<LoanPlanDTO> GetHistory(Guid caseId,ServiceHeader h);
  LoanPlanDTO SavePlan(LoanPlanDTO input,ServiceHeader h);
  LoanAgeingResult GetReport(DateTime asAt,Guid? branchId,int pageIndex,int pageSize,Guid? accountId,ServiceHeader h);
 }
}
