using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;
using Application.MainBoundedContext.DTO.RegistryModule;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Linq;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO;
using System.Globalization;

namespace SwiftFinancials.AccountAlertDispatcher.Configuration
{
    public class AccountAlertMessageProcessor : MessageProcessor<List<QueueDTO>>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly AccountAlertDispatcherConfigSection _accountAlertDispatcherConfigSection;

        private readonly NumberFormatInfo _nfi;

        public AccountAlertMessageProcessor(IChannelService channelService, ILogger logger, AccountAlertDispatcherConfigSection accountAlertDispatcherConfigSection)
            : base(accountAlertDispatcherConfigSection.AccountAlertDispatcherSettingsItems.QueuePath, accountAlertDispatcherConfigSection.AccountAlertDispatcherSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _accountAlertDispatcherConfigSection = accountAlertDispatcherConfigSection;

            _nfi = new NumberFormatInfo();
            _nfi.CurrencySymbol = string.Empty;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->AccountAlertMessageProcessor...", exception, _accountAlertDispatcherConfigSection.AccountAlertDispatcherSettingsItems.QueuePath);
        }

        protected override async Task Process(List<QueueDTO> @object, int appSpecific)
        {
            foreach (var settingsItem in _accountAlertDispatcherConfigSection.AccountAlertDispatcherSettingsItems)
            {
                var accountAlertDispatcherSettingsElement = (AccountAlertDispatcherSettingsElement)settingsItem;

                if (accountAlertDispatcherSettingsElement != null && accountAlertDispatcherSettingsElement.Enabled == 1)
                {
                    var messageCategory = (MessageCategory)appSpecific;

                    if (messageCategory.In(MessageCategory.AccountAlert))
                    {
                        foreach (var queueDTO in @object)
                        {
                            if (!accountAlertDispatcherSettingsElement.UniqueId.Equals(queueDTO.AppDomainName, StringComparison.OrdinalIgnoreCase))
                                continue;

                            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

                            var userDTO = new UserDTO();

                            var customerDTO = new CustomerDTO();

                            switch ((AccountAlertTrigger)queueDTO.AccountAlertTrigger)
                            {
                                case AccountAlertTrigger.MembershipAccountRegistration:

                                    #region Do we need to send alerts?

                                    userDTO = await _channelService.FindMembershipAsync(queueDTO.RecordId.ToString(), serviceHeader);

                                    if (userDTO != null)
                                    {
                                        var branchDTO = await _channelService.FindBranchAsync((Guid)userDTO.BranchId, serviceHeader);

                                        #region Email Alert

                                        var emailAlertTemplatePath = Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.MembershipAccountRegistration));

                                        if (File.Exists(emailAlertTemplatePath))
                                        {
                                            var template = File.ReadAllText(emailAlertTemplatePath);

                                            dynamic expando = new ExpandoObject();

                                            var model = expando as IDictionary<string, object>;

                                            model.Add("FirstName", userDTO.FirstName);
                                            model.Add("Username", userDTO.Email);
                                            model.Add("UserPassword", queueDTO.UserPassword);
                                            model.Add("CallbackUrl", queueDTO.CallbackUrl);
                                            model.Add("CompanyDescription", branchDTO.Description);

                                            var emailResult = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", "AccountRegistration"), null, model);

                                            EmailAlertDTO emailAlertDTO = new EmailAlertDTO
                                            {
                                                BranchId = userDTO.BranchId.Value,
                                                MailMessageFrom = branchDTO.CompanyAddressEmail,
                                                MailMessageTo = userDTO.Email,
                                                MailMessageSubject = "Account Registration",
                                                MailMessageBody = emailResult,
                                                MailMessageIsBodyHtml = true,
                                                MailMessageDLRStatus = (int)DLRStatus.Pending,
                                                MailMessageOrigin = (int)MessageOrigin.Within,
                                                MailMessageSecurityCritical = true
                                            };

                                            await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                        }

                                        #endregion
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.MembershipAccountVerification:

                                    #region Do we need to send alerts?

                                    userDTO = await _channelService.FindMembershipAsync(queueDTO.RecordId.ToString(), serviceHeader);

                                    if (userDTO != null)
                                    {
                                        var branchDTO = await _channelService.FindBranchAsync((Guid)userDTO.BranchId, serviceHeader);

                                        switch ((TwoFactorProviders)queueDTO.Provider)
                                        {
                                            case TwoFactorProviders.PhoneCode:

                                                #region Text Alert

                                                var textAlertTemplatePath = Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.MembershipAccountVerification));

                                                if (File.Exists(textAlertTemplatePath))
                                                {
                                                    var template = File.ReadAllText(textAlertTemplatePath);

                                                    dynamic expando = new ExpandoObject();

                                                    var model = expando as IDictionary<string, object>;

                                                    model.Add("FirstName", userDTO.FirstName);
                                                    model.Add("Token", queueDTO.Token);
                                                    model.Add("CompanyDescription", branchDTO.CompanyDescription);

                                                    var textResult = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.MembershipAccountVerification), null, model);

                                                    TextAlertDTO textAlertDTO = new TextAlertDTO
                                                    {
                                                        BranchId = userDTO.BranchId.Value,
                                                        TextMessageBody = textResult,
                                                        TextMessageRecipient = userDTO.PhoneNumber,
                                                        TextMessageDLRStatus = (int)DLRStatus.Pending,
                                                        TextMessageOrigin = (int)MessageOrigin.Within,
                                                        TextMessagePriority = (int)QueuePriority.High,
                                                        TextMessageSecurityCritical = true
                                                    };

                                                //    await _channelService.AddTextAlertAsync(textAlertDTO, serviceHeader);
                                                }

                                                #endregion

                                                break;

                                            case TwoFactorProviders.EmailCode:

                                                #region Email Alert

                                                var emailAlertTemplatePath = Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.MembershipAccountVerification));

                                                if (File.Exists(emailAlertTemplatePath))
                                                {
                                                    var template = File.ReadAllText(emailAlertTemplatePath);

                                                    dynamic expando = new ExpandoObject();

                                                    var model = expando as IDictionary<string, object>;

                                                    model.Add("FirstName", userDTO.FirstName);
                                                    model.Add("Token", queueDTO.Token);
                                                    model.Add("CompanyDescription", branchDTO.CompanyDescription);

                                                    var emailResult = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.MembershipAccountVerification), null, model);

                                                    EmailAlertDTO emailAlertDTO = new EmailAlertDTO
                                                    {
                                                        BranchId = userDTO.BranchId.Value,
                                                        MailMessageFrom = branchDTO.CompanyAddressEmail,
                                                        MailMessageTo = userDTO.Email,
                                                        MailMessageSubject = "Account Verification",
                                                        MailMessageBody = emailResult,
                                                        MailMessageIsBodyHtml = true,
                                                        MailMessageDLRStatus = (int)DLRStatus.Pending,
                                                        MailMessageOrigin = (int)MessageOrigin.Within,
                                                        MailMessageSecurityCritical = true
                                                    };

