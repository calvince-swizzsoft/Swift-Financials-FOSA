using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IDirectDebitService
    {
        #region Direct Debit

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DirectDebitDTO AddDirectDebit(DirectDebitDTO directDebitDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDirectDebit(DirectDebitDTO directDebitDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DirectDebitDTO> FindDirectDebits();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DirectDebitDTO> FindDirectDebitsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DirectDebitDTO> FindDirectDebitsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DirectDebitDTO FindDirectDebit(Guid directDebitId);

        #endregion
    }
}
