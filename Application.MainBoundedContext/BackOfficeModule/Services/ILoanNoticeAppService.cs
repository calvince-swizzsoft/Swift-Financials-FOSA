using System;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public interface ILoanNoticeAppService
 {
  LoanNoticeLoanRow RecoveryLoan(Guid loanCaseId,ServiceHeader h);
  LoanNoticeLoanPage LoanWorkflow(DateTime asAt,string stage,int pageIndex,int pageSize,ServiceHeader h);
  LoanNoticeLoanRow LoanStageAction(LoanNoticeLoanAction input,ServiceHeader h);
  string PrintableLoanStage(Guid loanCaseId,string stage,ServiceHeader h);
  LoanNoticeEligibility Eligible(DateTime asAt,int pageIndex,int pageSize,ServiceHeader h,bool otherOnly=false);
  LoanNoticeEligibility Workflow(DateTime asAt,string stage,int pageIndex,int pageSize,ServiceHeader h);
  LoanNoticeDTO Send(Guid id,LoanNoticeSendRequest input,ServiceHeader h);
  LoanNoticeDTO RecordSent(Guid id,LoanNoticeDispatchRequest input,ServiceHeader h);
  LoanNoticeDTO Generate(LoanNoticeRequest input,ServiceHeader h);
  LoanNoticePage History(int pageIndex,int pageSize,ServiceHeader h);
  LoanNoticeDTO Get(Guid id,ServiceHeader h);
  LoanNoticeDTO RefreshApproval(Guid id,ServiceHeader h);
  LoanNoticeDTO Approve(Guid id,ServiceHeader h);
  LoanNoticeDTO Cancel(Guid id,ServiceHeader h);
  string Printable(Guid id,ServiceHeader h);
 }
}
