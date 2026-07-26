using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface ILoaningRemarkAppService
    {
        LoaningRemarkDTO AddNewLoaningRemark(LoaningRemarkDTO loaningRemarkDTO, ServiceHeader serviceHeader);

        bool UpdateLoaningRemark(LoaningRemarkDTO loaningRemarkDTO, ServiceHeader serviceHeader);

        List<LoaningRemarkDTO> FindLoaningRemarks(ServiceHeader serviceHeader);

        PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarks(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarks(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LoaningRemarkDTO FindLoaningRemark(Guid loaningRemarkId, ServiceHeader serviceHeader);
    }
}