                                                    await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                }

                                                #endregion

                                                break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.MembershipResetPassword:

                                    #region Do we need to send alerts?

                                    userDTO = await _channelService.FindMembershipAsync(queueDTO.RecordId.ToString(), serviceHeader);

                                    if (userDTO != null)
                                    {
                                        var branchDTO = await _channelService.FindBranchAsync((Guid)userDTO.BranchId, serviceHeader);

                                        #region Email Alert

                                        var emailAlertTemplatePath = Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.MembershipResetPassword));

                                        if (File.Exists(emailAlertTemplatePath))
                                        {
                                            var template = File.ReadAllText(emailAlertTemplatePath);

                                            dynamic expando = new ExpandoObject();

                                            var model = expando as IDictionary<string, object>;

                                            model.Add("FirstName", userDTO.FirstName);
                                            model.Add("CallbackUrl", queueDTO.CallbackUrl);
                                            model.Add("CompanyDescription", branchDTO.CompanyDescription);

                                            var emailResult = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.MembershipResetPassword), null, model);

                                            EmailAlertDTO emailAlertDTO = new EmailAlertDTO
                                            {
                                                BranchId = userDTO.BranchId.Value,
                                                MailMessageFrom = branchDTO.CompanyAddressEmail,
                                                MailMessageTo = userDTO.Email,
                                                MailMessageSubject = "Reset Password",
                                                MailMessageBody = emailResult,
                                                MailMessageIsBodyHtml = true,
                                                MailMessageDLRStatus = (int)DLRStatus.Pending,
                                                MailMessageOrigin = (int)MessageOrigin.Within,
                                                MailMessageSecurityCritical = true
                                            };

                                            await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                        }

                                        #endregion
                                    }

                                    #endregion

                                    break;;

                                case AccountAlertTrigger.Transaction:

                                    #region Do we need to send alerts?

                                    var journalEntryDTOs = await _channelService.FindJournalEntriesByJournalIdAsync(queueDTO.RecordId, serviceHeader);

                                    if (journalEntryDTOs != null && journalEntryDTOs.Any(x => x.CustomerAccountId.HasValue))
                                    {
                                        var skipList = new List<Guid>();

                                        foreach (var journalEntryDTO in journalEntryDTOs)
                                        {
                                            if (journalEntryDTO.CustomerAccountId.HasValue && journalEntryDTO.CustomerAccountId.Value != Guid.Empty)
                                            {
                                                if (journalEntryDTO.JournalIsLocked)
                                                    continue;
                                                else if (journalEntryDTO.JournalSuppressAccountAlert)
                                                    continue;
                                                else if (skipList.Contains(journalEntryDTO.CustomerAccountCustomerId.Value))
                                                    continue;
                                                else skipList.Add(journalEntryDTO.CustomerAccountCustomerId.Value);

                                                var customerAccountDTO = await _channelService.FindCustomerAccountAsync(journalEntryDTO.CustomerAccountId.Value, true, true, false, false, serviceHeader);

                                                var transactionAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(customerAccountDTO.CustomerId, journalEntryDTO.JournalTransactionCode, serviceHeader);

                                                if (transactionAccountAlertDTOs != null && transactionAccountAlertDTOs.Any())
                                                {
                                                    foreach (var accountAlertDTO in transactionAccountAlertDTOs)
                                                    {
                                                        if (journalEntryDTO.JournalTotalValue > accountAlertDTO.Threshold)
                                                        {
                                                            #region Text Alert

                                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine.Trim(), @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Trim().Length >= 13)
                                                            {
                                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", (SystemTransactionCode)journalEntryDTO.JournalTransactionCode));

                                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                                {
                                                                    dynamic expando = new ExpandoObject();

                                                                    var model = expando as IDictionary<string, object>;

                                                                    model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", journalEntryDTO.JournalTotalValue).ReplaceNumbers() : string.Format(_nfi, "{0:C}", journalEntryDTO.JournalTotalValue));
                                                                    model.Add("PrimaryDescription", journalEntryDTO.JournalPrimaryDescription);
                                                                    model.Add("SecondaryDescription", journalEntryDTO.JournalSecondaryDescription);
                                                                    model.Add("Reference", journalEntryDTO.JournalReference);
                                                                    model.Add("CompanyDescription", journalEntryDTO.JournalBranchCompanyDescription);
                                                                    model.Add("BranchDescription", journalEntryDTO.JournalBranchDescription);
                                                                    model.Add("ValueDate", journalEntryDTO.ValueDate.HasValue ? journalEntryDTO.ValueDate.Value.ToString("dd/MMM/yyyy") : "N/A");

                                                                    var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                                    var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", journalEntryDTO.JournalTransactionCode), null, model);

                                                                    var textAlertDTO = new TextAlertDTO
                                                                    {
                                                                        BranchId = journalEntryDTO.JournalBranchId,
                                                                        TextMessageOrigin = (int)MessageOrigin.Within,
                                                                        TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                        TextMessageBody = result,
                                                                        MessageCategory = (int)MessageCategory.SMSAlert,
                                                                        AppendSignature = false,
                                                                        TextMessagePriority = (int)QueuePriority.Highest,
                                                                    };

                                                                    serviceHeader.ApplicationUserName = textAlertDTO.CreatedBy;

                                                                    var textMessageTariffs = new ObservableCollection<TariffWrapper>();

                                                                    switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                                                                    {
                                                                        case ProductCode.Savings:

                                                                            textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync(journalEntryDTO.JournalTransactionCode, journalEntryDTO.JournalTotalValue, customerAccountDTO, serviceHeader);

                                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                                            if ((customerAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                                            {
                                                                                if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                                {
                                                                                    if (textMessageTariffs.Any())
                                                                                    {
                                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(journalEntryDTO.JournalId, journalEntryDTO.JournalBranchId, journalEntryDTO.JournalAlternateChannelLogId ?? Guid.Empty, journalEntryDTO.JournalSecondaryDescription, journalEntryDTO.JournalReference, journalEntryDTO.JournalModuleNavigationItemCode, journalEntryDTO.JournalTransactionCode, customerAccountDTO, customerAccountDTO, textMessageTariffs, serviceHeader);
                                                                                    }
                                                                                }
                                                                            }

                                                                            break;

                                                                        case ProductCode.Investment:
                                                                        case ProductCode.Loan:

                                                                            CustomerAccountDTO customerSavingsAccountDTO = null;

                                                                            var defaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                                                            var customerSavingsAccounts = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(customerAccountDTO.CustomerId, defaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                                                            if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                                                                            {
                                                                                customerSavingsAccountDTO = customerSavingsAccounts.First();

                                                                                textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync(journalEntryDTO.JournalTransactionCode, journalEntryDTO.JournalTotalValue, customerSavingsAccountDTO, serviceHeader);

                                                                                var tariffAmount_1 = textMessageTariffs.Sum(x => x.Amount);

                                                                                if ((customerSavingsAccountDTO.AvailableBalance - tariffAmount_1) >= 0)
                                                                                {
                                                                                    if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                                    {
                                                                                        if (textMessageTariffs.Any())
                                                                                        {
                                                                                            await _channelService.AddTariffJournalsWithCustomerAccountAsync(journalEntryDTO.JournalId, journalEntryDTO.JournalBranchId, journalEntryDTO.JournalAlternateChannelLogId ?? Guid.Empty, journalEntryDTO.JournalSecondaryDescription, journalEntryDTO.JournalReference, journalEntryDTO.JournalModuleNavigationItemCode, journalEntryDTO.JournalTransactionCode, customerSavingsAccountDTO, customerSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                                        }
                                                                                    }
                                                                                }

                                                                            }
                                                                            break;
                                                                    }
                                                                }
                                                            }

                                                            #endregion

                                                            #region Email Alert

                                                            if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail.Trim(), @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(journalEntryDTO.JournalBranchAddressEmail) && Regex.IsMatch(journalEntryDTO.JournalBranchAddressEmail.Trim(), @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                                {
                                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (SystemTransactionCode)journalEntryDTO.JournalTransactionCode));

                                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                                    {
                                                                        dynamic expando = new ExpandoObject();

                                                                        var model = expando as IDictionary<string, object>;

                                                                        model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", journalEntryDTO.JournalTotalValue).ReplaceNumbers() : string.Format(_nfi, "{0:C}", journalEntryDTO.JournalTotalValue));
                                                                        model.Add("PrimaryDescription", journalEntryDTO.JournalPrimaryDescription);
                                                                        model.Add("SecondaryDescription", journalEntryDTO.JournalSecondaryDescription);
                                                                        model.Add("Reference", journalEntryDTO.JournalReference);
                                                                        model.Add("CompanyDescription", journalEntryDTO.JournalBranchCompanyDescription);
                                                                        model.Add("BranchDescription", journalEntryDTO.JournalBranchDescription);
                                                                        model.Add("ValueDate", journalEntryDTO.ValueDate.HasValue ? journalEntryDTO.ValueDate.Value.ToString("dd/MMM/yyyy") : "N/A");

                                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", journalEntryDTO.JournalTransactionCode), null, model);

                                                                        var emailAlertDTO = new EmailAlertDTO
                                                                        {
                                                                            MailMessageFrom = journalEntryDTO.JournalBranchAddressEmail,
                                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription((SystemTransactionCode)journalEntryDTO.JournalTransactionCode)),
                                                                            MailMessageBody = result,
                                                                            MailMessageIsBodyHtml = true,
                                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                                        };

                                                                        serviceHeader.ApplicationUserName = emailAlertDTO.CreatedBy;

                                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                                    }
                                                                }
                                                            }

                                                            #endregion
                                                        }

                                                        break;/*precaution to avoid multiple alerts*/
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.LoanGuarantee:

                                    #region Do we need to send alerts?

                                    var loanCaseDTO = await _channelService.FindLoanCaseAsync(queueDTO.RecordId, serviceHeader);

                                    var loanGuarantorDTOs = await _channelService.FindLoanGuarantorsByLoanCaseIdAsync(loanCaseDTO.Id, serviceHeader);

                                    if (loanGuarantorDTOs != null && loanGuarantorDTOs.Any())
                                    {
                                        CustomerAccountDTO loaneeSavingsAccountDTO = null;

                                        var loaneeSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(loanCaseDTO.CustomerId, loanCaseDTO.SavingsProductId ?? Guid.Empty, true, true, false, false, serviceHeader);

                                        if (loaneeSavingsAccountDTOs != null && loaneeSavingsAccountDTOs.Any())
                                            loaneeSavingsAccountDTO = loaneeSavingsAccountDTOs[0];

                                        if (loaneeSavingsAccountDTO != null)
                                        {
                                            foreach (var item in loanGuarantorDTOs)
                                            {
                                                var loanGuaranteeAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(item.CustomerId, (int)SystemTransactionCode.LoanGuarantee, serviceHeader);

                                                if (loanGuaranteeAccountAlertDTOs != null && loanGuaranteeAccountAlertDTOs.Any())
                                                {
                                                    foreach (var accountAlertDTO in loanGuaranteeAccountAlertDTOs)
                                                    {
                                                        #region Text Alert

                                                        if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                        {
                                                            var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.LoanGuarantee));

                                                            if (System.IO.File.Exists(textAlertTemplatePath))
                                                            {
                                                                var loanee = await _channelService.FindCustomerAsync(loanCaseDTO.CustomerId, serviceHeader);

                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("LoaneeFullName", loanee.FullName);
                                                                model.Add("LoaneePersonalFileNumber", loanee.Reference3);
                                                                model.Add("LoanProductName", loanCaseDTO.LoanProductDescription);
                                                                model.Add("AmountGuaranteed", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", item.AmountGuaranteed).ReplaceNumbers() : string.Format(_nfi, "{0:C}", item.AmountGuaranteed));
                                                                model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", loanCaseDTO.AmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", loanCaseDTO.AmountApplied));
                                                                model.Add("CompanyDescription", loanCaseDTO.BranchCompanyDescription);
                                                                model.Add("BranchDescription", loanCaseDTO.BranchDescription);

                                                                var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.LoanGuarantee), null, model);

                                                                var textAlertDTO = new TextAlertDTO
                                                                {
                                                                    BranchId = loanCaseDTO.BranchId,
                                                                    TextMessageOrigin = (int)MessageOrigin.Within,
                                                                    TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                    TextMessageBody = result,
                                                                    MessageCategory = (int)MessageCategory.SMSAlert,
                                                                    AppendSignature = false,
                                                                    TextMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                serviceHeader.ApplicationUserName = textAlertDTO.CreatedBy;

                                                                var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.LoanGuarantee, loanCaseDTO.AmountApplied, loaneeSavingsAccountDTO, serviceHeader);

                                                                var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                                if ((loaneeSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                                {
                                                                    if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                    {
                                                                        if (textMessageTariffs.Any())
                                                                        {
                                                                            await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, loanCaseDTO.BranchId, Guid.Empty, loanCaseDTO.LoanProductDescription, loanee.Reference3, 0x9999, (int)SystemTransactionCode.LoanGuarantee, loaneeSavingsAccountDTO, loaneeSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        #endregion

                                                        #region Email Alert

                                                        if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(loanCaseDTO.BranchAddressEmail) && Regex.IsMatch(loanCaseDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                            {
                                                                var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.LoanGuarantee));

                                                                if (System.IO.File.Exists(emailAlertTemplatePath))
                                                                {
                                                                    var loanee = await _channelService.FindCustomerAsync(loanCaseDTO.CustomerId, serviceHeader);

                                                                    dynamic expando = new ExpandoObject();

                                                                    var model = expando as IDictionary<string, object>;

                                                                    model.Add("LoaneeFullName", loanee.FullName);
                                                                    model.Add("LoaneePersonalFileNumber", loanee.Reference3);
                                                                    model.Add("LoanProductName", loanCaseDTO.LoanProductDescription);
                                                                    model.Add("AmountGuaranteed", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", item.AmountGuaranteed).ReplaceNumbers() : string.Format(_nfi, "{0:C}", item.AmountGuaranteed));
                                                                    model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", loanCaseDTO.AmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", loanCaseDTO.AmountApplied));
                                                                    model.Add("CompanyDescription", loanCaseDTO.BranchCompanyDescription);
                                                                    model.Add("BranchDescription", loanCaseDTO.BranchDescription);

                                                                    var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                    var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.LoanGuarantee), null, model);

                                                                    var emailAlertDTO = new EmailAlertDTO
                                                                    {
                                                                        MailMessageFrom = loanCaseDTO.BranchAddressEmail,
                                                                        MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                        MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.LoanGuarantee)),
                                                                        MailMessageBody = result,
                                                                        MailMessageIsBodyHtml = true,
                                                                        MailMessageOrigin = (int)MessageOrigin.Within,
                                                                        MailMessagePriority = (int)QueuePriority.Highest,
                                                                    };

                                                                    serviceHeader.ApplicationUserName = emailAlertDTO.CreatedBy;

                                                                    await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                                }
                                                            }
                                                        }

                                                        #endregion

                                                        break;/*precaution to avoid multiple alerts*/
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.GuarantorSubstitution:

                                    #region Do we need to send alerts?

                                    var substitutionDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (substitutionDefaultSavingsProductDTO != null)
                                    {
                                        var substitutionLoanGuarantorDTO = await _channelService.FindLoanGuarantorAsync(queueDTO.RecordId, serviceHeader);

                                        CustomerAccountDTO substitutionLoanGuarantorSavingsAccountDTO = null;

                                        var substitutionLoanGuarantorSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(queueDTO.AccountAlertCustomerId, substitutionDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (substitutionLoanGuarantorSavingsAccountDTOs != null && substitutionLoanGuarantorSavingsAccountDTOs.Any())
                                            substitutionLoanGuarantorSavingsAccountDTO = substitutionLoanGuarantorSavingsAccountDTOs[0];

                                        if (substitutionLoanGuarantorSavingsAccountDTO != null)
                                        {
                                            var guarantorSubstitutionAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(substitutionLoanGuarantorDTO.CustomerId, (int)SystemTransactionCode.GuarantorSubstitution, serviceHeader);

                                            if (guarantorSubstitutionAccountAlertDTOs != null && guarantorSubstitutionAccountAlertDTOs.Any())
                                            {
                                                foreach (var accountAlertDTO in guarantorSubstitutionAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.GuarantorSubstitution));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            dynamic expando = new ExpandoObject();

                                                            var model = expando as IDictionary<string, object>;

                                                            model.Add("PreviousGuarantorFullName", queueDTO.AccountAlertPrimaryDescription);
                                                            model.Add("PreviousGuarantorPersonalFileNumber", queueDTO.AccountAlertReference);
                                                            model.Add("LoaneeFullName", queueDTO.LoaneeCustomerFullName);
                                                            model.Add("LoanProductName", queueDTO.AccountAlertSecondaryDescription);
                                                            model.Add("AmountGuaranteed", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.AmountGuaranteed).ReplaceNumbers() : string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.AmountGuaranteed));
                                                            model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.LoanCaseAmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.LoanCaseAmountApplied));
                                                            model.Add("CompanyDescription", substitutionLoanGuarantorDTO.LoanCaseBranchCompanyDescription);
                                                            model.Add("BranchDescription", substitutionLoanGuarantorDTO.LoanCaseBranchDescription);

                                                            var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                            var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.GuarantorSubstitution), null, model);

                                                            var textAlertDTO = new TextAlertDTO
                                                            {
                                                                BranchId = substitutionLoanGuarantorSavingsAccountDTO.BranchId,
                                                                TextMessageOrigin = (int)MessageOrigin.Within,
                                                                TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                TextMessageBody = result,
                                                                MessageCategory = (int)MessageCategory.SMSAlert,
                                                                AppendSignature = false,
                                                                TextMessagePriority = (int)QueuePriority.Highest,
                                                            };

                                                            var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.GuarantorSubstitution, 0m, substitutionLoanGuarantorSavingsAccountDTO, serviceHeader);

                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                            if ((substitutionLoanGuarantorSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                            {
                                                                if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                {
                                                                    if (textMessageTariffs.Any())
                                                                    {
                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, substitutionLoanGuarantorSavingsAccountDTO.BranchId, Guid.Empty, queueDTO.AccountAlertSecondaryDescription, queueDTO.AccountAlertReference, 0x9999, (int)SystemTransactionCode.GuarantorSubstitution, substitutionLoanGuarantorSavingsAccountDTO, substitutionLoanGuarantorSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region Email Alert

                                                    if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(substitutionLoanGuarantorDTO.LoanCaseBranchAddressEmail) && Regex.IsMatch(substitutionLoanGuarantorDTO.LoanCaseBranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.GuarantorSubstitution));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("PreviousGuarantorFullName", queueDTO.AccountAlertPrimaryDescription);
                                                                model.Add("PreviousGuarantorPersonalFileNumber", queueDTO.AccountAlertReference);
                                                                model.Add("LoaneeFullName", queueDTO.LoaneeCustomerFullName);
                                                                model.Add("LoanProductName", queueDTO.AccountAlertSecondaryDescription);
                                                                model.Add("AmountGuaranteed", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.AmountGuaranteed).ReplaceNumbers() : string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.AmountGuaranteed));
                                                                model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.LoanCaseAmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", substitutionLoanGuarantorDTO.LoanCaseAmountApplied));
                                                                model.Add("CompanyDescription", substitutionLoanGuarantorDTO.LoanCaseBranchCompanyDescription);
                                                                model.Add("BranchDescription", substitutionLoanGuarantorDTO.LoanCaseBranchDescription);

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.GuarantorSubstitution), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = substitutionLoanGuarantorDTO.LoanCaseBranchAddressEmail,
                                                                    MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.GuarantorSubstitution)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.GuarantorAttachment:

                                    #region Do we need to send alerts?

                                    var attachmentDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (attachmentDefaultSavingsProductDTO != null)
                                    {
                                        var attachmentLoanGuarantorDTO = await _channelService.FindLoanGuarantorAsync(queueDTO.RecordId, serviceHeader);

                                        CustomerAccountDTO attachmentLoanGuarantorSavingsAccountDTO = null;

                                        var attachmentLoanGuarantorSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(attachmentLoanGuarantorDTO.CustomerId, attachmentDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (attachmentLoanGuarantorSavingsAccountDTOs != null && attachmentLoanGuarantorSavingsAccountDTOs.Any())
                                            attachmentLoanGuarantorSavingsAccountDTO = attachmentLoanGuarantorSavingsAccountDTOs[0];

                                        if (attachmentLoanGuarantorSavingsAccountDTO != null)
                                        {
                                            var guarantorAttachmentAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(attachmentLoanGuarantorDTO.CustomerId, (int)SystemTransactionCode.GuarantorAttachment, serviceHeader);

                                            if (guarantorAttachmentAccountAlertDTOs != null && guarantorAttachmentAccountAlertDTOs.Any())
                                            {
                                                foreach (var accountAlertDTO in guarantorAttachmentAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.GuarantorAttachment));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            dynamic expando = new ExpandoObject();

                                                            var model = expando as IDictionary<string, object>;

                                                            model.Add("GuarantorPersonalFileNumber", queueDTO.AccountAlertPrimaryDescription);
                                                            model.Add("LoaneeFullName", queueDTO.LoaneeCustomerFullName);
                                                            model.Add("LoanProductName", queueDTO.AccountAlertSecondaryDescription);
                                                            model.Add("LoanCasePaddedCaseNumber", queueDTO.AccountAlertReference);
                                                            model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue).ReplaceNumbers() : string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue));
                                                            model.Add("CompanyDescription", attachmentLoanGuarantorSavingsAccountDTO.BranchCompanyDescription);
                                                            model.Add("BranchDescription", attachmentLoanGuarantorSavingsAccountDTO.BranchDescription);

                                                            var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                            var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.GuarantorAttachment), null, model);

                                                            var textAlertDTO = new TextAlertDTO
                                                            {
                                                                BranchId = attachmentLoanGuarantorSavingsAccountDTO.BranchId,
                                                                TextMessageOrigin = (int)MessageOrigin.Within,
                                                                TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                TextMessageBody = result,
                                                                MessageCategory = (int)MessageCategory.SMSAlert,
                                                                AppendSignature = false,
                                                                TextMessagePriority = (int)QueuePriority.Highest,
                                                            };

                                                            var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.GuarantorAttachment, 0m, attachmentLoanGuarantorSavingsAccountDTO, serviceHeader);

                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                            if ((attachmentLoanGuarantorSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                            {
                                                                if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                {
                                                                    if (textMessageTariffs.Any())
                                                                    {
                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, attachmentLoanGuarantorSavingsAccountDTO.BranchId, Guid.Empty, queueDTO.AccountAlertSecondaryDescription, queueDTO.AccountAlertReference, 0x9999, (int)SystemTransactionCode.GuarantorAttachment, attachmentLoanGuarantorSavingsAccountDTO, attachmentLoanGuarantorSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region Email Alert

                                                    if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(attachmentLoanGuarantorSavingsAccountDTO.BranchAddressEmail) && Regex.IsMatch(attachmentLoanGuarantorSavingsAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.GuarantorAttachment));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("GuarantorPersonalFileNumber", queueDTO.AccountAlertPrimaryDescription);
                                                                model.Add("LoaneeFullName", queueDTO.LoaneeCustomerFullName);
                                                                model.Add("LoanProductName", queueDTO.AccountAlertSecondaryDescription);
                                                                model.Add("LoanCasePaddedCaseNumber", queueDTO.AccountAlertReference);
                                                                model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue).ReplaceNumbers() : string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue));
                                                                model.Add("CompanyDescription", attachmentLoanGuarantorSavingsAccountDTO.BranchCompanyDescription);
                                                                model.Add("BranchDescription", attachmentLoanGuarantorSavingsAccountDTO.BranchDescription);

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.GuarantorAttachment), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = attachmentLoanGuarantorSavingsAccountDTO.BranchAddressEmail,
                                                                    MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.GuarantorAttachment)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.GuarantorRelieving:

                                    #region Do we need to send alerts?

                                    var relievingDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (relievingDefaultSavingsProductDTO != null)
                                    {
                                        var loanGuarantorAttachmentHistoryEntryDTO = await _channelService.FindLoanGuarantorAttachmentHistoryEntryAsync(queueDTO.RecordId, serviceHeader);

                                        CustomerAccountDTO loaneeSavingsAccountDTO = null;

                                        var loaneeSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(queueDTO.AccountAlertCustomerId, relievingDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (loaneeSavingsAccountDTOs != null && loaneeSavingsAccountDTOs.Any())
                                            loaneeSavingsAccountDTO = loaneeSavingsAccountDTOs[0];

                                        if (loaneeSavingsAccountDTO != null)
                                        {
                                            var guarantorRelievingAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(loanGuarantorAttachmentHistoryEntryDTO.DestinationCustomerAccountCustomerId, (int)SystemTransactionCode.GuarantorRelieving, serviceHeader);

                                            if (guarantorRelievingAccountAlertDTOs != null && guarantorRelievingAccountAlertDTOs.Any())
                                            {
                                                foreach (var accountAlertDTO in guarantorRelievingAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.GuarantorRelieving));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            dynamic expando = new ExpandoObject();

                                                            var model = expando as IDictionary<string, object>;

                                                            model.Add("GuarantorPersonalFileNumber", queueDTO.AccountAlertPrimaryDescription);
                                                            model.Add("LoaneeFullName", queueDTO.LoaneeCustomerFullName);
                                                            model.Add("LoanProductName", queueDTO.AccountAlertSecondaryDescription);
                                                            model.Add("Reference", queueDTO.AccountAlertReference);
                                                            model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue).ReplaceNumbers() : string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue));
                                                            model.Add("CompanyDescription", loaneeSavingsAccountDTO.BranchCompanyDescription);
                                                            model.Add("BranchDescription", loaneeSavingsAccountDTO.BranchDescription);

                                                            var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                            var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.GuarantorRelieving), null, model);

                                                            var textAlertDTO = new TextAlertDTO
                                                            {
                                                                BranchId = loaneeSavingsAccountDTO.BranchId,
                                                                TextMessageOrigin = (int)MessageOrigin.Within,
                                                                TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                TextMessageBody = result,
                                                                MessageCategory = (int)MessageCategory.SMSAlert,
                                                                AppendSignature = false,
                                                                TextMessagePriority = (int)QueuePriority.Highest,
                                                            };

                                                            var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.GuarantorRelieving, 0m, loaneeSavingsAccountDTO, serviceHeader);

                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                            if ((loaneeSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                            {
                                                                if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                {
                                                                    if (textMessageTariffs.Any())
                                                                    {
                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, loaneeSavingsAccountDTO.BranchId, Guid.Empty, queueDTO.AccountAlertSecondaryDescription, queueDTO.AccountAlertReference, 0x9999, (int)SystemTransactionCode.GuarantorRelieving, loaneeSavingsAccountDTO, loaneeSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region Email Alert

                                                    if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(loaneeSavingsAccountDTO.BranchAddressEmail) && Regex.IsMatch(loaneeSavingsAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.GuarantorRelieving));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("GuarantorPersonalFileNumber", queueDTO.AccountAlertPrimaryDescription);
                                                                model.Add("LoaneeFullName", queueDTO.LoaneeCustomerFullName);
                                                                model.Add("LoanProductName", queueDTO.AccountAlertSecondaryDescription);
                                                                model.Add("Reference", queueDTO.AccountAlertReference);
                                                                model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue).ReplaceNumbers() : string.Format(_nfi, "{0:C}", queueDTO.AccountAlertTotalValue));
                                                                model.Add("CompanyDescription", loaneeSavingsAccountDTO.BranchCompanyDescription);
                                                                model.Add("BranchDescription", loaneeSavingsAccountDTO.BranchDescription);

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.GuarantorRelieving), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = loaneeSavingsAccountDTO.BranchAddressEmail,
                                                                    MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.GuarantorRelieving)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.ElectronicStatement:

                                    #region Generate email with attachment?

                                    var electronicStatementOrderHistoryDTO = await _channelService.FindElectronicStatementOrderHistoryAsync(queueDTO.RecordId, serviceHeader);

                                    if (electronicStatementOrderHistoryDTO != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(electronicStatementOrderHistoryDTO.CustomerAccountCustomerAddressEmail) && Regex.IsMatch(electronicStatementOrderHistoryDTO.CustomerAccountCustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
                                            && !string.IsNullOrWhiteSpace(electronicStatementOrderHistoryDTO.Sender) && Regex.IsMatch(electronicStatementOrderHistoryDTO.Sender, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                        {
                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.ElectronicStatementOrder));

                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                            {
                                                dynamic expando = new ExpandoObject();

                                                var model = expando as IDictionary<string, object>;

                                                model.Add("PrimaryDescription", queueDTO.AccountAlertPrimaryDescription);
                                                model.Add("SecondaryDescription", electronicStatementOrderHistoryDTO.CustomerAccountCustomerFullName);
                                                model.Add("Reference", queueDTO.AccountAlertReference);
                                                model.Add("BranchDescription", electronicStatementOrderHistoryDTO.CustomerAccountBranchDescription);
                                                model.Add("CompanyDescription", electronicStatementOrderHistoryDTO.CustomerAccountBranchCompanyDescription);

                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.ElectronicStatementOrder), null, model);

                                                var emailAlertDTO = new EmailAlertDTO
                                                {
                                                    MailMessageFrom = electronicStatementOrderHistoryDTO.Sender,
                                                    MailMessageTo = electronicStatementOrderHistoryDTO.CustomerAccountCustomerAddressEmail,
                                                    MailMessageSubject = string.Format("{0}_{1}", EnumHelper.GetDescription(SystemTransactionCode.ElectronicStatementOrder), queueDTO.AccountAlertReference),
                                                    MailMessageBody = result,
                                                    MailMessageIsBodyHtml = true,
                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                    MailMessageAttachments = string.Join(",", electronicStatementOrderHistoryDTO.Id)
                                                };

                                                serviceHeader.ApplicationUserName = electronicStatementOrderHistoryDTO.CreatedBy;

                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.LoanRequest:

                                    #region Do we need to send alerts?

                                    var loanRequestDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (loanRequestDefaultSavingsProductDTO != null)
                                    {
                                        var loanRequestDTO = await _channelService.FindLoanRequestAsync(queueDTO.RecordId, serviceHeader);

                                        CustomerAccountDTO loaneeSavingsAccountDTO = null;

                                        var loaneeSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(loanRequestDTO.CustomerId, loanRequestDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (loaneeSavingsAccountDTOs != null && loaneeSavingsAccountDTOs.Any())
                                            loaneeSavingsAccountDTO = loaneeSavingsAccountDTOs[0];

                                        if (loaneeSavingsAccountDTO != null)
                                        {
                                            var loanRequestAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(loanRequestDTO.CustomerId, (int)SystemTransactionCode.LoanRequest, serviceHeader);

                                            if (loanRequestAccountAlertDTOs != null && loanRequestAccountAlertDTOs.Any())
                                            {
                                                foreach (var accountAlertDTO in loanRequestAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.LoanRequest));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            dynamic expando = new ExpandoObject();

                                                            var model = expando as IDictionary<string, object>;

                                                            model.Add("LoanProductName", loanRequestDTO.LoanProductDescription);
                                                            model.Add("LoanPurpose", loanRequestDTO.LoanPurposeDescription);
                                                            model.Add("Reference", loanRequestDTO.Reference);
                                                            model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", loanRequestDTO.AmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", loanRequestDTO.AmountApplied));
                                                            model.Add("CompanyDescription", loaneeSavingsAccountDTO.BranchCompanyDescription);
                                                            model.Add("BranchDescription", loaneeSavingsAccountDTO.BranchDescription);

                                                            var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                            var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.LoanRequest), null, model);

                                                            var textAlertDTO = new TextAlertDTO
                                                            {
                                                                BranchId = loaneeSavingsAccountDTO.BranchId,
                                                                TextMessageOrigin = (int)MessageOrigin.Within,
                                                                TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                TextMessageBody = result,
                                                                MessageCategory = (int)MessageCategory.SMSAlert,
                                                                AppendSignature = false,
                                                                TextMessagePriority = (int)QueuePriority.Highest,
                                                            };

                                                            var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.LoanRequest, 0m, loaneeSavingsAccountDTO, serviceHeader);

                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                            if ((loaneeSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                            {
                                                                if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                {
                                                                    if (textMessageTariffs.Any())
                                                                    {
                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, loaneeSavingsAccountDTO.BranchId, Guid.Empty, queueDTO.AccountAlertSecondaryDescription, queueDTO.AccountAlertReference, 0x9999, (int)SystemTransactionCode.LoanRequest, loaneeSavingsAccountDTO, loaneeSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }

                                                    #endregion

                                                    #region Email Alert

                                                    if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(loaneeSavingsAccountDTO.BranchAddressEmail) && Regex.IsMatch(loaneeSavingsAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.LoanRequest));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("LoanProductName", loanRequestDTO.LoanProductDescription);
                                                                model.Add("LoanPurpose", loanRequestDTO.LoanPurposeDescription);
                                                                model.Add("Reference", loanRequestDTO.Reference);
                                                                model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", loanRequestDTO.AmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", loanRequestDTO.AmountApplied));
                                                                model.Add("CompanyDescription", loaneeSavingsAccountDTO.BranchCompanyDescription);
                                                                model.Add("BranchDescription", loaneeSavingsAccountDTO.BranchDescription);

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.LoanRequest), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = loaneeSavingsAccountDTO.BranchAddressEmail,
                                                                    MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.LoanRequest)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.MobileToBankSenderAcknowledgement:

                                    #region Do we need to send alerts?

                                    var mobileToBankDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (mobileToBankDefaultSavingsProductDTO != null)
                                    {
                                        var mobileToBankRequestDTO = await _channelService.FindMobileToBankRequestAsync(queueDTO.RecordId, serviceHeader);

                                        CustomerAccountDTO mobileToBankRecipientSavingsAccountDTO = null;

                                        var mobileToBankRecipientSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(mobileToBankRequestDTO.CustomerAccountCustomerId, mobileToBankDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (mobileToBankRecipientSavingsAccountDTOs != null && mobileToBankRecipientSavingsAccountDTOs.Any())
                                            mobileToBankRecipientSavingsAccountDTO = mobileToBankRecipientSavingsAccountDTOs[0];

                                        if (mobileToBankRecipientSavingsAccountDTO != null)
                                        {
                                            var mobileToBankSenderAcknowledgementAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(mobileToBankRequestDTO.CustomerAccountCustomerId, (int)SystemTransactionCode.MobileToBankSenderAcknowledgement, serviceHeader);

                                            if (mobileToBankSenderAcknowledgementAccountAlertDTOs != null && mobileToBankSenderAcknowledgementAccountAlertDTOs.Any())
                                            {
                                                foreach (var accountAlertDTO in mobileToBankSenderAcknowledgementAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        if (string.Format("{0}", accountAlertDTO.CustomerAddressMobileLine).Trim().Equals(string.Format("+{0}", mobileToBankRequestDTO.MSISDN).Trim(), StringComparison.OrdinalIgnoreCase))
                                                            continue; /*ignore if sender is same as recipient*/

                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.MobileToBankSenderAcknowledgement));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            dynamic expando = new ExpandoObject();

                                                            var model = expando as IDictionary<string, object>;

                                                            model.Add("CustomerAccountCustomerFullName", mobileToBankRequestDTO.CustomerAccountCustomerFullName);
                                                            model.Add("BillRefNumber", mobileToBankRequestDTO.BillRefNumber);
                                                            model.Add("TransID", mobileToBankRequestDTO.TransID);
                                                            model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", mobileToBankRequestDTO.TransAmount).ReplaceNumbers() : string.Format(_nfi, "{0:C}", mobileToBankRequestDTO.TransAmount));
                                                            model.Add("CompanyDescription", mobileToBankRecipientSavingsAccountDTO.BranchCompanyDescription);
                                                            model.Add("BranchDescription", mobileToBankRecipientSavingsAccountDTO.BranchDescription);

                                                            var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                            var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.MobileToBankSenderAcknowledgement), null, model);

                                                            var textAlertDTO = new TextAlertDTO
                                                            {
                                                                BranchId = mobileToBankRecipientSavingsAccountDTO.BranchId,
                                                                TextMessageOrigin = (int)MessageOrigin.Within,
                                                                TextMessageRecipient = string.Format("+{0}", mobileToBankRequestDTO.MSISDN),
                                                                TextMessageBody = result,
                                                                MessageCategory = (int)MessageCategory.SMSAlert,
                                                                AppendSignature = false,
                                                                TextMessagePriority = (int)QueuePriority.Highest,
                                                            };

                                                            var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.MobileToBankSenderAcknowledgement, 0m, mobileToBankRecipientSavingsAccountDTO, serviceHeader);

                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                            if ((mobileToBankRecipientSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                            {
                                                                if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                {
                                                                    if (textMessageTariffs.Any())
                                                                    {
                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, mobileToBankRecipientSavingsAccountDTO.BranchId, Guid.Empty, mobileToBankRequestDTO.KYCInfo, mobileToBankRequestDTO.TransID, 0x9999, (int)SystemTransactionCode.MobileToBankSenderAcknowledgement, mobileToBankRecipientSavingsAccountDTO, mobileToBankRecipientSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.AccountClosure:

                                    #region Do we need to send alerts?

                                    var closureDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (closureDefaultSavingsProductDTO != null)
                                    {
                                        CustomerAccountDTO customerSavingsAccountDTO = null;

                                        var accountClosureRequestDTO = await _channelService.FindAccountClosureRequestAsync(queueDTO.RecordId, true, serviceHeader);

                                        if (accountClosureRequestDTO != null)
                                        {
                                            var customerAccount = await _channelService.FindCustomerAccountAsync(accountClosureRequestDTO.CustomerAccountId, false, false, false, false, serviceHeader);

                                            var customerSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(customerAccount.CustomerId, closureDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                            if (customerSavingsAccountDTOs != null && customerSavingsAccountDTOs.Any())
                                                customerSavingsAccountDTO = customerSavingsAccountDTOs[0];

                                            if (customerSavingsAccountDTO != null)
                                            {
                                                var accountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(customerAccount.CustomerId, (int)SystemTransactionCode.AccountClosure, serviceHeader);

                                                if (accountAlertDTOs != null && accountAlertDTOs.Any())
                                                {
                                                    foreach (var accountAlertDTO in accountAlertDTOs)
                                                    {
                                                        #region Text alert

                                                        if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                        {
                                                            var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AccountClosure));

                                                            if (System.IO.File.Exists(textAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("ProductName", customerAccount.CustomerAccountTypeTargetProductDescription);
                                                                model.Add("TotalValue", queueDTO.AccountAlertTotalValue.ToString());
                                                                model.Add("Reason", accountClosureRequestDTO.Reason);
                                                                model.Add("CompanyDescription", customerAccount.BranchCompanyDescription);
                                                                model.Add("BranchDescription", customerAccount.BranchDescription);

                                                                var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AccountClosure), null, model);

                                                                var textAlertDTO = new TextAlertDTO
                                                                {
                                                                    BranchId = accountClosureRequestDTO.BranchId,
                                                                    TextMessageOrigin = (int)MessageOrigin.Within,
                                                                    TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                    TextMessageBody = result,
                                                                    MessageCategory = (int)MessageCategory.SMSAlert,
                                                                    AppendSignature = false,
                                                                    TextMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.AccountClosure, 0m, customerSavingsAccountDTO, serviceHeader);

                                                                var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                                if ((customerSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                                {
                                                                    if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                    {
                                                                        if (textMessageTariffs.Any())
                                                                        {
                                                                            await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, accountClosureRequestDTO.BranchId, Guid.Empty, customerAccount.CustomerAccountTypeTargetProductDescription, accountClosureRequestDTO.Reason, 0x9999, (int)SystemTransactionCode.AccountClosure, customerSavingsAccountDTO, customerSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                        }
                                                                    }
                                                                }


                                                            }
                                                        }

                                                        #endregion

                                                        #region Email Alert

                                                        if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(customerAccount.BranchAddressEmail) && Regex.IsMatch(customerAccount.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                            {
                                                                var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AccountClosure));

                                                                if (System.IO.File.Exists(emailAlertTemplatePath))
                                                                {
                                                                    dynamic expando = new ExpandoObject();

                                                                    var model = expando as IDictionary<string, object>;

                                                                    model.Add("ProductName", customerAccount.CustomerAccountTypeTargetProductDescription);
                                                                    model.Add("TotalValue", queueDTO.AccountAlertTotalValue.ToString());
                                                                    model.Add("Reason", accountClosureRequestDTO.Reason);
                                                                    model.Add("CompanyDescription", customerAccount.BranchCompanyDescription);
                                                                    model.Add("BranchDescription", customerAccount.BranchDescription);

                                                                    var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                    var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AccountClosure), null, model);

                                                                    var emailAlertDTO = new EmailAlertDTO
                                                                    {
                                                                        MailMessageFrom = customerAccount.BranchAddressEmail,
                                                                        MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                        MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AccountClosure)),
                                                                        MailMessageBody = result,
                                                                        MailMessageIsBodyHtml = true,
                                                                        MailMessageOrigin = (int)MessageOrigin.Within,
                                                                        MailMessagePriority = (int)QueuePriority.Highest,
                                                                    };

                                                                    await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                                }
                                                            }
                                                        }

                                                        #endregion

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    break;
                                case AccountAlertTrigger.MembershipApproval:

                                    #region Do we need to send alerts?

                                    var customer = await _channelService.FindCustomerAsync(queueDTO.RecordId, serviceHeader);

                                    if (customer != null)
                                    {

                                        #region Text Alert

                                        if (!string.IsNullOrWhiteSpace(customer.AddressMobileLine) && Regex.IsMatch(customer.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && customer.AddressMobileLine.Length >= 13)
                                        {
                                            var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.MembershipApproval));

                                            if (System.IO.File.Exists(textAlertTemplatePath))
                                            {
                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    dynamic expando = new ExpandoObject();

                                                    var model = expando as IDictionary<string, object>;

                                                    model.Add("FullName", customer.FullName);
                                                    model.Add("TotalValue", queueDTO.AccountAlertTotalValue.ToString());
                                                    model.Add("MembershipNumber", customer.Reference2);
                                                    model.Add("AccountNumber", customer.Reference1);
                                                    model.Add("BranchDescription", customer.BranchDescription);
                                                    model.Add("CompanyDescription", customer.BranchCompanyDescription);

                                                    var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                    var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.MembershipApproval), null, model);

                                                    var textAlertDTO = new TextAlertDTO
                                                    {
                                                        BranchId = customer.BranchId,
                                                        TextMessageOrigin = (int)MessageOrigin.Within,
                                                        TextMessageRecipient = customer.AddressMobileLine,
                                                        TextMessageBody = result,
                                                        MessageCategory = (int)MessageCategory.SMSAlert,
                                                        AppendSignature = false,
                                                        TextMessagePriority = (int)QueuePriority.Highest,
                                                    };

                                                    await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader);
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Email Alert

                                        if (!string.IsNullOrWhiteSpace(customer.AddressEmail) && Regex.IsMatch(customer.AddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                        {
                                            if (!string.IsNullOrWhiteSpace(customer.BranchAddressEmail) && Regex.IsMatch(customer.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.MembershipApproval));

                                                if (System.IO.File.Exists(emailAlertTemplatePath))
                                                {
                                                    dynamic expando = new ExpandoObject();

                                                    var model = expando as IDictionary<string, object>;

                                                    model.Add("FullName", customer.FullName);
                                                    model.Add("TotalValue", queueDTO.AccountAlertTotalValue.ToString());
                                                    model.Add("MembershipNumber", customer.Reference2);
                                                    model.Add("AccountNumber", customer.Reference1);
                                                    model.Add("BranchDescription", customer.BranchDescription);
                                                    model.Add("CompanyDescription", customer.BranchCompanyDescription);

                                                    var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                    var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.MembershipApproval), null, model);

                                                    var emailAlertDTO = new EmailAlertDTO
                                                    {
                                                        MailMessageFrom = customer.BranchAddressEmail,
                                                        MailMessageTo = customer.AddressEmail,
                                                        MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.MembershipApproval)),
                                                        MailMessageBody = result,
                                                        MailMessageIsBodyHtml = true,
                                                        MailMessageOrigin = (int)MessageOrigin.Within,
                                                        MailMessagePriority = (int)QueuePriority.Highest,
                                                    };

                                                    await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                }
                                            }
                                        }

                                        #endregion

                                        break;

                                    }
                                    #endregion

                                    break;
                                case AccountAlertTrigger.AccountFreezing:

                                    #region Do we need to send alerts?

                                    var freezingDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (freezingDefaultSavingsProductDTO != null)
                                    {
                                        CustomerAccountDTO customerSavingsAccountDTO = null;

                                        var freezedCustomerAccountDTO = await _channelService.FindCustomerAccountAsync(queueDTO.RecordId, false, false, false, false, serviceHeader);

                                        var customerSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(freezedCustomerAccountDTO.CustomerId, freezingDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (customerSavingsAccountDTOs != null && customerSavingsAccountDTOs.Any())
                                            customerSavingsAccountDTO = customerSavingsAccountDTOs[0];

                                        if (customerSavingsAccountDTO != null)
                                        {
                                            var accountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(freezedCustomerAccountDTO.CustomerId, (int)SystemTransactionCode.AccountFreezing, serviceHeader);

                                            if (accountAlertDTOs != null && accountAlertDTOs.Any())
                                            {
                                                foreach (var accountAlertDTO in accountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (!string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AccountFreezing));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            if (System.IO.File.Exists(textAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("CustomerFullName", freezedCustomerAccountDTO.CustomerFullName);
                                                                model.Add("Remarks", freezedCustomerAccountDTO.Remarks);
                                                                model.Add("FullAccountNumber", freezedCustomerAccountDTO.FullAccountNumber);
                                                                model.Add("ValueDate", queueDTO.ValueDate);
                                                                model.Add("BranchDescription", freezedCustomerAccountDTO.BranchDescription);
                                                                model.Add("CompanyDescription", freezedCustomerAccountDTO.BranchCompanyDescription);

                                                                var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AccountFreezing), null, model);

                                                                var textAlertDTO = new TextAlertDTO
                                                                {
                                                                    BranchId = freezedCustomerAccountDTO.BranchId,
                                                                    TextMessageOrigin = (int)MessageOrigin.Within,
                                                                    TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                                    TextMessageBody = result,
                                                                    MessageCategory = (int)MessageCategory.SMSAlert,
                                                                    AppendSignature = false,
                                                                    TextMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.AccountFreezing, 0m, customerSavingsAccountDTO, serviceHeader);

                                                                var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                                if ((customerSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                                {
                                                                    if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                    {
                                                                        if (textMessageTariffs.Any())
                                                                        {
                                                                            await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, freezedCustomerAccountDTO.BranchId, Guid.Empty, freezedCustomerAccountDTO.Remarks, freezedCustomerAccountDTO.FullAccountNumber, 0x9999, (int)SystemTransactionCode.AccountFreezing, customerSavingsAccountDTO, customerSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region Email Alert

                                                    if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(freezedCustomerAccountDTO.BranchAddressEmail) && Regex.IsMatch(freezedCustomerAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AccountFreezing));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("CustomerFullName", freezedCustomerAccountDTO.CustomerFullName);
                                                                model.Add("Remarks", freezedCustomerAccountDTO.Remarks);
                                                                model.Add("FullAccountNumber", freezedCustomerAccountDTO.FullAccountNumber);
                                                                model.Add("ValueDate", queueDTO.ValueDate);
                                                                model.Add("BranchDescription", freezedCustomerAccountDTO.BranchDescription);
                                                                model.Add("CompanyDescription", freezedCustomerAccountDTO.BranchCompanyDescription);

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AccountFreezing), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = freezedCustomerAccountDTO.BranchAddressEmail,
                                                                    MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AccountFreezing)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;
                                case AccountAlertTrigger.AccountDormant:

                                    #region Do we need to send alerts?

                                    #endregion

                                    break;
                                case AccountAlertTrigger.LoanDeffered:

                                    #region Do we need to send alerts?

                                    var deferredLoanCaseDTO = await _channelService.FindLoanCaseAsync(queueDTO.RecordId, serviceHeader);

                                    CustomerAccountDTO deferredLoaneeSavingsAccountDTO = null;

                                    var deferredLoaneeSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(deferredLoanCaseDTO.CustomerId, deferredLoanCaseDTO.SavingsProductId ?? Guid.Empty, true, true, false, false, serviceHeader);

                                    if (deferredLoaneeSavingsAccountDTOs != null && deferredLoaneeSavingsAccountDTOs.Any())
                                        deferredLoaneeSavingsAccountDTO = deferredLoaneeSavingsAccountDTOs[0];

                                    if (deferredLoaneeSavingsAccountDTO != null)
                                    {
                                        var deferredLoaneeAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(deferredLoanCaseDTO.CustomerId, (int)SystemTransactionCode.LoanDeffered, serviceHeader);

                                        foreach (var accountAlertDTO in deferredLoaneeAlertDTOs)
                                        {
                                            #region Text Alert

                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                            {
                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.LoanDeffered));

                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    var loanee = await _channelService.FindCustomerAsync(deferredLoanCaseDTO.CustomerId, serviceHeader);

                                                    dynamic expando = new ExpandoObject();

                                                    var model = expando as IDictionary<string, object>;

                                                    model.Add("LoaneeFullName", loanee.FullName);
                                                    model.Add("LoanProductName", deferredLoanCaseDTO.LoanProductDescription);
                                                    model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", deferredLoanCaseDTO.AmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", deferredLoanCaseDTO.AmountApplied));
                                                    model.Add("CompanyDescription", deferredLoanCaseDTO.BranchCompanyDescription);
                                                    model.Add("BranchDescription", deferredLoanCaseDTO.BranchDescription);

                                                    var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                    var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.LoanDeffered), null, model);

                                                    var textAlertDTO = new TextAlertDTO
                                                    {
                                                        BranchId = deferredLoanCaseDTO.BranchId,
                                                        TextMessageOrigin = (int)MessageOrigin.Within,
                                                        TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                        TextMessageBody = result,
                                                        MessageCategory = (int)MessageCategory.SMSAlert,
                                                        AppendSignature = false,
                                                        TextMessagePriority = (int)QueuePriority.Highest,
                                                    };

                                                    serviceHeader.ApplicationUserName = textAlertDTO.CreatedBy;

                                                    var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.LoanDeffered, 0m, deferredLoaneeSavingsAccountDTO, serviceHeader);

                                                    var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                    if ((deferredLoaneeSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                    {
                                                        if (await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                        {
                                                            if (textMessageTariffs.Any())
                                                            {
                                                                await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, deferredLoanCaseDTO.BranchId, Guid.Empty, deferredLoanCaseDTO.LoanProductDescription, loanee.Reference3, 0x9999, (int)SystemTransactionCode.LoanDeffered, deferredLoaneeSavingsAccountDTO, deferredLoaneeSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region Email Alert

                                            if (accountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                if (!string.IsNullOrWhiteSpace(deferredLoanCaseDTO.BranchAddressEmail) && Regex.IsMatch(deferredLoanCaseDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                {
                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.LoanDeffered));

                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                    {
                                                        var loanee = await _channelService.FindCustomerAsync(deferredLoanCaseDTO.CustomerId, serviceHeader);

                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("LoaneeFullName", loanee.FullName);
                                                        model.Add("LoanProductName", deferredLoanCaseDTO.LoanProductDescription);
                                                        model.Add("TotalValue", accountAlertDTO.MaskTransactionValue ? string.Format(_nfi, "{0:C}", deferredLoanCaseDTO.AmountApplied).ReplaceNumbers() : string.Format(_nfi, "{0:C}", deferredLoanCaseDTO.AmountApplied));
                                                        model.Add("CompanyDescription", deferredLoanCaseDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", deferredLoanCaseDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.LoanDeffered), null, model);

                                                        var emailAlertDTO = new EmailAlertDTO
                                                        {
                                                            MailMessageFrom = deferredLoanCaseDTO.BranchAddressEmail,
                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.LoanDeffered)),
                                                            MailMessageBody = result,
                                                            MailMessageIsBodyHtml = true,
                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        serviceHeader.ApplicationUserName = emailAlertDTO.CreatedBy;

                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.AlternateChannelLinking:

                                    #region Do we need to send alerts?

                                    var channelLinkingCustomerAccountDTO = await _channelService.FindCustomerAccountAsync(queueDTO.RecordId, false, false, false, false, serviceHeader);

                                    var channelLinkingAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(channelLinkingCustomerAccountDTO.CustomerId, (int)SystemTransactionCode.AlternateChannelLinking, serviceHeader);

                                    if (channelLinkingAccountAlertDTOs != null && channelLinkingAccountAlertDTOs.Any())
                                    {
                                        foreach (var accountAlertDTO in channelLinkingAccountAlertDTOs)
                                        {
                                            #region Text Alert

                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                            {
                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AlternateChannelLinking));

                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    if (System.IO.File.Exists(textAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelLinkingCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelLinkingCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelLinkingCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AlternateChannelLinking), null, model);

                                                        var textAlertDTO = new TextAlertDTO
                                                        {
                                                            BranchId = channelLinkingCustomerAccountDTO.BranchId,
                                                            TextMessageOrigin = (int)MessageOrigin.Within,
                                                            TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                            TextMessageBody = result,
                                                            MessageCategory = (int)MessageCategory.SMSAlert,
                                                            AppendSignature = false,
                                                            TextMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region Email Alert

                                            if (!string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                if (!string.IsNullOrWhiteSpace(channelLinkingCustomerAccountDTO.BranchAddressEmail) && Regex.IsMatch(channelLinkingCustomerAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                {
                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AlternateChannelLinking));

                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelLinkingCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelLinkingCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelLinkingCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AlternateChannelLinking), null, model);

                                                        var emailAlertDTO = new EmailAlertDTO
                                                        {
                                                            MailMessageFrom = channelLinkingCustomerAccountDTO.BranchAddressEmail,
                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AlternateChannelLinking)),
                                                            MailMessageBody = result,
                                                            MailMessageIsBodyHtml = true,
                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.AlternateChannelReplacement:

                                    #region Do we need to send alerts?

                                    var channelReplacementCustomerAccountDTO = await _channelService.FindCustomerAccountAsync(queueDTO.RecordId, false, false, false, false, serviceHeader);

                                    var channelReplacementAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(channelReplacementCustomerAccountDTO.CustomerId, (int)SystemTransactionCode.AlternateChannelReplacement, serviceHeader);

                                    if (channelReplacementAccountAlertDTOs != null && channelReplacementAccountAlertDTOs.Any())
                                    {
                                        foreach (var accountAlertDTO in channelReplacementAccountAlertDTOs)
                                        {
                                            #region Text Alert

                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                            {
                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AlternateChannelReplacement));

                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    if (System.IO.File.Exists(textAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelReplacementCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelReplacementCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelReplacementCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AlternateChannelReplacement), null, model);

                                                        var textAlertDTO = new TextAlertDTO
                                                        {
                                                            BranchId = channelReplacementCustomerAccountDTO.BranchId,
                                                            TextMessageOrigin = (int)MessageOrigin.Within,
                                                            TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                            TextMessageBody = result,
                                                            MessageCategory = (int)MessageCategory.SMSAlert,
                                                            AppendSignature = false,
                                                            TextMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region Email Alert

                                            if (!string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                if (!string.IsNullOrWhiteSpace(channelReplacementCustomerAccountDTO.BranchAddressEmail) && Regex.IsMatch(channelReplacementCustomerAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                {
                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AlternateChannelReplacement));

                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelReplacementCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelReplacementCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelReplacementCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AlternateChannelReplacement), null, model);

                                                        var emailAlertDTO = new EmailAlertDTO
                                                        {
                                                            MailMessageFrom = channelReplacementCustomerAccountDTO.BranchAddressEmail,
                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AlternateChannelReplacement)),
                                                            MailMessageBody = result,
                                                            MailMessageIsBodyHtml = true,
                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.AlternateChannelRenewal:

                                    #region Do we need to send alerts?

                                    var channelRenewalCustomerAccountDTO = await _channelService.FindCustomerAccountAsync(queueDTO.RecordId, false, false, false, false, serviceHeader);

                                    var channelRenewalAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(channelRenewalCustomerAccountDTO.CustomerId, (int)SystemTransactionCode.AlternateChannelRenewal, serviceHeader);

                                    if (channelRenewalAccountAlertDTOs != null && channelRenewalAccountAlertDTOs.Any())
                                    {
                                        foreach (var accountAlertDTO in channelRenewalAccountAlertDTOs)
                                        {
                                            #region Text Alert

                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                            {
                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AlternateChannelRenewal));

                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    if (System.IO.File.Exists(textAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelRenewalCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelRenewalCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelRenewalCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AlternateChannelRenewal), null, model);

                                                        var textAlertDTO = new TextAlertDTO
                                                        {
                                                            BranchId = channelRenewalCustomerAccountDTO.BranchId,
                                                            TextMessageOrigin = (int)MessageOrigin.Within,
                                                            TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                            TextMessageBody = result,
                                                            MessageCategory = (int)MessageCategory.SMSAlert,
                                                            AppendSignature = false,
                                                            TextMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region Email Alert

                                            if (!string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                if (!string.IsNullOrWhiteSpace(channelRenewalCustomerAccountDTO.BranchAddressEmail) && Regex.IsMatch(channelRenewalCustomerAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                {
                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AlternateChannelRenewal));

                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelRenewalCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelRenewalCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelRenewalCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AlternateChannelRenewal), null, model);

                                                        var emailAlertDTO = new EmailAlertDTO
                                                        {
                                                            MailMessageFrom = channelRenewalCustomerAccountDTO.BranchAddressEmail,
                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AlternateChannelRenewal)),
                                                            MailMessageBody = result,
                                                            MailMessageIsBodyHtml = true,
                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.AlternateChannelDelinking:

                                    #region Do we need to send alerts?

                                    var channelDelinkingCustomerAccountDTO = await _channelService.FindCustomerAccountAsync(queueDTO.RecordId, false, false, false, false, serviceHeader);

                                    var channelDelinkingAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(channelDelinkingCustomerAccountDTO.CustomerId, (int)SystemTransactionCode.AlternateChannelDelinking, serviceHeader);

                                    if (channelDelinkingAccountAlertDTOs != null && channelDelinkingAccountAlertDTOs.Any())
                                    {
                                        foreach (var accountAlertDTO in channelDelinkingAccountAlertDTOs)
                                        {
                                            #region Text Alert

                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                            {
                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AlternateChannelDelinking));

                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    if (System.IO.File.Exists(textAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelDelinkingCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelDelinkingCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelDelinkingCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AlternateChannelDelinking), null, model);

                                                        var textAlertDTO = new TextAlertDTO
                                                        {
                                                            BranchId = channelDelinkingCustomerAccountDTO.BranchId,
                                                            TextMessageOrigin = (int)MessageOrigin.Within,
                                                            TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                            TextMessageBody = result,
                                                            MessageCategory = (int)MessageCategory.SMSAlert,
                                                            AppendSignature = false,
                                                            TextMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region Email Alert

                                            if (!string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                if (!string.IsNullOrWhiteSpace(channelDelinkingCustomerAccountDTO.BranchAddressEmail) && Regex.IsMatch(channelDelinkingCustomerAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                {
                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AlternateChannelDelinking));

                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelDelinkingCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelDelinkingCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelDelinkingCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AlternateChannelDelinking), null, model);

                                                        var emailAlertDTO = new EmailAlertDTO
                                                        {
                                                            MailMessageFrom = channelDelinkingCustomerAccountDTO.BranchAddressEmail,
                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AlternateChannelDelinking)),
                                                            MailMessageBody = result,
                                                            MailMessageIsBodyHtml = true,
                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.AlternateChannelStoppage:

                                    #region Do we need to send alerts?

                                    var channelStoppageCustomerAccountDTO = await _channelService.FindCustomerAccountAsync(queueDTO.RecordId, false, false, false, false, serviceHeader);

                                    var channelStoppageAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(channelStoppageCustomerAccountDTO.CustomerId, (int)SystemTransactionCode.AlternateChannelStoppage, serviceHeader);

                                    if (channelStoppageAccountAlertDTOs != null && channelStoppageAccountAlertDTOs.Any())
                                    {
                                        foreach (var accountAlertDTO in channelStoppageAccountAlertDTOs)
                                        {
                                            #region Text Alert

                                            if (accountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(accountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && accountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                            {
                                                var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.AlternateChannelStoppage));

                                                if (System.IO.File.Exists(textAlertTemplatePath))
                                                {
                                                    if (System.IO.File.Exists(textAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelStoppageCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelStoppageCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelStoppageCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.AlternateChannelStoppage), null, model);

                                                        var textAlertDTO = new TextAlertDTO
                                                        {
                                                            BranchId = channelStoppageCustomerAccountDTO.BranchId,
                                                            TextMessageOrigin = (int)MessageOrigin.Within,
                                                            TextMessageRecipient = accountAlertDTO.CustomerAddressMobileLine,
                                                            TextMessageBody = result,
                                                            MessageCategory = (int)MessageCategory.SMSAlert,
                                                            AppendSignature = false,
                                                            TextMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddTextAlertsAsync(new System.Collections.ObjectModel.ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region Email Alert

                                            if (!string.IsNullOrWhiteSpace(accountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(accountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                            {
                                                if (!string.IsNullOrWhiteSpace(channelStoppageCustomerAccountDTO.BranchAddressEmail) && Regex.IsMatch(channelStoppageCustomerAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                {
                                                    var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", (int)SystemTransactionCode.AlternateChannelStoppage));

                                                    if (System.IO.File.Exists(emailAlertTemplatePath))
                                                    {
                                                        dynamic expando = new ExpandoObject();

                                                        var model = expando as IDictionary<string, object>;

                                                        model.Add("CustomerAccountCustomerType", queueDTO.AccountAlertPrimaryDescription);
                                                        model.Add("AccountNumber", channelStoppageCustomerAccountDTO.CustomerReference1);
                                                        model.Add("DailyLimit", queueDTO.AccountAlertTotalValue.ToString());
                                                        model.Add("ChannelType", queueDTO.AccountAlertSecondaryDescription);
                                                        model.Add("ExpiryDate", queueDTO.ValueDate);
                                                        model.Add("CompanyDescription", channelStoppageCustomerAccountDTO.BranchCompanyDescription);
                                                        model.Add("BranchDescription", channelStoppageCustomerAccountDTO.BranchDescription);

                                                        var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                        var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.AlternateChannelStoppage), null, model);

                                                        var emailAlertDTO = new EmailAlertDTO
                                                        {
                                                            MailMessageFrom = channelStoppageCustomerAccountDTO.BranchAddressEmail,
                                                            MailMessageTo = accountAlertDTO.CustomerAddressEmail,
                                                            MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.AlternateChannelStoppage)),
                                                            MailMessageBody = result,
                                                            MailMessageIsBodyHtml = true,
                                                            MailMessageOrigin = (int)MessageOrigin.Within,
                                                            MailMessagePriority = (int)QueuePriority.Highest,
                                                        };

                                                        await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.LeaveApproval:

                                    #region Do we need to send alerts?

                                    var leaveApplicationDTO = await _channelService.FindLeaveApplicationAsync(queueDTO.RecordId, serviceHeader);

                                    var leaveApplicationDefaultSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (leaveApplicationDTO != null && leaveApplicationDefaultSavingsProductDTO != null)
                                    {
                                        CustomerAccountDTO customerSavingsAccountDTO = null;

                                        var customerSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(leaveApplicationDTO.EmployeeCustomerId, leaveApplicationDefaultSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (customerSavingsAccountDTOs != null && customerSavingsAccountDTOs.Any())
                                            customerSavingsAccountDTO = customerSavingsAccountDTOs[0];

                                        if (customerSavingsAccountDTO != null)
                                        {
                                            var leaveApprovalAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(customerSavingsAccountDTO.CustomerId, (int)SystemTransactionCode.LeaveApproval, serviceHeader);

                                            if (leaveApprovalAccountAlertDTOs != null && leaveApprovalAccountAlertDTOs.Any())
                                            {
                                                foreach (var leaveApprovalAccountAlertDTO in leaveApprovalAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (leaveApprovalAccountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(leaveApprovalAccountAlertDTO.CustomerAddressMobileLine) && Regex.IsMatch(leaveApprovalAccountAlertDTO.CustomerAddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && leaveApprovalAccountAlertDTO.CustomerAddressMobileLine.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.LeaveApproval));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            if (System.IO.File.Exists(textAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("EmployeeCustomerFullName", leaveApplicationDTO.EmployeeCustomerFullName);
                                                                model.Add("LeaveTypeDescription", leaveApplicationDTO.LeaveTypeDescription);
                                                                model.Add("StatusDescription", leaveApplicationDTO.StatusDescription);
                                                                model.Add("AuthorizationRemarks", leaveApplicationDTO.AuthorizationRemarks);
                                                                model.Add("DurationStartDate", leaveApplicationDTO.DurationStartDate.ToString("dd/MM/yy"));
                                                                model.Add("DurationEndDate", leaveApplicationDTO.DurationEndDate.ToString("dd/MM/yy"));

                                                                var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.LeaveApproval), null, model);

                                                                var textAlertDTO = new TextAlertDTO
                                                                {
                                                                    BranchId = leaveApplicationDTO.EmployeeBranchId,
                                                                    TextMessageOrigin = (int)MessageOrigin.Within,
                                                                    TextMessageRecipient = leaveApprovalAccountAlertDTO.CustomerAddressMobileLine,
                                                                    TextMessageBody = result,
                                                                    MessageCategory = (int)MessageCategory.SMSAlert,
                                                                    AppendSignature = false,
                                                                    TextMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.LeaveApproval, 0m, customerSavingsAccountDTO, serviceHeader);

                                                                var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                                if ((customerSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                                {
                                                                    if (await _channelService.AddTextAlertsAsync(new ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                    {
                                                                        if (textMessageTariffs.Any())
                                                                        {
                                                                            await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, customerSavingsAccountDTO.BranchId, Guid.Empty, leaveApplicationDTO.LeaveTypeDescription, string.Format("{0} - {1}", leaveApplicationDTO.StatusDescription, leaveApplicationDTO.AuthorizationRemarks), 0x9999, (int)SystemTransactionCode.LeaveApproval, customerSavingsAccountDTO, customerSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region Email Alert

                                                    if (leaveApprovalAccountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(leaveApprovalAccountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(leaveApprovalAccountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(customerSavingsAccountDTO.BranchAddressEmail) && Regex.IsMatch(customerSavingsAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.LeaveApproval));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("EmployeeCustomerFullName", leaveApplicationDTO.EmployeeCustomerFullName);
                                                                model.Add("LeaveTypeDescription", leaveApplicationDTO.LeaveTypeDescription);
                                                                model.Add("StatusDescription", leaveApplicationDTO.StatusDescription);
                                                                model.Add("AuthorizationRemarks", leaveApplicationDTO.AuthorizationRemarks);
                                                                model.Add("DurationStartDate", leaveApplicationDTO.DurationStartDate.ToString("dd/MM/yy"));
                                                                model.Add("DurationEndDate", leaveApplicationDTO.DurationEndDate.ToString("dd/MM/yy"));

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.LeaveApproval), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = customerSavingsAccountDTO.BranchAddressEmail,
                                                                    MailMessageTo = customerSavingsAccountDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.LeaveApproval)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;

                                case AccountAlertTrigger.CustomerDetailsEditing:

                                    #region Do we need to send alerts?

                                    var customerDetailsEditingSavingsProductDTO = await _channelService.FindDefaultSavingsProductAsync(serviceHeader);

                                    if (customerDetailsEditingSavingsProductDTO != null)
                                    {
                                        CustomerAccountDTO customerSavingsAccountDTO = null;

                                        var customerSavingsAccountDTOs = await _channelService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(queueDTO.RecordId, customerDetailsEditingSavingsProductDTO.Id, true, true, false, false, serviceHeader);

                                        if (customerSavingsAccountDTOs != null && customerSavingsAccountDTOs.Any())
                                            customerSavingsAccountDTO = customerSavingsAccountDTOs[0];

                                        if (customerSavingsAccountDTO != null)
                                        {
                                            var customerDetailsEditingAccountAlertDTOs = await _channelService.FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(customerSavingsAccountDTO.CustomerId, (int)SystemTransactionCode.CustomerDetailsEditing, serviceHeader);

                                            if (customerDetailsEditingAccountAlertDTOs != null && customerDetailsEditingAccountAlertDTOs.Any())
                                            {
                                                foreach (var customerDetailsEditingAccountAlertDTO in customerDetailsEditingAccountAlertDTOs)
                                                {
                                                    #region Text Alert

                                                    if (customerDetailsEditingAccountAlertDTO.ReceiveTextAlert && !string.IsNullOrWhiteSpace(queueDTO.AccountAlertSecondaryDescription) && Regex.IsMatch(queueDTO.AccountAlertSecondaryDescription, @"^\+(?:[0-9]??){6,14}[0-9]$") && queueDTO.AccountAlertSecondaryDescription.Length >= 13)
                                                    {
                                                        var textAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_TextTemplate.cshtml", SystemTransactionCode.CustomerDetailsEditing));

                                                        if (System.IO.File.Exists(textAlertTemplatePath))
                                                        {
                                                            dynamic expando = new ExpandoObject();

                                                            var model = expando as IDictionary<string, object>;

                                                            model.Add("AccountAlertReference", queueDTO.AccountAlertReference);
                                                            model.Add("AccountAlertPrimaryDescription", queueDTO.AccountAlertPrimaryDescription);
                                                            model.Add("CustomerFullName", customerSavingsAccountDTO.CustomerFullName);

                                                            var template = System.IO.File.ReadAllText(textAlertTemplatePath);

                                                            var result = Engine.Razor.RunCompile(template, string.Format("{0}_TextTemplate", SystemTransactionCode.CustomerDetailsEditing), null, model);

                                                            var textAlertDTO = new TextAlertDTO
                                                            {
                                                                BranchId = customerSavingsAccountDTO.BranchId,
                                                                TextMessageOrigin = (int)MessageOrigin.Within,
                                                                TextMessageRecipient = queueDTO.AccountAlertSecondaryDescription,
                                                                TextMessageBody = result,
                                                                MessageCategory = (int)MessageCategory.SMSAlert,
                                                                AppendSignature = false,
                                                                TextMessagePriority = (int)QueuePriority.Highest,
                                                            };

                                                            var textMessageTariffs = await _channelService.ComputeTariffsByTextAlertAsync((int)SystemTransactionCode.CustomerDetailsEditing, 0m, customerSavingsAccountDTO, serviceHeader);

                                                            var tariffAmount = textMessageTariffs.Sum(x => x.Amount);

                                                            if ((customerSavingsAccountDTO.AvailableBalance - tariffAmount) >= 0)
                                                            {
                                                                if (await _channelService.AddTextAlertsAsync(new ObservableCollection<TextAlertDTO> { textAlertDTO }, serviceHeader))
                                                                {
                                                                    if (textMessageTariffs.Any())
                                                                    {
                                                                        await _channelService.AddTariffJournalsWithCustomerAccountAsync(null, customerSavingsAccountDTO.BranchId, Guid.Empty, "Customer Details Editing", "Customer Details Editing", 0x9999, (int)SystemTransactionCode.CustomerDetailsEditing, customerSavingsAccountDTO, customerSavingsAccountDTO, textMessageTariffs, serviceHeader);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region Email Alert

                                                    if (customerDetailsEditingAccountAlertDTO.ReceiveEmailAlert && !string.IsNullOrWhiteSpace(customerDetailsEditingAccountAlertDTO.CustomerAddressEmail) && Regex.IsMatch(customerDetailsEditingAccountAlertDTO.CustomerAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(customerSavingsAccountDTO.BranchAddressEmail) && Regex.IsMatch(customerSavingsAccountDTO.BranchAddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        {
                                                            var emailAlertTemplatePath = System.IO.Path.Combine(accountAlertDispatcherSettingsElement.TemplatesPath, string.Format("{0}_EmailTemplate.cshtml", SystemTransactionCode.CustomerDetailsEditing));

                                                            if (System.IO.File.Exists(emailAlertTemplatePath))
                                                            {
                                                                dynamic expando = new ExpandoObject();

                                                                var model = expando as IDictionary<string, object>;

                                                                model.Add("AccountAlertReference", queueDTO.AccountAlertReference);
                                                                model.Add("AccountAlertPrimaryDescription", queueDTO.AccountAlertPrimaryDescription);
                                                                model.Add("CustomerFullName", customerSavingsAccountDTO.CustomerFullName);

                                                                var template = System.IO.File.ReadAllText(emailAlertTemplatePath);

                                                                var result = Engine.Razor.RunCompile(template, string.Format("{0}_EmailTemplate", SystemTransactionCode.LeaveApproval), null, model);

                                                                var emailAlertDTO = new EmailAlertDTO
                                                                {
                                                                    MailMessageFrom = customerSavingsAccountDTO.BranchAddressEmail,
                                                                    MailMessageTo = customerSavingsAccountDTO.CustomerAddressEmail,
                                                                    MailMessageSubject = string.Format("{0} Alert", EnumHelper.GetDescription(SystemTransactionCode.CustomerDetailsEditing)),
                                                                    MailMessageBody = result,
                                                                    MailMessageIsBodyHtml = true,
                                                                    MailMessageOrigin = (int)MessageOrigin.Within,
                                                                    MailMessagePriority = (int)QueuePriority.Highest,
                                                                };

                                                                await _channelService.AddEmailAlertAsync(emailAlertDTO, serviceHeader);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}