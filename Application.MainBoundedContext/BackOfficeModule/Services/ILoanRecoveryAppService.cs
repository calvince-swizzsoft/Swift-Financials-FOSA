using System;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public interface ILoanRecoveryAppService
 {
  LoanRecoveryPreview Preview(Guid loanCaseId,ServiceHeader h);
  LoanRecoveryPreview Preview(LoanRecoveryRequest input,ServiceHeader h);
  LoanRecoveryReceipt Post(LoanRecoveryRequest input,ServiceHeader h);
 }
}
