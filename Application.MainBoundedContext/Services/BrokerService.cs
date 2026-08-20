using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Configuration;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq;
//using System.Messaging;

namespace Application.MainBoundedContext.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly ServiceBrokerConfigSection _serviceBrokerConfigSection;

        public BrokerService(
            IMessageQueueService messageQueueService)
        {
            if (messageQueueService == null)
                throw new ArgumentNullException(nameof(messageQueueService));
            
            _messageQueueService = messageQueueService;
            _serviceBrokerConfigSection = ConfigurationHelper.GetServiceBrokerConfiguration();
        }

        public bool ProcessMembershipAccountRegistrationAlerts(DMLCommand command, ServiceHeader serviceHeader, params UserDTO[] data)
        {
            var result = default(bool);

            if (data != null && _serviceBrokerConfigSection != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = new Guid(item.Id),
                                            UserPassword = item.Password,
                                            CallbackUrl = item.CallbackUrl,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.MembershipAccountRegistration,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessMembershipResetPasswordAlerts(DMLCommand command, ServiceHeader serviceHeader, params UserDTO[] data)
        {
            var result = default(bool);

            if (data != null && _serviceBrokerConfigSection != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = new Guid(item.Id),
                                            CallbackUrl = item.CallbackUrl,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.MembershipResetPassword,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessMembershipAccountVerificationAlerts(DMLCommand command, ServiceHeader serviceHeader, params UserDTO[] data)
        {
            var result = default(bool);

            if (data != null && _serviceBrokerConfigSection != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = new Guid(item.Id),
                                            Token = item.Token,
                                            Provider = item.Provider,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.MembershipAccountVerification,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }


        public bool ProcessRecurringBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params RecurringBatchEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.Insert:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.RecurringBatchPostingQueuePath, queueDTO, MessageCategory.RecurringBatchEntry, (MessagePriority)item.RecurringBatchPriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessLoanDisbursementBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params LoanDisbursementBatchEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.LoanDisbursementBatchPostingQueuePath, queueDTO, MessageCategory.LoanDisbursementBatchEntry, (MessagePriority)item.LoanDisbursementBatchPriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessCreditBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params CreditBatchEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.CreditBatchPostingQueuePath, queueDTO, MessageCategory.CreditBatchEntry, (MessagePriority)item.CreditBatchPriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessWireTransferBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params WireTransferBatchEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.WireTransferBatchPostingQueuePath, queueDTO, MessageCategory.WireTransferBatchEntry, (MessagePriority)item.WireTransferBatchPriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessDebitBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params DebitBatchEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.DebitBatchPostingQueuePath, queueDTO, MessageCategory.DebitBatchEntry, (MessagePriority)item.DebitBatchPriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessTextAlerts(DMLCommand command, ServiceHeader serviceHeader, params TextAlertDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.Insert:

                        Array.ForEach(data, (textAlert) =>
                        {
                            switch ((DLRStatus)textAlert.TextMessageDLRStatus)
                            {
                                case DLRStatus.UnKnown:
                                case DLRStatus.Pending:

                                    if (textAlert.TextMessageSendRetry == 0)
                                    {
                                        var queueDTO = new QueueDTO
                                        {
                                            RecordId = textAlert.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                        };

                                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.TextDispatcherQueuePath, queueDTO, MessageCategory.SMSAlert, (MessagePriority)textAlert.TextMessagePriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        });

                        result = true;

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessEmailAlerts(DMLCommand command, ServiceHeader serviceHeader, params EmailAlertDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.Insert:

                        Array.ForEach(data, (emailAlert) =>
                        {
                            switch ((DLRStatus)emailAlert.MailMessageDLRStatus)
                            {
                                case DLRStatus.UnKnown:
                                case DLRStatus.Pending:

                                    if (emailAlert.MailMessageSendRetry == 0)
                                    {
                                        var queueDTO = new QueueDTO
                                        {
                                            RecordId = emailAlert.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                        };

                                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.EmailDispatcherQueuePath, queueDTO, MessageCategory.EmailAlert, (MessagePriority)emailAlert.MailMessagePriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        });

                        result = true;

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAuditLogs(DMLCommand command, ServiceHeader serviceHeader, params AuditLogBCP[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.Insert:

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditLogDispatcherQueuePath, data.ToList(), MessageCategory.AuditLog, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        result = true;

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessGuarantorAttachmentAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanGuarantorDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.GuarantorAttachment,
                                            AccountAlertPrimaryDescription = item.AccountAlertPrimaryDescription,
                                            LoaneeCustomerFullName = item.LoaneeCustomerFullName,
                                            AccountAlertSecondaryDescription = item.AccountAlertSecondaryDescription,
                                            AccountAlertReference = item.AccountAlertReference,
                                            AccountAlertTotalValue = item.PrincipalAttached + item.InterestAttached,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessGuarantorRelievingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanGuarantorAttachmentHistoryEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.GuarantorRelieving,
                                            AccountAlertCustomerId = item.AccountAlertCustomerId,
                                            AccountAlertPrimaryDescription = item.AccountAlertPrimaryDescription,
                                            AccountAlertSecondaryDescription = item.AccountAlertSecondaryDescription,
                                            AccountAlertReference = item.AccountAlertReference,
                                            AccountAlertTotalValue = item.AccountAlertTotalValue,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessFrozenAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params CustomerAccountDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AccountFreezing,
                                            AccountAlertSecondaryDescription = item.Remarks,
                                            ValueDate = DateTime.Today.ToString("dd/MMM/yyyy"),
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessMembershipApprovalAlerts(DMLCommand command, ServiceHeader serviceHeader, params CustomerDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.MembershipApproval,
                                            AccountAlertPrimaryDescription = item.FullName,
                                            AccountAlertSecondaryDescription = item.Reference2,
                                            AccountAlertReference = item.Reference3,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAccountClosureRequestAlerts(DMLCommand command, ServiceHeader serviceHeader, params AccountClosureRequestDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AccountClosure,
                                            AccountAlertTotalValue = item.NetRefundable,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessGuarantorSubstitutionAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanGuarantorDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.GuarantorSubstitution,
                                            AccountAlertCustomerId = item.AccountAlertCustomerId,
                                            AccountAlertPrimaryDescription = item.AccountAlertPrimaryDescription,
                                            LoaneeCustomerFullName = item.LoaneeCustomerFullName,
                                            AccountAlertSecondaryDescription = item.AccountAlertSecondaryDescription,
                                            AccountAlertReference = item.AccountAlertReference,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessLoanDeferredAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanCaseDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.LoanDeffered,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessLoanGuaranteeAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanCaseDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.LoanGuarantee,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessLoanRequestAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanRequestDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.LoanRequest,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessMobileToBankSenderAcknowledgementAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params MobileToBankRequestDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.MobileToBankSenderAcknowledgement,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessPaySlips(DMLCommand command, ServiceHeader serviceHeader, params PaySlipDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.SalaryPeriodPostingQueuePath, queueDTO, MessageCategory.PaySlip, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessElectronicStatements(DMLCommand command, ServiceHeader serviceHeader, params MediaDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.SKU,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.ElectronicStatement,
                                            AccountAlertPrimaryDescription = item.FileType,
                                            AccountAlertReference = item.FileRemarks,
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessWorkflow(DMLCommand command, ServiceHeader serviceHeader, WorkflowDTO data)
        {
           var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTO = new QueueDTO
                        {
                            RecordId = data.RecordId,
                            AppDomainName = serviceHeader.ApplicationDomainName,
                            WorkflowRecordType = data.SystemPermissionType,
                            WorkflowRecordStatus = data.Status
                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.WorkflowProcessorQueuePath, queueDTO, MessageCategory.Workflow, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAlternateChannelLinkingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.CustomerAccountId,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AlternateChannelLinking,
                                            AccountAlertPrimaryDescription = item.CustomerAccountCustomerTypeDescription,
                                            AccountAlertSecondaryDescription = item.TypeDescription,
                                            AccountAlertTotalValue = item.DailyLimit,
                                            ValueDate = item.Expires.ToString(("dd/MMM/yyyy"))
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAlternateChannelReplacementAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.CustomerAccountId,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AlternateChannelReplacement,
                                            AccountAlertPrimaryDescription = item.CustomerAccountCustomerTypeDescription,
                                            AccountAlertSecondaryDescription = item.TypeDescription,
                                            AccountAlertTotalValue = item.DailyLimit,
                                            ValueDate = item.Expires.ToString(("dd/MMM/yyyy"))
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAlternateChannelRenewalAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.CustomerAccountId,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AlternateChannelRenewal,
                                            AccountAlertPrimaryDescription = item.CustomerAccountCustomerTypeDescription,
                                            AccountAlertSecondaryDescription = item.TypeDescription,
                                            AccountAlertTotalValue = item.DailyLimit,
                                            ValueDate = item.Expires.ToString(("dd/MMM/yyyy"))
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAlternateChannelDelinkingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.CustomerAccountId,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AlternateChannelDelinking,
                                            AccountAlertPrimaryDescription = item.CustomerAccountCustomerTypeDescription,
                                            AccountAlertSecondaryDescription = item.TypeDescription,
                                            AccountAlertTotalValue = item.DailyLimit,
                                            ValueDate = item.Expires.ToString(("dd/MMM/yyyy"))
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessAlternateChannelStoppageAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.CustomerAccountId,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.AlternateChannelStoppage,
                                            AccountAlertPrimaryDescription = item.CustomerAccountCustomerTypeDescription,
                                            AccountAlertSecondaryDescription = item.TypeDescription,
                                            AccountAlertTotalValue = item.DailyLimit,
                                            ValueDate = item.Expires.ToString(("dd/MMM/yyyy"))
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessLeaveApprovalAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LeaveApplicationDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertPrimaryDescription = item.LeaveTypeDescription,
                                            AccountAlertSecondaryDescription = item.StatusDescription,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.LeaveApproval
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessCustomerRegistrationAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params CustomerDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.CustomerRegistration
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessEmployeeRegistrationAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params EmployeeDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data
                                        select new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            AccountAlertTrigger = (int)AccountAlertTrigger.EmployeeRegistration
                                        };

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessJournalReversalBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params JournalReversalBatchEntryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.JournalReversalBatchPostingQueuePath, queueDTO, MessageCategory.JournalReversalBatchEntry, (MessagePriority)item.JournalReversalBatchPriority, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessCustomerDetailsEditingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params QueueDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        var queueDTOs = from item in data select item;

                        _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, queueDTOs.ToList(), MessageCategory.AccountAlert, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessBankToMobileRequests(DMLCommand command, ServiceHeader serviceHeader, params BankToMobileRequestDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.Update:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.BankToMobileIPNQueuePath, queueDTO, MessageCategory.BankToMobileIPN, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessBrokerRequests(DMLCommand command, ServiceHeader serviceHeader, params BrokerRequestDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.Update:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.BrokerIPNQueuePath, queueDTO, MessageCategory.BrokerIPN, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ProcessPopulationRegisterQueries(DMLCommand command, ServiceHeader serviceHeader, params PopulationRegisterQueryDTO[] data)
        {
            var result = default(bool);

            if (data != null)
            {
                switch (command)
                {
                    case DMLCommand.None:

                        Array.ForEach(data, (item) =>
                        {
                            var queueDTO = new QueueDTO
                            {
                                RecordId = item.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                            };

                            _messageQueueService.Send(_serviceBrokerConfigSection.ServiceBrokerSettingsItems.PopulationRegisterQueryQueuePath, queueDTO, MessageCategory.PopulationRegisterQuery, MessagePriority.High, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);
                        });

                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}