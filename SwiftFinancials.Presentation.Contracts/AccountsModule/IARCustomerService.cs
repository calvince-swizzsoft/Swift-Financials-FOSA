using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{

    [ServiceContract(Name = "IARCustomerService")]
    public interface IARCustomerService
    {

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddARCustomer(ARCustomerDTO arCustomerDTO, AsyncCallback callback, object state);
        ARCustomerDTO EndAddARCustomer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindARCustomerById(Guid id, AsyncCallback callback, object state);
        ARCustomerDTO EndFindARCustomerById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindARCustomers(AsyncCallback callback, object state);
        List<ARCustomerDTO> EndFindARCustomers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateARCustomer(ARCustomerDTO arCustomerDTO, AsyncCallback callback, object state);
        bool EndUpdateARCustomer(IAsyncResult result);
    }
}
