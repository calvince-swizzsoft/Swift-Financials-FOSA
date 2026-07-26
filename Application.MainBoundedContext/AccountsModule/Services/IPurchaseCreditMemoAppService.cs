using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IPurchaseCreditMemoAppService
    {

        PurchaseCreditMemoDTO AddNewPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, ServiceHeader serviceHeader);

        bool UpdatePurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, ServiceHeader serviceHeader);

        List<PurchaseCreditMemoDTO> FindPurchaseCreditMemos(ServiceHeader serviceHeader);

        JournalDTO PostPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

    }
}
