using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BankAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BankBranchAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class BankAppService : IBankAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Bank> _bankRepository;
        private readonly IRepository<BankBranch> _bankBranchRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public BankAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Bank> bankRepository,
           IRepository<BankBranch> bankBranchRepository,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (bankRepository == null)
                throw new ArgumentNullException(nameof(bankRepository));

            if (bankBranchRepository == null)
                throw new ArgumentNullException(nameof(bankBranchRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public BankDTO AddNewBank(BankDTO bankDTO, ServiceHeader serviceHeader)
        {
            if (bankDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var bank = BankFactory.CreateBank(bankDTO.Code, bankDTO.Description, bankDTO.Address, bankDTO.SwiftCode, bankDTO.IbanNo, bankDTO.City);

                    _bankRepository.Add(bank, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return bank.ProjectedAs<BankDTO>();
                }
            }
            else return null;
        }

        public bool UpdateBank(BankDTO bankDTO, ServiceHeader serviceHeader)
        {
            if (bankDTO == null || bankDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _bankRepository.Get(bankDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = BankFactory.CreateBank(bankDTO.Code, bankDTO.Description, bankDTO.Address, bankDTO.SwiftCode, bankDTO.IbanNo, bankDTO.City);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                    _bankRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<BankDTO> FindBanks(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var banks = _bankRepository.GetAll(serviceHeader);

                if (banks != null && banks.Any())
                {
                    return banks.ProjectedAsCollection<BankDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankDTO> FindBanks(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankSpecifications.DefaultSpec();

                ISpecification<Bank> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankPagedCollection = _bankRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankPagedCollection != null)
                {
                    var pageCollection = bankPagedCollection.PageCollection.ProjectedAsCollection<BankDTO>();

                    var itemsCount = bankPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BankDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankDTO> FindBanks(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? BankSpecifications.DefaultSpec() : BankSpecifications.BankFullText(text);

                ISpecification<Bank> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankPagedCollection = _bankRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankPagedCollection != null)
                {
                    var pageCollection = bankPagedCollection.PageCollection.ProjectedAsCollection<BankDTO>();

                    var itemsCount = bankPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BankDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public BankDTO FindBank(Guid bankId, ServiceHeader serviceHeader)
        {
            if (bankId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var bank = _bankRepository.Get(bankId, serviceHeader);

                    if (bank != null)
                    {
                        return bank.ProjectedAs<BankDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<BankBranchDTO> FindBankBranches(Guid bankId, ServiceHeader serviceHeader)
        {
            if (bankId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = BankBranchSpecifications.BankBranchWithBankId(bankId);

                    ISpecification<BankBranch> spec = filter;

                    var bankBranches = _bankBranchRepository.AllMatching(spec, serviceHeader);

                    if (bankBranches != null)
                    {
                        return bankBranches.ProjectedAsCollection<BankBranchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateBankBranches(Guid bankId, List<BankBranchDTO> bankBranches, ServiceHeader serviceHeader)
        {
            if (bankId != null && bankBranches != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _bankRepository.Get(bankId, serviceHeader);

                    if (persisted != null)
                    {
                        if (persisted.BankBranches != null && persisted.BankBranches.Any())
                        {
                            persisted.BankBranches.ToList().ForEach(x => _bankBranchRepository.Remove(x, serviceHeader));
                        }

                        if (bankBranches.Any())
                        {
                            foreach (var item in bankBranches)
                            {
                                var address = new Address(item.AddressAddressLine1, item.AddressAddressLine2, item.AddressStreet, item.AddressPostalCode, item.AddressCity, item.AddressEmail, item.AddressLandLine, item.AddressMobileLine);

                                var bankBranch = BankBranchFactory.CreateBankBranch(persisted.Id, item.Code, item.Description, address);

                                _bankBranchRepository.Add(bankBranch, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool BulkImport(List<string> bankCodes, List<string> bankNames, List<string> branchCodes, List<string> branchNames, List<int> branchIndexes, ServiceHeader serviceHeader)
        {
            #region branches

            var tupleList = new List<Tuple<long, long>>();

            var indexCounter = 0;

            foreach (var item in branchIndexes)
            {
                if (indexCounter + 1 < branchIndexes.Count)
                {
                    var tuple = new Tuple<long, long>(branchIndexes[indexCounter], branchIndexes[indexCounter + 1]);

                    tupleList.Add(tuple);
                }
                else
                {
                    var tuple = new Tuple<long, long>(branchIndexes.Last(), branchCodes.Count);

                    tupleList.Add(tuple);
                }

                indexCounter += 1;
            }

            var branchNameSets = new List<string[]>();

            var branchCodeSets = new List<string[]>();

            foreach (var tuple in tupleList)
            {
                var skip = (int)tuple.Item1;

                var take = (int)tuple.Item2 - (int)tuple.Item1;

                var targetBranchNames = branchNames.Skip(skip).Take(take).ToArray();

                branchNameSets.Add(targetBranchNames);

                var targetBranchCodes = branchCodes.Skip(skip).Take(take).ToArray();

                branchCodeSets.Add(targetBranchCodes);
            }

            #endregion

            #region banks

            var banksList = new List<BankDTO>();

            var bankCounter = 0;

            foreach (var bankName in bankNames)
            {
                var bank = new BankDTO { Code = int.Parse(bankCodes[bankCounter]), Description = bankName };

                var branchCounter = 0;

                foreach (var branchName in branchNameSets[bankCounter])
                {
                    var branch = new BankBranchDTO { Code = int.Parse(branchCodeSets[bankCounter][branchCounter]), Description = branchName };

                    bank.BankBranche.Add(branch);

                    branchCounter += 1;
                }

                banksList.Add(bank);

                bankCounter += 1;
            }

            #endregion

            return BulkImportBanks(banksList, serviceHeader);
        }

        private bool BulkImportBanks(List<BankDTO> banks, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var newBanks = new List<Bank>();

            var newBankBranches = new List<BankBranch>();

            var counter = default(int);

            banks.ForEach(bankDTO =>
            {
                counter += 1;

                var bank = BankFactory.CreateBank(bankDTO.Code, bankDTO.Description, bankDTO.Address, bankDTO.SwiftCode, bankDTO.IbanNo, bankDTO.City);

                newBanks.Add(bank);

                foreach (var bankBranchDTO in bankDTO.BankBranche)
                {
                    var address = new Address(bankBranchDTO.AddressAddressLine1, bankBranchDTO.AddressAddressLine2, bankBranchDTO.AddressStreet, bankBranchDTO.AddressPostalCode, bankBranchDTO.AddressCity, bankBranchDTO.AddressEmail, bankBranchDTO.AddressLandLine, bankBranchDTO.AddressMobileLine);

                    var bankBranch = BankBranchFactory.CreateBankBranch(bank.Id, bankBranchDTO.Code, bankBranchDTO.Description, address);

                    newBankBranches.Add(bankBranch);
                }
            });

            var bcpBanks = new List<BankBulkCopyDTO>();

            newBanks.ForEach(c =>
            {
                BankBulkCopyDTO bcpc =
                    new BankBulkCopyDTO
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Description = c.Description,
                        CreatedDate = c.CreatedDate
                    };

                bcpBanks.Add(bcpc);
            });

            var bcpBankBranches = new List<BankBranchBulkCopyDTO>();

            newBankBranches.ForEach(c =>
            {
                BankBranchBulkCopyDTO bcpc =
                    new BankBranchBulkCopyDTO
                    {
                        Id = c.Id,
                        BankId = c.BankId,
                        Code = c.Code,
                        Description = c.Description,
                        Address_Email = c.Address.Email,
                        Address_City = c.Address.City,
                        Address_PostalCode = c.Address.PostalCode,
                        Address_AddressLine1 = c.Address.AddressLine1,
                        Address_AddressLine2 = c.Address.AddressLine2,
                        Address_Street = c.Address.Street,
                        Address_MobileLine = c.Address.MobileLine,
                        Address_LandLine = c.Address.LandLine,
                        CreatedDate = c.CreatedDate
                    };

                bcpBankBranches.Add(bcpc);
            });

            result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _bankRepository.Pluralize()), bcpBanks, serviceHeader);

            if (result)
            {
                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _bankBranchRepository.Pluralize()), bcpBankBranches, serviceHeader);
            }

            return result;
        }
    }
}
