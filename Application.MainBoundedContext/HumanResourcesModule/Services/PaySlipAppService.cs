using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class PaySlipAppService : IPaySlipAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<PaySlip> _paySlipRepository;
        private readonly IRepository<PaySlipEntry> _paySlipEntryRepository;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public PaySlipAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<PaySlip> paySlipRepository,
           IRepository<PaySlipEntry> paySlipEntryRepository,
           ILoanProductAppService loanProductAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (paySlipRepository == null)
                throw new ArgumentNullException(nameof(paySlipRepository));

            if (paySlipEntryRepository == null)
                throw new ArgumentNullException(nameof(paySlipEntryRepository));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _paySlipRepository = paySlipRepository;
            _paySlipEntryRepository = paySlipEntryRepository;
            _loanProductAppService = loanProductAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public bool PurgePaySlips(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (salaryPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var filter = PaySlipSpecifications.PaySlipWithSalaryPeriodId(salaryPeriodDTO.Id);

                    ISpecification<PaySlip> spec = filter;

                    var persistedPaySlips = _paySlipRepository.AllMatching(spec, serviceHeader);

                    if (persistedPaySlips != null && persistedPaySlips.Any())
                    {
                        persistedPaySlips.ToList().ForEach(paySlip =>
                        {
                            paySlip.PaySlipEntries.ToList().ForEach(paySlipEntry =>
                            {
                                _paySlipEntryRepository.Remove(paySlipEntry, serviceHeader);
                            });

                            _paySlipRepository.Remove(paySlip, serviceHeader);
                        });
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return result;
        }

        public bool AddNewPaySlips(List<PaySlipDTO> paySlipDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (paySlipDTOs != null && paySlipDTOs.Any())
            {
                var paySlips = new List<PaySlip> { };

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    paySlipDTOs.ForEach(paySlipDTO =>
                    {
                        // get the specification
                        var filter = PaySlipSpecifications.PaySlipWithSalaryPeriodIdAndMonthAndEmployeeId(paySlipDTO.SalaryPeriodId, paySlipDTO.SalaryPeriodMonth, paySlipDTO.SalaryCardId);

                        ISpecification<PaySlip> spec = filter;

                        //Query this criteria
                        var persistedPaySlips = _paySlipRepository.AllMatching(spec, serviceHeader);

                        if (persistedPaySlips != null && persistedPaySlips.Any())
                        {
                            persistedPaySlips.ToList().ForEach(paySlip =>
                            {
                                paySlip.PaySlipEntries.ToList().ForEach(paySlipEntry =>
                                {
                                    _paySlipEntryRepository.Remove(paySlipEntry, serviceHeader);
                                });

                                _paySlipRepository.Remove(paySlip, serviceHeader);
                            });
                        }

                        var newPaySlip = PaySlipFactory.CreatePaySlip(paySlipDTO.SalaryPeriodId, paySlipDTO.SalaryCardId, paySlipDTO.Remarks);

                        newPaySlip.Status = (int)PaySlipStatus.Pending;
                        newPaySlip.CreatedBy = serviceHeader.ApplicationUserName;

                        paySlips.Add(newPaySlip);

                        foreach (var paySlipEntryDTO in paySlipDTO.PaySlipEntries)
                        {
                            var charge = new Charge(paySlipEntryDTO.SalaryCardEntryChargeType, paySlipEntryDTO.SalaryCardEntryChargePercentage, paySlipEntryDTO.SalaryCardEntryChargeFixedAmount);

                            var newPaySlipEntry = PaySlipEntryFactory.CreatePaySlipEntry(newPaySlip.Id, paySlipEntryDTO.CustomerAccountId, paySlipEntryDTO.ChartOfAccountId, paySlipEntryDTO.Description, paySlipEntryDTO.SalaryHeadType, paySlipEntryDTO.SalaryHeadCategory, paySlipEntryDTO.Principal, paySlipEntryDTO.Interest, paySlipEntryDTO.RoundingType, charge);

                            newPaySlipEntry.CreatedBy = serviceHeader.ApplicationUserName;

                            newPaySlip.PaySlipEntries.Add(newPaySlipEntry);
                        }

                    });

                    dbContextScope.SaveChanges(serviceHeader);
                }

                #region Bulk-Insert pay slips && pay slip entries

                if (paySlips.Any())
                {
                    var paySlipEntries = new List<PaySlipEntry>();

                    paySlips.ForEach(p => paySlipEntries.AddRange(p.PaySlipEntries));

                    var bcpPaySlips = new List<PaySlipBulkCopyDTO>();

                    paySlips.ForEach(c =>
                    {
                        PaySlipBulkCopyDTO bcpc =
                            new PaySlipBulkCopyDTO
                            {
                                Id = c.Id,
                                SalaryPeriodId = c.SalaryPeriodId,
                                SalaryCardId = c.SalaryCardId,
                                Remarks = c.Remarks,
                                Status = c.Status,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate
                            };

                        bcpPaySlips.Add(bcpc);
                    });

                    var bcpPaySlipEntries = new List<PaySlipEntryBulkCopyDTO>();

                    paySlipEntries.ForEach(c =>
                    {
                        PaySlipEntryBulkCopyDTO bcpc =
                            new PaySlipEntryBulkCopyDTO
                            {
                                Id = c.Id,
                                PaySlipId = c.PaySlipId,
                                CustomerAccountId = c.CustomerAccountId,
                                ChartOfAccountId = c.ChartOfAccountId,
                                Description = c.Description,
                                SalaryHeadType = c.SalaryHeadType,
                                SalaryHeadCategory = c.SalaryHeadCategory,
                                Principal = c.Principal,
                                Interest = c.Interest,
                                RoundingType = c.RoundingType,
                                SalaryCardEntryCharge_Type = c.SalaryCardEntryCharge.Type,
                                SalaryCardEntryCharge_FixedAmount = c.SalaryCardEntryCharge.FixedAmount,
                                SalaryCardEntryCharge_Percentage = c.SalaryCardEntryCharge.Percentage,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate
                            };

                        bcpPaySlipEntries.Add(bcpc);
                    });

                    result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _paySlipRepository.Pluralize()), bcpPaySlips, serviceHeader);

                    result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _paySlipEntryRepository.Pluralize()), bcpPaySlipEntries, serviceHeader);
                }

                #endregion
            }

            return result;
        }

        public bool MarkPaySlipPosted(Guid paySlipId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (paySlipId == null || paySlipId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _paySlipRepository.Get(paySlipId, serviceHeader);

                if (persisted != null)
                {
                    switch ((PaySlipStatus)persisted.Status)
                    {
                        case PaySlipStatus.Pending:
                            persisted.Status = (int)PaySlipStatus.Posted;
                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        public List<PaySlipDTO> FindPaySlips(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var paySlips = _paySlipRepository.GetAll(serviceHeader);

                if (paySlips != null && paySlips.Any())
                {
                    return paySlips.ProjectedAsCollection<PaySlipDTO>();
                }
                else return null;
            }
        }

        public List<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PaySlipSpecifications.PaySlipWithSalaryPeriodId(salaryPeriodId);

                ISpecification<PaySlip> spec = filter;

                var paySlips = _paySlipRepository.AllMatching(spec, serviceHeader);

                if (paySlips != null && paySlips.Any())
                {
                    return paySlips.ProjectedAsCollection<PaySlipDTO>();
                }
                else return null;
            }
        }

        public int CountPaySlipsBySalaryPeriodId(Guid salaryPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PaySlipSpecifications.PaySlipWithSalaryPeriodId(salaryPeriodId);

                ISpecification<PaySlip> spec = filter;

                return _paySlipRepository.AllMatchingCount(spec, serviceHeader);
            }
        }

        public int CountPostedPaySlipsBySalaryPeriodId(Guid salaryPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PaySlipSpecifications.PostedPaySlipWithSalaryPeriodId(salaryPeriodId);

                ISpecification<PaySlip> spec = filter;

                return _paySlipRepository.AllMatchingCount(spec, serviceHeader);
            }
        }

        public PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PaySlipSpecifications.PaySlipWithSalaryPeriodId(salaryPeriodId);

                ISpecification<PaySlip> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var paySlipPagedCollection = _paySlipRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (paySlipPagedCollection != null)
                {
                    var pageCollection = paySlipPagedCollection.PageCollection.ProjectedAsCollection<PaySlipDTO>();

                    var itemsCount = paySlipPagedCollection.ItemsCount;

                    return new PageCollectionInfo<PaySlipDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PaySlipSpecifications.PaySlipFullText(salaryPeriodId, text);

                ISpecification<PaySlip> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var paySlipPagedCollection = _paySlipRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (paySlipPagedCollection != null)
                {
                    var pageCollection = paySlipPagedCollection.PageCollection.ProjectedAsCollection<PaySlipDTO>();

                    var itemsCount = paySlipPagedCollection.ItemsCount;

                    return new PageCollectionInfo<PaySlipDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<PaySlipDTO> FindQueablePaySlips(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PaySlipSpecifications.QueablePaySlips();

                ISpecification<PaySlip> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var paySlipPagedCollection = _paySlipRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (paySlipPagedCollection != null)
                {
                    var pageCollection = paySlipPagedCollection.PageCollection.ProjectedAsCollection<PaySlipDTO>();

                    var itemsCount = paySlipPagedCollection.ItemsCount;

                    return new PageCollectionInfo<PaySlipDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PaySlipDTO FindPaySlip(Guid paySlipId, ServiceHeader serviceHeader)
        {
            if (paySlipId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var paySlip = _paySlipRepository.Get(paySlipId, serviceHeader);

                    if (paySlip != null)
                    {
                        return paySlip.ProjectedAs<PaySlipDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PaySlipEntryDTO FindPaySlipEntry(Guid paySlipEntryId, ServiceHeader serviceHeader)
        {
            if (paySlipEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var paySlipEntry = _paySlipEntryRepository.Get(paySlipEntryId, serviceHeader);

                    if (paySlipEntry != null)
                    {
                        return paySlipEntry.ProjectedAs<PaySlipEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<PaySlipEntryDTO> FindPaySlipEntriesByPaySlipId(Guid paySlipId, ServiceHeader serviceHeader)
        {
            if (paySlipId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = PaySlipEntrySpecifications.PaySlipEntryWithPaySlipId(paySlipId);

                    ISpecification<PaySlipEntry> spec = filter;

                    var paySlipEntries = _paySlipEntryRepository.AllMatching(spec, serviceHeader);

                    if (paySlipEntries != null)
                    {
                        return paySlipEntries.OrderBy(x => x.SalaryHeadType).ProjectedAsCollection<PaySlipEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<PaySlipDTO> FindLoanAppraisalPaySlipsByCustomerId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            var loanProduct = _loanProductAppService.FindLoanProduct(loanProductId, serviceHeader);

            if (loanProduct != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var endDate = UberUtil.GetLastDayOfMonth(DateTime.Today.AddMonths(-1)); // last day of previous month

                    var startDate = endDate.AddMonths(loanProduct.LoanRegistrationConsecutiveIncome/*consecutive months*/ * -1);

                    var filter = PaySlipSpecifications.PaySlipWithDateRangeAndCustomerId(startDate, endDate, customerId);

                    ISpecification<PaySlip> spec = filter;

                    var paySlips = _paySlipRepository.AllMatching(spec, serviceHeader);

                    if (paySlips != null && paySlips.Any())
                    {
                        return paySlips.ProjectedAsCollection<PaySlipDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
