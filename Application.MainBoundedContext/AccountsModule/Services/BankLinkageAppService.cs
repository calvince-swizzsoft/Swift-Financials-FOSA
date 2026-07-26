using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankLinkageAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class BankLinkageAppService : IBankLinkageAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<BankLinkage> _bankLinkageRepository;

        public BankLinkageAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<BankLinkage> bankLinkageRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (bankLinkageRepository == null)
                throw new ArgumentNullException(nameof(bankLinkageRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _bankLinkageRepository = bankLinkageRepository;
        }

        public BankLinkageDTO AddNewBankLinkage(BankLinkageDTO bankLinkageDTO, ServiceHeader serviceHeader)
        {
            if (bankLinkageDTO != null && bankLinkageDTO.BranchId != Guid.Empty && bankLinkageDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var bankLinkage = BankLinkageFactory.CreateBankLinkage(bankLinkageDTO.BranchId, bankLinkageDTO.BankId, bankLinkageDTO.ChartOfAccountId, bankLinkageDTO.BankName, bankLinkageDTO.BankBranchName, bankLinkageDTO.BankAccountNumber, bankLinkageDTO.Remarks);

                    if (bankLinkageDTO.IsLocked)
                        bankLinkage.Lock();
                    else bankLinkage.UnLock();

                    _bankLinkageRepository.Add(bankLinkage, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return bankLinkage.ProjectedAs<BankLinkageDTO>();
                }
            }
            else return null;
        }

        public bool UpdateBankLinkage(BankLinkageDTO bankLinkageDTO, ServiceHeader serviceHeader)
        {
            if (bankLinkageDTO == null || bankLinkageDTO.Id == Guid.Empty || bankLinkageDTO.BranchId == Guid.Empty || bankLinkageDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _bankLinkageRepository.Get(bankLinkageDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = BankLinkageFactory.CreateBankLinkage(bankLinkageDTO.BranchId, bankLinkageDTO.BankId, bankLinkageDTO.ChartOfAccountId, bankLinkageDTO.BankName, bankLinkageDTO.BankBranchName, bankLinkageDTO.BankAccountNumber, bankLinkageDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (bankLinkageDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _bankLinkageRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<BankLinkageDTO> FindBankLinkages(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var bankLinkages = _bankLinkageRepository.GetAll(serviceHeader);

                if (bankLinkages != null && bankLinkages.Any())
                {
                    return bankLinkages.ProjectedAsCollection<BankLinkageDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankLinkageDTO> FindBankLinkages(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankLinkageSpecifications.DefaultSpec();

                ISpecification<BankLinkage> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankLinkagePagedCollection = _bankLinkageRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankLinkagePagedCollection != null)
                {
                    var pageCollection = bankLinkagePagedCollection.PageCollection.ProjectedAsCollection<BankLinkageDTO>();

                    var itemsCount = bankLinkagePagedCollection.ItemsCount;

                    return new PageCollectionInfo<BankLinkageDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankLinkageDTO> FindBankLinkages(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankLinkageSpecifications.BankLinkageFullText(text);

                ISpecification<BankLinkage> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankLinkageCollection = _bankLinkageRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankLinkageCollection != null)
                {
                    var pageCollection = bankLinkageCollection.PageCollection.ProjectedAsCollection<BankLinkageDTO>();

                    var itemsCount = bankLinkageCollection.ItemsCount;

                    return new PageCollectionInfo<BankLinkageDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public BankLinkageDTO FindBankLinkage(Guid bankLinkageId, ServiceHeader serviceHeader)
        {
            if (bankLinkageId != Guid.Empty)
            {

                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var bankLinkage = _bankLinkageRepository.Get(bankLinkageId, serviceHeader);

                    if (bankLinkage != null)
                    {
                        return bankLinkage.ProjectedAs<BankLinkageDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }



        public BankLinkageDTO FindBankLinkageByBankAccountId(Guid bankAccountId, ServiceHeader serviceHeader)
        {
            if (bankAccountId == Guid.Empty)
                return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var spec = BankLinkageSpecifications.BankLinkageWithBankAccountId(bankAccountId);

                var bankLinkages = _bankLinkageRepository.AllMatching(spec, serviceHeader);

                if (bankLinkages != null && bankLinkages.Any())
                {
                    // Return just one
                    var bankLinkage = bankLinkages.First();  // or .Single()
                    return bankLinkage.ProjectedAs<BankLinkageDTO>();
                }

                return null;
            }
        }

    }
}
