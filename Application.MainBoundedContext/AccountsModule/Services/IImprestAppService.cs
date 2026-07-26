using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IImprestAppService
    {

        ImprestDTO AddNewImprest(ImprestDTO imprestDTO, ServiceHeader serviceHeader);

        bool UpdateImprest(ImprestDTO imprestDTO, ServiceHeader serviceHeader);

        List<ImprestDTO> FindImprests(ServiceHeader serviceHeader);

        ImprestDTO FindImprest(Guid imprestId, ServiceHeader serviceHeader);

        List<ImprestLineDTO> FindImprestLines(ServiceHeader serviceHeader);

        JournalDTO PostImprest(ImprestDTO imprestDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        JournalDTO PostSurrender(ImprestDTO imprestDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader); 
    }
}
