using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.Services
{
    public interface IBrokerService
    {
        bool ProcessMembershipAccountRegistrationAlerts(DMLCommand command, ServiceHeader serviceHeader, params UserDTO[] data);

        bool ProcessMembershipResetPasswordAlerts(DMLCommand command, ServiceHeader serviceHeader, params UserDTO[] data);

        bool ProcessMembershipAccountVerificationAlerts(DMLCommand command, ServiceHeader serviceHeader, params UserDTO[] data);

        bool ProcessRecurringBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params RecurringBatchEntryDTO[] data);

        bool ProcessLoanDisbursementBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params LoanDisbursementBatchEntryDTO[] data);

        bool ProcessCreditBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params CreditBatchEntryDTO[] data);

        bool ProcessWireTransferBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params WireTransferBatchEntryDTO[] data);

        bool ProcessDebitBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params DebitBatchEntryDTO[] data);

        bool ProcessTextAlerts(DMLCommand command, ServiceHeader serviceHeader, params TextAlertDTO[] data);

        bool ProcessEmailAlerts(DMLCommand command, ServiceHeader serviceHeader, params EmailAlertDTO[] data);

        bool ProcessAuditLogs(DMLCommand command, ServiceHeader serviceHeader, params AuditLogBCP[] data);

        bool ProcessGuarantorAttachmentAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanGuarantorDTO[] data);

        bool ProcessGuarantorRelievingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanGuarantorAttachmentHistoryEntryDTO[] data);

        bool ProcessFrozenAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params CustomerAccountDTO[] data);

        bool ProcessMembershipApprovalAlerts(DMLCommand command, ServiceHeader serviceHeader, params CustomerDTO[] data);

        bool ProcessAccountClosureRequestAlerts(DMLCommand command, ServiceHeader serviceHeader, params AccountClosureRequestDTO[] data);

        bool ProcessGuarantorSubstitutionAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanGuarantorDTO[] data);

        bool ProcessLoanDeferredAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanCaseDTO[] data);

        bool ProcessLoanGuaranteeAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanCaseDTO[] data);

        bool ProcessLoanRequestAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LoanRequestDTO[] data);

        bool ProcessMobileToBankSenderAcknowledgementAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params MobileToBankRequestDTO[] data);

        bool ProcessPaySlips(DMLCommand command, ServiceHeader serviceHeader, params PaySlipDTO[] data);

        bool ProcessElectronicStatements(DMLCommand command, ServiceHeader serviceHeader, params MediaDTO[] data);

        bool ProcessWorkflow(DMLCommand command, ServiceHeader serviceHeader, WorkflowDTO data);

        bool ProcessAlternateChannelLinkingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data);

        bool ProcessAlternateChannelReplacementAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data);

        bool ProcessAlternateChannelRenewalAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data);

        bool ProcessAlternateChannelDelinkingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data);

        bool ProcessAlternateChannelStoppageAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params AlternateChannelDTO[] data);

        bool ProcessLeaveApprovalAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params LeaveApplicationDTO[] data);

        bool ProcessJournalReversalBatchEntries(DMLCommand command, ServiceHeader serviceHeader, params JournalReversalBatchEntryDTO[] data);

        bool ProcessCustomerDetailsEditingAccountAlerts(DMLCommand command, ServiceHeader serviceHeader, params QueueDTO[] data);

        bool ProcessBankToMobileRequests(DMLCommand command, ServiceHeader serviceHeader, params BankToMobileRequestDTO[] data);

        bool ProcessBrokerRequests(DMLCommand command, ServiceHeader serviceHeader, params BrokerRequestDTO[] data);

        bool ProcessPopulationRegisterQueries(DMLCommand command, ServiceHeader serviceHeader, params PopulationRegisterQueryDTO[] data);
    }
}
