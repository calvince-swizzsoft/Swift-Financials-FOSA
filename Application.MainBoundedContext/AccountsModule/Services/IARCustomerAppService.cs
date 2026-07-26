using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IARCustomerAppService
    {

        ARCustomerDTO AddNewARCustomer(ARCustomerDTO arCustomerDTO, ServiceHeader serviceHeader);

        ARCustomerDTO FindARCustomerById(Guid id, ServiceHeader serviceHeader);

        List<ARCustomerDTO> FindARCustomers(ServiceHeader serviceHeader);

        bool UpdateARCustomer(ARCustomerDTO arCustomerDTO, ServiceHeader serviceHeader);
    }
}
