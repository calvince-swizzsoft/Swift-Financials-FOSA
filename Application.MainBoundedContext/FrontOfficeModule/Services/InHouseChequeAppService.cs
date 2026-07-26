using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.InHouseChequeAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class InHouseChequeAppService : IInHouseChequeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<InHouseCheque> _inHouseChequeRepository;
        private readonly IJournalAppService _journalAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public InHouseChequeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<InHouseCheque> inHouseChequeRepository,
           IJournalAppService journalAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICommissionAppService commissionAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (inHouseChequeRepository == null)
                throw new ArgumentNullException(nameof(inHouseChequeRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _inHouseChequeRepository = inHouseChequeRepository;
            _journalAppService = journalAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _commissionAppService = commissionAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public InHouseChequeDTO AddNewInHouseCheque(InHouseChequeDTO inHouseChequeDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var chequesSuspenseChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InHouseChequesControl, serviceHeader);

            if (inHouseChequeDTO != null && chequesSuspenseChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var inHouseCheque = InHouseChequeFactory.CreateInHouseCheque(inHouseChequeDTO.BranchId, inHouseChequeDTO.ChequeTypeId, inHouseChequeDTO.DebitChartOfAccountId, inHouseChequeDTO.DebitCustomerAccountId, inHouseChequeDTO.Funding, inHouseChequeDTO.Amount, inHouseChequeDTO.Payee, inHouseChequeDTO.Reference, inHouseChequeDTO.Chargeable);

                    inHouseCheque.CreatedBy = serviceHeader.ApplicationUserName;

                    _inHouseChequeRepository.Add(inHouseCheque, serviceHeader);

                    if (inHouseChequeDTO.ChequeTypeId != null && inHouseChequeDTO.ChequeTypeId != Guid.Empty)
                    {
                        switch ((InHouseChequeFunding)inHouseCheque.Funding)
                        {
                            case InHouseChequeFunding.DebitCustomerAccount:

                                if (inHouseChequeDTO.DebitCustomerAccountId.HasValue && inHouseChequeDTO.DebitCustomerAccountId.Value != Guid.Empty)
                                {
                                    var customerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(inHouseChequeDTO.DebitCustomerAccountId.Value, serviceHeader);

                                    var chequeWritingTariffs = _commissionAppService.ComputeTariffsByChequeType(inHouseChequeDTO.ChequeTypeId.Value, inHouseChequeDTO.Amount, customerAccountDTO, serviceHeader);

                                    _journalAppService.AddNewJournal(inHouseChequeDTO.BranchId, null, inHouseChequeDTO.Amount, "Cheque Writing", inHouseChequeDTO.Payee, inHouseChequeDTO.Reference, moduleNavigationItemCode, (int)SystemTransactionCode.InHouseCheque, null, chequesSuspenseChartOfAccountId, inHouseChequeDTO.DebitChartOfAccountId, customerAccountDTO, customerAccountDTO, chequeWritingTariffs, serviceHeader);
                                }

                                break;
                            case InHouseChequeFunding.DebitGeneralLedgerAccount:

                                if (inHouseChequeDTO.Chargeable)
                                {
                                    var chequeWritingTariffs = _commissionAppService.ComputeTariffsByChequeType(inHouseChequeDTO.ChequeTypeId.Value, inHouseChequeDTO.Amount, inHouseChequeDTO.DebitChartOfAccountId, inHouseChequeDTO.DebitChartOfAccountAccountCode, inHouseChequeDTO.DebitChartOfAccountAccountName, serviceHeader);

                                    _journalAppService.AddNewJournal(inHouseChequeDTO.BranchId, null, inHouseChequeDTO.Amount, "Cheque Writing", inHouseChequeDTO.Payee, inHouseChequeDTO.Reference, moduleNavigationItemCode, (int)SystemTransactionCode.InHouseCheque, null, chequesSuspenseChartOfAccountId, inHouseChequeDTO.DebitChartOfAccountId, chequeWritingTariffs, serviceHeader);
                                }
                                else _journalAppService.AddNewJournal(inHouseChequeDTO.BranchId, null, inHouseChequeDTO.Amount, "Cheque Writing", inHouseChequeDTO.Payee, inHouseChequeDTO.Reference, moduleNavigationItemCode, (int)SystemTransactionCode.InHouseCheque, null, chequesSuspenseChartOfAccountId, inHouseChequeDTO.DebitChartOfAccountId, serviceHeader);

                                break;
                            default:
                                break;
                        }
                    }

                    dbContextScope.SaveChanges(serviceHeader);

                    return inHouseCheque.ProjectedAs<InHouseChequeDTO>();
                }
            }
            else throw new InvalidOperationException("Sorry, but the in-house cheques control account has not been setup!");
        }

        public bool AddNewInHouseCheques(List<InHouseChequeDTO> inHouseChequeDTOs, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (inHouseChequeDTOs != null && inHouseChequeDTOs.Any())
            {
                var resultList = new List<bool>();

                inHouseChequeDTOs.ForEach(inHouseChequeDTO =>
                {
                    inHouseChequeDTO = AddNewInHouseCheque(inHouseChequeDTO, moduleNavigationItemCode, serviceHeader);

                    if (inHouseChequeDTO != null)
                        resultList.Add(true);
                    else resultList.Add(false);
                });

                result = !resultList.Any(x => x == false);
            }

            return result;
        }

        public List<InHouseChequeDTO> FindInHouseCheques(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var inHouseCheques = _inHouseChequeRepository.GetAll(serviceHeader);

                if (inHouseCheques != null && inHouseCheques.Any())
                {
                    return inHouseCheques.ProjectedAsCollection<InHouseChequeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<InHouseChequeDTO> FindInHouseCheques(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InHouseChequeSpecifications.DefaultSpec();

                ISpecification<InHouseCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var inHouseChequePagedCollection = _inHouseChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (inHouseChequePagedCollection != null)
                {
                    var pageCollection = inHouseChequePagedCollection.PageCollection.ProjectedAsCollection<InHouseChequeDTO>();

                    var itemsCount = inHouseChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<InHouseChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<InHouseChequeDTO> FindInHouseCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InHouseChequeSpecifications.InHouseChequeFullText(text);

                ISpecification<InHouseCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var inHouseChequePagedCollection = _inHouseChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (inHouseChequePagedCollection != null)
                {
                    var pageCollection = inHouseChequePagedCollection.PageCollection.ProjectedAsCollection<InHouseChequeDTO>();

                    var itemsCount = inHouseChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<InHouseChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<InHouseChequeDTO> FindInHouseCheques(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InHouseChequeSpecifications.InHouseChequeFullText(startDate, endDate, text);

                ISpecification<InHouseCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var inHouseChequePagedCollection = _inHouseChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (inHouseChequePagedCollection != null)
                {
                    var pageCollection = inHouseChequePagedCollection.PageCollection.ProjectedAsCollection<InHouseChequeDTO>();

                    var itemsCount = inHouseChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<InHouseChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public InHouseChequeDTO FindInHouseCheque(Guid inHouseChequeId, ServiceHeader serviceHeader)
        {
            if (inHouseChequeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var inHouseCheque = _inHouseChequeRepository.Get(inHouseChequeId, serviceHeader);

                    if (inHouseCheque != null)
                    {
                        return inHouseCheque.ProjectedAs<InHouseChequeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<InHouseChequeDTO> FindUnPrintedInHouseChequesByBranchId(Guid branchId, string text, ServiceHeader serviceHeader)
        {
            if (branchId != null && branchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = InHouseChequeSpecifications.UnPrintedInHouseChequesWithBranchId(branchId, text);

                    ISpecification<InHouseCheque> spec = filter;

                    var inHouseCheques = _inHouseChequeRepository.AllMatching(spec, serviceHeader);

                    if (inHouseCheques != null && inHouseCheques.Any())
                    {
                        return inHouseCheques.ProjectedAsCollection<InHouseChequeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<InHouseChequeDTO> FindUnPrintedInHouseChequesByBranchId(Guid branchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InHouseChequeSpecifications.UnPrintedInHouseChequesWithBranchId(branchId, text);

                ISpecification<InHouseCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var inHouseChequePagedCollection = _inHouseChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (inHouseChequePagedCollection != null)
                {
                    var pageCollection = inHouseChequePagedCollection.PageCollection.ProjectedAsCollection<InHouseChequeDTO>();

                    var itemsCount = inHouseChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<InHouseChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool PrintInHouseCheque(InHouseChequeDTO inHouseChequeDTO, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (inHouseChequeDTO != null)
            {
                var chequesSuspenseChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InHouseChequesControl, serviceHeader);

                if (bankLinkageDTO != null && chequesSuspenseChartOfAccountId != Guid.Empty)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        var persisted = _inHouseChequeRepository.Get(inHouseChequeDTO.Id, serviceHeader);

                        if (persisted != null && !persisted.IsPrinted)
                        {
                            persisted.IsPrinted = true;

                            persisted.PrintedNumber = inHouseChequeDTO.PrintedNumber;

                            persisted.PrintedBy = serviceHeader.ApplicationUserName;

                            persisted.PrintedDate = DateTime.Now;

                            _journalAppService.AddNewJournal(bankLinkageDTO.BranchId, null, inHouseChequeDTO.Amount, string.Format("In-House Cheque #{0}", inHouseChequeDTO.PrintedNumber), bankLinkageDTO.BankBranchName, "Printing", moduleNavigationItemCode, (int)SystemTransactionCode.InHouseCheque, null, bankLinkageDTO.ChartOfAccountId, chequesSuspenseChartOfAccountId, serviceHeader);
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else throw new InvalidOperationException("Sorry, but the requisite bank-linkage and/or in-house cheques control account has not been setup!");
            }
            else return false;
        }
    }
}
