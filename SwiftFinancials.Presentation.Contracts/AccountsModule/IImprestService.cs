using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using SwiftFinancials.Presentation.Shared.Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{

    [ServiceContract(Name = "IImprestService")]
    public interface IImprestService
    {

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddImprest(ImprestDTO imprestDTO, AsyncCallback callback, Object state);
        ImprestDTO EndAddImprest(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateImprest(ImprestDTO imprestDTO, AsyncCallback callback, Object state);
        bool EndUpdateImprest(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindImprests(AsyncCallback callback, Object state);

        List<ImprestDTO> EndFindImprests(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindImprestLines(AsyncCallback callback, Object state);

        List<ImprestLineDTO> EndFindImprestLines(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginPostImprest(ImprestDTO imprestDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        JournalDTO EndPostImprest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginPostSurrender(ImprestDTO imprestDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        JournalDTO EndPostSurrender(IAsyncResult result);

        
    }
}
