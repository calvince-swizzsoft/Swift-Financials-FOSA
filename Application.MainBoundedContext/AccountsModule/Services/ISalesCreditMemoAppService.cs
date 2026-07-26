using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ISalesCreditMemoAppService
    {

        SalesCreditMemoDTO AddNewSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, ServiceHeader serviceHeader);

        bool UpdateSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, ServiceHeader serviceHeader);

        List<SalesCreditMemoDTO> FindSalesCreditMemos(ServiceHeader serviceHeader);

        JournalDTO PostSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

    }
}
