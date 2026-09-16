using System;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public interface ILoanNoticeAppService
 {
  LoanNoticeEligibility Eligible(DateTime asAt,int pageIndex,int pageSize,ServiceHeader h);
  LoanNoticeDTO Generate(LoanNoticeRequest input,ServiceHeader h);
  LoanNoticePage History(int pageIndex,int pageSize,ServiceHeader h);
  LoanNoticeDTO Get(Guid id,ServiceHeader h);
  LoanNoticeDTO Approve(Guid id,ServiceHeader h);
  LoanNoticeDTO Cancel(Guid id,ServiceHeader h);
  string Printable(Guid id,ServiceHeader h);
 }
}
