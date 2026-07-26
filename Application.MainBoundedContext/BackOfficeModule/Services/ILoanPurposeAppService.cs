using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface ILoanPurposeAppService
    {
        LoanPurposeDTO AddNewLoanPurpose(LoanPurposeDTO loanPurposeDTO, ServiceHeader serviceHeader);

        bool UpdateLoanPurpose(LoanPurposeDTO loanPurposeDTO, ServiceHeader serviceHeader);

        List<LoanPurposeDTO> FindLoanPurposes(ServiceHeader serviceHeader);

        PageCollectionInfo<LoanPurposeDTO> FindLoanPurposes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanPurposeDTO> FindLoanPurposes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LoanPurposeDTO FindLoanPurpose(Guid loanPurposeId, ServiceHeader serviceHeader);
    }
}
