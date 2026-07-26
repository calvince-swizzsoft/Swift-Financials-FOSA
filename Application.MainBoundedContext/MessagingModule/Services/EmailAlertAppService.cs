using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public class EmailAlertAppService : IEmailAlertAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmailAlert> _emailAlertRepository;
        private readonly IBranchAppService _branchAppService;
        private readonly IMessageGroupAppService _messageGroupAppService;
        private readonly IBrokerService _brokerService;

        public EmailAlertAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<EmailAlert> emailAlertRepository,
            IBranchAppService branchAppService,
            IMessageGroupAppService messageGroupAppService,
            IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (emailAlertRepository == null)
                throw new ArgumentNullException(nameof(emailAlertRepository));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (messageGroupAppService == null)
                throw new ArgumentNullException(nameof(messageGroupAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _emailAlertRepository = emailAlertRepository;
            _branchAppService = branchAppService;
            _messageGroupAppService = messageGroupAppService;
            _brokerService = brokerService;
        }

        public EmailAlertDTO AddNewEmailAlert(EmailAlertDTO emailAlertDTO, ServiceHeader serviceHeader)
        {
            if (emailAlertDTO != null && !string.IsNullOrWhiteSpace(emailAlertDTO.MailMessageTo) && Regex.IsMatch(emailAlertDTO.MailMessageTo, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                var dlrStatus = DLRStatus.UnKnown;

                switch ((MessageOrigin)emailAlertDTO.MailMessageOrigin)
                {
                    case MessageOrigin.Within:
                    case MessageOrigin.Without:
                        dlrStatus = DLRStatus.Pending;
                        break;
                    case MessageOrigin.Other:
                        dlrStatus = DLRStatus.NotApplicable;
                        break;
                    default:
                        break;
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var mailMessage = new MailMessage(emailAlertDTO.MailMessageFrom, emailAlertDTO.MailMessageTo, emailAlertDTO.MailMessageCC, emailAlertDTO.MailMessageSubject, emailAlertDTO.MailMessageBody, emailAlertDTO.MailMessageIsBodyHtml, (int)dlrStatus, emailAlertDTO.MailMessageOrigin, emailAlertDTO.MailMessagePriority, 0, emailAlertDTO.MailMessageSecurityCritical, emailAlertDTO.MailMessageAttachments);

                    var emailAlert = EmailAlertFactory.CreateEmailAlert(emailAlertDTO.BranchId,mailMessage);

                    emailAlert.CreatedBy = serviceHeader.ApplicationUserName;

                    _emailAlertRepository.Add(emailAlert, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        var projection = emailAlert.ProjectedAs<EmailAlertDTO>();

                        if (_brokerService.ProcessEmailAlerts(DMLCommand.Insert, serviceHeader, projection))
                        {
                            return projection;
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool AddNewEmailAlerts(List<EmailAlertDTO> emailAlertDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (emailAlertDTOs != null && emailAlertDTOs.Any())
            {
                emailAlertDTOs.ForEach(item =>
                {
                    AddNewEmailAlert(item, serviceHeader);
                });

                result = true;
            }

            return result;
        }

        public bool UpdateEmailAlert(EmailAlertDTO emailAlertDTO, ServiceHeader serviceHeader)
        {
            if (emailAlertDTO == null || emailAlertDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _emailAlertRepository.Get(emailAlertDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var mailMessage = new MailMessage(persisted.MailMessage.From, persisted.MailMessage.To, persisted.MailMessage.CC, persisted.MailMessage.Subject, persisted.MailMessage.Body, persisted.MailMessage.IsBodyHtml, emailAlertDTO.MailMessageDLRStatus, persisted.MailMessage.Origin, persisted.MailMessage.Priority, emailAlertDTO.MailMessageSendRetry, persisted.MailMessage.SecurityCritical, persisted.MailMessage.Attachments);

                    var current = EmailAlertFactory.CreateEmailAlert(emailAlertDTO.BranchId, mailMessage);

                    current.CreatedBy = persisted.CreatedBy;

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _emailAlertRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<EmailAlertDTO> FindEmailAlerts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var emailAlerts = _emailAlertRepository.GetAll(serviceHeader);

                if (emailAlerts != null && emailAlerts.Any())
                {
                    return emailAlerts.ProjectedAsCollection<EmailAlertDTO>();
                }
                else return null;
            }
        }

        public List<EmailAlertDTO> FindEmailAlertsByDLRStatus(int dlrStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmailAlertSpecifications.EmailAlertWithDLRStatus(dlrStatus);

                ISpecification<EmailAlert> spec = filter;

                var emailAlerts = _emailAlertRepository.AllMatching(spec, serviceHeader);

                if (emailAlerts != null && emailAlerts.Any())
                {
                    return emailAlerts.ProjectedAsCollection<EmailAlertDTO>();
                }
                else return null;
            }
        }

        public EmailAlertDTO FindEmailAlert(Guid emailAlertId, ServiceHeader serviceHeader)
        {
            if (emailAlertId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var emailAlert = _emailAlertRepository.Get(emailAlertId, serviceHeader);

                    if (emailAlert != null)
                    {
                        return emailAlert.ProjectedAs<EmailAlertDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<EmailAlertDTO> FindEmailAlerts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmailAlertSpecifications.DefaultSpec();

                ISpecification<EmailAlert> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var emailAlertPagedCollection = _emailAlertRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (emailAlertPagedCollection != null)
                {
                    var pageCollection = emailAlertPagedCollection.PageCollection.ProjectedAsCollection<EmailAlertDTO>();

                    var itemsCount = emailAlertPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmailAlertDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmailAlertDTO> FindEmailAlerts(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmailAlertSpecifications.EmailAlertFullText(dlrStatus, text, daysCap);

                ISpecification<EmailAlert> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var emailAlertPagedCollection = _emailAlertRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (emailAlertPagedCollection != null)
                {
                    var pageCollection = emailAlertPagedCollection.PageCollection.ProjectedAsCollection<EmailAlertDTO>();

                    var itemsCount = emailAlertPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmailAlertDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmailAlertDTO> FindEmailAlerts(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmailAlertSpecifications.EmailAlertFullText(dlrStatus, startDate, endDate, text);

                ISpecification<EmailAlert> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var emailAlertPagedCollection = _emailAlertRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (emailAlertPagedCollection != null)
                {
                    var pageCollection = emailAlertPagedCollection.PageCollection.ProjectedAsCollection<EmailAlertDTO>();

                    var itemsCount = emailAlertPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmailAlertDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<EmailAlertDTO> FindEmailAlertsByDLRStatusAndOrigin(int dlrStatus, int origin, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmailAlertSpecifications.EmailAlertWithDLRStatusAndOrigin(dlrStatus, origin);

                ISpecification<EmailAlert> spec = filter;

                var emailAlerts = _emailAlertRepository.AllMatching(spec, serviceHeader);

                if (emailAlerts != null && emailAlerts.Any())
                {
                    return emailAlerts.ProjectedAsCollection<EmailAlertDTO>();
                }
                else return null;
            }
        }

        public bool AddEmailAlertsWithHistory(GroupEmailAlertDTO groupEmailAlertDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var emailAlerts = new List<EmailAlert>();

            var messageHistories = new List<CustomerMessageHistory>();

            var buffer = groupEmailAlertDTO.Recipients.Trim().Split(new char[] { ',' }).Select(Guid.Parse);

            var customers = _messageGroupAppService.FindCustomersByMessageGroupIds(buffer.ToArray(), (int)MessageCategory.EmailAlert, serviceHeader);

            if (customers != null && customers.Any())
            {
                var currentBranch = _branchAppService.FindBranch(groupEmailAlertDTO.BranchId, serviceHeader);

                foreach (var customer in customers)
                {
                    if (!string.IsNullOrEmpty(customer.AddressEmail) && Regex.IsMatch(customer.AddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                    {
                        var mailMessageBody = groupEmailAlertDTO.MailMessageBody;

                        if (currentBranch != null && !string.IsNullOrWhiteSpace(currentBranch.Description) && !string.IsNullOrWhiteSpace(currentBranch.CompanyDescription))
                            mailMessageBody = string.Format("{0}{1}{2}{3}{4}", mailMessageBody, Environment.NewLine, currentBranch.Description, Environment.NewLine, currentBranch.CompanyDescription);

                        var mailMessage = new MailMessage(groupEmailAlertDTO.MailMessageFrom, customer.AddressEmail, string.Empty, groupEmailAlertDTO.MailMessageSubject, mailMessageBody, groupEmailAlertDTO.MailMessageIsBodyHtml, (int)DLRStatus.Pending, (int)MessageOrigin.Within, (int)QueuePriority.Normal, 0, false, string.Empty);

                        var emailAlert = EmailAlertFactory.CreateEmailAlert(groupEmailAlertDTO.CompanyId, mailMessage);

                        emailAlert.CreatedBy = serviceHeader.ApplicationUserName;

                        emailAlerts.Add(emailAlert);

                        var customerMessageHistory = CustomerMessageHistoryFactory.CreateCustomerMessageHistory(customer.Id, groupEmailAlertDTO.MailMessageBody, groupEmailAlertDTO.MailMessageSubject, (int)MessageCategory.EmailAlert, mailMessage.To);

                        messageHistories.Add(customerMessageHistory);
                    }
                }
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (emailAlerts.Any())
                {
                    emailAlerts.ForEach(item =>
                    {
                        _emailAlertRepository.Add(item, serviceHeader);
                    });

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        if (emailAlerts.Any())
                        {
                            var data = from t in emailAlerts
                                       select new EmailAlertDTO
                                       {
                                           Id = t.Id,
                                           MailMessageDLRStatus = (int)DLRStatus.Pending,
                                       };

                            _brokerService.ProcessEmailAlerts(DMLCommand.Insert, serviceHeader, data.ToArray());
                        }
                    }
                }
            }

            return result;
        }

        public bool AddQuickEmailAlert(QuickEmailAlertDTO quickEmailAlertDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var emailAlerts = new List<EmailAlert>();

            var buffer = quickEmailAlertDTO.Recipients.Trim().Split(new char[] { ',' });

            if (buffer != null && buffer.Any())
            {
                var currentBranch = _branchAppService.FindBranch(quickEmailAlertDTO.BranchId, serviceHeader);

                Array.ForEach(buffer, recipient =>
                {
                    if (!string.IsNullOrEmpty(recipient) && Regex.IsMatch(recipient, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                    {
                        var mailMessageBody = quickEmailAlertDTO.MailMessageBody;

                        if (currentBranch != null && !string.IsNullOrWhiteSpace(currentBranch.Description) && !string.IsNullOrWhiteSpace(currentBranch.CompanyDescription))
                            mailMessageBody = string.Format("{0}{1}{2}{3}{4}", mailMessageBody, Environment.NewLine, currentBranch.Description, Environment.NewLine, currentBranch.Description);

                        var mailMessage = new MailMessage(quickEmailAlertDTO.MailMessageFrom, recipient, string.Empty, quickEmailAlertDTO.MailMessageSubject, mailMessageBody, quickEmailAlertDTO.MailMessageIsBodyHtml, (int)DLRStatus.Pending, (int)MessageOrigin.Within, (int)QueuePriority.Highest, 0, false, string.Empty);

                        var emailAlert = EmailAlertFactory.CreateEmailAlert(quickEmailAlertDTO.CompanyId, mailMessage);

                        emailAlert.CreatedBy = serviceHeader.ApplicationUserName;

                        emailAlerts.Add(emailAlert);
                    }
                });
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (emailAlerts.Any())
                {
                    emailAlerts.ForEach(item =>
                    {
                        _emailAlertRepository.Add(item, serviceHeader);
                    });

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        if (emailAlerts.Any())
                        {
                            var data = from t in emailAlerts
                                       select new EmailAlertDTO
                                       {
                                           Id = t.Id,
                                           MailMessageDLRStatus = (int)DLRStatus.Pending,
                                       };

                            _brokerService.ProcessEmailAlerts(DMLCommand.Insert, serviceHeader, data.ToArray());
                        }
                    }
                }
            }

            return result;
        }
    }
}

