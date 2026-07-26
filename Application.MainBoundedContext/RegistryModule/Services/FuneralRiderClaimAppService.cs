using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FuneralRiderClaimPayable;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class FuneralRiderClaimAppService : IFuneralRiderClaimAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<FuneralRiderClaim> _funeralRiderClaimRepository;
        private readonly IRepository<FuneralRiderClaimPayable> _funeralRiderClaimPayableRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public FuneralRiderClaimAppService(IDbContextScopeFactory dbContextScopeFactory,
            IRepository<FuneralRiderClaim> funeralRiderClaimRepository,
            IRepository<FuneralRiderClaimPayable> funeralRiderClaimPayableRepository,
            IPostingPeriodAppService postingPeriodAppService,
            IJournalEntryPostingService journalEntryPostingService,
            IChartOfAccountAppService chartOfAccountAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (funeralRiderClaimRepository == null)
                throw new ArgumentNullException(nameof(funeralRiderClaimRepository));

            if (funeralRiderClaimPayableRepository == null)
                throw new ArgumentNullException(nameof(funeralRiderClaimPayableRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _funeralRiderClaimRepository = funeralRiderClaimRepository;
            _funeralRiderClaimPayableRepository = funeralRiderClaimPayableRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _chartOfAccountAppService = chartOfAccountAppService;
        }

        #region FuneralRiderClaimDTO
        public FuneralRiderClaimDTO AddNewFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO, ServiceHeader serviceHeader)
        {
            if (funeralRiderClaimDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    switch ((FuneralRiderClaimType)funeralRiderClaimDTO.ClaimType)
                    {
                        case FuneralRiderClaimType.MemberClaim:

                            var filter = FuneralRiderClaimSpecifications.FindFuneralRiderClaimWithCustomerIdAndType(funeralRiderClaimDTO.CustomerId, funeralRiderClaimDTO.ClaimType);

                            ISpecification<FuneralRiderClaim> spec = filter;

                            var result = _funeralRiderClaimRepository.AllMatching(spec, serviceHeader);

                            if (result.Any())
                            {
                                throw new InvalidOperationException(string.Format("Sorry, but the funeral rider claim of type '{0}'  already exists for the specified customer!", funeralRiderClaimDTO.ClaimTypeDescription));
                            }
                            break;

                        case FuneralRiderClaimType.SpouseClaim:

                            var filter1 = FuneralRiderClaimSpecifications.FindFuneralRiderClaimByCustomerIdTypeAndClaimantIdentity(funeralRiderClaimDTO.CustomerId, funeralRiderClaimDTO.ClaimType, funeralRiderClaimDTO.FuneralRiderClaimantIdentityCardNumber);

                            ISpecification<FuneralRiderClaim> spec1 = filter1;

                            var result1 = _funeralRiderClaimRepository.AllMatching(spec1, serviceHeader);

                            if (result1.Any())
                            {
                                throw new InvalidOperationException(string.Format("Sorry, but the funeral rider claim of type '{0}' already exists for the specified spouse!", funeralRiderClaimDTO.ClaimTypeDescription));
                            }
                            break;
                    }

                    var funeralRiderClaimant = new FuneralRiderClaimant(funeralRiderClaimDTO.FuneralRiderClaimantIdentityCardNumber, funeralRiderClaimDTO.FuneralRiderClaimantTscNumber, funeralRiderClaimDTO.FuneralRiderClaimantName, funeralRiderClaimDTO.FuneralRiderClaimantMobileNumber, funeralRiderClaimDTO.FuneralRiderClaimantRelationship, funeralRiderClaimDTO.FuneralRiderClaimantSignatureDate);

                    var immediateSuperior = new ImmediateSuperior(funeralRiderClaimDTO.ImmediateSuperiorIdentityCardNumber, funeralRiderClaimDTO.ImmediateSuperiorName, funeralRiderClaimDTO.ImmediateSuperiorSignatureDate);

                    var areaChief = new AreaChief(funeralRiderClaimDTO.AreaChiefIdentityCardNumber, funeralRiderClaimDTO.AreaChiefName, funeralRiderClaimDTO.AreaChiefSignatureDate);

                    var areaDelegate = new AreaDelegate(funeralRiderClaimDTO.AreaDelegateIdentityCardNumber, funeralRiderClaimDTO.AreaDelegateName, funeralRiderClaimDTO.AreaDelegateTscNumber, funeralRiderClaimDTO.AreaDelegateSignatureDate);

                    var areaBoardMember = new AreaBoardMember(funeralRiderClaimDTO.AreaBoardMemberIdentityCardNumber, funeralRiderClaimDTO.AreaBoardMemberName, funeralRiderClaimDTO.AreaBoardMemberTscNumber, funeralRiderClaimDTO.AreaBoardMemberSignatureDate);

                    var funeralRiderClaim = FuneralRiderClaimFactory.CreateFuneralRiderClaim(funeralRiderClaimDTO.CustomerId, funeralRiderClaimDTO.BranchId, funeralRiderClaimant, immediateSuperior, areaChief, areaDelegate, areaBoardMember, funeralRiderClaimDTO.Status, funeralRiderClaimDTO.ClaimType, funeralRiderClaimDTO.ClaimDate, funeralRiderClaimDTO.DateOfDeath, funeralRiderClaimDTO.FileName, funeralRiderClaimDTO.FileTitle, funeralRiderClaimDTO.FileDescription, funeralRiderClaimDTO.FileMIMEType, funeralRiderClaimDTO.Remarks);

                    funeralRiderClaim.CreatedBy = serviceHeader.ApplicationUserName;

                    _funeralRiderClaimRepository.Add(funeralRiderClaim, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return funeralRiderClaim.ProjectedAs<FuneralRiderClaimDTO>();
                }
            }
            else return null;
        }

        public bool UpdateFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO, ServiceHeader serviceHeader)
        {
            if (funeralRiderClaimDTO == null || funeralRiderClaimDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _funeralRiderClaimRepository.Get(funeralRiderClaimDTO.Id, serviceHeader);

                var funeralRiderClaimant = new FuneralRiderClaimant(funeralRiderClaimDTO.FuneralRiderClaimantIdentityCardNumber, funeralRiderClaimDTO.FuneralRiderClaimantTscNumber, funeralRiderClaimDTO.FuneralRiderClaimantName, funeralRiderClaimDTO.FuneralRiderClaimantMobileNumber, funeralRiderClaimDTO.FuneralRiderClaimantRelationship, funeralRiderClaimDTO.FuneralRiderClaimantSignatureDate);

                var immediateSuperior = new ImmediateSuperior(funeralRiderClaimDTO.ImmediateSuperiorIdentityCardNumber, funeralRiderClaimDTO.ImmediateSuperiorName, funeralRiderClaimDTO.ImmediateSuperiorSignatureDate);

                var areaChief = new AreaChief(funeralRiderClaimDTO.AreaChiefIdentityCardNumber, funeralRiderClaimDTO.AreaChiefName, funeralRiderClaimDTO.AreaChiefSignatureDate);

                var areaDelegate = new AreaDelegate(funeralRiderClaimDTO.AreaDelegateIdentityCardNumber, funeralRiderClaimDTO.AreaDelegateName, funeralRiderClaimDTO.AreaDelegateTscNumber, funeralRiderClaimDTO.AreaDelegateSignatureDate);

                var areaBoardMember = new AreaBoardMember(funeralRiderClaimDTO.AreaBoardMemberIdentityCardNumber, funeralRiderClaimDTO.AreaBoardMemberName, funeralRiderClaimDTO.AreaBoardMemberTscNumber, funeralRiderClaimDTO.AreaBoardMemberSignatureDate);

                if (persisted != null)
                {
                    var current = FuneralRiderClaimFactory.CreateFuneralRiderClaim(funeralRiderClaimDTO.CustomerId, funeralRiderClaimDTO.BranchId, funeralRiderClaimant, immediateSuperior, areaChief, areaDelegate, areaBoardMember, funeralRiderClaimDTO.Status, funeralRiderClaimDTO.ClaimType, funeralRiderClaimDTO.ClaimDate, funeralRiderClaimDTO.DateOfDeath, funeralRiderClaimDTO.FileName, funeralRiderClaimDTO.FileTitle, funeralRiderClaimDTO.FileDescription, funeralRiderClaimDTO.FileMIMEType, funeralRiderClaimDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _funeralRiderClaimRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaims(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FuneralRiderClaimSpecifications.FuneralRiderClaimByStatusWithDateRange(status, text, startDate, endDate);

                ISpecification<FuneralRiderClaim> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var funeralRiderClaimPagedCollection = _funeralRiderClaimRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (funeralRiderClaimPagedCollection != null)
                {
                    var pageCollection = funeralRiderClaimPagedCollection.PageCollection.ProjectedAsCollection<FuneralRiderClaimDTO>();

                    var itemsCount = funeralRiderClaimPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FuneralRiderClaimDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaims(int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FuneralRiderClaimSpecifications.FuneralRiderClaimByStatusWithFilter(status, text);

                ISpecification<FuneralRiderClaim> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var funeralRiderClaimPagedCollection = _funeralRiderClaimRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (funeralRiderClaimPagedCollection != null)
                {
                    var pageCollection = funeralRiderClaimPagedCollection.PageCollection.ProjectedAsCollection<FuneralRiderClaimDTO>();

                    var itemsCount = funeralRiderClaimPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FuneralRiderClaimDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<FuneralRiderClaimDTO> FindFuneralRiderClaims(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var funeralRiderClaims = _funeralRiderClaimRepository.GetAll(serviceHeader);

                if (funeralRiderClaims != null && funeralRiderClaims.Any())
                {
                    return funeralRiderClaims.ProjectedAsCollection<FuneralRiderClaimDTO>();
                }
                else return null;
            }
        }

        public List<FuneralRiderClaimDTO> FindFuneralRiderClaimsByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FuneralRiderClaimSpecifications.FindFuneralRiderClaimWithCustomerId(customerId);

                    ISpecification<FuneralRiderClaim> spec = filter;

                    var funeralRiderClaims = _funeralRiderClaimRepository.AllMatching(spec, serviceHeader);

                    if (funeralRiderClaims != null)
                    {
                        return funeralRiderClaims.ProjectedAsCollection<FuneralRiderClaimDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public FuneralRiderClaimDTO FindFuneralRiderClaim(Guid funeralRiderClaimId, ServiceHeader serviceHeader)
        {
            if (funeralRiderClaimId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var funeralRiderClaim = _funeralRiderClaimRepository.Get(funeralRiderClaimId, serviceHeader);

                    if (funeralRiderClaim != null)
                    {
                        return funeralRiderClaim.ProjectedAs<FuneralRiderClaimDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaims(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FuneralRiderClaimSpecifications.FuneralRiderClaimFullText(text);

                ISpecification<FuneralRiderClaim> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var funeralRiderClaimCollection = _funeralRiderClaimRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (funeralRiderClaimCollection != null)
                {
                    var pageCollection = funeralRiderClaimCollection.PageCollection.ProjectedAsCollection<FuneralRiderClaimDTO>();

                    var itemsCount = funeralRiderClaimCollection.ItemsCount;

                    return new PageCollectionInfo<FuneralRiderClaimDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        #endregion

        #region FuneralRiderClaimPayableDTO

        public FuneralRiderClaimPayableDTO AddNewFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, ServiceHeader serviceHeader)
        {
            if (funeralRiderClaimPayableDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var funeralRiderClaimPayable = FuneralRiderClaimPayableFactory.CreateFuneralRiderClaimPayable(funeralRiderClaimPayableDTO.FuneralRiderClaimId, funeralRiderClaimPayableDTO.BranchId, funeralRiderClaimPayableDTO.Amount, funeralRiderClaimPayableDTO.Remarks);

                    funeralRiderClaimPayable.ReferenceNumber = _funeralRiderClaimPayableRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(ReferenceNumber),0) + 1 AS Expr1 FROM {0}FuneralRiderClaimPayables", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    funeralRiderClaimPayable.RecordStatus = (byte)OriginationVerificationAuthorizationStatus.Pending;
                    funeralRiderClaimPayable.PaymentStatus = (byte)FuneralRiderClaimPaymentStatus.Unpaid;

                    funeralRiderClaimPayable.CreatedBy = serviceHeader.ApplicationUserName;

                    _funeralRiderClaimPayableRepository.Add(funeralRiderClaimPayable, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return funeralRiderClaimPayable.ProjectedAs<FuneralRiderClaimPayableDTO>();
                }
            }
            else return null;
        }

        public bool UpdateFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, ServiceHeader serviceHeader)
        {
            if (funeralRiderClaimPayableDTO == null || funeralRiderClaimPayableDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _funeralRiderClaimPayableRepository.Get(funeralRiderClaimPayableDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = FuneralRiderClaimPayableFactory.CreateFuneralRiderClaimPayable(funeralRiderClaimPayableDTO.FuneralRiderClaimId, funeralRiderClaimPayableDTO.BranchId, funeralRiderClaimPayableDTO.Amount, funeralRiderClaimPayableDTO.Remarks);

                    current.ReferenceNumber = persisted.ReferenceNumber;

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _funeralRiderClaimPayableRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool AuditFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int verificationOption, ServiceHeader serviceHeader)
        {
            if (funeralRiderClaimPayableDTO == null || !Enum.IsDefined(typeof(VerificationOption), verificationOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _funeralRiderClaimPayableRepository.Get(funeralRiderClaimPayableDTO.Id, serviceHeader);

                if (persisted == null || persisted.RecordStatus != (int)OriginationVerificationAuthorizationStatus.Pending)
                    return false;

                switch ((VerificationOption)verificationOption)
                {
                    case VerificationOption.Verified:

                        persisted.RecordStatus = (int)OriginationVerificationAuthorizationStatus.Verified;
                        persisted.AuditRemarks = funeralRiderClaimPayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case VerificationOption.Rejected:

                        persisted.RecordStatus = (int)OriginationVerificationAuthorizationStatus.Rejected;
                        persisted.AuditRemarks = funeralRiderClaimPayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int authorizationOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (funeralRiderClaimPayableDTO == null || !Enum.IsDefined(typeof(AuthorizationOption), authorizationOption))
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _funeralRiderClaimPayableRepository.Get(funeralRiderClaimPayableDTO.Id, serviceHeader);

                if (persisted == null || persisted.RecordStatus != (int)OriginationVerificationAuthorizationStatus.Verified)
                    return result;

                switch ((AuthorizationOption)authorizationOption)
                {
                    case AuthorizationOption.Posted:

                        persisted.RecordStatus = (int)OriginationVerificationAuthorizationStatus.Posted;
                        persisted.AuthorizationRemarks = funeralRiderClaimPayableDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;

                    case AuthorizationOption.Rejected:

                        persisted.RecordStatus = (int)OriginationVerificationAuthorizationStatus.Rejected;
                        persisted.AuthorizationRemarks = funeralRiderClaimPayableDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;

                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            if (result)
            {
                var funeralRiderClaim = FindFuneralRiderClaim(funeralRiderClaimPayableDTO.FuneralRiderClaimId, serviceHeader);

                switch ((AuthorizationOption)authorizationOption)
                {
                    case AuthorizationOption.Posted:
                        funeralRiderClaim.Status = (int)OriginationVerificationAuthorizationStatus.Posted;
                        break;

                    case AuthorizationOption.Rejected:
                        funeralRiderClaim.Status = (int)OriginationVerificationAuthorizationStatus.Rejected;
                        break;
                }

                result = UpdateFuneralRiderClaim(funeralRiderClaim, serviceHeader);
            }

            return result;
        }

        public PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayables(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FuneralRiderClaimPayableSpecifications.FuneralRiderClaimPayableWithStatusAndDateRange(recordStatus, text, startDate, endDate);

                ISpecification<FuneralRiderClaimPayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var funeralRiderClaimPayablePagedCollection = _funeralRiderClaimPayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (funeralRiderClaimPayablePagedCollection != null)
                {
                    var pageCollection = funeralRiderClaimPayablePagedCollection.PageCollection.ProjectedAsCollection<FuneralRiderClaimPayableDTO>();

                    var itemsCount = funeralRiderClaimPayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<FuneralRiderClaimPayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayables(int recordStatus, int paymentStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FuneralRiderClaimPayableSpecifications.FuneralRiderClaimPayableWithRecordStatusPaymentStatusAndDateRange(recordStatus, paymentStatus, text, startDate, endDate);

                ISpecification<FuneralRiderClaimPayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var funeralRiderClaimPayablePagedCollection = _funeralRiderClaimPayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (funeralRiderClaimPayablePagedCollection != null)
                {
                    var pageCollection = funeralRiderClaimPayablePagedCollection.PageCollection.ProjectedAsCollection<FuneralRiderClaimPayableDTO>();

                    var itemsCount = funeralRiderClaimPayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<FuneralRiderClaimPayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayables(int recordStatus, int paymentStatus, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FuneralRiderClaimPayableSpecifications.FuneralRiderClaimWithRecordStatusPaymentStatusAndFilter(recordStatus, paymentStatus, text);

                ISpecification<FuneralRiderClaimPayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var funeralRiderClaimPayablePagedCollection = _funeralRiderClaimPayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (funeralRiderClaimPayablePagedCollection != null)
                {
                    var pageCollection = funeralRiderClaimPayablePagedCollection.PageCollection.ProjectedAsCollection<FuneralRiderClaimPayableDTO>();

                    var itemsCount = funeralRiderClaimPayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<FuneralRiderClaimPayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool PostFuneralRiderClaimPayable(Guid funeralRiderClaimPayableId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (funeralRiderClaimPayableId == null || funeralRiderClaimPayableId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _funeralRiderClaimPayableRepository.Get(funeralRiderClaimPayableId, serviceHeader);

                if (persisted != null && persisted.RecordStatus == (int)OriginationVerificationAuthorizationStatus.Posted)
                {
                    switch ((FuneralRiderClaimPaymentStatus)persisted.PaymentStatus)
                    {
                        case FuneralRiderClaimPaymentStatus.Unpaid:
                            persisted.PaymentStatus = (int)FuneralRiderClaimPaymentStatus.Paid;
                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        public async Task<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayableAsync(Guid funeralRiderClaimPayableId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _funeralRiderClaimPayableRepository.GetAsync<FuneralRiderClaimPayableDTO>(funeralRiderClaimPayableId, serviceHeader);
            }
        }

        #endregion
    }
}
