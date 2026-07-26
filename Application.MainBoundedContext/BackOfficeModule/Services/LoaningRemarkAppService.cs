using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoaningRemarkAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class LoaningRemarkAppService : ILoaningRemarkAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LoaningRemark> _loaningRemarkRepository;

        public LoaningRemarkAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LoaningRemark> loaningRemarkRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (loaningRemarkRepository == null)
                throw new ArgumentNullException(nameof(loaningRemarkRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _loaningRemarkRepository = loaningRemarkRepository;
        }

        public LoaningRemarkDTO AddNewLoaningRemark(LoaningRemarkDTO loaningRemarkDTO, ServiceHeader serviceHeader)
        {
            if (loaningRemarkDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<LoaningRemark> spec = LoaningRemarkSpecifications.LoaningRemarkDescription(loaningRemarkDTO.Description);

                    var matchedLoaningRemark = _loaningRemarkRepository.AllMatching(spec, serviceHeader);

                    if (matchedLoaningRemark != null && matchedLoaningRemark.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        loaningRemarkDTO.ErrorMessageResult = string.Format("Loaning Remark \"{0}\" already exists!", loaningRemarkDTO.Description.ToUpper());
                        return loaningRemarkDTO;
                    }
                    else
                    {
                        var loaningRemark = LoaningRemarkFactory.CreateLoaningRemark(loaningRemarkDTO.Description);

                        if (loaningRemarkDTO.IsLocked)
                            loaningRemark.Lock();
                        else loaningRemark.UnLock();

                        _loaningRemarkRepository.Add(loaningRemark, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return loaningRemark.ProjectedAs<LoaningRemarkDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateLoaningRemark(LoaningRemarkDTO loaningRemarkDTO, ServiceHeader serviceHeader)
        {
            if (loaningRemarkDTO == null || loaningRemarkDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loaningRemarkRepository.Get(loaningRemarkDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = LoaningRemarkFactory.CreateLoaningRemark(loaningRemarkDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (loaningRemarkDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _loaningRemarkRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<LoaningRemarkDTO> FindLoaningRemarks(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var loaningRemarks = _loaningRemarkRepository.GetAll(serviceHeader);

                if (loaningRemarks != null && loaningRemarks.Any())
                {
                    return loaningRemarks.ProjectedAsCollection<LoaningRemarkDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarks(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoaningRemarkSpecifications.DefaultSpec();

                ISpecification<LoaningRemark> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loaningRemarkPagedCollection = _loaningRemarkRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loaningRemarkPagedCollection != null)
                {
                    var pageCollection = loaningRemarkPagedCollection.PageCollection.ProjectedAsCollection<LoaningRemarkDTO>();

                    var itemsCount = loaningRemarkPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoaningRemarkDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarks(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoaningRemarkSpecifications.LoaningRemarkFullText(text);

                ISpecification<LoaningRemark> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loaningRemarkCollection = _loaningRemarkRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loaningRemarkCollection != null)
                {
                    var pageCollection = loaningRemarkCollection.PageCollection.ProjectedAsCollection<LoaningRemarkDTO>();

                    var itemsCount = loaningRemarkCollection.ItemsCount;

                    return new PageCollectionInfo<LoaningRemarkDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LoaningRemarkDTO FindLoaningRemark(Guid loaningRemarkId, ServiceHeader serviceHeader)
        {
            if (loaningRemarkId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loaningRemark = _loaningRemarkRepository.Get(loaningRemarkId, serviceHeader);

                    if (loaningRemark != null)
                    {
                        return loaningRemark.ProjectedAs<LoaningRemarkDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
