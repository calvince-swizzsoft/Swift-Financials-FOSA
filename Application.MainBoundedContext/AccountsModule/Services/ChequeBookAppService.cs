using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ChequeBookAppService : IChequeBookAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ChequeBook> _chequeBookRepository;
        private readonly IRepository<PaymentVoucher> _paymentVoucherRepository;
        private readonly IJournalAppService _journalAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ICommissionAppService _commissionAppService;

        public ChequeBookAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ChequeBook> chequeBookRepository,
           IRepository<PaymentVoucher> paymentVoucherRepository,
           IJournalAppService journalAppService,
           ICustomerAccountAppService customerAccountAppService,
           ICommissionAppService commissionAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (chequeBookRepository == null)
                throw new ArgumentNullException(nameof(chequeBookRepository));

            if (paymentVoucherRepository == null)
                throw new ArgumentNullException(nameof(paymentVoucherRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _chequeBookRepository = chequeBookRepository;
            _paymentVoucherRepository = paymentVoucherRepository;
            _journalAppService = journalAppService;
            _customerAccountAppService = customerAccountAppService;
            _commissionAppService = commissionAppService;
        }

        public ChequeBookDTO AddNewChequeBook(ChequeBookDTO chequeBookDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (chequeBookDTO != null)
            {
                var lastVoucherNumber = (chequeBookDTO.InitialVoucherNumber + chequeBookDTO.NumberOfVouchers) - 1;

                if (lastVoucherNumber > 0)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        var chequeBook = ChequeBookFactory.CreateChequeBook(chequeBookDTO.CustomerAccountId, chequeBookDTO.Type, chequeBookDTO.NumberOfVouchers, chequeBookDTO.InitialVoucherNumber, chequeBookDTO.Reference, chequeBookDTO.Remarks);

                        chequeBook.SerialNumber = _chequeBookRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(SerialNumber),0) + 1 AS Expr1 FROM {0}ChequeBooks", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                        chequeBook.CreatedBy = serviceHeader.ApplicationUserName;

                        _chequeBookRepository.Add(chequeBook, serviceHeader);

                        for (int i = chequeBook.InitialVoucherNumber; i <= lastVoucherNumber; i++)
                        {
                            var paymentVoucher = PaymentVoucherFactory.CreatePaymentVoucher(chequeBook.Id, i);

                            paymentVoucher.CreatedBy = serviceHeader.ApplicationUserName;

                            _paymentVoucherRepository.Add(paymentVoucher, serviceHeader);
                        }

                        var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(chequeBookDTO.CustomerAccountId, serviceHeader);

                        var chequeBookTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(customerAccount.CustomerAccountTypeTargetProductId, (int)SavingsProductKnownChargeType.ChequeBookCharges, 0m, customerAccount, serviceHeader);

                        if (chequeBookTariffs != null && chequeBookTariffs.Any())
                        {
                            chequeBookTariffs.ForEach(tariff =>
                            {
                                _journalAppService.AddNewJournal(customerAccount.BranchId, null, tariff.Amount, tariff.Description, "Cheque Book", string.Format("{0}", chequeBook.SerialNumber).PadLeft(6, '0'), moduleNavigationItemCode, 0, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccount, customerAccount, serviceHeader);
                            });
                        }

                        dbContextScope.SaveChanges(serviceHeader);

                        return chequeBook.ProjectedAs<ChequeBookDTO>();
                    }
                }
                else return null;
            }
            else return null;
        }

        public bool UpdateChequeBook(ChequeBookDTO chequeBookDTO, ServiceHeader serviceHeader)
        {
            if (chequeBookDTO == null || chequeBookDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _chequeBookRepository.Get(chequeBookDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ChequeBookFactory.CreateChequeBook(chequeBookDTO.CustomerAccountId, chequeBookDTO.Type, chequeBookDTO.NumberOfVouchers, chequeBookDTO.InitialVoucherNumber, chequeBookDTO.Reference, chequeBookDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.SerialNumber = persisted.SerialNumber;
                    current.InitialVoucherNumber = persisted.InitialVoucherNumber;
                    current.NumberOfVouchers = persisted.NumberOfVouchers;
                    current.CreatedBy = persisted.CreatedBy;
                    

                    if (chequeBookDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _chequeBookRepository.Merge(persisted, current, serviceHeader);

                    // Activate?
                    if (chequeBookDTO.IsActive && !persisted.IsActive)
                        ActivateChequeBook(persisted.Id, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool PayVoucher(PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader)
        {
            if (paymentVoucherDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _paymentVoucherRepository.Get(paymentVoucherDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status != (int)PaymentVoucherStatus.Paid)
                    {
                        persisted.Status = (int)PaymentVoucherStatus.Paid;
                        persisted.Payee = paymentVoucherDTO.Payee;
                        persisted.Amount = paymentVoucherDTO.Amount;
                        persisted.WriteDate = paymentVoucherDTO.WriteDate;
                        persisted.Reference = paymentVoucherDTO.Reference;
                        persisted.PaidBy = serviceHeader.ApplicationUserName;
                        persisted.PaidDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool FlagVoucher(PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader)
        {
            if (paymentVoucherDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _paymentVoucherRepository.Get(paymentVoucherDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status != (int)PaymentVoucherStatus.Paid)
                    {
                        switch ((PaymentVoucherManagementAction)paymentVoucherDTO.ManagementAction)
                        {
                            case PaymentVoucherManagementAction.Flag:
                                persisted.Status = (int)PaymentVoucherStatus.Flagged;
                                break;
                            case PaymentVoucherManagementAction.Unflag:
                                persisted.Status = (int)PaymentVoucherStatus.Active;
                                break;
                            default:
                                break;
                        }

                        persisted.Reference = paymentVoucherDTO.Reference;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<ChequeBookDTO> FindChequeBooks(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var chequeBookes = _chequeBookRepository.GetAll(serviceHeader);

                if (chequeBookes != null && chequeBookes.Any())
                {
                    return chequeBookes.ProjectedAsCollection<ChequeBookDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ChequeBookDTO> FindChequeBooks(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChequeBookSpecifications.ChequeBookFullText(text);

                ISpecification<ChequeBook> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var chequeBookPagedCollection = _chequeBookRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (chequeBookPagedCollection != null)
                {
                    var pageCollection = chequeBookPagedCollection.PageCollection.ProjectedAsCollection<ChequeBookDTO>();

                    var itemsCount = chequeBookPagedCollection.ItemsCount;

                    return new PageCollectionInfo<ChequeBookDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ChequeBookDTO> FindChequeBooks(int type, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChequeBookSpecifications.ChequeBookFullText(text, type);

                ISpecification<ChequeBook> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var chequeBookPagedCollection = _chequeBookRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (chequeBookPagedCollection != null)
                {
                    var pageCollection = chequeBookPagedCollection.PageCollection.ProjectedAsCollection<ChequeBookDTO>();

                    var itemsCount = chequeBookPagedCollection.ItemsCount;

                    return new PageCollectionInfo<ChequeBookDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public ChequeBookDTO FindChequeBook(Guid chequeBookId, ServiceHeader serviceHeader)
        {
            if (chequeBookId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var chequeBook = _chequeBookRepository.Get(chequeBookId, serviceHeader);

                    if (chequeBook != null)
                    {
                        return chequeBook.ProjectedAs<ChequeBookDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<PaymentVoucherDTO> FindPaymentVouchersByChequeBookId(Guid chequeBookId, ServiceHeader serviceHeader)
        {
            if (chequeBookId != null && chequeBookId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = PaymentVoucherSpecifications.PaymentVoucherWithChequeBookId(chequeBookId, string.Empty);

                    ISpecification<PaymentVoucher> spec = filter;

                    var chequeBookEntries = _paymentVoucherRepository.AllMatching(spec, serviceHeader);

                    if (chequeBookEntries != null && chequeBookEntries.Any())
                    {
                        return chequeBookEntries.ProjectedAsCollection<PaymentVoucherDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<PaymentVoucherDTO> FindPaymentVouchersByVoucherNumberAndChequeBookReference(int chequeBookType, int voucherNumber, string chequeBookReference, ServiceHeader serviceHeader)
        {
            if (!string.IsNullOrWhiteSpace(chequeBookReference))
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = PaymentVoucherSpecifications.PaymentVoucherWithVoucherNumberAndChequeBookReference(chequeBookType, voucherNumber, chequeBookReference);

                    ISpecification<PaymentVoucher> spec = filter;

                    var chequeBookEntries = _paymentVoucherRepository.AllMatching(spec, serviceHeader);

                    if (chequeBookEntries != null && chequeBookEntries.Any())
                    {
                        return chequeBookEntries.ProjectedAsCollection<PaymentVoucherDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<PaymentVoucherDTO> FindPaymentVouchersByChequeBookId(Guid chequeBookId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (chequeBookId != null && chequeBookId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = PaymentVoucherSpecifications.PaymentVoucherWithChequeBookId(chequeBookId, text);

                    ISpecification<PaymentVoucher> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var chequeBookPagedCollection = _paymentVoucherRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (chequeBookPagedCollection != null)
                    {
                        var pageCollection = chequeBookPagedCollection.PageCollection.ProjectedAsCollection<PaymentVoucherDTO>();

                        var itemsCount = chequeBookPagedCollection.ItemsCount;

                        return new PageCollectionInfo<PaymentVoucherDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        private bool ActivateChequeBook(Guid chequeBookId, ServiceHeader serviceHeader)
        {
            if (chequeBookId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _chequeBookRepository.Get(chequeBookId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Activate();

                    var otherChequeBooks = _chequeBookRepository.AllMatching(ChequeBookSpecifications.ChequeBookWithCustomerAccountId(persisted.CustomerAccountId), serviceHeader);

                    foreach (var item in otherChequeBooks)
                    {
                        if (item.Id != persisted.Id)
                        {
                            var chequeBook = _chequeBookRepository.Get(item.Id, serviceHeader);

                            chequeBook.DeActivate();
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }
    }
}
