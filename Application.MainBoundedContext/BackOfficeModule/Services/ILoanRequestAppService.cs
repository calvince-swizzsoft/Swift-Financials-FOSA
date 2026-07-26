using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface ILoanRequestAppService
    {
        LoanRequestDTO AddNewLoanRequest(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader);

        bool CancelLoanRequest(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader);

        bool RegisterLoanRequest(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader);

        bool RemoveLoanRequest(Guid loanRequestId, ServiceHeader serviceHeader);

        LoanRequestDTO FindLoanRequest(Guid loanCaseId, ServiceHeader serviceHeader);

        List<LoanRequestDTO> FindLoanRequests(ServiceHeader serviceHeader);

        List<LoanRequestDTO> FindLoanRequests(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);

        List<LoanRequestDTO> FindLoanRequestsByCustomerIdInProcess(Guid customerId, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanRequestDTO> FindLoanRequests(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanRequestDTO> FindLoanRequests(string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanRequestDTO> FindLoanRequests(DateTime startDate, DateTime endDate, string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanRequestDTO> FindLoanRequests(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
