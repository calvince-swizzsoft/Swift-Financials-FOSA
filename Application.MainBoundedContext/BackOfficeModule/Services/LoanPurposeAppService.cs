using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class LoanPurposeAppService : ILoanPurposeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LoanPurpose> _loanPurposeRepository;

        public LoanPurposeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LoanPurpose> loanPurposeRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (loanPurposeRepository == null)
                throw new ArgumentNullException(nameof(loanPurposeRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _loanPurposeRepository = loanPurposeRepository;
        }

        public LoanPurposeDTO AddNewLoanPurpose(LoanPurposeDTO loanPurposeDTO, ServiceHeader serviceHeader)
        {
            if (loanPurposeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<LoanPurpose> spec = LoanPurposeSpecifications.LoanPurposeDescription(loanPurposeDTO.Description);

                    var matchedLoanPurpose = _loanPurposeRepository.AllMatching(spec, serviceHeader);

                    if (matchedLoanPurpose != null && matchedLoanPurpose.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        loanPurposeDTO.ErrorMessageResult = string.Format("Loan Purpose \"{0}\" already exists!", loanPurposeDTO.Description.ToUpper());
                        return loanPurposeDTO;
                    }
                    else
                    {
                        var loanPurpose = LoanPurposeFactory.CreateLoanPurpose(loanPurposeDTO.Description);

                        if (loanPurposeDTO.IsLocked)
                            loanPurpose.Lock();
                        else loanPurpose.UnLock();

                        _loanPurposeRepository.Add(loanPurpose, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return loanPurpose.ProjectedAs<LoanPurposeDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateLoanPurpose(LoanPurposeDTO loanPurposeDTO, ServiceHeader serviceHeader)
        {
            if (loanPurposeDTO == null || loanPurposeDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanPurposeRepository.Get(loanPurposeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = LoanPurposeFactory.CreateLoanPurpose(loanPurposeDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    
                    if (loanPurposeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _loanPurposeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<LoanPurposeDTO> FindLoanPurposes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var loanPurposes = _loanPurposeRepository.GetAll(serviceHeader);

                if (loanPurposes != null && loanPurposes.Any())
                {
                    return loanPurposes.ProjectedAsCollection<LoanPurposeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanPurposeDTO> FindLoanPurposes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanPurposeSpecifications.DefaultSpec();

                ISpecification<LoanPurpose> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanPurposePagedCollection = _loanPurposeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanPurposePagedCollection != null)
                {
                    var pageCollection = loanPurposePagedCollection.PageCollection.ProjectedAsCollection<LoanPurposeDTO>();

                    var itemsCount = loanPurposePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanPurposeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanPurposeDTO> FindLoanPurposes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanPurposeSpecifications.LoanPurposeFullText(text);

                ISpecification<LoanPurpose> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanPurposeCollection = _loanPurposeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanPurposeCollection != null)
                {
                    var pageCollection = loanPurposeCollection.PageCollection.ProjectedAsCollection<LoanPurposeDTO>();

                    var itemsCount = loanPurposeCollection.ItemsCount;

                    return new PageCollectionInfo<LoanPurposeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LoanPurposeDTO FindLoanPurpose(Guid loanPurposeId, ServiceHeader serviceHeader)
        {
            if (loanPurposeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanPurpose = _loanPurposeRepository.Get(loanPurposeId, serviceHeader);

                    if (loanPurpose != null)
                    {
                        return loanPurpose.ProjectedAs<LoanPurposeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
