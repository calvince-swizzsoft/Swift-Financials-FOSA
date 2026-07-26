using Application.MainBoundedContext.DTO.MessagingModule;
using Application.Seedwork;
using Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public class CustomerMessageHistoryAppService : ICustomerMessageHistoryAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CustomerMessageHistory> _customerMessageHistoryRepository;

        public CustomerMessageHistoryAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<CustomerMessageHistory> customerMessageHistoryRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (customerMessageHistoryRepository == null)
                throw new ArgumentNullException(nameof(customerMessageHistoryRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _customerMessageHistoryRepository = customerMessageHistoryRepository;
        }

        public CustomerMessageHistoryDTO AddNewCustomerMessageHistory(CustomerMessageHistoryDTO customerMessageHistoryDTO, ServiceHeader serviceHeader)
        {
            if (customerMessageHistoryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var customerMessageHistory = CustomerMessageHistoryFactory.CreateCustomerMessageHistory(customerMessageHistoryDTO.CustomerId, customerMessageHistoryDTO.Body, customerMessageHistoryDTO.Subject, customerMessageHistoryDTO.MessageCategory, customerMessageHistoryDTO.Recipient);

                    _customerMessageHistoryRepository.Add(customerMessageHistory, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return customerMessageHistory.ProjectedAs<CustomerMessageHistoryDTO>();
                }
            }
            else return null;
        }

        public List<CustomerMessageHistoryDTO> FindCustomerMessageHistories(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var customerMessageHistories = _customerMessageHistoryRepository.GetAll(serviceHeader);

                if (customerMessageHistories != null && customerMessageHistories.Any())
                {
                    return customerMessageHistories.ProjectedAsCollection<CustomerMessageHistoryDTO>();
                }
                else return null;
            }
        }
    }
}
