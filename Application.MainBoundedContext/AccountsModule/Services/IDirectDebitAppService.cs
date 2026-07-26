using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IDirectDebitAppService
    {
        DirectDebitDTO AddNewDirectDebit(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader);

        bool UpdateDirectDebit(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader);

        List<DirectDebitDTO> FindDirectDebits(ServiceHeader serviceHeader);

        PageCollectionInfo<DirectDebitDTO> FindDirectDebits(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DirectDebitDTO> FindDirectDebits(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DirectDebitDTO FindDirectDebit(Guid directDebitId, ServiceHeader serviceHeader);

        void FetchDirectDebitsProductDescription(List<DirectDebitDTO> directDebits, ServiceHeader serviceHeader);
    }
}
