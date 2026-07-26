using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InsuranceCompanyAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class InsuranceCompanyAppService : IInsuranceCompanyAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;

        public InsuranceCompanyAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<InsuranceCompany> insuranceCompanyRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (insuranceCompanyRepository == null)
                throw new ArgumentNullException(nameof(insuranceCompanyRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _insuranceCompanyRepository = insuranceCompanyRepository;
        }

        public InsuranceCompanyDTO AddInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO, ServiceHeader serviceHeader)
        {
            if (insuranceCompanyDTO != null && insuranceCompanyDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var address = new Address(insuranceCompanyDTO.AddressAddressLine1, insuranceCompanyDTO.AddressAddressLine2, insuranceCompanyDTO.AddressStreet, insuranceCompanyDTO.AddressPostalCode, insuranceCompanyDTO.AddressCity, insuranceCompanyDTO.AddressEmail, insuranceCompanyDTO.AddressLandLine, insuranceCompanyDTO.AddressMobileLine);

                    var insuranceCompany = InsuranceCompanyFactory.CreateInsuranceCompany(insuranceCompanyDTO.ChartOfAccountId, insuranceCompanyDTO.Description, address);

                    if (insuranceCompanyDTO.IsLocked)
                        insuranceCompany.Lock();
                    else insuranceCompany.UnLock();

                    _insuranceCompanyRepository.Add(insuranceCompany, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return insuranceCompany.ProjectedAs<InsuranceCompanyDTO>();
                }
            }

            return null;
        }

        public bool UpdateInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO, ServiceHeader serviceHeader)
        {
            if (insuranceCompanyDTO == null || insuranceCompanyDTO.Id == Guid.Empty || insuranceCompanyDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _insuranceCompanyRepository.Get(insuranceCompanyDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var address = new Address(insuranceCompanyDTO.AddressAddressLine1, insuranceCompanyDTO.AddressAddressLine2, insuranceCompanyDTO.AddressStreet, insuranceCompanyDTO.AddressPostalCode, insuranceCompanyDTO.AddressCity, insuranceCompanyDTO.AddressEmail, insuranceCompanyDTO.AddressLandLine, insuranceCompanyDTO.AddressMobileLine);

                    var current = InsuranceCompanyFactory.CreateInsuranceCompany(insuranceCompanyDTO.ChartOfAccountId, insuranceCompanyDTO.Description, address);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (insuranceCompanyDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _insuranceCompanyRepository.Merge(persisted, current, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return true;
                }
                else return false;
            }
        }

        public List<InsuranceCompanyDTO> FindInsuranceCompanies(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var insuranceCompanies = _insuranceCompanyRepository.GetAll(serviceHeader);

                if (insuranceCompanies != null && insuranceCompanies.Any())
                {
                    return insuranceCompanies.ProjectedAsCollection<InsuranceCompanyDTO>();
                }
                else return null;
            }
        }

        public InsuranceCompanyDTO FindInsuranceCompany(Guid insuranceCompanyId, ServiceHeader serviceHeader)
        {
            if (insuranceCompanyId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var insuranceCompany = _insuranceCompanyRepository.Get(insuranceCompanyId, serviceHeader);

                    if (insuranceCompany != null)
                    {
                        return insuranceCompany.ProjectedAs<InsuranceCompanyDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompanies(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<InsuranceCompany> spec = InsuranceCompanySpecifications.DefaultSpec();

                var sortFields = new List<string> { "SequentialId" };

                var insuranceCompanyPagedCollection = _insuranceCompanyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (insuranceCompanyPagedCollection != null)
                {
                    var pageCollection = insuranceCompanyPagedCollection.PageCollection.ProjectedAsCollection<InsuranceCompanyDTO>();

                    var itemsCount = insuranceCompanyPagedCollection.ItemsCount;

                    return new PageCollectionInfo<InsuranceCompanyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompanies(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? InsuranceCompanySpecifications.DefaultSpec() : InsuranceCompanySpecifications.InsuranceFullText(text);

                ISpecification<InsuranceCompany> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var insuranceCompanyPagedCollection = _insuranceCompanyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (insuranceCompanyPagedCollection != null)
                {
                    var pageCollection = insuranceCompanyPagedCollection.PageCollection.ProjectedAsCollection<InsuranceCompanyDTO>();

                    var itemsCount = insuranceCompanyPagedCollection.ItemsCount;

                    return new PageCollectionInfo<InsuranceCompanyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }
    }
}
