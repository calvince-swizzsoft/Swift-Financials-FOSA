using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountHistoryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class AlternateChannelAppService : IAlternateChannelAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AlternateChannel> _alternateChannelRepository;
        private readonly IRepository<CustomerAccountHistory> _customerAccountHistoryRepository;
        private readonly IRepository<AlternateChannelTypeCommission> _alternateChannelTypeCommissionRepository;
        private readonly IRepository<Commission> _commissionRepository;
        private readonly IAppCache _appCache;
        //private readonly IBrokerService _brokerService;

        public AlternateChannelAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<AlternateChannel> alternateChannelRepository,
            IRepository<CustomerAccountHistory> customerAccountHistoryRepository,
            IRepository<AlternateChannelTypeCommission> alternateChannelTypeCommissionRepository,
            IRepository<Commission> commissionRepository,
            IAppCache appCache
           // IBrokerService brokerService
            )
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (alternateChannelRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelRepository));

            if (customerAccountHistoryRepository == null)
                throw new ArgumentNullException(nameof(customerAccountHistoryRepository));

            if (alternateChannelTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelTypeCommissionRepository));

            if (commissionRepository == null)
                throw new ArgumentNullException(nameof(commissionRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            //if (brokerService == null)
            //    throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _alternateChannelRepository = alternateChannelRepository;
            _customerAccountHistoryRepository = customerAccountHistoryRepository;
            _alternateChannelTypeCommissionRepository = alternateChannelTypeCommissionRepository;
            _commissionRepository = commissionRepository;
            _appCache = appCache;
            //_brokerService = brokerService;
        }

        public AlternateChannelDTO AddNewAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (alternateChannelDTO != null && alternateChannelDTO.CustomerAccountId != Guid.Empty)
            {
                var duplicateAlternateChannels = FindAlternateChannelsByCardNumberAndCardType(alternateChannelDTO.CardNumber, alternateChannelDTO.Type, serviceHeader);

                if (duplicateAlternateChannels != null && duplicateAlternateChannels.Any())
                    alternateChannelDTO.ErrorMessageResult = string.Format("Sorry, but Primary Account Number {0} has already been assigned to an account!"+ alternateChannelDTO.CardNumber);
                
                switch ((AlternateChannelType)alternateChannelDTO.Type)
                {
                    case AlternateChannelType.SaccoLink:
                    case AlternateChannelType.Sparrow:
                    case AlternateChannelType.AgencyBanking:
                    case AlternateChannelType.AbcBank:

                        var existingAlternateChannels = FindAlternateChannelsByCustomerAccountIdAndType(alternateChannelDTO.CustomerAccountId, alternateChannelDTO.Type, serviceHeader);

                        if (existingAlternateChannels != null && existingAlternateChannels.Any())
                            alternateChannelDTO.ErrorMessageResult = (string.Format("Sorry, but card type {0} has already been assigned to the selected account!"+ EnumHelper.GetDescription((AlternateChannelType)alternateChannelDTO.Type)));

                        break;
                    case AlternateChannelType.MCoopCash:
                    case AlternateChannelType.Citius:
                    case AlternateChannelType.SpotCash:
                    case AlternateChannelType.PesaPepe:
                        break;
                    default:
                        break;
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var alternateChannel = AlternateChannelFactory.CreateAlternateChannel(alternateChannelDTO.CustomerAccountId, alternateChannelDTO.Type, alternateChannelDTO.CardNumber, alternateChannelDTO.ValidFrom, alternateChannelDTO.Expires, alternateChannelDTO.DailyLimit, alternateChannelDTO.RecruitedBy);

                    alternateChannel.Remarks = alternateChannelDTO.Remarks;
                    alternateChannel.CreatedBy = serviceHeader.ApplicationUserName;

                    _alternateChannelRepository.Add(alternateChannel, serviceHeader);

                    var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(alternateChannelDTO.CustomerAccountId, (int)AlternateChannelManagementAction.Linking, alternateChannelDTO.Remarks, string.Format("{0} {1}", alternateChannelDTO.TypeDescription, alternateChannelDTO.MaskedCardNumber), serviceHeader.ApplicationUserName);

                    _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        #region Do we need to send alerts?

                        //_brokerService.ProcessAlternateChannelLinkingAccountAlerts(DMLCommand.None, serviceHeader, alternateChannelDTO);

                        #endregion
                    }

                    return alternateChannel.ProjectedAs<AlternateChannelDTO>();
                }
            }

            else return null;
        }

        public bool UpdateAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader)
        {
            if (alternateChannelDTO == null || alternateChannelDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelRepository.Get(alternateChannelDTO.Id, serviceHeader);

                if (persisted != null && persisted.CustomerAccountId == alternateChannelDTO.CustomerAccountId)
                {
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCardNumberAndType(alternateChannelDTO.CardNumber, alternateChannelDTO.Type);

                    ISpecification<AlternateChannel> spec = filter;

                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Except(new List<AlternateChannel> { persisted }).Any())
                    {
                        alternateChannelDTO.ErrorMessageResult = (string.Format("Sorry, but Primary Account Number {0} has already been assigned to an account!", alternateChannelDTO.CardNumber));
                        return false;
                    }
                    else
                    {
                        persisted.CardNumber = alternateChannelDTO.CardNumber;
                        persisted.Remarks = alternateChannelDTO.Remarks;
                        persisted.DailyLimit = alternateChannelDTO.DailyLimit;
                        persisted.IsThirdPartyNotified = alternateChannelDTO.IsThirdPartyNotified;
                        persisted.ThirdPartyResponse = alternateChannelDTO.IsThirdPartyNotified ? alternateChannelDTO.ThirdPartyResponse : null;

                        persisted.RecordStatus = (byte)alternateChannelDTO.RecordStatus;
                        persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                        persisted.ModifiedDate = DateTime.Now;

                        if (alternateChannelDTO.IsLocked)
                            persisted.Lock();
                        else persisted.UnLock();

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
        }

        public bool ReplaceAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (alternateChannelDTO == null || alternateChannelDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelRepository.Get(alternateChannelDTO.Id, serviceHeader);

                if (persisted != null && persisted.CustomerAccountId == alternateChannelDTO.CustomerAccountId)
                {
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCardNumberAndType(alternateChannelDTO.CardNumber, alternateChannelDTO.Type);

                    ISpecification<AlternateChannel> spec = filter;

                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Except(new List<AlternateChannel> { persisted }).Any())
                    {
                        alternateChannelDTO.ErrorMessageResult = (string.Format("Sorry, but Primary Account Number {0} has already been assigned to an account!", alternateChannelDTO.CardNumber));
                        return false;
                    }
                    else
                    {
                        persisted.CardNumber = alternateChannelDTO.CardNumber;
                        persisted.ValidFrom = alternateChannelDTO.ValidFrom;
                        persisted.Expires = alternateChannelDTO.Expires;
                        persisted.Remarks = alternateChannelDTO.Remarks;
                        persisted.DailyLimit = alternateChannelDTO.DailyLimit;
                        persisted.IsThirdPartyNotified = alternateChannelDTO.IsThirdPartyNotified;
                        persisted.ThirdPartyResponse = alternateChannelDTO.IsThirdPartyNotified ? alternateChannelDTO.ThirdPartyResponse : null;

                        persisted.RecordStatus = (int)RecordStatus.Edited;
                        persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                        persisted.ModifiedDate = DateTime.Now;

                        if (alternateChannelDTO.IsLocked)
                            persisted.Lock();
                        else persisted.UnLock();

                        var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(alternateChannelDTO.CustomerAccountId, (int)AlternateChannelManagementAction.Replacement, alternateChannelDTO.Remarks, string.Format("{0} {1}", alternateChannelDTO.TypeDescription, alternateChannelDTO.MaskedCardNumber), serviceHeader.ApplicationUserName);

                        _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                        if (result)
                        {
                            #region Do we need to send alerts?

                            //_brokerService.ProcessAlternateChannelReplacementAccountAlerts(DMLCommand.None, serviceHeader, alternateChannelDTO);

                            #endregion

                        }
                    }
                }
            }

            return result;
        }

        public bool RenewAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (alternateChannelDTO == null || alternateChannelDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelRepository.Get(alternateChannelDTO.Id, serviceHeader);

                if (persisted != null && persisted.CustomerAccountId == alternateChannelDTO.CustomerAccountId)
                {
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCardNumberAndType(alternateChannelDTO.CardNumber, alternateChannelDTO.Type);

                    ISpecification<AlternateChannel> spec = filter;

                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Except(new List<AlternateChannel> { persisted }).Any())
                    {
                        alternateChannelDTO.ErrorMessageResult = (string.Format("Sorry, but Primary Account Number {0} has already been assigned to an account!", alternateChannelDTO.CardNumber));
                        return false;
                    }
                    else
                    {
                        persisted.CardNumber = alternateChannelDTO.CardNumber;
                        persisted.ValidFrom = alternateChannelDTO.ValidFrom;
                        persisted.Expires = alternateChannelDTO.Expires;
                        persisted.Remarks = alternateChannelDTO.Remarks;
                        persisted.DailyLimit = alternateChannelDTO.DailyLimit;
                        persisted.IsThirdPartyNotified = alternateChannelDTO.IsThirdPartyNotified;
                        persisted.ThirdPartyResponse = alternateChannelDTO.IsThirdPartyNotified ? alternateChannelDTO.ThirdPartyResponse : null;

                        persisted.RecordStatus = (int)RecordStatus.Edited;
                        persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                        persisted.ModifiedDate = DateTime.Now;

                        if (alternateChannelDTO.IsLocked)
                            persisted.Lock();
                        else persisted.UnLock();

                        var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(alternateChannelDTO.CustomerAccountId, (int)AlternateChannelManagementAction.Renewal, alternateChannelDTO.Remarks, string.Format("{0} {1}", alternateChannelDTO.TypeDescription, alternateChannelDTO.MaskedCardNumber), serviceHeader.ApplicationUserName);

                        _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                        if (result)
                        {
                            #region Do we need to send alerts?

                            //_brokerService.ProcessAlternateChannelRenewalAccountAlerts(DMLCommand.None, serviceHeader, alternateChannelDTO);

                            #endregion

                        }
                    }
                }
            }

            return result;
        }

        public bool DelinkAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (alternateChannelDTO == null || alternateChannelDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelRepository.Get(alternateChannelDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    _alternateChannelRepository.Remove(persisted, serviceHeader);

                    var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(alternateChannelDTO.CustomerAccountId, (int)AlternateChannelManagementAction.Delinking, alternateChannelDTO.Remarks, string.Format("{0} {1}", alternateChannelDTO.TypeDescription, alternateChannelDTO.MaskedCardNumber), serviceHeader.ApplicationUserName);

                    _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        #region Do we need to send alerts?

                        //_brokerService.ProcessAlternateChannelDelinkingAccountAlerts(DMLCommand.None, serviceHeader, alternateChannelDTO);

                        #endregion
                    }
                }
            }

            return result;
        }

        public bool StopAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (alternateChannelDTO == null || alternateChannelDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelRepository.Get(alternateChannelDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Lock();
                    persisted.Remarks = alternateChannelDTO.Remarks;

                    persisted.RecordStatus = (int)RecordStatus.Edited;
                    persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                    persisted.ModifiedDate = DateTime.Now;

                    var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(alternateChannelDTO.CustomerAccountId, (int)AlternateChannelManagementAction.Stoppage, alternateChannelDTO.Remarks, string.Format("{0} {1}", alternateChannelDTO.TypeDescription, alternateChannelDTO.MaskedCardNumber), serviceHeader.ApplicationUserName);

                    _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        #region Do we need to send alerts?

                        //_brokerService.ProcessAlternateChannelStoppageAccountAlerts(DMLCommand.None, serviceHeader, alternateChannelDTO);

                        #endregion
                    }
                }
            }

            return result;
        }

        public List<AlternateChannelDTO> FindAlternateChannels(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var enabledAlternateChannels = AlternateChannelSpecifications.DefaultSpec();

                ISpecification<AlternateChannel> spec = enabledAlternateChannels;

                var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                if (alternateChannels != null && alternateChannels.Any())
                {
                    return alternateChannels.OrderByDescending(x => x.CreatedDate).ProjectedAsCollection<AlternateChannelDTO>();
                }
                else return null;
            }
        }

        public AlternateChannelDTO FindAlternateChannel(Guid alternateChannelId, ServiceHeader serviceHeader)
        {
            if (alternateChannelId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var alternateChannel = _alternateChannelRepository.Get(alternateChannelId, serviceHeader);

                    if (alternateChannel != null)
                    {
                        return alternateChannel.ProjectedAs<AlternateChannelDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<AlternateChannelDTO> FindAlternateChannels(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelSpecifications.DefaultSpec();

                ISpecification<AlternateChannel> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelPagedCollection = _alternateChannelRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelPagedCollection != null)
                {
                    var pageCollection = alternateChannelPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelDTO>();

                    var itemsCount = alternateChannelPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AlternateChannelDTO> FindAlternateChannels(string text, int alternateChannelFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelSpecifications.AlternateChannelFullText(text, alternateChannelFilter);

                ISpecification<AlternateChannel> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelPagedCollection = _alternateChannelRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelPagedCollection != null)
                {
                    var pageCollection = alternateChannelPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelDTO>();

                    var itemsCount = alternateChannelPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AlternateChannelDTO> FindAlternateChannels(int type, int recordStatus, string text, int alternateChannelFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelSpecifications.AlternateChannelWithTypeAndFullText(type, recordStatus, text, alternateChannelFilter);

                ISpecification<AlternateChannel> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelPagedCollection = _alternateChannelRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelPagedCollection != null)
                {
                    var pageCollection = alternateChannelPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelDTO>();

                    var itemsCount = alternateChannelPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AlternateChannelDTO> FindThirdPartyNotifiableAlternateChannels(int type, string text, int alternateChannelFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelSpecifications.ThirdPartyNotifiableAlternateChannels(type, text, alternateChannelFilter);

                ISpecification<AlternateChannel> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelPagedCollection = _alternateChannelRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelPagedCollection != null)
                {
                    var pageCollection = alternateChannelPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelDTO>();

                    var itemsCount = alternateChannelPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    //get the specification
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCustomerAccountId(customerAccountId);

                    ISpecification<AlternateChannel> spec = filter;

                    //Query this criteria
                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Any())
                    {
                        return alternateChannels.ProjectedAsCollection<AlternateChannelDTO>();
                    }
                    else // no results
                        return null;
                }
            }
            else // no results
                return null;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    //get the specification
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCustomerId(customerId);

                    ISpecification<AlternateChannel> spec = filter;

                    //Query this criteria
                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Any())
                    {
                        return alternateChannels.ProjectedAsCollection<AlternateChannelDTO>();
                    }
                    else // no results
                        return null;
                }
            }
            else // no results
                return null;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCustomerAccountIdAndType(Guid customerAccountId, int type, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    // get the specification
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCustomerAccountIdAndType(customerAccountId, type);

                    ISpecification<AlternateChannel> spec = filter;

                    //Query this criteria
                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Any())
                    {
                        return alternateChannels.ProjectedAsCollection<AlternateChannelDTO>();
                    }
                    else // no results
                        return null;
                }
            }
            else // no results 
                return null;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCardNumberAndCardType(string cardNumber, int cardType, ServiceHeader serviceHeader)
        {
            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    // get the specification
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCardNumberAndType(cardNumber, cardType);

                    ISpecification<AlternateChannel> spec = filter;

                    //Query this criteria
                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Any())
                    {
                        return alternateChannels.ProjectedAsCollection<AlternateChannelDTO>();
                    }
                    else // no results
                        return null;
                }
            }
            else // no results 
                return null;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCardNumber(string cardNumber, ServiceHeader serviceHeader)
        {
            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    // get the specification
                    var filter = AlternateChannelSpecifications.AlternateChannelWithCardNumber(cardNumber);

                    ISpecification<AlternateChannel> spec = filter;

                    //Query this criteria
                    var alternateChannels = _alternateChannelRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Any())
                    {
                        return alternateChannels.ProjectedAsCollection<AlternateChannelDTO>();
                    }
                    else // no results
                        return null;
                }
            }
            else // no results 
                return null;
        }

        public List<CommissionDTO> FindCommissions(int alternateChannelType, int alternateChannelTypeKnownChargeType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelTypeCommissionSpecifications.AlternateChannelTypeCommission(alternateChannelType, alternateChannelTypeKnownChargeType);

                ISpecification<AlternateChannelTypeCommission> spec = filter;

                var alternateChannelTypeCommissions = _alternateChannelTypeCommissionRepository.AllMatching(spec, serviceHeader);

                if (alternateChannelTypeCommissions != null)
                {
                    var alternateChannelTypeCommissionDTOs = alternateChannelTypeCommissions.ProjectedAsCollection<AlternateChannelTypeCommissionDTO>();

                    var projection = (from p in alternateChannelTypeCommissionDTOs
                                      select new
                                      {
                                          p.ChargeBenefactor,
                                          p.Commission
                                      });

                    foreach (var item in projection)
                        item.Commission.ChargeBenefactor = item.ChargeBenefactor; // map benefactor

                    return (from p in projection select p.Commission).ToList();
                }
                else return null;
            }
        }

        public List<CommissionDTO> FindCachedCommissions(int alternateChannelType, int alternateChannelTypeKnownChargeType, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByAlternateChannelTypeAndAlternateChannelTypeKnownChargeType_{0}_{1}_{2}", serviceHeader.ApplicationDomainName, alternateChannelType, alternateChannelTypeKnownChargeType), () =>
            {
                return FindCommissions(alternateChannelType, alternateChannelTypeKnownChargeType, serviceHeader);
            });
        }

        public bool UpdateCommissions(int alternateChannelType, List<CommissionDTO> commissionDTOs, int alternateChannelTypeKnownChargeType, int chargeBenefactor, ServiceHeader serviceHeader)
        {
            if (!Enum.IsDefined(typeof(AlternateChannelType), alternateChannelType))
                throw new InvalidOperationException("Select a valid alternate channel type.");
            if (!Enum.IsDefined(typeof(AlternateChannelKnownChargeType), alternateChannelTypeKnownChargeType))
                throw new InvalidOperationException("Select a valid alternate channel charge type.");
            if (!Enum.IsDefined(typeof(ChargeBenefactor), chargeBenefactor))
                throw new InvalidOperationException("Select whether the charges are borne by the customer or the institution.");
            if (commissionDTOs == null || commissionDTOs.Count == 0)
                throw new InvalidOperationException("Select at least one applicable charge.");
            if (commissionDTOs.Any(item => item == null || item.Id == Guid.Empty) || commissionDTOs.Select(item => item.Id).Distinct().Count() != commissionDTOs.Count)
                throw new InvalidOperationException("Applicable charges must contain unique, valid charge identifiers.");

            if (commissionDTOs != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var filter = AlternateChannelTypeCommissionSpecifications.AlternateChannelTypeCommission(alternateChannelType, alternateChannelTypeKnownChargeType);

                    ISpecification<AlternateChannelTypeCommission> spec = filter;

                    var alternateChannelTypeCommissions = _alternateChannelTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (alternateChannelTypeCommissions != null)
                    {
                        alternateChannelTypeCommissions.ToList().ForEach(x => _alternateChannelTypeCommissionRepository.Remove(x, serviceHeader));
                    }

                    if (commissionDTOs.Any())
                    {
                        foreach (var item in commissionDTOs)
                        {
                            var alternateChannelTypeCommission = AlternateChannelTypeCommissionFactory.CreateAlternateChannelTypeCommission(alternateChannelType, item.Id, alternateChannelTypeKnownChargeType, chargeBenefactor);

                            alternateChannelTypeCommission.CreatedBy = serviceHeader.ApplicationUserName;

                            _alternateChannelTypeCommissionRepository.Add(alternateChannelTypeCommission, serviceHeader);
                        }
                    }

                    var updated = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    if (updated)
                        _appCache.Remove(string.Format("CommissionsByAlternateChannelTypeAndAlternateChannelTypeKnownChargeType_{0}_{1}_{2}", serviceHeader.ApplicationDomainName, alternateChannelType, alternateChannelTypeKnownChargeType));
                    return updated;
                }
            }
            else return false;
        }

        public bool UpdateCommissionsByIds(int alternateChannelType, List<Guid> commissionIds, int alternateChannelTypeKnownChargeType, int chargeBenefactor, ServiceHeader serviceHeader)
        {
            if (commissionIds == null || commissionIds.Count == 0)
                throw new InvalidOperationException("Select at least one applicable charge.");
            if (commissionIds.Any(id => id == Guid.Empty) || commissionIds.Distinct().Count() != commissionIds.Count)
                throw new InvalidOperationException("Applicable charges must contain unique, valid charge identifiers.");

            var commissions = new List<CommissionDTO>();
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                foreach (var commissionId in commissionIds)
                {
                    var commission = _commissionRepository.Get(commissionId, serviceHeader);
                    if (commission == null)
                        throw new InvalidOperationException("One or more selected charges no longer exist.");
                    if (commission.IsLocked)
                        throw new InvalidOperationException(string.Format("The charge '{0}' is locked and cannot be assigned.", commission.Description));
                    commissions.Add(commission.ProjectedAs<CommissionDTO>());
                }
            }

            return UpdateCommissions(alternateChannelType, commissions, alternateChannelTypeKnownChargeType, chargeBenefactor, serviceHeader);
        }
    }
}
