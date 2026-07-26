using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IARCustomerService
    {

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        ARCustomerDTO AddARCustomer(ARCustomerDTO arCustomerDTO);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        ARCustomerDTO FindARCustomerById(Guid id);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ARCustomerDTO> FindARCustomers();

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateARCustomer(ARCustomerDTO arCustomerDTO);
    }
}