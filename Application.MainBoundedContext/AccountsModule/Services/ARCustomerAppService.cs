using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ARCustomerAgg;

using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ARCustomerAppService : IARCustomerAppService
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ARCustomer> _arCustomerRepository;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        public ARCustomerAppService(
          IDbContextScopeFactory dbContextScopeFactory,
         IRepository<ARCustomer> arCustomerRepository,
         INumberSeriesGenerator numberSeriesGenerator
            ) 
        
        { 

            if (dbContextScopeFactory == null)
                throw new ArgumentNullException("dbContextScopeFactory");
            if (arCustomerRepository == null)
                throw new ArgumentNullException("arCustomerRepository");
            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));


            _dbContextScopeFactory = dbContextScopeFactory;
            _arCustomerRepository = arCustomerRepository; 
            _numberSeriesGenerator = numberSeriesGenerator;

        }


        public ARCustomerDTO AddNewARCustomer(ARCustomerDTO arCustomerDTO, ServiceHeader serviceHeader)
        {
            if (arCustomerDTO != null)
            {

                //purchaseIn

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {

                    var arCustomerNo = _numberSeriesGenerator.GetNextNumber("ARC", serviceHeader);

                    var arCustomer = ARCustomerFactory.CreateARCustomer(arCustomerNo, arCustomerDTO.Name, arCustomerDTO.Address, arCustomerDTO.MobilePhoneNumber, arCustomerDTO.Town, arCustomerDTO.City, arCustomerDTO.Country, arCustomerDTO.ContactPersonName, arCustomerDTO.ContactPersonPhoneNo);

                
                    arCustomer.CreatedBy = serviceHeader.ApplicationUserName;

                    _arCustomerRepository.Add(arCustomer, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return arCustomer.ProjectedAs<ARCustomerDTO>();
                }
            }
            else return null;
        }



        public bool UpdateARCustomer(ARCustomerDTO arCustomerDTO, ServiceHeader serviceHeader)
        {
           

            if (arCustomerDTO == null || arCustomerDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _arCustomerRepository.Get(arCustomerDTO.Id, serviceHeader);

                if (persisted != null)
                {

                    var current = ARCustomerFactory.CreateARCustomer(persisted.No, arCustomerDTO.Name, arCustomerDTO.Address, arCustomerDTO.MobilePhoneNumber, arCustomerDTO.Town, arCustomerDTO.City, arCustomerDTO.Country, arCustomerDTO.ContactPersonName, arCustomerDTO.ContactPersonPhoneNo);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _arCustomerRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }



        public List<ARCustomerDTO> FindARCustomers(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var arCustomers = _arCustomerRepository.GetAll(serviceHeader);

                if (arCustomers != null && arCustomers.Any())
                {
                    return arCustomers.ProjectedAsCollection<ARCustomerDTO>();
                }
                else return null;
            }
        }


        public ARCustomerDTO FindARCustomerById(Guid Id, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var arCustomer = _arCustomerRepository.Get(Id, serviceHeader);

                if (arCustomer != null)
                {
                    return arCustomer.ProjectedAs<ARCustomerDTO>();
                }
                else return null;
            }
        }






    }
}
