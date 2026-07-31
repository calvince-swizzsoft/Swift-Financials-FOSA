using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.Presentation.Infrastructure.Models;
using SwiftFinancials.Presentation.Shared.Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Infrastructure.Services
{
    public interface IChannelService
    {
        #region Membership

        Task<bool> AuthenticateAsync(string domain, string userName, string password, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<string>> GetRolesForUserAsync(string userName, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<PermissionWrapperDTO>> IsUserAuthorizedToAccessNavigationItemsAsync(string userName, List<PermissionWrapperDTO> permissionWrappers, ServiceHeader serviceHeader = null);

        Task<bool> IsUserAuthorizedToAccessSystemPermissionTypeAsync(string userName, int systemPermissionType, Guid? targetBranchId, ServiceHeader serviceHeader = null);

        Task<bool> IsUserAuthorizedToAccessCustomerFileAsync(string userName, Guid customerId, ServiceHeader serviceHeader = null);

        Task<EmployeeDTO> GetUserInfoAsync(string userName, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployeeDTO>> GetUserInfoCollectionAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployeeDTO>> GetUserInfoCollectionInRoleAsync(string roleName, ServiceHeader serviceHeader = null);
        //Task FindFileMovementHistoryByIdAsync(Guid value, ServiceHeader serviceHeader);
        Task<PageCollectionInfo<EmployeeDTO>> GetUserInfoCollectionInPageAsync(int pageIndex, int pageSize, string filter, ServiceHeader serviceHeader = null);

        Task<int> CreateUserAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateUserAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<string>> GetAllRolesAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<string>> GetUsersInRoleAsync(string roleName, ServiceHeader serviceHeader = null);

        Task<bool> CreateRoleAsync(string roleName, ServiceHeader serviceHeader = null);

        Task<bool> RemoveUserFromRolesAsync(string userName, ObservableCollection<string> roles, ServiceHeader serviceHeader = null);

        Task<bool> AddUserToRolesAsync(string userName, ObservableCollection<string> roles, ServiceHeader serviceHeader = null);

        Task<PasswordSettings> GetSettingsAsync(ServiceHeader serviceHeader = null);

        Task<bool> ChangePasswordWithAnswerAsync(string userName, string newPassword, ServiceHeader serviceHeader = null);

        #endregion

        #region MembershipService

        Task<PageCollectionInfo<UserDTO>> FindMembershipByFilterInPageAsync(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending, ServiceHeader serviceHeader);
        Task AddCommissionAsync(DynamicChargeDTO levyDTO, ServiceHeader serviceHeader);
        Task<PageCollectionInfo<RoleDTO>> FindMembershipRolesByFilterInPageAsync(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending, ServiceHeader serviceHeader);

        Task<UserDTO> AddNewMembershipAsync(UserDTO userDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateMembershipAsync(UserDTO userDTO, ServiceHeader serviceHeader);

        Task<int> GetApplicationUsersCountAsync(ServiceHeader serviceHeader);

        Task<int> GetApplicationUsersCountByCompanyIdAsync(Guid companyId, ServiceHeader serviceHeader);

        Task<UserDTO> FindMembershipAsync(string id, ServiceHeader serviceHeader);

        Task<bool> ResetMembershipPasswordAsync(UserDTO userDTO, ServiceHeader serviceHeader);

        Task<bool> VerifyMembershipAsync(UserDTO userDTO, ServiceHeader serviceHeader);

        #endregion

        #region CompanyDTO

        Task<List<CompanyDTO>> FindCompaniesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CompanyDTO>> FindCompaniesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CompanyDTO>> FindCompaniesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CompanyDTO> AddCompanyAsync(CompanyDTO companyDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCompanyAsync(CompanyDTO companyDTO, ServiceHeader serviceHeader = null);

        Task<CompanyDTO> FindCompanyAsync(Guid companyId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DebitTypeDTO>> FindDebitTypesByCompanyIdAsync(Guid companyId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDebitTypesByCompanyIdAsync(Guid companyId, ObservableCollection<DebitTypeDTO> debitTypes, ServiceHeader serviceHeader = null);

        Task<ProductCollectionInfo> FindAttachedProductsByCompanyIdAsync(Guid companyId, ServiceHeader serviceHeader = null);
        Task FindCustomersByStationIdAndFilterInPageAsync(Func<StationDTO> mapTo);
        Task<bool> UpdateAttachedProductsByCompanyIdAsync(Guid companyId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader = null);

        Task<int> GetCompaniesCountAsync(ServiceHeader serviceHeader);
        #endregion

        #region AuditLogDTO

        Task<AuditLogDTO> AddAuditLogAsync(AuditLogDTO auditLogDTO, ServiceHeader serviceHeader = null);

        Task<bool> AddAuditLogsAsync(List<AuditLogDTO> auditLogDTOs, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AuditLogDTO>> FindAuditLogsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AuditLogDTO>> FindAuditLogsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter, ServiceHeader serviceHeader = null);
        Task AddCustomerAsync(CustomerDTO customerBindingModel, ServiceHeader serviceHeader);

        #endregion

        #region AuditTrailDTO

        Task<bool> AddAuditTrailsAsync(List<AuditTrailDTO> auditTrailDTOs, ServiceHeader serviceHeader = null);

        Task<AuditTrailDTO> AddAuditTrailAsync(AuditTrailDTO auditTrailDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AuditTrailDTO>> FindAuditTrailsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter, ServiceHeader serviceHeader = null);

        #endregion

        #region FileUpload

        Task<string> FileUploadAsync(FileData fileData, ServiceHeader serviceHeader = null);

        Task<bool> FileUploadDoneAsync(string filename, ServiceHeader serviceHeader = null);

        Task<bool> PingNetworkAsync(string hostNameOrAddress, ServiceHeader serviceHeader = null);

        #endregion

        #region MediaDTO

        string GetMediaHostName();

        Task<string> MediaUploadAsync(FileData fileData, ServiceHeader serviceHeader = null);

        Task<bool> MediaUploadDoneAsync(string filename, ServiceHeader serviceHeader = null);

        Task<MediaDTO> GetMediaAsync(Guid sku, ServiceHeader serviceHeader = null);

        Task<bool> PostFileAsync(MediaDTO mediaDTO, ServiceHeader serviceHeader = null);

        Task<bool> PostImageAsync(MediaDTO mediaDTO, ServiceHeader serviceHeader = null);

        Task<MediaDTO> PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool chargeForPrinting, bool includeInterestStatement, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<MediaDTO> PrintLoanRepaymentScheduleAsync(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader = null);

        #endregion

        #region BranchDTO

        Task<PageCollectionInfo<BranchDTO>> FindBranchesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BranchDTO>> FindBranchesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BranchDTO>> FindBranchesAsync(ServiceHeader serviceHeader = null);

        Task<BranchDTO> AddBranchAsync(BranchDTO branchDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBranchAsync(BranchDTO branchDTO, ServiceHeader serviceHeader = null);

        Task<BranchDTO> FindBranchAsync(Guid branchId, ServiceHeader serviceHeader = null);

        Task<BranchDTO> FindBranchAsync(int branchCode, ServiceHeader serviceHeader = null);

        #endregion

        #region LocationDTO

        Task<PageCollectionInfo<LocationDTO>> FindLocationsByFilterInPageAsync(string filter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LocationDTO>> FindLocationsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LocationDTO> AddLocationAsync(LocationDTO locationDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLocationAsync(LocationDTO locationDTO, ServiceHeader serviceHeader = null);

        Task<LocationDTO> FindLocationAsync(Guid locationId, ServiceHeader serviceHeader = null);

        #endregion

        #region BankDTO

        Task<PageCollectionInfo<BankDTO>> FindBanksInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BankDTO>> FindBanksByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<BankDTO> AddBankAsync(BankDTO bankDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBankAsync(BankDTO bankDTO, ServiceHeader serviceHeader = null);

        Task<BankDTO> FindBankAsync(Guid bankId, ServiceHeader serviceHeader = null);

        #endregion

        #region BankBranchDTO

        Task<ObservableCollection<BankBranchDTO>> FindBankBranchesByBankIdAsync(Guid bankId, ServiceHeader serviceHeader = null);
        //Task AddJournalAsync(TransactionModel journalDTO, bool v, ServiceHeader serviceHeader);
        Task<bool> UpdateBankBranchesByBankIdAsync(Guid bankId, ObservableCollection<BankBranchDTO> bankBranches, ServiceHeader serviceHeader = null);

        #endregion

        #region ReportDTO

        Task<ObservableCollection<ReportDTO>> FindReportsAsync(bool updateDepth = false, bool traverseTree = true, ServiceHeader serviceHeader = null);

        Task<ReportDTO> AddReportAsync(ReportDTO reportDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateReportAsync(ReportDTO reportDTO, ServiceHeader serviceHeader = null);

        Task<ReportDTO> FindReportAsync(Guid reportId, ServiceHeader serviceHeader = null);


        #region ReportDTO

        Task<Tuple<ReportDTO, string>> AddNewReportAsync(ReportDTO reportDTO, ServiceHeader serviceHeader,
         double timeoutMinutes = 10d);

        Task<bool> UpdateReportAsync(ReportDTO reportDTO, ServiceHeader serviceHeader,
         double timeoutMinutes = 10d);

        Task<PageCollectionInfo<ReportDTO>> FindReportFilterInPageAsync(int startIndex, int pageSize, IList<string> sortedColumns, string searchString, bool sortAscending, ServiceHeader serviceHeader,
         double timeoutMinutes = 10d);

        Task<PageCollectionInfo<ReportDTO>> FindDetailedReportFilterInPageAsync(int startIndex, int pageSize, IList<string> sortedColumns, string searchString, bool sortAscending, ServiceHeader serviceHeader,
        double timeoutMinutes = 10d);

        Task<PageCollectionInfo<ReportDTO>> FindReportHeadersFilterInPageAsync(int startIndex, int pageSize, IList<string> sortedColumns, string searchString, bool sortAscending, ServiceHeader serviceHeader,
        double timeoutMinutes = 10d);

        Task<ReportDTO> FindReportByIdAsync(Guid id, ServiceHeader serviceHeader, double timeoutMinutes = 10d);

        Task<List<ReportDTO>> FindReportsAsync(ServiceHeader serviceHeader = null);

        #endregion
        #endregion

      

        #region NavigationItemDTO

        Task<bool> AddNavigationItemsAsync(List<NavigationItemDTO> navigationItems, ServiceHeader serviceHeader, double timeoutMinutes = 10d);

        Task<NavigationItemDTO> FindNavigationItemByIdAsync(Guid navigationItemId, ServiceHeader serviceHeader);

        Task<NavigationItemDTO> FindNavigationItemByCodeAsync(int navigationItemCode, ServiceHeader serviceHeader);

        Task<List<NavigationItemDTO>> FindNavigationItemsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<NavigationItemDTO>> FindNavigationItemsFilterPageCollectionInfoAsync(int pageIndex, int pageSize, List<string> sortedColumns, string text, bool sortAscending, ServiceHeader serviceHeader);

        Task<List<NavigationItemDTO>> FindModuleNavigationActionByControllerNameAsync(string controllerName, ServiceHeader serviceHeader);

        Task<bool> BulkInsertNavigationItemAsync(List<Guid> modulesNavigationIds, string roleName, ServiceHeader serviceHeader);

        Task<bool> BulkInsertNavigationItemInRolesAsync(List<Guid> NavigationItemInRole, string roleName, ServiceHeader serviceHeader);

        #endregion

        #region NavigationItemInRoleDTO

        Task<List<NavigationItemInRoleDTO>> GetNavigationItemsInRoleAsync(string roleName, ServiceHeader serviceHeader);

        Task<List<NavigationItemInRoleDTO>> GetRolesForNavigationItemCodeAsync(int navigationItemCode, ServiceHeader serviceHeader);

        Task<bool> IsNavigationItemInRoleAsync(int navigationItemCode, string roleName, ServiceHeader serviceHeader);

        Task<bool> AddNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO, ServiceHeader serviceHeader);

        Task<bool> RemoveNavigationItemRoleAsync(Guid navigationItemId, string roleName, ServiceHeader serviceHeader);

        Task<bool> ValidateModuleAccessAsync(string controllerName, string roleName, ServiceHeader serviceHeader);

        #endregion

        #region WorkflowDTO

        Task<WorkflowDTO> FindWorkflowByRecordAndSystemPermissionTypeAsync(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader = null);

        Task<bool> AddWorkflowAsync(WorkflowDTO workflowDTO, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader = null);

        Task<bool> ProcessWorkflowQueueAsync(Guid recordId, int workflowRecordType, int workflowRecordStatus, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<WorkflowDTO>> FindQueableWorkflowsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region WorkflowItemDTO

        Task<PageCollectionInfo<WorkflowItemDTO>> FindWorkflowItemsByFilterInPageAsync(int systemPermissionType, int status, string filter, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<List<WorkflowItemDTO>> FindWorkflowItemsAsync(Guid workflowId, ServiceHeader serviceHeader = null);

        Task<bool> ApproveWorkflowItemAsync(WorkflowItemDTO workflowItemDTO, bool usedBiometrics, ServiceHeader serviceHeader = null);

        #endregion

        #region WorkflowItemEntryDTO

        Task<List<WorkflowItemEntryDTO>> FindWorkflowItemEntriesByWorkflowAsync(Guid workflowId, ServiceHeader serviceHeader = null);

        #endregion

        #region WorkflowSettingDTO

        Task<bool> MapWorkflowSettingToSystemPermissionTypeAsync(WorkflowSettingDTO workflowSettingDTO, ServiceHeader serviceHeader = null);

        Task<WorkflowSettingDTO> FindWorkflowSettingAsync(int systemPermissionType, ServiceHeader serviceHeader = null);

        #endregion

        #region SystemPermissionType

        Task<ObservableCollection<string>> GetRolesForSystemPermissionTypeAsync(int systemPermissionType, ServiceHeader serviceHeader = null);

        Task<List<SystemPermissionTypeInRoleDTO>> GetRolesListForSystemPermissionTypeAsync(int systemPermissionType, ServiceHeader serviceHeader = null);

        Task<bool> RemoveSystemPermissionTypeFromRolesAsync(int systemPermissionType, ObservableCollection<string> roleNames, ServiceHeader serviceHeader = null);

        Task<bool> AddSystemPermissionTypeToRolesAsync(int systemPermissionType, ObservableCollection<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader = null);

        Task<bool> MapSystemPermissionTypeToRolesAsync(int systemPermissionType, ObservableCollection<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BranchDTO>> GetBranchesForSystemPermissionTypeAsync(int systemPermissionType, ServiceHeader serviceHeader = null);

        Task<bool> RemoveSystemPermissionTypeFromBranchesAsync(int systemPermissionType, ObservableCollection<BranchDTO> branches, ServiceHeader serviceHeader = null);

        Task<bool> AddSystemPermissionTypeToBranchesAsync(int systemPermissionType, ObservableCollection<BranchDTO> branches, ServiceHeader serviceHeader = null);

        Task<bool> MapSystemPermissionTypeToBranchesAsync(int systemPermissionType, ObservableCollection<BranchDTO> branches, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BranchDTO>> GetBranchesForEmployeeAsync(Guid employeeId, ServiceHeader serviceHeader = null);

        Task<bool> RemoveEmployeeFromBranchesAsync(Guid employeeId, ObservableCollection<BranchDTO> branches, ServiceHeader serviceHeader = null);

        Task<bool> AddEmployeeToBranchesAsync(Guid employeeId, ObservableCollection<BranchDTO> branches, ServiceHeader serviceHeader = null);

        Task<bool> MapEmployeeToBranchesAsync(Guid employeeId, ObservableCollection<BranchDTO> branches, ServiceHeader serviceHeader = null);


        #endregion

        #region MicroCreditOfficerDTO

        Task<PageCollectionInfo<MicroCreditOfficerDTO>> FindMicroCreditOfficersByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<MicroCreditOfficerDTO>> FindMicroCreditOfficersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<MicroCreditOfficerDTO> AddMicroCreditOfficerAsync(MicroCreditOfficerDTO microCreditOfficerDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateMicroCreditOfficerAsync(MicroCreditOfficerDTO microCreditOfficerDTO, ServiceHeader serviceHeader = null);

        Task<MicroCreditOfficerDTO> FindMicroCreditOfficerAsync(Guid microCreditOfficerId, ServiceHeader serviceHeader = null);

        #endregion

        #region MicroCreditGroupDTO

        Task<BatchImportParseInfo> ParseMicroCreditGroupImportAsync(Guid microCreditGroupCustomerId, string fileName, ServiceHeader serviceHeader = null);

        Task<bool> MicroCreditGroupMemberExistsAsync(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<MicroCreditGroupDTO>> FindMicroCreditGroupsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<MicroCreditGroupDTO> AddMicroCreditGroupAsync(MicroCreditGroupDTO microCreditGroupDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateMicroCreditGroupAsync(MicroCreditGroupDTO microCreditGroupDTO, ServiceHeader serviceHeader = null);

        Task<MicroCreditGroupDTO> FindMicroCreditGroupAsync(Guid microCreditGroupId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<MicroCreditGroupMemberDTO>> FindMicroCreditGroupMembersByMicroCreditGroupIdAsync(Guid microCreditGroupId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<MicroCreditGroupMemberDTO>> FindMicroCreditGroupMembersByMicroCreditGroupCustomerIdAsync(Guid microCreditGroupCustomerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<MicroCreditGroupMemberDTO>> FindMicroCreditGroupMembersByMicroCreditGroupIdInPageAsync(Guid microCreditGroupId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<MicroCreditGroupMemberDTO> AddMicroCreditGroupMemberAsync(MicroCreditGroupMemberDTO microCreditGroupMemberDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveMicroCreditGroupMembersAsync(ObservableCollection<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs, ServiceHeader serviceHeader = null);

        Task<MicroCreditGroupMemberDTO> FindMicroCreditGroupMemberByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateMicroCreditMemberGroupCollectionByMicroCreditGroupIdAsync(Guid microCreditGroupId, ObservableCollection<MicroCreditGroupMemberDTO> microCreditGroupMemberCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region TextAlertDTO

        Task<PageCollectionInfo<TextAlertDTO>> FindTextAlertsByFilterInPageAsync(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TextAlertDTO>> FindTextAlertsByDateRangeAndFilterInPageAsync(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TextAlertDTO>> FindTextAlertsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> AddBulkMessageAsync(BulkMessageDTO bulkMessageDTO, ServiceHeader serviceHeader = null);

        Task<bool> AddUSSDMessageAsync(USSDMessageDTO ussdMessageDTO, ServiceHeader serviceHeader = null);

        Task<bool> AddTextAlertsAsync(ObservableCollection<TextAlertDTO> textAlertDTOs, ServiceHeader serviceHeader = null);

        Task<bool> AddTextAlertsWithHistoryAsync(GroupTextAlertDTO groupTextAlertDTO, ServiceHeader serviceHeader = null);

        Task<List<TextAlertDTO>> FindTextAlertsByDLRStatusAsync(int dlrStatus, ServiceHeader serviceHeader = null);

        Task<bool> UpdateTextAlertAsync(TextAlertDTO textAlertDTO, ServiceHeader serviceHeader = null);

        Task<TextAlertDTO> FindTextAlertAsync(Guid textAlertId, ServiceHeader serviceHeader = null);

        Task<bool> AddQuickTextAlertAsync(QuickTextAlertDTO quickTextAlertDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsBySystemTransactionCodeAsync(int systemTransactionCode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsBySystemTransactionCodeAsync(int systemTransactionCode, ObservableCollection<CommissionDTO> commissions, int chargeBenefactor, ServiceHeader serviceHeader = null);

        Task<List<MonthlySummaryValuesDTO>> FindTextAlertsMonthlyStatisticsAsync(Guid companyId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader);

        #endregion

        #region EmailAlertDTO

        Task<PageCollectionInfo<EmailAlertDTO>> FindEmailAlertsByFilterInPageAsync(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmailAlertDTO>> FindEmailAlertsByDateRangeAndFilterInPageAsync(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmailAlertDTO>> FindEmailAlertsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> AddEmailAlertsAsync(ObservableCollection<EmailAlertDTO> emailAlertDTOs, ServiceHeader serviceHeader = null);

        Task<bool> AddEmailAlertsWithHistoryAsync(GroupEmailAlertDTO groupEmailAlertDTO, ServiceHeader serviceHeader = null);

        Task<EmailAlertDTO> AddEmailAlertAsync(EmailAlertDTO emailAlertDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmailAlertAsync(EmailAlertDTO emailAlertDTO, ServiceHeader serviceHeader = null);

        Task<EmailAlertDTO> FindEmailAlertAsync(Guid emailAlertId, ServiceHeader serviceHeader = null);

        Task<List<EmailAlertDTO>> FindEmailAlertsByDLRStatusAsync(int dlrStatus, ServiceHeader serviceHeader = null);

        Task<bool> AddQuickEmailAlertAsync(QuickEmailAlertDTO quickEmailAlertDTO, ServiceHeader serviceHeader = null);

        Task<List<MonthlySummaryValuesDTO>> FindEmailAlertsMonthlyStatisticsAsync(Guid companyId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader);

        #endregion

        #region MessageGroupDTO

        Task<PageCollectionInfo<MessageGroupDTO>> FindMessageGroupsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<MessageGroupDTO>> FindMessageGroupsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<MessageGroupDTO> AddNewMessageGroupAsync(MessageGroupDTO messageGroupDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateMessageGroupAsync(MessageGroupDTO messageGroupDTO, ServiceHeader serviceHeader = null);

        Task<MessageGroupDTO> FindMessageGroupAsync(Guid messageGroupId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<MessageGroupDTO>> FindMessageGroupsAsync(ServiceHeader serviceHeader = null);

        Task<BatchImportParseInfo> ParseQuickAlertImportAsync(string fileName, int messageCategory, ServiceHeader serviceHeader = null);

        Task<BatchImportParseInfo> ParseCustomersCustomMessageGroupImportAsync(string fileName, ServiceHeader serviceHeader = null);

        #endregion

        #region FuneralRiderClaimDTO

        Task<FuneralRiderClaimDTO> AddNewFuneralRiderClaimAsync(FuneralRiderClaimDTO funeralRiderClaimDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateFuneralRiderClaimAsync(FuneralRiderClaimDTO funeralRiderClaimDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FuneralRiderClaimDTO>> FindFuneralRiderClaimsByFilterAndDateInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FuneralRiderClaimDTO>> FindFuneralRiderClaimsByStatusAndFilterInPageAsync(int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<FuneralRiderClaimDTO> FindFuneralRiderClaimAsync(Guid funeralRiderClaimId, ServiceHeader serviceHeader = null);

        Task<List<FuneralRiderClaimDTO>> FindFuneralRiderClaimsByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FuneralRiderClaimDTO>> FindFuneralRiderClaimsByFilterInPageAsync(string filter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region FuneralRiderClaimPayableDTO

        Task<FuneralRiderClaimPayableDTO> AddNewFuneralRiderClaimPayableAsync(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, ServiceHeader serviceHeader = null);

        Task<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayableAsync(Guid funeralRiderClaimPayableId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateFuneralRiderClaimPayableAsync(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditFuneralRiderClaimPayableAsync(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int verificationOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeFuneralRiderClaimPayableAsync(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int authorizationOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> PostFuneralRiderClaimPayableAsync(Guid funeralRiderClaimPayableId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FuneralRiderClaimPayableDTO>> FindFuneralRiderClaimPayablesByRecordStatusFilterAndDateInPageAsync(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FuneralRiderClaimPayableDTO>> FindFuneralRiderClaimPayablesByRecordStatusPaymentStatusFilterAndDateInPageAsync(int recordStatus, int paymentStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FuneralRiderClaimPayableDTO>> FindFuneralRiderClaimPayablesByRecordStatusPaymentStatusAndFilterInPageAsync(int recordStatus, int paymentStatus, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region CashWithdrawalRequestDTO

        Task<PageCollectionInfo<CashWithdrawalRequestDTO>> FindCashWithdrawalRequestsByFilterInPageAsync(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashWithdrawalRequestDTO>> FindMatureCashWithdrawalRequestsByCustomerAccountIdAsync(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashWithdrawalRequestDTO>> FindMatureCashWithdrawalRequestsByChartOfAccountIdAsync(Guid chartOfAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashWithdrawalRequestDTO>> FindCashWithdrawalRequestsAsync(ServiceHeader serviceHeader = null);

        Task<CashWithdrawalRequestDTO> AddCashWithdrawalRequestAsync(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, ServiceHeader serviceHeader = null);

        Task<CashWithdrawalRequestDTO> FindCashWithdrawalRequestAsync(Guid cashWithdrawalRequestId, ServiceHeader serviceHeader = null);

        Task<bool> PayCashWithdrawalRequestAsync(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeCashWithdrawalRequestAsync(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, int customerTransactionAuthOption, ServiceHeader serviceHeader = null);

        #endregion

        #region CashTransferRequestDTO

        Task<CashTransferRequestDTO> AddCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> AcknowledgeCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashTransferRequestDTO>> FindMatureCashTransferRequestsByEmployeeIdAsync(Guid employeeId, ServiceHeader serviceHeader = null);

        Task<CashTransferRequestDTO> FindCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader = null);

        Task<bool> UtilizeCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsByFilterInPageAsync(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsByStatusAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int status, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashTransferRequestDTO>> FindCashTransferRequestsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region CashDepositRequestDTO

        Task<PageCollectionInfo<CashDepositRequestDTO>> FindCashDepositRequestsByFilterInPageAsync(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashDepositRequestDTO>> FindActionableCashDepositRequestsByCustomerAccountAsync(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<CashDepositRequestDTO> AddCashDepositRequestAsync(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader = null);

        Task<CashDepositRequestDTO> FindCashDepositRequestAsync(Guid cashDepositRequestId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CashDepositRequestDTO>> FindCashDepositRequestsAsync(ServiceHeader serviceHeader = null);

        Task<bool> PostCashDepositRequestAsync(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeCashDepositRequestAsync(CashDepositRequestDTO cashDepositRequestDTO, int customerTransactionAuthOption, ServiceHeader serviceHeader = null);

        #endregion

        #region ExternalChequeDTO

        Task<PageCollectionInfo<ExternalChequeDTO>> FindExternalChequesByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExternalChequeDTO>> FindExternalChequesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ExternalChequeDTO> AddExternalChequeAsync(ExternalChequeDTO externalChequeDTO, ServiceHeader serviceHeader = null);

        Task<bool> MarkExternalChequeClearedAsync(Guid externalChequeId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ExternalChequeDTO>> FindUnClearedExternalChequesByCustomerAccountIdAsync(Guid customerAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ExternalChequeDTO>> FindUnTransferredExternalChequesByTellerId(Guid tellerId, string text, ServiceHeader serviceHeader);
        Task<PageCollectionInfo<ExternalChequeDTO>> FindUnTransferredExternalChequesByTellerIdAndFilterInPageAsync(Guid tellerId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ExternalChequeDTO>> FindUnTransferredExternalChequesByTellerIdAndFilterAsync(Guid tellerId, string text, ServiceHeader serviceHeader = null);

        Task<bool> TransferExternalChequesAsync(ObservableCollection<ExternalChequeDTO> externalChequeDTOs, TellerDTO tellerDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExternalChequeDTO>> FindUnClearedExternalChequesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExternalChequeDTO>> FindUnClearedExternalChequesByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> ClearExternalChequeAsync(ExternalChequeDTO externalChequeDTO, int clearingOption, int moduleNavigationItemCode, UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExternalChequeDTO>> FindUnBankedExternalChequesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> BankExternalChequesAsync(ObservableCollection<ExternalChequeDTO> externalChequeDTOs, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ExternalChequePayableDTO>> FindExternalChequePayablesByExternalChequeIdAsync(Guid externalChequeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateExternalChequePayablesByExternalChequeIdAsync(Guid externalChequeId, ObservableCollection<ExternalChequePayableDTO> externalChequePayables, ServiceHeader serviceHeader = null);

        #endregion

        #region FiscalCountDTO

        Task<PageCollectionInfo<FiscalCountDTO>> FindFiscalCountsByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FiscalCountDTO>> FindFiscalCountsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<FiscalCountDTO> AddFiscalCountAsync(FiscalCountDTO fiscalCountDTO, ServiceHeader serviceHeader = null);

        Task<FiscalCountDTO> FindFiscalCountAsync(Guid fiscalCountId, ServiceHeader serviceHeader = null);

        Task<bool> EndOfDayExecutedAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader = null);

        #endregion

        #region ElectronicJournalDTO //Elect

        Task<ElectronicJournalDTO> ParseElectronicJournalImportAsync(string fileName, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ElectronicJournalDTO>> FindElectronicJournalsByFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TruncatedChequeDTO>> FindTruncatedChequesByElectronicJournalIdAndFilterInPageAsync(Guid electronicJournalId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TruncatedChequeDTO>> FindTruncatedChequesByElectronicJournalIdAndStatusAndFilterInPageAsync(Guid electronicJournalId, int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> CloseElectronicJournalAsync(ElectronicJournalDTO electronicJournalDTO, ServiceHeader serviceHeader = null);

        Task<bool> ClearTruncatedChequeAsync(TruncatedChequeDTO truncatedChequeDTO, ServiceHeader serviceHeader = null);

        Task<bool> MatchTruncatedChequePaymentVoucherAsync(TruncatedChequeDTO truncatedChequeDTO, ServiceHeader serviceHeader = null);

        Task<TruncatedChequeDTO> FindTruncatedChequeAsync(Guid truncatedChequeId, ServiceHeader serviceHeader = null);

        #endregion

        #region InHouseChequeDTO

        Task<PageCollectionInfo<InHouseChequeDTO>> FindInHouseChequesByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<InHouseChequeDTO> AddInHouseChequeAsync(InHouseChequeDTO inHouseChequeDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> AddInHouseChequesAsync(ObservableCollection<InHouseChequeDTO> inHouseChequeDTOs, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InHouseChequeDTO>> FindInHouseChequesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InHouseChequeDTO>> FindInHouseChequesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InHouseChequeDTO>> FindUnPrintedInHouseChequesByBranchIdAndFilterInPageAsync(Guid branchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> PrintInHouseChequeAsync(InHouseChequeDTO inHouseChequeDTO, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        #endregion
       

        #region FixedDepositDTO

        Task<PageCollectionInfo<FixedDepositDTO>> FindFixedDepositsByStatusAndFilterInPageAsync(int status, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FixedDepositDTO>> FindFixedDepositsByFilterInPageAsync(string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FixedDepositDTO>> FindFixedDepositsByBranchIdInPageAsync(Guid branchId, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FixedDepositDTO>> FindPayableFixedDepositsByFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FixedDepositDTO>> FindRevocableFixedDepositsByFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<FixedDepositDTO> InvokeFixedDepositAsync(FixedDepositDTO fixedDepositDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditFixedDepositAsync(FixedDepositDTO fixedDepositDTO, int fixedDepositAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> RevokeFixedDepositsAsync(ObservableCollection<FixedDepositDTO> fixedDepositDTOs, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> PayFixedDepositAsync(FixedDepositDTO fixedDepositDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<FixedDepositDTO>> FindFixedDepositsByCustomerAccountIdAsync(Guid customerAccountId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<FixedDepositPayableDTO>> FindFixedDepositPayablesByFixedDepositIdAsync(Guid fixedDepositId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateFixedDepositPayablesByFixedDepositIdAsync(Guid fixedDepositId, ObservableCollection<FixedDepositPayableDTO> fixedDepositPayables, ServiceHeader serviceHeader = null);

        Task<bool> ExecutePayableFixedDepositsAsync(DateTime targetDate, int pageSize, ServiceHeader serviceHeader = null);

        Task<FixedDepositDTO> FindFixedDepositAsync(Guid fixedDepositId, ServiceHeader serviceHeader = null);

        #endregion

        #region SuperSaverPayableDTO

        Task<SuperSaverPayableDTO> AddNewSuperSaverPayableAsync(SuperSaverPayableDTO superSaverPayableDTO, ServiceHeader serviceHeader = null);

        Task<SuperSaverPayableDTO> FindSuperSaverPayableAsync(Guid superSaverPayableId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SuperSaverPayableDTO>> FindSuperSaverPayablesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> AuditSuperSaverPayableAsync(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuditOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeSuperSaverPayableAsync(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<SuperSaverInterestDTO> FindCustomerSuperSaverPayableAsync(Guid customerId, ServiceHeader serviceHeader = null);

        #endregion

        #region LoanPurposeDTO

        Task<PageCollectionInfo<LoanPurposeDTO>> FindLoanPurposesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanPurposeDTO>> FindLoanPurposesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LoanPurposeDTO> AddLoanPurposeAsync(LoanPurposeDTO loanPurposeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanPurposeAsync(LoanPurposeDTO loanPurposeDTO, ServiceHeader serviceHeader = null);

        Task<LoanPurposeDTO> FindLoanPurposeAsync(Guid loanPurposeId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanPurposeDTO>> FindLoanPurposesAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region LoanCaseDTO

        Task<bool> SubstituteLoanGuarantorsAsync(Guid substituteGuarantorCustomerId, ObservableCollection<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> AuditLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAuditOption, ServiceHeader serviceHeader = null);

        Task<bool> CancelLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanCancellationOption, ServiceHeader serviceHeader = null);

        Task<bool> ApproveLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanApprovalOption, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanAppraisalFactorDTO>> FindLoanAppraisalFactorsByLoanCaseIdAsync(Guid loanCaseId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanAppraisalFactorsAsync(Guid loanCaseId, ObservableCollection<LoanAppraisalFactorDTO> loanAppraisalFactors, ServiceHeader serviceHeader = null);

        Task<bool> AppraiseLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanCaseDTO>> FindLoanCasesInPageAsync(int pageIndex, int pageSize, bool includeBatchStatus = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanCaseDTO>> FindLoanCasesByFilterInPageAsync(string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus = false, ServiceHeader serviceHeader = null);

        Task<LoanCaseDTO> AddLoanCaseAsync(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanCaseAsync(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanGuarantorsByLoanCaseIdAsync(Guid loanCaseId, ObservableCollection<LoanGuarantorDTO> loanGuarantors, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanCollateralsByLoanCaseIdAsync(Guid loanCaseId, ObservableCollection<CustomerDocumentDTO> customerDocuments, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAttachedLoansByLoanCaseIdAsync(Guid loanCaseId, ObservableCollection<AttachedLoanDTO> attachedLoans, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanGuarantorDTO>> FindLoanGuarantorsByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanGuarantorDTO>> FindLoanGuarantorsByCustomerIdAndFilterInPageAsync(Guid customerId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanGuarantorDTO>> FindLoanGuarantorsByLoanCaseIdAsync(Guid loanCaseId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanCaseDTO>> FindLoanCasesByStatusAndFilterInPageAsync(int status, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanGuarantorDTO>> FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductIdAsync(Guid loanCaseCustomerId, Guid loanCaseLoanProductId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanGuarantorDTO>> FindLoanGuarantorsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanCaseDTO>> FindLoanCasesByLoanProductSectionAndFilterInPageAsync(int loanProductSection, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanCaseDTO>> FindLoanCasesByLoanProductCategoryAndFilterInPageAsync(int loanProductCategory, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, decimal approvedAmountThreshold, int pageIndex, int pageSize, bool includeBatchStatus = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AttachedLoanDTO>> FindAttachedLoansByLoanCaseIdAsync(Guid loanCaseId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanCaseDTO>> FindLoanCaseByLoanCaseNumberAsync(int caseNumber, ServiceHeader serviceHeader = null);

        Task<LoanCaseDTO> FindLoanCaseAsync(Guid loanCaseId, ServiceHeader serviceHeader = null);

        Task<LoanGuarantorDTO> FindLoanGuarantorAsync(Guid loanGuarantorId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanCaseDTO>> FindLoanCasesByLoanProductSectionAndFilterInPageAsync(int loanProductSection, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus = false, ServiceHeader serviceHeader = null);

        Task<LoanCaseDTO> FindLastLoanCaseByCustomerIdAsync(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanCaseDTO>> FindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanConditionAsync(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition, ServiceHeader serviceHeader = null);

        Task<bool> AttachLoanGuarantorsAsync(Guid sourceCustomerAccountId, Guid destinationLoanProductId, ObservableCollection<LoanGuarantorDTO> loanGuarantors, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanCaseDTO>> FindLoanCasesByCustomerIdInProcessAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<bool> RestructureLoanAsync(Guid branchId, Guid customerAccountId, double NPer, double Pmt, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanCollateralDTO>> FindLoanCollateralsByLoanCaseIdAsync(Guid loanCaseId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanGuarantorAttachmentHistoryEntryDTO>> FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryIdAsync(Guid loanGuarantorAttachmentHistoryId, ServiceHeader serviceHeader = null);

        Task<LoanGuarantorAttachmentHistoryEntryDTO> FindLoanGuarantorAttachmentHistoryEntryAsync(Guid loanGuarantorAttachmentHistoryEntryId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO>> FindLoanGuarantorAttachmentHistoryByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> RelieveLoanGuarantorsAsync(Guid loanGuarantorAttachmentHistoryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> RemoveLoanGuarantorsAsync(ObservableCollection<LoanGuarantorDTO> loanGuarantorDTOs, ServiceHeader serviceHeader = null);

        Task<LoanGuarantorDTO> AddLoanGuarantorAsync(LoanGuarantorDTO loanGuarantorDTO, ServiceHeader serviceHeader = null);

        Task<bool> ReleaseLoanGuarantorsByLoaneeCustomerAccountAsync(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanCaseDTO>> FindLoanCasesAsync(ServiceHeader serviceHeader = null);
        Task<bool> MarkLoanCaseDisbursed(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader = null);


        #endregion

        #region IncomeAdjustmentDTO

        Task<PageCollectionInfo<IncomeAdjustmentDTO>> FindIncomeAdjustmentsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<IncomeAdjustmentDTO>> FindIncomeAdjustmentsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<IncomeAdjustmentDTO> AddIncomeAdjustmentAsync(IncomeAdjustmentDTO incomeAdjustmentDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateIncomeAdjustmentAsync(IncomeAdjustmentDTO incomeAdjustmentDTO, ServiceHeader serviceHeader = null);

        Task<IncomeAdjustmentDTO> FindIncomeAdjustmentAsync(Guid incomeAdjustmentId, ServiceHeader serviceHeader = null);

        #endregion

        #region LoaningRemarkDTO

        Task<PageCollectionInfo<LoaningRemarkDTO>> FindLoaningRemarksByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoaningRemarkDTO>> FindLoaningRemarksInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LoaningRemarkDTO> AddLoaningRemarkAsync(LoaningRemarkDTO loaningRemarkDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoaningRemarkAsync(LoaningRemarkDTO loaningRemarkDTO, ServiceHeader serviceHeader = null);

        Task<LoaningRemarkDTO> FindLoaningRemarkAsync(Guid loaningRemarkId, ServiceHeader serviceHeader = null);

        #endregion

        #region DataAttachmentPeriodDTO

        Task<PageCollectionInfo<DataAttachmentPeriodDTO>> FindDataAttachmentPeriodsByFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DataAttachmentPeriodDTO>> FindDataAttachmentPeriodsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<DataAttachmentPeriodDTO> AddDataAttachmentPeriodAsync(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDataAttachmentPeriodAsync(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader = null);

        Task<DataAttachmentPeriodDTO> FindDataAttachmentPeriodAsync(Guid dataAttachmentPeriodId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DataAttachmentEntryDTO>> FindDataAttachmentEntriesByDataAttachmentPeriodIdAndFilterInPageAsync(Guid dataAttachmentPeriodId, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<DataAttachmentEntryDTO> AddDataAttachmentEntryAsync(DataAttachmentEntryDTO dataAttachmentEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveDataAttachmentEntriesAsync(ObservableCollection<DataAttachmentEntryDTO> dataAttachmentEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> CloseDataAttachmentPeriodAsync(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader = null);

        Task<DataAttachmentPeriodDTO> FindCurrentDataAttachmentPeriodAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeDTO

        Task<PageCollectionInfo<EmployeeDTO>> FindEmployeesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeDTO>> FindEmployeesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeDTO>> FindEmployeesByDepartmentIdAndFilterInPageAsync(Guid departmentId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EmployeeDTO> AddEmployeeAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader = null);

        Task<bool> CustomerIsEmployeeAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<EmployeeDTO> FindEmployeeAsync(Guid employeeId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployeeDTO>> FindEmployeesBySalaryGroupsBranchesAndDepartmentsAsync(SalaryProcessingDTO salaryPeriodDTO, ObservableCollection<SalaryGroupDTO> salaryGroups, ObservableCollection<BranchDTO> branches, ObservableCollection<DepartmentDTO> departments, ServiceHeader serviceHeader = null);

        Task<EmployeeDTO> FindEmployeeBySerialNumberAsync(int serialNumber, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployeeDTO>> FindEmployeesAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeDocumentDTO

        Task<PageCollectionInfo<EmployeeDocumentDTO>> FindEmployeeDocumentsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeDocumentDTO>> FindEmployeeDocumentsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EmployeeDocumentDTO> AddEmployeeDocumentAsync(EmployeeDocumentDTO employeeDocumentDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeDocumentAsync(EmployeeDocumentDTO employeeDocumentDTO, ServiceHeader serviceHeader = null);

        Task<EmployeeDocumentDTO> FindEmployeeDocumentAsync(Guid employeeDocumentId, ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeAppraisalTargetDTO

        Task<PageCollectionInfo<EmployeeAppraisalTargetDTO>> FindEmployeeAppraisalTargetsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeAppraisalTargetDTO>> FindEmployeeAppraisalTargetsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeAppraisalTargetDTO>> FindChildEmployeeAppraisalTargetsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<List<EmployeeAppraisalTargetDTO>> FindChildEmployeeAppraisalTargetsAsync(ServiceHeader serviceHeader = null);

        Task<EmployeeAppraisalTargetDTO> AddEmployeeAppraisalTargetAsync(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeAppraisalTargetAsync(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, ServiceHeader serviceHeader = null);

        Task<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargetAsync(Guid employeeAppraisalTargetId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployeeAppraisalTargetDTO>> FindEmployeeAppraisalTargetsAsync(bool updateDepth = false, bool traverseTree = true, ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeAppraisalDTO

        Task<bool> AddEmployeeAppraisalAsync(List<EmployeeAppraisalDTO> employeeAppraisalDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeAppraisalAsync(EmployeeAppraisalDTO employeeAppraisalDTO, ServiceHeader serviceHeader = null);

        Task<bool> AppraiseEmployeeAppraisalAsync(EmployeeAppraisalDTO employeeAppraisalDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeAppraisalDTO>> FindEmployeeAppraisalsByPeriodInPageAsync(Guid employeeId, Guid employeeAppraisalPeriodId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<List<EmployeeAppraisalDTO>> FindEmployeeAppraisalsByPeriodAsync(Guid employeeId, Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeAppraisalPeriodDTO

        Task<PageCollectionInfo<EmployeeAppraisalPeriodDTO>> FindEmployeeAppraisalPeriodsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeAppraisalPeriodDTO>> FindEmployeeAppraisalPeriodsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EmployeeAppraisalPeriodDTO> AddEmployeeAppraisalPeriodAsync(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeAppraisalPeriodAsync(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, ServiceHeader serviceHeader = null);

        Task<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriodAsync(Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader = null);

        Task<EmployeeAppraisalPeriodDTO> FindCurrentEmployeeAppraisalPeriodAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeAppraisalPeriodRecommendationDTO

        Task<bool> UpdateEmployeeAppraisalPeriodRecommendationAsync(EmployeeAppraisalPeriodRecommendationDTO employeeAppraisalPeriodRecommendationDTO, ServiceHeader serviceHeader = null);

        Task<EmployeeAppraisalPeriodRecommendationDTO> FindEmployeeAppraisalPeriodRecommendationAsync(Guid employeeId, Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeExitDTO

        Task<EmployeeExitDTO> AddNewEmployeeExitAsync(EmployeeExitDTO employeeExitDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeExitAsync(EmployeeExitDTO employeeExitDTO, ServiceHeader serviceHeader = null);

        Task<bool> VerifyEmployeeExitAsync(EmployeeExitDTO employeeExitDTO, int auditOption, ServiceHeader serviceHeader = null);

        Task<bool> ApproveEmployeeExitAsync(EmployeeExitDTO employeeExitDTO, int authorizationOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeExitDTO>> FindEmployeeExitsByFilterAndDateInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region DesignationDTO

        Task<List<DesignationDTO>> FindDesignationsAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DesignationDTO>> FindDesignationsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DesignationDTO>> FindDesignationsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DesignationDTO>> FindDesignationsTraverseAsync(bool updateDepth = false, bool traverseTree = true, ServiceHeader serviceHeader = null);

        Task<DesignationDTO> AddDesignationAsync(DesignationDTO designationDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDesignationAsync(DesignationDTO designationDTO, ServiceHeader serviceHeader = null);

        Task<DesignationDTO> FindDesignationAsync(Guid designationId, ServiceHeader serviceHeader = null);

        #endregion

        #region DepartmentDTO

        Task<PageCollectionInfo<DepartmentDTO>> FindDepartmentsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DepartmentDTO>> FindDepartmentsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DepartmentDTO>> FindDepartmentsAsync(ServiceHeader serviceHeader = null);

        Task<DepartmentDTO> AddDepartmentAsync(DepartmentDTO departmentDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDepartmentAsync(DepartmentDTO departmentDTO, ServiceHeader serviceHeader = null);

        Task<DepartmentDTO> FindDepartmentAsync(Guid departmentId, ServiceHeader serviceHeader = null);

        #endregion

        #region HolidayDTO

        Task<PageCollectionInfo<HolidayDTO>> FindHolidaysByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<HolidayDTO>> FindHolidaysByPostingPeriodAsync(Guid postingPeriodId, ServiceHeader serviceHeader = null);

        Task<HolidayDTO> AddHolidayAsync(HolidayDTO holidayDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateHolidayAsync(HolidayDTO holidayDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveHolidayAsync(Guid holidayId, ServiceHeader serviceHeader = null);

        Task<HolidayDTO> FindHolidayAsync(Guid holidayId, ServiceHeader serviceHeader = null);


        Task<DateTime?> FindBusinessDayAsync(int addValue, bool nextDay = true, ServiceHeader serviceHeader = null);
        Task<ObservableCollection<HolidayDTO>> FindHolidaysAsync(ServiceHeader serviceHeader = null);


        

        #endregion

        #region SalaryHeadDTO

        Task<PageCollectionInfo<SalaryHeadDTO>> FindSalaryHeadsByFilterInPageAsync(string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SalaryHeadDTO>> FindSalaryHeadsInPageAsync(int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<SalaryHeadDTO> AddSalaryHeadAsync(SalaryHeadDTO salaryHeadDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalaryHeadAsync(SalaryHeadDTO salaryHeadDTO, ServiceHeader serviceHeader = null);

        Task<SalaryHeadDTO> FindSalaryHeadAsync(Guid salaryHeadId, ServiceHeader serviceHeader = null);

        #endregion

        #region SalaryGroupDTO

        Task<PageCollectionInfo<SalaryGroupDTO>> FindSalaryGroupsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalaryGroupEntriesBySalaryGroupIdAsync(Guid salaryGroupId, ObservableCollection<SalaryGroupEntryDTO> salaryGroupEntries, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SalaryGroupEntryDTO>> FindSalaryGroupEntriesBySalaryGroupIdAsync(Guid salaryGroupId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SalaryGroupDTO>> FindSalaryGroupsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<SalaryGroupDTO> AddSalaryGroupAsync(SalaryGroupDTO salaryGroupDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalaryGroupAsync(SalaryGroupDTO salaryGroupDTO, ServiceHeader serviceHeader = null);

        Task<SalaryGroupDTO> FindSalaryGroupAsync(Guid salaryGroupId, ServiceHeader serviceHeader = null);

        Task<SalaryGroupEntryDTO> FindSalaryGroupEntryAsync(Guid salaryGroupEntryId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SalaryGroupDTO>> FindSalaryGroupsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region SalaryCardDTO

        Task<PageCollectionInfo<SalaryCardDTO>> FindSalaryCardsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SalaryCardDTO>> FindSalaryCardsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<SalaryCardDTO> AddSalaryCardAsync(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalaryCardAsync(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader = null);

        Task<SalaryCardDTO> FindSalaryCardAsync(Guid salaryCardId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SalaryCardEntryDTO>> FindSalaryCardEntriesBySalaryCardIdAsync(Guid salaryCardId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalaryCardEntryAsync(SalaryCardEntryDTO salaryCardEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> ResetSalaryCardEntriesAsync(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader = null);
        Task<ObservableCollection<SalaryCardDTO>> FindSalaryCardsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region SalaryPeriodDTO

        Task<PageCollectionInfo<SalaryProcessingDTO>> FindSalaryPeriodsByFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);
        Task<PageCollectionInfo<SalaryProcessingDTO>> FindSalaryPeriodsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<SalaryProcessingDTO> AddSalaryPeriodAsync(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalaryPeriodAsync(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader = null);

        Task<SalaryProcessingDTO> FindSalaryPeriodAsync(Guid salaryPeriodId, ServiceHeader serviceHeader = null);

        Task<bool> ProcessSalaryPeriodAsync(SalaryProcessingDTO salaryPeriodDTO, ObservableCollection<EmployeeDTO> employees, ServiceHeader serviceHeader = null);

        Task<bool> CloseSalaryPeriodAsync(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> PostPaySlipAsync(Guid paySlipId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);
        Task<List<SalaryProcessingDTO>> FindSalaryPeriodsAsync(ServiceHeader serviceHeader = null);


        #endregion

        #region PaySlipDTO

        Task<ObservableCollection<PaySlipDTO>> FindLoanAppraisalPaySlipsByCustomerIdAsync(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<PaySlipDTO>> FindPaySlipsBySalaryPeriodIdAsync(Guid salaryPeriodId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<PaySlipEntryDTO>> FindPaySlipEntriesByPaySlipIdAsync(Guid paySlipId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PaySlipDTO>> FindPaySlipsBySalaryPeriodIdInPageAsync(Guid salaryPeriodId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PaySlipDTO>> FindPaySlipsBySalaryPeriodIdAndFilterInPageAsync(Guid salaryPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PaySlipDTO>> FindQueablePaySlipsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PaySlipDTO> FindPaySlipAsync(Guid paySlipId, ServiceHeader serviceHeader = null);

        Task<PaySlipEntryDTO> FindPaySlipEntryAsync(Guid paySlipEntryId, ServiceHeader serviceHeader = null);

        #endregion

        #region CustomerDTO

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByStationIdAndFilterInPageAsync(Guid stationId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByTypeAndFilterInPageAsync(int type, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByRecordStatusAndFilterInPageAsync(int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDTO, List<DebitTypeDTO> mandatoryDebitTypes, List<InvestmentProductDTO> mandatoryInvestmentProducts, List<SavingsProductDTO> mandatorySavingsProducts, ProductCollectionInfo mandatoryProducts, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCustomerAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader = null);

        Task<CustomerDTO> FindCustomerAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerDTO>> FindCustomersAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerDTO>> FindCustomersByPayrollNumbersAsync(string payrollNumbers, bool matchExtact, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerDTO>> FindCustomersByIdentityCardNumberAsync(string identityCardNumber, bool matchExtact, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerDTO>> FindCustomersByIDNumberAsync(string identityCardNumber, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCustomerStationAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader = null);

        Task<bool> ResetCustomerStationAsync(ObservableCollection<CustomerDTO> customerDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCustomerBranchAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader = null);

        Task<decimal> ComputeDividendsPayableByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<PopulationRegisterQueryDTO> AddPopulationRegisterQueryAsync(PopulationRegisterQueryDTO populationRegisterQueryDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizePopulationRegisterQueryAsync(PopulationRegisterQueryDTO populationRegisterQueryDTO, int populationRegisterQueryAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> SyncPopulationRegisterQueryResponseAsync(PopulationRegisterQueryDTO populationRegisterQueryDTO, ServiceHeader serviceHeader = null);

        Task<PopulationRegisterQueryDTO> FindPopulationRegisterQueryAsync(Guid populationRegisterQueryId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PopulationRegisterQueryDTO>> FindPopulationRegisterQueriesByFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PopulationRegisterQueryDTO>> FindThirdPartyNotifiablePopulationRegisterQueriesByFilterInPageAsync(string text, int populationRegisterFilter, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader = null);

        Task<int> GetCustomersCountAsync(ServiceHeader serviceHeader);

        #endregion

        #region AccountAlertDTO

        Task<ObservableCollection<AccountAlertDTO>> FindAccountAlertCollectionByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AccountAlertDTO>> FindAccountAlertCollectionByCustomerIdAndAccountAlertTypeAsync(Guid customerId, int accountAlertType, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAccountAlertCollectionByCustomerIdAsync(Guid customerId, ObservableCollection<AccountAlertDTO> accountAlertCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region WithdrawalNotificationDTO

        Task<PageCollectionInfo<WithdrawalNotificationDTO>> FindWithdrawalNotificationsByStatusAndFilterInPageAsync(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<WithdrawalNotificationDTO>> FindWithdrawalNotificationsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<WithdrawalNotificationDTO>> FindWithdrawalNotificationsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<WithdrawalNotificationDTO> AddWithdrawalNotificationAsync(WithdrawalNotificationDTO withdrawalNotificationDTO, ServiceHeader serviceHeader = null);

        Task<bool> ApproveWithdrawalNotificationAsync(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalApprovalOption, ServiceHeader serviceHeader = null);

        Task<bool> AuditWithdrawalNotificationAsync(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalAuditOption, ServiceHeader serviceHeader = null);

        Task<bool> SettleWithdrawalNotificationAsync(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalSettlementOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> ProcessDeathSettlementsAsync(WithdrawalNotificationDTO withdrawalNotificationDTO, ObservableCollection<WithdrawalSettlementDTO> withdrawalSettlementDTOs, InsuranceCompanyDTO insuranceCompanyDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<WithdrawalNotificationDTO> FindWithdrawalNotificationAsync(Guid withdrawalNotificationId, ServiceHeader serviceHeader = null);
        Task<bool> UpdateWithdrawalNotificationAsync(WithdrawalNotificationDTO withdrawalNotificationDTO, ServiceHeader serviceHeader = null);
        Task<ObservableCollection<WithdrawalSettlementDTO>> FindWithdrawalSettlementsByWithdrawalNotificationIdAsync(Guid withdrawalNotificationId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        #endregion

        #region RefereeDTO

        Task<ObservableCollection<RefereeDTO>> FindRefereeCollectionByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateRefereeCollectionByCustomerIdAsync(Guid customerId, ObservableCollection<RefereeDTO> refereeCollection, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CreditTypeDTO>> FindCreditTypesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCreditTypesByCustomerIdAsync(Guid customerId, ObservableCollection<CreditTypeDTO> creditTypeCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region DelegateDTO

        Task<PageCollectionInfo<DelegateDTO>> FindDelegatesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DelegateDTO>> FindDelegatesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<DelegateDTO> AddDelegateAsync(DelegateDTO delegateDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDelegateAsync(DelegateDTO delegateDTO, ServiceHeader serviceHeader = null);

        Task<DelegateDTO> FindDelegateAsync(Guid delegateId, ServiceHeader serviceHeader = null);

        Task<List<DelegateDTO>> FindDelegatesAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region DirectorDTO
        Task<List<DirectorDTO>> FindDirectorsAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DirectorDTO>> FindDirectorsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DirectorDTO>> FindDirectorsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<DirectorDTO> AddDirectorAsync(DirectorDTO directorDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDirectorAsync(DirectorDTO directorDTO, ServiceHeader serviceHeader = null);

        Task<DirectorDTO> FindDirectorAsync(Guid directorId, ServiceHeader serviceHeader = null);

        #endregion

        #region CustomerDocumentDTO

        Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CustomerDocumentDTO> AddCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO, ServiceHeader serviceHeader = null);

        Task<CustomerDocumentDTO> FindCustomerDocumentAsync(Guid customerDocumentId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerDocumentDTO>> FindCustomerDocumentsByCustomerIdAndTypeAsync(Guid customerId, int type, ServiceHeader serviceHeader = null);

        #endregion

        #region NextOfKinDTO

        Task<ObservableCollection<NextOfKinDTO>> FindNextOfKinCollectionByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateNextOfKinCollectionAsync(CustomerDTO customerDTO, ObservableCollection<NextOfKinDTO> nextOfKinCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region PartnershipMemberDTO

        Task<ObservableCollection<PartnershipMemberDTO>> FindPartnershipMemberCollectionByPartnershipIdAsync(Guid partnershipId, ServiceHeader serviceHeader = null);

        Task<bool> UpdatePartnershipMemberCollectionByPartnershipIdAsync(Guid partnershipId, ObservableCollection<PartnershipMemberDTO> partnershipMemberCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region CorporationMemberDTO

        Task<ObservableCollection<CorporationMemberDTO>> FindCorporationMemberCollectionByCorporationIdAsync(Guid corporationId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCorporationMemberCollectionByCorporationIdAsync(Guid corporationId, ObservableCollection<CorporationMemberDTO> corporationMemberCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region EmployerDTO

        Task<bool> UpdateDivisionsByEmployerIdAsync(Guid employerId, ObservableCollection<DivisionDTO> divisions, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DivisionDTO>> FindDivisionsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ZoneDTO>> FindZonesByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployerDTO>> FindEmployersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployerDTO>> FindEmployersByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EmployerDTO> AddEmployerAsync(EmployerDTO employerDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployerAsync(EmployerDTO employerDTO, ServiceHeader serviceHeader = null);

        Task<EmployerDTO> FindEmployerAsync(Guid employerId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployerDTO>> FindEmployersAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DivisionDTO>> FindDivisionsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeDisciplinaryCaseDTO

        Task<EmployeeDisciplinaryCaseDTO> AddNewEmployeeDisciplinaryCaseAsync(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeDisciplinaryCaseAsync(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, ServiceHeader serviceHeader = null);

        Task<List<EmployeeDisciplinaryCaseDTO>> FindEmployeeDisciplinaryCasesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeDisciplinaryCaseDTO>> FindEmployeeDisciplinaryCasesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeDisciplinaryCaseDTO>> FindEmployeeDisciplinaryCasesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCaseAsync(Guid employeeDisciplinaryCaseId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeDisciplinaryCaseDTO>> FindEmployeeDisciplinaryCasesByEmployeeIdAsync(Guid employeeId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region TrainingPeriodDTO

        Task<TrainingPeriodDTO> AddNewTrainingPeriodAsync(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateTrainingPeriodAsync(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader = null);

        Task<TrainingPeriodDTO> FindTrainingPeriodAsync(Guid trainingPeriodId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TrainingPeriodDTO>> FindTrainingPeriodsFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region TrainingPeriodEntryDTO

        Task<TrainingPeriodEntryDTO> AddTrainingPeriodEntryAsync(TrainingPeriodEntryDTO trainingPeriodEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateTrainingPeriodEntriesByTrainingPeriodIdAsync(Guid trainingPeriodId, ObservableCollection<TrainingPeriodEntryDTO> trainingPeriodEntries, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TrainingPeriodEntryDTO>> FindTrainingPeriodEntriesByTrainingPeriodIdFilterInPageAsync(Guid trainingPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<TrainingPeriodEntryDTO>> FindTrainingPeriodEntriesByEmployeeIdFilterInPageAsync(Guid employeeId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> RemoveTrainingPeriodEntriesAsync(ObservableCollection<TrainingPeriodEntryDTO> trainingPeriodEntries, ServiceHeader serviceHeader = null);

        #endregion

        #region ExitInterviewQuestionDTO

        Task<PageCollectionInfo<ExitInterviewQuestionDTO>> FindExitInterviewQuestionsByFilterInPageAsync(string filter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ExitInterviewQuestionDTO> AddExitInterviewQuestionAsync(ExitInterviewQuestionDTO exitInterviewQuestionDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateExitInterviewQuestionAsync(ExitInterviewQuestionDTO exitInterviewQuestionDTO, ServiceHeader serviceHeader = null);

        Task<ExitInterviewQuestionDTO> FindExitInterviewQuestionAsync(Guid exitInterviewQuestionId, ServiceHeader serviceHeader = null);

        Task<List<ExitInterviewQuestionDTO>> FindUnlockedExitInterviewQuestionsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region ExitInterviewAnswerDTO

        Task<bool> AddExitInterviewAnswerAsync(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateExitInterviewAnswerAsync(ExitInterviewAnswerDTO exitInterviewAnswerDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExitInterviewAnswerDTO>> FindExitInterviewAnswersInPageAsync(Guid employeeId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<List<ExitInterviewAnswerDTO>> FindExitInterviewAnswersAsync(Guid employeeId, ServiceHeader serviceHeader = null);

        #endregion

        #region ZoneDTO

        Task<bool> UpdateStationsByZoneIdAsync(Guid zoneId, ObservableCollection<StationDTO> stations, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StationDTO>> FindStationsByZoneIdAsync(Guid zoneId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StationDTO>> FindStationsByDivisionIdAsync(Guid divisionId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StationDTO>> FindStationsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ZoneDTO>> FindZonesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ZoneDTO>> FindZonesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ZoneDTO> AddZoneAsync(ZoneDTO zoneDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateZoneAsync(ZoneDTO zoneDTO, ServiceHeader serviceHeader = null);

        Task<ZoneDTO> FindZoneAsync(Guid zoneId, ServiceHeader serviceHeader = null);

        Task<StationDTO> FindStationAsync(Guid stationId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StationDTO>> FindStationsAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ZoneDTO>> FindZonesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<StationDTO>> FindStationsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> RemoveStationAsync(Guid stationId, ServiceHeader serviceHeader = null);

        Task<bool> RemoveZoneAsync(Guid zoneId, ServiceHeader serviceHeader = null);

        Task<bool> RemoveDivisionAsync(Guid divisionId, ServiceHeader serviceHeader = null);

        Task<bool> RemoveEmployerAsync(Guid employerId, ServiceHeader serviceHeader = null);

        #endregion

        #region FileRegisterDTO

        Task<PageCollectionInfo<FileRegisterDTO>> FindFileRegistersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FileRegisterDTO>> FindFileRegistersByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FileMovementHistoryDTO>> FindFileMovementHistoryByFileRegisterIdInPageAsync(Guid fileRegisterId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<FileMovementHistoryDTO>> FindFileMovementHistoryByFileRegisterIdAsync(Guid fileRegisterId, ServiceHeader serviceHeader = null);

        Task<CustomerFileRegisterLastDepartmentInfo> FindFileRegisterAndLastDepartmentByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<bool> MultiDestinationDispatchAsync(ObservableCollection<FileMovementHistoryDTO> fileMovementHistoryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> SingleDestinationDispatchAsync(SingleDestinationDispatchModel singleDestinationDispatchModel, ObservableCollection<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader = null);

        Task<bool> ReceiveFilesAsync(ObservableCollection<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader = null);

        Task<bool> RecallFilesAsync(ObservableCollection<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FileRegisterDTO>> FindFileRegistersByFilterStatusAndLastFileMovementDestinationDepartmentIdInPageAsync(string text, int customerFilter, int fileMovementStatus, Guid lastDestinationDepartmentId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FileRegisterDTO>> FindFileRegistersByFilterExcludingLastDestinationDepartmentIdInPageAsync(string text, int customerFilter, Guid lastDestinationDepartmentId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region AlternateChannelDTO

        Task<AlternateChannelDTO> AddAlternateChannelAsync(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAlternateChannelAsync(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader = null);

        Task<bool> ReplaceAlternateChannelAsync(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader = null);

        Task<bool> RenewAlternateChannelAsync(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader = null);

        Task<bool> DelinkAlternateChannelAsync(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader = null);

        Task<bool> StopAlternateChannelAsync(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader = null);

        Task<AlternateChannelDTO> FindAlternateChannelAsync(Guid alternateChannelId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AlternateChannelDTO>> FindAlternateChannelsByCustomerIdAsync(Guid customerId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AlternateChannelDTO>> FindAlternateChannelsByCustomerAccountIdAsync(Guid customerAccountId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AlternateChannelDTO>> FindAlternateChannelsInPageAsync(int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AlternateChannelDTO>> FindAlternateChannelsByFilterInPageAsync(string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AlternateChannelDTO>> FindAlternateChannelsByCardNumberAndCardTypeAsync(string cardNumber, int cardType, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AlternateChannelDTO>> FindAlternateChannelsByTypeAndFilterInPageAsync(int type, int recordStatus, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AlternateChannelDTO>> FindThirdPartyNotifiableAlternateChannelsByTypeAndFilterInPageAsync(int type, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByAlternateChannelTypeAsync(int alternateChannelType, int alternateChannelKnownChargeType, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByAlternateChannelTypeAsync(int alternateChannelType, ObservableCollection<CommissionDTO> commissions, int alternateChannelKnownChargeType, int chargeBenefactor, ServiceHeader serviceHeader = null);

        #endregion

        #region ChartOfAccountDTO

        Task<PageCollectionInfo<ChartOfAccountDTO>> FindChartOfAccountsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<List<ChartOfAccountDTO>> FindChartOfAccountsAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<GeneralLedgerAccount>> FindGeneralLedgerAccountsAsync(bool includeBalances = false, bool updateDepth = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<GeneralLedgerAccount>> FindGeneralLedgerAccountsWithCategoryAndTextAsync(int? accountCategory, string text, bool updateDepth = false, ServiceHeader serviceHeader = null);

        //Task<ObservableCollection<GeneralLedgerAccount>> FindGeneralLedgerAccountsWithCategoryAndTextAsync(string text, int accountCategory, bool updateDepth = false, ServiceHeader serviceHeader = null);
        Task<GeneralLedgerAccount> FindGeneralLedgerAccountAsync(Guid chartOfAccountId, bool includeBalances, ServiceHeader serviceHeader = null);

        Task<ChartOfAccountDTO> FindChartOfAccountAsync(Guid chartOfAccountId, ServiceHeader serviceHeader = null);

        Task<ChartOfAccountDTO> AddChartOfAccountAsync(ChartOfAccountDTO chartOfAccountDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateChartOfAccountAsync(ChartOfAccountDTO chartOfAccountDTO, ServiceHeader serviceHeader = null);

        

        #endregion

        #region CustomerAccountDTO

        Task<bool> UpdateCustomerAccountAsync(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountDTO>> FindCustomerAccountsByCustomerIdAsync(Guid customerId, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<bool> ManageCustomerAccountAsync(Guid customerAccountId, int managementAction, string remarks, int remarkType, ServiceHeader serviceHeader = null);

        Task<bool> ChargeAccountActivationFeeAsync(Guid customerAccountId, string remarks, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountHistoryDTO>> FindCustomerAccountHistoryByCustomerAccountIdAsync(Guid customerAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountHistoryDTO>> FindCustomerAccountHistoryByCustomerAccountIdAndManagementActionAsync(Guid customerAccountId, int managementAction, ServiceHeader serviceHeader = null);

        Task<CustomerAccountDTO> FindCustomerAccountAsync(Guid customerAccountId, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);


       // Task<CustomerAccountDTO> FindCustomerAccountAsync(Guid customerAccountId, ServiceHeader serviceHeader = null);


        Task<CustomerAccountDTO> FindCustomerAccountByFullAccountNumberAsync(string fullAccountNumber, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountDTO>> FindCustomerAccountsInPageAsync(int pageIndex, int pageSize, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountDTO>> FindCustomerAccountsByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize, bool includeBalances, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountDTO>> FindCustomerAccountsByProductCodeAndFilterInPageAsync(int productCode, string text, int customerFilter, int pageIndex, int pageSize, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountDTO>> FindCustomerAccountsByProductCodeAndRecordStatusAndFilterInPageAsync(int productCode, int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountDTO>> FindCustomerAccountsByCustomerIdAndFilterInPageAsync(Guid customerId, int pageIndex, int pageSize, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountDTO>> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductIdAsync(Guid customerId, Guid targetProductId, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<CustomerAccountDTO> AddCustomerAccountAsync(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountDTO>> FindCustomerAccountsByCustomerIdAndProductCodesAsync(Guid customerId, int[] targetProductCodes, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountCarryForwardDTO>> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdInPageAsync(Guid benefactorCustomerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountCarryForwardDTO>> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdInPageAsync(Guid beneficiaryCustomerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountCarryForwardDTO>> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAsync(Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountCarryForwardDTO>> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountIdAsync(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountCarryForwardDTO>> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdAsync(Guid benefactorCustomerAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountArrearageDTO>> FindCustomerAccountArrearagesByCustomerAccountIdAsync(Guid customerAccountId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountArrearageDTO>> FindCustomerAccountArrearagesByCustomerAccountIdAndCategoryAsync(Guid customerAccountId, int category, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CustomerAccountSignatoryDTO>> FindCustomerAccountSignatoriesByCustomerAccountIdAsync(Guid customerAccountId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountSignatoryDTO>> FindCustomerAccountSignatoriesByCustomerAccountIdInPageAsync(Guid customerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CustomerAccountSignatoryDTO> AddCustomerAccountSignatoryAsync(CustomerAccountSignatoryDTO customerAccountSignatoryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveCustomerAccountSignatoriesAsync(ObservableCollection<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountDTO>> FindCustomerAccountsByCustomerAccountTypeTargetProductIdInPageAsync(Guid targetProductId, int pageIndex, int pageSize, bool includeBalances = false, bool includeProductDescription = false, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false, ServiceHeader serviceHeader = null);

        Task<bool> AddCustomerAccountsAsync(CustomerDTO customerDTO, ObservableCollection<SavingsProductDTO> savingsProducts, ObservableCollection<InvestmentProductDTO> investmentProducts, ObservableCollection<LoanProductDTO> loanProducts, ServiceHeader serviceHeader = null);

        Task<decimal> ComputeEligibleLoanAppraisalInvestmentsBalanceAsync(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<double> GetLoaneeAppraisalFactorByCustomerClassificationAsync(int customerClassification, Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<CustomerAccountArrearageDTO> AddCustomerAccountArrearageAsync(CustomerAccountArrearageDTO customerAccountArrearageDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCustomerAccountArrearagesAsync(ObservableCollection<CustomerAccountArrearageDTO> customerAccountArrearages, ServiceHeader serviceHeader = null);

        Task<CustomerAccountCarryForwardDTO> AddCustomerAccountCarryForwardAsync(CustomerAccountCarryForwardDTO customerAccountCarryForwardDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCustomerAccountCarryForwardInstallmentAsync(CustomerAccountCarryForwardInstallmentDTO customerAccountCarryForwardInstallmentDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO>> FindCustomerAccountCarryForwardInstallmentsByFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> AdjustCustomerAccountLoanInterestAsync(LoanInterestAdjustmentDTO loanInterestAdjustmentDTO, ServiceHeader serviceHeader = null);

        #endregion

        #region JournalDTO

        Task<double> FVAsync(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV, int Due, ServiceHeader serviceHeader = null);

        Task<double> PVAsync(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV, int Due, ServiceHeader serviceHeader = null);

        Task<double> PmtAsync(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV, int Due, ServiceHeader serviceHeader = null);

        Task<double> NPerAsync(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV, int Due, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AmortizationTableEntry>> RepaymentScheduleAsync(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV = 0, int Due = 0, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddJournalAsync(TransactionModel transactionModel, ObservableCollection<TariffWrapper> tariffs = null, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddJournalSingleEntryAsync(TransactionModel transactionModel, ObservableCollection<TariffWrapper> tariffs = null, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddJournalWithApportionmentsAsync(TransactionModel transactionModel, ObservableCollection<ApportionmentWrapper> apportionments, ObservableCollection<TariffWrapper> tariffs = null, ObservableCollection<DynamicChargeDTO> dynamicCharges = null, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddCashManagementJournalAsync(FiscalCountDTO fiscalCountDTO, TransactionModel transactionModel, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddJournalWithCustomerAccountAsync(CustomerTransactionModel customerTransactionModel, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddJournalWithCustomerAccountAndTariffsAsync(CustomerTransactionModel customerTransactionModel, ObservableCollection<TariffWrapper> tariffs, ServiceHeader serviceHeader = null);

        Task<bool> AddTariffJournalsWithCustomerAccountAsync(CustomerTransactionModel customerTransactionModel, ObservableCollection<TariffWrapper> tariffs, ServiceHeader serviceHeader = null);

        Task<bool> AddTariffJournalsWithCustomerAccountAsync(Guid? parentJournalId, Guid branchId, Guid alternateChannelLogId, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ObservableCollection<TariffWrapper> tariffs, ServiceHeader serviceHeader = null);

        Task<JournalDTO> AddJournalWithCustomerAccountAndAlternateChannelLogAndTariffsAsync(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ObservableCollection<TariffWrapper> tariffs, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalDTO>> FindReversibleJournalsByDateRangeAndFilterInPageAsync(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> ReverseJournalsAsync(ObservableCollection<JournalDTO> journalDTOs, string description, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> ReverseAlternateChannelJournalsAsync(Guid[] alternateChannelLogIds, ServiceHeader serviceHeader = null);

        Task<JournalDTO> FindJournalAsync(Guid journalId, ServiceHeader serviceHeader = null);

        Task<List<JournalEntryDTO>> FindJournalEntriesByJournalIdAsync(Guid journalId, ServiceHeader serviceHeader = null);

        #endregion

        #region JournalEntryDTO

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindLastXGeneralLedgerTransactionsByCustomerAccountIdAsync(CustomerAccountDTO customerAccountDTO, int lastXItems, bool tallyDebitsCredits = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool tallyDebitsCredits = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits = false, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalEntryDTO>> FindReversibleJournalEntriesByDateRangeAndFilterInPageAsync(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region PostingPeriodDTO

        Task<PageCollectionInfo<PostingPeriodDTO>> FindPostingPeriodsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PostingPeriodDTO>> FindPostingPeriodsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PostingPeriodDTO> AddPostingPeriodAsync(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader = null);

        Task<TrainingPeriodDTO> AddTrainingPeriodAsync(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdatePostingPeriodAsync(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader = null);

        Task<PostingPeriodDTO> FindPostingPeriodAsync(Guid postingPeriodId, ServiceHeader serviceHeader = null);

        Task<PostingPeriodDTO> FindCurrentPostingPeriodAsync(ServiceHeader serviceHeader = null);

        Task<bool> ClosePostingPeriodAsync(PostingPeriodDTO postingPeriodDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<List<PostingPeriodDTO>> FindPostingPeriodsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region CommissionDTO

        Task<PageCollectionInfo<CommissionDTO>> FindCommissionsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CommissionDTO>> FindCommissionsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsAsync(ServiceHeader serviceHeader = null);

        Task<CommissionDTO> AddCommissionAsync(CommissionDTO commissionDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionAsync(CommissionDTO commissionDTO, ServiceHeader serviceHeader = null);

        Task<CommissionDTO> FindCommissionAsync(Guid commissionId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> GetCommissionsForSystemTransactionTypeAsync(int systemTransactionType, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionSplitDTO>> FindCommissionSplitsByCommissionIdAsync(Guid commissionId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionSplitsByCommissionIdAsync(Guid commissionId, ObservableCollection<CommissionSplitDTO> commissionSplits, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionLeviesByCommissionIdAsync(Guid commissionId, ObservableCollection<CommissionLevyDTO> commissionLevies, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<GraduatedScaleDTO>> FindGraduatedScalesByCommissionIdAsync(Guid commissionId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateGraduatedScalesByCommissionIdAsync(Guid commissionId, ObservableCollection<GraduatedScaleDTO> graduatedScales, ServiceHeader serviceHeader = null);

        Task<bool> MapSystemTransactionTypeToCommissionsAsync(int systemTransactionType, ObservableCollection<CommissionDTO> commissions, ChargeDTO chargeDTO, ServiceHeader serviceHeader = null);
        Task<bool> RemoveSystemTransactionTypeFromCommissions(int systemPermissionType, List<CommissionDTO> CommissionDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> GetCommissionsForSystemTransactionType(int systemTransactionType, ServiceHeader serviceHeader = null);


        Task<bool> UpdateCommissionSplitAsync(CommissionSplitDTO commissionSplitDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LevyDTO>> FindLeviesByCommissionIdAsync(Guid commissionId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLeviesByCommissionIdAsync(Guid commissionId, ObservableCollection<LevyDTO> levies, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsAsync(int systemTransactionType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsByChequeTypeAsync(Guid chequeTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsByCreditBatchEntryAsync(CreditBatchEntryDTO creditBatchEntry, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsBySavingsProductAsync(Guid savingsProductId, int savingsProductKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsByLoanProductAsync(Guid loanProductId, int loanProductKnownChargeType, decimal bookBalance, decimal principalBalance, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsByAlternateChannelTypeAsync(int alternateChannelType, int alternateChannelTypeKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTariffsByTextAlertAsync(int systemTransactionCode, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        #endregion

        #region LevyDTO

        Task<PageCollectionInfo<LevyDTO>> FindLeviesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LevyDTO>> FindLeviesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LevyDTO>> FindLeviesAsync(ServiceHeader serviceHeader = null);

        Task<LevyDTO> AddLevyAsync(LevyDTO levyDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLevyAsync(LevyDTO levyDTO, ServiceHeader serviceHeader = null);

        Task<LevyDTO> FindLevyAsync(Guid levyId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LevySplitDTO>> FindLevySplitsByLevyIdAsync(Guid levyId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLevySplitsByLevyIdAsync(Guid levyId, ObservableCollection<LevySplitDTO> levySplits, ServiceHeader serviceHeader = null);

        #endregion

        #region SavingsProductDTO

        Task<PageCollectionInfo<SavingsProductDTO>> FindSavingsProductsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SavingsProductDTO>> FindSavingsProductsAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SavingsProductDTO>> FindMandatorySavingsProductsAsync(bool isMandatory, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SavingsProductDTO>> FindSavingsProductsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<SavingsProductDTO> AddSavingsProductAsync(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSavingsProductAsync(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader = null);

        Task<SavingsProductDTO> FindSavingsProductAsync(Guid savingsProductId, ServiceHeader serviceHeader = null);

        Task<SavingsProductDTO> FindDefaultSavingsProductAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsBySavingsProductIdAsync(Guid savingsProductId, int savingsProductKnownChargeType, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsBySavingsProductIdAsync(Guid savingsProductId, ObservableCollection<CommissionDTO> commissions, int savingsProductKnownChargeType, int chargeBenefactor, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SavingsProductExemptionDTO>> FindSavingsProductExemptionsBySavingsProductIdAsync(Guid savingsProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSavingsProductExemptionsBySavingsProductIdAsync(Guid savingsProductId, ObservableCollection<SavingsProductExemptionDTO> savingsProductExemptions, ServiceHeader serviceHeader = null);

        #endregion

        #region InvestmentProductDTO

        Task<PageCollectionInfo<InvestmentProductDTO>> FindInvestmentProductsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<InvestmentProductDTO>> FindInvestmentProductsAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<InvestmentProductDTO>> FindMandatoryInvestmentProductsAsync(bool isMandatory, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<InvestmentProductDTO>> FindInvestmentProductsByCodeAsync(int code, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InvestmentProductDTO>> FindInvestmentProductsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<InvestmentProductDTO> AddInvestmentProductAsync(InvestmentProductDTO investmentProductDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateInvestmentProductAsync(InvestmentProductDTO investmentProductDTO, ServiceHeader serviceHeader = null);

        Task<InvestmentProductDTO> FindInvestmentProductAsync(Guid investmentProductId, ServiceHeader serviceHeader = null);

        Task<InvestmentProductDTO> FindSuperSaverInvestmentProductAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<InvestmentProductExemptionDTO>> FindInvestmentProductExemptionsByInvestmentProductIdAsync(Guid investmentProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateInvestmentProductExemptionsByInvestmentProductIdAsync(Guid investmentProductId, ObservableCollection<InvestmentProductExemptionDTO> investmentProductExemptions, ServiceHeader serviceHeader = null);

        #endregion

        #region LoanProductDTO

        Task<PageCollectionInfo<LoanProductDTO>> FindLoanProductsByLoanProductSectionAndFilterInPageAsync(int loanProductSection, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanProductDTO>> FindLoanProductsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanProductDTO>> FindLoanProductsAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanProductDTO>> FindLoanProductsByCodeAsync(int code, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanProductDTO>> FindLoanProductsByInterestChargeModeAsync(int interestChargeMode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanProductDTO>> FindLoanProductsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LoanProductDTO> AddLoanProductAsync(LoanProductDTO loanProductDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanProductAsync(LoanProductDTO loanProductDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DynamicChargeDTO>> FindDynamicChargesByLoanProductIdAsync(Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DynamicChargeDTO>> FindDynamicChargesByLoanProductIdAndRecoveryModeAsync(Guid loanProductId, int recoveryMode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDynamicChargesByLoanProductIdAsync(Guid loanProductId, ObservableCollection<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader = null);

        Task<ProductCollectionInfo> FindAppraisalProductsByLoanProductIdAsync(Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAppraisalProductsByLoanProductIdAsync(Guid loanProductId, ProductCollectionInfo appraisalProductsTuple, ServiceHeader serviceHeader = null);

        Task<LoanProductDTO> FindLoanProductAsync(Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanCycleDTO>> FindLoanCyclesByLoanProductIdAsync(Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanCyclesByLoanProductIdAsync(Guid loanProductId, ObservableCollection<LoanCycleDTO> loanCycles, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanProductDeductibleDTO>> FindLoanProductDeductiblesByLoanProductIdAsync(Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanProductDeductiblesByLoanProductIdAsync(Guid loanProductId, ObservableCollection<LoanProductDeductibleDTO> loanProductDeductibles, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanProductAuxilliaryAppraisalFactorDTO>> FindLoanProductAuxilliaryAppraisalFactorsByLoanProductIdAsync(Guid loanProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanProductAuxilliaryAppraisalFactorsByLoanProductIdAsync(Guid loanProductId, ObservableCollection<LoanProductAuxilliaryAppraisalFactorDTO> loanProductAuxilliaryAppraisalFactors, ServiceHeader serviceHeader = null);

        Task<double> GetLoaneeAppraisalFactorAsync(Guid loanProductId, decimal totalValue, ServiceHeader serviceHeader = null);

        Task<double> GetGuarantorAppraisalFactorAsync(Guid loanProductId, decimal totalValue, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByLoanProductIdAsync(Guid loanProductId, int loanProductKnownChargeType, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByLoanProductIdAsync(Guid loanProductId, ObservableCollection<CommissionDTO> commissions, int loanProductKnownChargeType, int loanProductChargeBasisValue, ServiceHeader serviceHeader = null);

        #endregion

        #region DynamicChargeDTO

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByDynamicChargeIdAsync(Guid dynamicChargeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByDynamicChargeIdAsync(Guid dynamicChargeId, ObservableCollection<CommissionDTO> commissions, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DynamicChargeDTO>> FindDynamicChargesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DynamicChargeDTO>> FindDynamicChargesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DynamicChargeDTO>> FindDynamicChargesAsync(ServiceHeader serviceHeader = null);

        Task<DynamicChargeDTO> AddDynamicChargeAsync(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDynamicChargeAsync(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader = null);

        Task<DynamicChargeDTO> FindDynamicChargeAsync(Guid dynamicChargeId, ServiceHeader serviceHeader = null);

        #endregion

        #region CostCenterDTO

        Task<PageCollectionInfo<CostCenterDTO>> FindCostCentersByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CostCenterDTO>> FindCostCentersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CostCenterDTO> AddCostCenterAsync(CostCenterDTO costCenterDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCostCenterAsync(CostCenterDTO costCenterDTO, ServiceHeader serviceHeader = null);

        Task<CostCenterDTO> FindCostCenterAsync(Guid costCenterId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CostCenterDTO>> FindCostCentersAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region TellerDTO

        Task<PageCollectionInfo<TellerDTO>> FindTellersByFilterInPageAsync(int tellerType, string text, int pageIndex, int pageSize, bool includeBalances = false, ServiceHeader serviceHeader = null);

        Task<TellerDTO> AddTellerAsync(TellerDTO tellerDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TellerDTO>> FindTellersAsync(ServiceHeader serviceHeader = null);

        Task<bool> UpdateTellerAsync(TellerDTO tellerDTO, ServiceHeader serviceHeader = null);

        Task<TellerDTO> FindTellerAsync(Guid tellerId, bool includeBalance = false, ServiceHeader serviceHeader = null);

        Task<TellerDTO> FindTellerByEmployeeIdAsync(Guid employeeId, bool includeBalance = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TellerDTO>> FindTellersByTypeAsync(int tellerType, string reference, bool includeBalances = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TellerDTO>> FindTellersByReferenceAsync(string reference, bool includeBalances = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TariffWrapper>> ComputeTellerCashTariffsAsync(CustomerAccountDTO customerAccountDTO, decimal totalValue, int frontOfficeTransactionType, ServiceHeader serviceHeader = null);

        #endregion

        #region BankLinkageDTO

        Task<PageCollectionInfo<BankLinkageDTO>> FindBankLinkagesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BankLinkageDTO>> FindBankLinkagesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<BankLinkageDTO> AddBankLinkageAsync(BankLinkageDTO bankLinkageDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBankLinkageAsync(BankLinkageDTO bankLinkageDTO, ServiceHeader serviceHeader = null);

        Task<BankLinkageDTO> FindBankLinkageAsync(Guid bankLinkageId, ServiceHeader serviceHeader = null);

        Task<List<BankLinkageDTO>> FindBankLinkagesAsync(ServiceHeader serviceHeader = null);

        Task<BankLinkageDTO> FindBankLinkageByBankAccountIdAsync(Guid bankAccountId, ServiceHeader serviceHeader = null);

        #endregion

        #region BudgetDTO

        Task<bool> UpdateBudgetEntriesByBudgetIdAsync(Guid budgetId, ObservableCollection<BudgetEntryDTO> budgetEntries, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BudgetDTO>> FindBudgetsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BudgetDTO>> FindBudgetsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<BudgetDTO> AddBudgetAsync(BudgetDTO budgetDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBudgetAsync(BudgetDTO budgetDTO, ServiceHeader serviceHeader = null);

        Task<BudgetDTO> FindBudgetAsync(Guid budgetId, ServiceHeader serviceHeader = null);

        Task<List<BudgetDTO>> FindBudgetsAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BudgetEntryDTO>> FindBudgetEntriesByBudgetIdAsync(Guid budgetId, bool includeBalances, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BudgetEntryDTO>> FindBudgetEntriesByBudgetIdInPageAsync(Guid budgetId, int pageIndex, int pageSize, bool includeBalances, ServiceHeader serviceHeader = null);

        Task<bool> RemoveBudgetEntriesAsync(ObservableCollection<BudgetEntryDTO> budgetEntries, ServiceHeader serviceHeader = null);

        Task<BudgetEntryDTO> AddBudgetEntryAsync(BudgetEntryDTO budgetEntryDTO, ServiceHeader serviceHeader = null);

        Task<decimal> FetchBudgetBalanceByBranchIdAsync(Guid branchId, int type, Guid typeIdentifier, ServiceHeader serviceHeader = null);

        #endregion

        #region JournalVoucherDTO

        Task<PageCollectionInfo<JournalVoucherDTO>> FindJournalVouchersByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalVoucherDTO>> FindJournalVouchersByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalVoucherDTO>> FindJournalVouchersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalVoucherDTO>> FindJournalVouchersByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<JournalVoucherEntryDTO>> FindJournalVoucherEntriesByJournalVoucherIdAsync(Guid journalVoucherId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalVoucherEntryDTO>> FindJournalVoucherEntriesByJournalVoucherIdInPageAsync(Guid journalVoucherId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<JournalVoucherDTO> AddJournalVoucherAsync(JournalVoucherDTO journalVoucherDTO, ServiceHeader serviceHeader = null);

        Task<JournalVoucherDTO> FindJournalVoucherAsync(Guid journalVoucherId, ServiceHeader serviceHeader = null);

        Task<JournalVoucherEntryDTO> AddJournalVoucherEntryAsync(JournalVoucherEntryDTO journalVoucherEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateJournalVoucherAsync(JournalVoucherDTO journalVoucherDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveJournalVoucherEntriesAsync(ObservableCollection<JournalVoucherEntryDTO> journalVoucherEntries, ServiceHeader serviceHeader = null);

        Task<bool> AuditJournalVoucherAsync(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeJournalVoucherAsync(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<JournalVoucherDTO>> FindJournalVouchersAsync(ServiceHeader serviceHeader = null);

        Task<bool> UpdateJournalVoucherEntryCollectionAsync(Guid journalVoucherId, ObservableCollection<JournalVoucherEntryDTO> journalVoucherEntryCollection, ServiceHeader serviceHeader = null);

        Task<bool> UpdateJournalVoucherEntriesByJournalVoucherIdAsync(Guid journalVoucherId, ObservableCollection<JournalVoucherEntryDTO> journalVoucherEntries, ServiceHeader serviceHeader = null);

        #endregion

        #region TreasuryDTO

        Task<PageCollectionInfo<TreasuryDTO>> FindTreasuriesByFilterInPageAsync(string text, int pageIndex, int pageSize, bool includeBalances = false, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TreasuryDTO>> FindTreasuriesAsync(bool includeBalances, ServiceHeader serviceHeader);

        Task<TreasuryDTO> AddTreasuryAsync(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateTreasuryAsync(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader = null);

        Task<TreasuryDTO> FindTreasuryAsync(Guid treasuryId, bool includeBalance = false, ServiceHeader serviceHeader = null);

        Task<TreasuryDTO> FindTreasuryByBranchIdAsync(Guid branchId, bool includeBalance = false, ServiceHeader serviceHeader = null);

        #endregion

        #region InsuranceCompanyDTO

        Task<PageCollectionInfo<InsuranceCompanyDTO>> FindInsuranceCompaniesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InsuranceCompanyDTO>> FindInsuranceCompaniesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<InsuranceCompanyDTO>> FindInsuranceCompaniesAsync(ServiceHeader serviceHeader = null);

        Task<InsuranceCompanyDTO> AddInsuranceCompanyAsync(InsuranceCompanyDTO insuranceCompanyDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateInsuranceCompanyAsync(InsuranceCompanyDTO insuranceCompanyDTO, ServiceHeader serviceHeader = null);

        Task<InsuranceCompanyDTO> FindInsuranceCompanyAsync(Guid insuranceCompanyId, ServiceHeader serviceHeader = null);

        #endregion

        #region UnPayReasonDTO

        Task<PageCollectionInfo<UnPayReasonDTO>> FindUnPayReasonsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<UnPayReasonDTO>> FindUnPayReasonsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<UnPayReasonDTO> AddUnPayReasonAsync(UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateUnPayReasonAsync(UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader = null);

        Task<UnPayReasonDTO> FindUnPayReasonAsync(Guid unPayReasonId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByUnPayReasonIdAsync(Guid unPayReasonId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByUnPayReasonIdAsync(Guid unPayReasonId, ObservableCollection<CommissionDTO> commissions, ServiceHeader serviceHeader = null);

        #endregion

        #region ChequeBookDTO

        Task<ChequeBookDTO> AddChequeBookAsync(ChequeBookDTO chequeBookDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateChequeBookAsync(ChequeBookDTO chequeBookDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ChequeBookDTO>> FindChequeBooksByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ChequeBookDTO>> FindChequeBooksByTypeAndFilterInPageAsync(int type, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ChequeBookDTO> FindChequeBookAsync(Guid chequeBookId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ChequeBookDTO>> FindChequeBooksAsync(ServiceHeader seriviceHeader = null);
        Task<ObservableCollection<PaymentVoucherDTO>> FindPaymentVouchersByChequeBookIdAsync(Guid chequeBookId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PaymentVoucherDTO>> FindPaymentVouchersByChequeBookIdAndFilterInPageAsync(Guid chequeBookId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<bool> FlagPaymentVoucherAsync(PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader = null);

        #endregion

        #region ReportTemplateDTO

        Task<ReportTemplateDTO> PopulateReportTemplateAsync(ReportTemplateDTO reportTemplateDTO, ObservableCollection<ReportTemplateDTO> templateBalances, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ReportTemplateDTO>> FindReportTemplateBalancesAsync(Guid rootTemplateId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ReportTemplateDTO>> FindReportTemplatesAsync(bool updateDepth = false, bool traverseTree = true, ServiceHeader serviceHeader = null);

        Task<ReportTemplateDTO> FindReportTemplateAsync(Guid reportTemplateId, ServiceHeader serviceHeader = null);

        Task<ReportTemplateDTO> AddReportTemplateAsync(ReportTemplateDTO reportTemplateDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateReportTemplateAsync(ReportTemplateDTO reportTemplateDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ReportTemplateEntryDTO>> FindReportTemplateEntriesByReportTemplateIdAsync(Guid reportTemplateId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateReportTemplateEntriesAsync(Guid reportTemplateId, ObservableCollection<ReportTemplateEntryDTO> reportTemplateEntries, ServiceHeader serviceHeader = null);

        #endregion

        #region StandingOrderDTO

        Task<ObservableCollection<StandingOrderDTO>> FindStandingOrdersAsync(bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StandingOrderDTO>> FindStandingOrdersByBenefactorCustomerIdAsync(Guid benefactorCustomerId, int benefactorCustomerAccountTypeProductCode, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StandingOrderDTO>> FindStandingOrdersByBenefactorCustomerAccountIdAsync(Guid benefactorCustomerAccountId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<StandingOrderDTO>> FindStandingOrdersByBeneficiaryCustomerAccountIdAsync(Guid beneficiaryCustomerAccountId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<StandingOrderDTO>> FindStandingOrdersInPageAsync(int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<StandingOrderDTO>> FindStandingOrdersByFilterInPageAsync(string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<StandingOrderDTO>> FindStandingOrdersByTriggerAndFilterInPageAsync(int trigger, string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<StandingOrderDTO> AddStandingOrderAsync(StandingOrderDTO standingOrderDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateStandingOrderAsync(StandingOrderDTO standingOrderDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<StandingOrderHistoryDTO>> FindStandingOrderHistoryByStandingOrderIdInPageAsync(Guid standingOrderId, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<bool> FixSkippedStandingOrdersAsync(DateTime targetDate, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region ChequeTypeDTO

        Task<ObservableCollection<ChequeTypeDTO>> FindChequeTypesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ChequeTypeDTO>> FindChequeTypesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ChequeTypeDTO>> FindChequeTypesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ChequeTypeDTO> AddChequeTypeAsync(ChequeTypeDTO chequeTypeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateChequeTypeAsync(ChequeTypeDTO chequeTypeDTO, ServiceHeader serviceHeader = null);

        Task<ChequeTypeDTO> FindChequeTypeAsync(Guid chequeTypeId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByChequeTypeIdAsync(Guid chequeTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByChequeTypeIdAsync(Guid chequeTypeId, ObservableCollection<CommissionDTO> commissions, ServiceHeader serviceHeader = null);

        Task<ProductCollectionInfo> FindAttachedProductsByChequeTypeIdAsync(Guid chequeTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAttachedProductsByChequeTypeIdAsync(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader = null);

        #endregion

        #region OverDeductionBatchDTO ... *** Refund ***

        Task<PageCollectionInfo<OverDeductionBatchDTO>> FindOverDeductionBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<OverDeductionBatchEntryDTO>> FindOverDeductionBatchEntriesByOverDeductionBatchIdAsync(Guid overDeductionBatchId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<OverDeductionBatchEntryDTO>> FindOverDeductionBatchEntriesByOverDeductionBatchIdInPageAsync(Guid overDeductionBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<OverDeductionBatchDTO> AddOverDeductionBatchAsync(OverDeductionBatchDTO overDeductionBatchDTO, ServiceHeader serviceHeader = null);

        Task<OverDeductionBatchDTO> FindOverDeductionBatchAsync(Guid overDeductionBatchId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateOverDeductionBatchAsync(OverDeductionBatchDTO overDeductionBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditOverDeductionBatchAsync(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeOverDeductionBatchAsync(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BatchImportEntryWrapper>> ParseOverDeductionBatchImportAsync(Guid overDeductionBatchId, string fileName, ServiceHeader serviceHeader = null);

        Task<OverDeductionBatchEntryDTO> AddOverDeductionBatchEntryAsync(OverDeductionBatchEntryDTO overDeductionBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveOverDeductionBatchEntriesAsync(ObservableCollection<OverDeductionBatchEntryDTO> overDeductionBatchEntryDTOs, ServiceHeader serviceHeader = null);

        #endregion

        #region CreditTypeDTO

        Task<PageCollectionInfo<CreditTypeDTO>> FindCreditTypesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CreditTypeDTO>> FindCreditTypesAsync(ServiceHeader serviceHeader = null);

        Task<CreditTypeDTO> FindCreditTypeAsync(Guid creditTypeDTOId, ServiceHeader serviceHeader = null);
       

        Task<PageCollectionInfo<CreditTypeDTO>> FindCreditTypesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CreditTypeDTO> AddCreditTypeAsync(CreditTypeDTO creditTypeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCreditTypeAsync(CreditTypeDTO creditTypeDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByCreditTypeIdAsync(Guid creditTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByCreditTypeIdAsync(Guid creditTypeId, ObservableCollection<CommissionDTO> commissions, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DirectDebitDTO>> FindDirectDebitsByCreditTypeIdAsync(Guid creditTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDirectDebitsByCreditTypeIdAsync(Guid creditTypeId, ObservableCollection<DirectDebitDTO> directDebits, ServiceHeader serviceHeader = null);

        Task<ProductCollectionInfo> FindAttachedProductsByCreditTypeIdAsync(Guid creditTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAttachedProductsByCreditTypeIdAsync(Guid creditTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader = null);

        Task<ProductCollectionInfo> FindConcessionExemptProductsByCreditTypeIdAsync(Guid creditTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateConcessionExemptProductsByCreditTypeIdAsync(Guid creditTypeId, ProductCollectionInfo concessionExemptProductsTuple, ServiceHeader serviceHeader = null);

        #endregion

        #region DebitTypeDTO

        Task<PageCollectionInfo<DebitTypeDTO>> FindDebitTypesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DebitTypeDTO>> FindDebitTypesAsync(ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DebitTypeDTO>> FindMandatoryDebitTypesAsync(bool isMandatory, ServiceHeader serviceHeader = null);

        Task<DebitTypeDTO> FindDebitTypeAsync(Guid debitTypeDTOId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DebitTypeDTO>> FindDebitTypesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<DebitTypeDTO> AddDebitTypeAsync(DebitTypeDTO debitTypeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDebitTypeAsync(DebitTypeDTO debitTypeDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByDebitTypeIdAsync(Guid debitTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByDebitTypeIdAsync(Guid debitTypeId, ObservableCollection<CommissionDTO> commissions, ServiceHeader serviceHeader = null);

        #endregion


        #region DirectDebitDTO

        Task<PageCollectionInfo<DirectDebitDTO>> FindDirectDebitsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DirectDebitDTO>> FindDirectDebitsAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DirectDebitDTO>> FindDirectDebitsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<DirectDebitDTO> AddDirectDebitAsync(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDirectDebitAsync(DirectDebitDTO directDebitDTO, ServiceHeader serviceHeader = null);

        #endregion

        #region CreditBatchDTO

        Task<ObservableCollection<BatchImportEntryWrapper>> ParseCreditBatchImportAsync(Guid creditBatchId, string fileName, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CreditBatchDTO>> FindCreditBatchesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchDTO>> FindCreditBatchesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CreditBatchEntryDTO>> FindLoanAppraisalCreditBatchEntriesByCustomerIdAsync(Guid customerId, Guid loanProductId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CreditBatchEntryDTO>> FindCreditBatchEntriesByCustomerIdAsync(int creditBatchType, Guid customerId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchDTO>> FindCreditBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CreditBatchEntryDTO>> FindCreditBatchEntriesByCreditBatchIdAsync(Guid creditBatchId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchEntryDTO>> FindCreditBatchEntriesByCreditBatchIdInPageAsync(Guid creditBatchId, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchDiscrepancyDTO>> FindCreditBatchDiscrepanciesByCreditBatchIdInPageAsync(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchDiscrepancyDTO>> FindCreditBatchDiscrepanciesByCreditBatchTypeInPageAsync(int creditBatchType, int status, int productCode, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchDiscrepancyDTO>> FindCreditBatchDiscrepanciesInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchEntryDTO>> FindCreditBatchEntriesByCreditBatchTypeInPageAsync(int creditBatchType, DateTime startDate, DateTime endDate, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<CreditBatchDTO> AddCreditBatchAsync(CreditBatchDTO creditBatchDTO, ServiceHeader serviceHeader = null);

        Task<CreditBatchDTO> FindCreditBatchAsync(Guid creditBatchId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCreditBatchAsync(CreditBatchDTO creditBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditCreditBatchAsync(CreditBatchDTO creditBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeCreditBatchAsync(CreditBatchDTO creditBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> MatchCreditBatchDiscrepancyByGeneralLedgerAccountAsync(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, Guid chartOfAccountId, int moduleNavigationItemCode, int discrepancyAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> MatchCreditBatchDiscrepancyByCustomerAccountAsync(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, CustomerAccountDTO customerAccountDTO, int discrepancyAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> MatchCreditBatchDiscrepanciesByCustomerAccountAsync(ObservableCollection<CreditBatchDiscrepancyDTO> creditBatchDiscrepancyDTOs, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader = null);

        Task<CreditBatchEntryDTO> AddCreditBatchEntryAsync(CreditBatchEntryDTO creditBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveCreditBatchEntriesAsync(ObservableCollection<CreditBatchEntryDTO> creditBatchEntryDTOs, ServiceHeader serviceHeader = null);

        Task<CreditBatchEntryDTO> FindLastCreditBatchEntryByCustomerAccountIdAsync(Guid customerAccountId, int creditBatchType, ServiceHeader serviceHeader = null);

        Task<bool> PostCreditBatchEntryAsync(Guid creditBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CreditBatchEntryDTO>> FindQueableCreditBatchEntriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region DebitBatchDTO

        Task<ObservableCollection<BatchImportEntryWrapper>> ParseDebitBatchImportAsync(Guid debitBatchId, string fileName, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DebitBatchEntryDTO>> FindDebitBatchEntriesByCustomerIdAsync(Guid customerId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DebitBatchDTO>> FindDebitBatchesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DebitBatchDTO>> FindDebitBatchesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DebitBatchDTO>> FindDebitBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DebitBatchEntryDTO>> FindDebitBatchEntriesByDebitBatchIdAsync(Guid debitBatchId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DebitBatchEntryDTO>> FindDebitBatchEntriesByDebitBatchIdInPageAsync(Guid debitBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<DebitBatchDTO> AddDebitBatchAsync(DebitBatchDTO debitBatchDTO, ServiceHeader serviceHeader = null);

        Task<DebitBatchDTO> FindDebitBatchAsync(Guid debitBatchId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDebitBatchAsync(DebitBatchDTO debitBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditDebitBatchAsync(DebitBatchDTO debitBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeDebitBatchAsync(DebitBatchDTO debitBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> PostDebitBatchEntryAsync(Guid debitBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<DebitBatchEntryDTO> AddDebitBatchEntryAsync(DebitBatchEntryDTO debitBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveDebitBatchEntriesAsync(ObservableCollection<DebitBatchEntryDTO> debitBatchEntryDTOs, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DebitBatchEntryDTO>> FindQueableDebitBatchEntriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region WireTransferBatchDTO

        Task<PageCollectionInfo<WireTransferBatchDTO>> FindWireTransferBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<WireTransferBatchEntryDTO>> FindWireTransferBatchEntriesByWireTransferBatchIdAsync(Guid wireTransferBatchId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<WireTransferBatchEntryDTO>> FindWireTransferBatchEntriesByWireTransferBatchIdInPageAsync(Guid wireTransferBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<WireTransferBatchDTO> AddWireTransferBatchAsync(WireTransferBatchDTO wireTransferBatchDTO, ServiceHeader serviceHeader = null);

        Task<WireTransferBatchDTO> FindWireTransferBatchAsync(Guid wireTransferBatchId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateWireTransferBatchAsync(WireTransferBatchDTO wireTransferBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditWireTransferBatchAsync(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeWireTransferBatchAsync(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<WireTransferBatchEntryDTO> AddWireTransferBatchEntryAsync(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveWireTransferBatchEntriesAsync(ObservableCollection<WireTransferBatchEntryDTO> wireTransferBatchEntryDTOs, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BatchImportEntryWrapper>> ParseWireTransferBatchImportAsync(Guid wireTransferBatchId, string fileName, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<WireTransferBatchEntryDTO>> FindQueableWireTransferBatchEntriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> PostWireTransferBatchEntryAsync(Guid wireTransferBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        #endregion

        #region LoanProductAuxiliaryConditionDTO

        Task<ObservableCollection<LoanProductAuxiliaryConditionDTO>> FindLoanProductAuxiliaryConditionsAsync(Guid baseLoanProductId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanProductAuxiliaryConditionsByBaseLoanProductIdAsync(Guid baseLoanProductId, ObservableCollection<LoanProductAuxiliaryConditionDTO> loanProductAuxiliaryConditions, ServiceHeader serviceHeader = null);

        #endregion

        #region AlternateChannelLogDTO

        Task<AlternateChannelLogDTO> AddAlternateChannelLogAsync(AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader = null);

        Task<ISO8583AlternateChannelLogDTO> AddISO8583AlternateChannelLogAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader = null);

        Task<SPARROWAlternateChannelLogDTO> AddSPARROWAlternateChannelLogAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader = null);

        Task<WALLETAlternateChannelLogDTO> AddWALLETAlternateChannelLogAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsByRetrievalReferenceNumberAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAlternateChannelLogResponseAsync(Guid alternateChannelLogId, string payload, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SPARROWAlternateChannelLogDTO>> MatchSPARROWAlternateChannelLogsAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<WALLETAlternateChannelLogDTO>> MatchWALLETAlternateChannelLogsAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader = null);

        Task<bool> UpdateWALLETAlternateChannelLogCallbackPayloadAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader);

        #endregion

        #region CommissionExemptionDTO

        Task<ObservableCollection<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CommissionExemptionDTO>> FindCommissionExemptionsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdInPageAsync(Guid commissionExemptionId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CommissionExemptionDTO> AddCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO, ServiceHeader serviceHeader = null);

        Task<CommissionExemptionEntryDTO> AddCommissionExemptionEntryAsync(CommissionExemptionEntryDTO commissionExemptionEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveCommissionExemptionEntriesAsync(ObservableCollection<CommissionExemptionEntryDTO> commissionExemptionEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionExemptionEntryCollectionByCommissionExemptionIdAsync(Guid commissionExemptionId, ObservableCollection<CommissionExemptionEntryDTO> commissionExemptionEntryCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region RecurringBatchDTO

        Task<PageCollectionInfo<RecurringBatchEntryDTO>> FindQueableRecurringBatchEntriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<RecurringBatchDTO>> FindRecurringBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        //Task<ObservableCollection<RecurringBatchDTO>> FindRecurringBatchesAsync(ServiceHeader serviceHeader);
        Task<PageCollectionInfo<RecurringBatchEntryDTO>> FindRecurringBatchEntriesByRecurringBatchIdInPageAsync(Guid recurringBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> CapitalizeInterestByEmployersAndLoanProductsAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<EmployerDTO> employerDTOs, ObservableCollection<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader = null);

        Task<bool> CapitalizeInterestByCustomersAndLoanProductsAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<CustomerDTO> customerDTOs, ObservableCollection<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader = null);

        Task<bool> CapitalizeInterestByCreditTypesAndLoanProductsAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<CreditTypeDTO> creditTypeDTOs, ObservableCollection<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader = null);

        Task<bool> CapitalizeInterestAsync(int priority, ServiceHeader serviceHeader = null);

        Task<bool> ProcessSavingsProductLedgerFeesAsync(int priority, ServiceHeader serviceHeader = null);

        Task<bool> ChargeLoanDynamicFeesAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader = null);

        Task<bool> ChargeSavingsDynamicFeesAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<SavingsProductDTO> savingsProductDTOs, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteStandingOrdersAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<StandingOrderDTO> standingOrderDTOs, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteStandingOrdersByBenefactorEmployerAndTriggerAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<EmployerDTO> employerDTOs, int standingOrderTrigger, ServiceHeader serviceHeader = null);

        Task<bool> PostRecurringBatchEntryAsync(Guid recurringBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> NormalizeInvestmentBalancesAsync(string investmentNormalizationSets, int priority, bool enforceCeiling, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteScheduledStandingOrdersAsync(DateTime targetDate, int targetDateOption, int priority, int maximumStandingOrderExecuteAttemptCount, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteStandingOrdersByBenefactorEmployersAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<EmployerDTO> employerDTOs, ObservableCollection<SavingsProductDTO> savingsProductDTOs, ObservableCollection<LoanProductDTO> loanProductDTOs, ObservableCollection<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteStandingOrdersByBenefactorCustomersAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<CustomerDTO> customerDTOs, ObservableCollection<SavingsProductDTO> savingsProductDTOs, ObservableCollection<LoanProductDTO> loanProductDTOs, ObservableCollection<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteStandingOrdersByBenefactorCreditTypesAsync(RecurringBatchDTO recurringBatchDTO, ObservableCollection<CreditTypeDTO> creditTypeDTOs, ObservableCollection<SavingsProductDTO> savingsProductDTOs, ObservableCollection<LoanProductDTO> loanProductDTOs, ObservableCollection<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteSweepingStandingOrdersAsync(int priority, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> PoolInvestmentBalancesAsync(int priority, ServiceHeader serviceHeader = null);

        Task<bool> ReleaseLoanGuarantorsAsync(int priority, ServiceHeader serviceHeader = null);

        Task<bool> ExecuteElectronicStatementOrdersAsync(DateTime targetDate, int targetDateOption, string sender, int priority, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> RecoverArrearsAsync(int priority, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> RecoverArrearsFromInvestmentProductAsync(int priority, string targetProductCodes, int pageSize, ServiceHeader serviceHeader = null);

        #endregion

        #region LeaveTypeDTO

        Task<LeaveTypeDTO> AddNewLeaveTypeAsync(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLeaveTypeAsync(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader = null);

        Task<List<LeaveTypeDTO>> FindLeaveTypesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LeaveTypeDTO>> FindLeaveTypesFilterInPageAsync(string filterText, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LeaveTypeDTO> FindLeaveTypeAsync(Guid leaveTypeId, ServiceHeader serviceHeader = null);

        #endregion

        #region LeaveApplicationDTO

        Task<PageCollectionInfo<LeaveApplicationDTO>> FindLeaveApplicationsByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LeaveApplicationDTO>> FindLeaveApplicationsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LeaveApplicationDTO>> FindLeaveApplicationsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LeaveApplicationDTO> AddLeaveApplicationAsync(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLeaveApplicationAsync(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeLeaveApplicationAsync(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader = null);

        Task<bool> RecallLeaveApplicationAsync(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader = null);

        Task<LeaveApplicationDTO> FindLeaveApplicationAsync(Guid leaveApplicationId, ServiceHeader serviceHeader = null);

        Task<List<LeaveApplicationDTO>> FindLeaveApplicationsByEmployeeIdAsync(Guid employeeId, ServiceHeader serviceHeader = null);

        Task<List<LeaveApplicationDTO>> FindLeaveApplicationsByEmployeeIdAndLeaveTypeIdAsync(Guid employeeId, Guid leaveTypeId, ServiceHeader serviceHeader = null);

        Task<decimal> FindEmployeeLeaveBalancesAsync(Guid employeeId, Guid leaveTypeId, ServiceHeader serviceHeader = null);

        #endregion

        #region LoanDisbursementBatchDTO

        Task<ObservableCollection<LoanDisbursementBatchEntryDTO>> FindLoanDisbursementBatchEntriesByCustomerIdAsync(int loanDisbursementBatchType, Guid customerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanDisbursementBatchDTO>> FindLoanDisbursementBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanDisbursementBatchEntryDTO>> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchIdAsync(Guid loanDisbursementBatchId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanDisbursementBatchEntryDTO>> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchIdInPageAsync(Guid loanDisbursementBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanDisbursementBatchEntryDTO>> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchTypeInPageAsync(int loanDisbursementBatchType, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LoanDisbursementBatchDTO> AddLoanDisbursementBatchAsync(LoanDisbursementBatchDTO loanDisbursementBatchDTO, ServiceHeader serviceHeader = null);

        Task<LoanDisbursementBatchDTO> FindLoanDisbursementBatchAsync(Guid loanDisbursementBatchId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanDisbursementBatchAsync(LoanDisbursementBatchDTO loanDisbursementBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditLoanDisbursementBatchAsync(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeLoanDisbursementBatchAsync(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<LoanDisbursementBatchEntryDTO> AddLoanDisbursementBatchEntryAsync(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveLoanDisbursementBatchEntriesAsync(ObservableCollection<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanDisbursementBatchEntriesAsync(Guid loanDisbursementBatchId, List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntries, ServiceHeader serviceHeader = null);

        Task<bool> PostLoanDisbursementBatchEntryAsync(Guid loanDisbursementBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<decimal> DisburseMicroLoanAsync(Guid alternateChannelLogId, Guid settlementChartOfAccountId, CustomerAccountDTO customerLoanAccountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanDisbursementBatchEntryDTO>> FindQueableLoanDisbursementBatchEntriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> ValidateLoanDisbursementBatchEntriesExceedTransactionThresholdAsync(Guid loanDisbursementBatchId, Guid designationId, int transactionThresholdType, ServiceHeader serviceHeader = null);

        #endregion

        #region ApplicationDomainWrapper

        Task<PageCollectionInfo<ApplicationDomainWrapper>> FindApplicationDomainsByFilterInPageAsync(int pageIndex, int pageSize, string filter, ServiceHeader serviceHeader = null);

        Task<bool> ConfigureApplicationDatabaseAsync(ServiceHeader serviceHeader = null, double timeoutMinutes = 10d);

        Task<bool> ConfigureAspNetIdentityDatabaseAsync(ServiceHeader serviceHeader = null, double timeoutMinutes = 10d);

        Task<bool> ConfigureAspNetMembershipDatabaseAsync(ServiceHeader serviceHeader = null, double timeoutMinutes = 10d);

        Task<bool> SeedEnumerationsAsync(ServiceHeader serviceHeader = null, double timeoutMinutes = 10d);

        #endregion

        #region BankReconciliationPeriodDTO

        Task<PageCollectionInfo<BankReconciliationPeriodDTO>> FindBankReconciliationPeriodsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BankReconciliationPeriodDTO>> FindBankReconciliationPeriodsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<BankReconciliationPeriodDTO> AddBankReconciliationPeriodAsync(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBankReconciliationPeriodAsync(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader = null);

        Task<BankReconciliationPeriodDTO> FindBankReconciliationPeriodAsync(Guid bankReconciliationPeriodId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BankReconciliationEntryDTO>> FindBankReconciliationEntriesByBankReconciliationPeriodIdAndFilterInPageAsync(Guid bankReconciliationPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<BankReconciliationEntryDTO> AddBankReconciliationEntryAsync(BankReconciliationEntryDTO bankReconciliationEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveBankReconciliationEntriesAsync(ObservableCollection<BankReconciliationEntryDTO> bankReconciliationEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> CloseBankReconciliationPeriodAsync(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, int bankReconciliationPeriodAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        #endregion

        #region JournalReversalBatchDTO

        Task<PageCollectionInfo<JournalReversalBatchDTO>> FindJournalReversalBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<JournalReversalBatchEntryDTO>> FindJournalReversalBatchEntriesByJournalReversalBatchIdAsync(Guid journalReversalBatchId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalReversalBatchEntryDTO>> FindJournalReversalBatchEntriesByJournalReversalBatchIdInPageAsync(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalEntryDTO>> FindJournalEntriesByJournalReversalBatchIdInPageAsync(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<JournalReversalBatchDTO> AddJournalReversalBatchAsync(JournalReversalBatchDTO journalReversalBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateJournalReversalBatchAsync(JournalReversalBatchDTO journalReversalBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditJournalReversalBatchAsync(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeJournalReversalBatchAsync(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<JournalReversalBatchEntryDTO> AddJournalReversalBatchEntryAsync(JournalReversalBatchEntryDTO journalReversalBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveJournalReversalBatchEntriesAsync(ObservableCollection<JournalReversalBatchEntryDTO> journalReversalBatchEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateJournalReversalBatchEntriesAsync(Guid journalReversalBatchId, List<JournalReversalBatchEntryDTO> journalReversalBatchEntries, ServiceHeader serviceHeader = null);

        Task<JournalReversalBatchDTO> FindJournalReversalBatchByIdAsync(Guid journalReversalBatchId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<JournalReversalBatchEntryDTO>> FindQueableJournalReversalBatchEntriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<bool> PostJournalReversalBatchEntryAsync(Guid journalReversalBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        #endregion

        #region InterAccountTransferBatchDTO

        Task<PageCollectionInfo<InterAccountTransferBatchDTO>> FindInterAccountTransferBatchesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InterAccountTransferBatchDTO>> FindInterAccountTransferBatchesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InterAccountTransferBatchDTO>> FindInterAccountTransferBatchesByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<InterAccountTransferBatchEntryDTO>> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchIdAsync(Guid interAccountTransferBatchId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InterAccountTransferBatchEntryDTO>> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchIdInPageAsync(Guid interAccountTransferBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<InterAccountTransferBatchDTO> AddInterAccountTransferBatchAsync(InterAccountTransferBatchDTO interAccountTransferBatchDTO, ServiceHeader serviceHeader = null);

        Task<InterAccountTransferBatchDTO> FindInterAccountTransferBatchAsync(Guid interAccountTransferBatchId, ServiceHeader serviceHeader = null);

        Task<InterAccountTransferBatchEntryDTO> AddInterAccountTransferBatchEntryAsync(InterAccountTransferBatchEntryDTO interAccountTransferBatchEntryDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DynamicChargeDTO>> FindDynamicChargesByInterAccountTransferBatchIdAsync(Guid interAccountTransferBatchId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateInterAccountTransferBatchAsync(InterAccountTransferBatchDTO interAccountTransferBatchDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveInterAccountTransferBatchEntriesAsync(ObservableCollection<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntries, ServiceHeader serviceHeader = null);

        Task<bool> AuditInterAccountTransferBatchAsync(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeInterAccountTransferBatchAsync(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateInterAccountTransferBatchEntryCollectionAsync(Guid interAccountTransferBatchId, ObservableCollection<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryCollection, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDynamicChargesByInterAccountTransferBatchIdAsync(Guid interAccountTransferBatchId, ObservableCollection<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader = null);

        #endregion

        #region ExpensePayableDTO

        Task<PageCollectionInfo<ExpensePayableDTO>> FindExpensePayablesByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExpensePayableDTO>> FindExpensePayablesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExpensePayableDTO>> FindExpensePayablesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExpensePayableDTO>> FindExpensePayablesByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ExpensePayableEntryDTO>> FindExpensePayableEntriesByExpensePayableIdAsync(Guid expensePayableId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ExpensePayableEntryDTO>> FindExpensePayableEntriesByExpensePayableIdInPageAsync(Guid expensePayableId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ExpensePayableDTO> AddExpensePayableAsync(ExpensePayableDTO expensePayableDTO, ServiceHeader serviceHeader = null);
    
    
        Task<bool> UpdateExpensePayableEntriesByExpensePayableIdAsync(Guid expensePayableId, ObservableCollection<ExpensePayableEntryDTO> expensePayableEntries, ServiceHeader serviceHeader = null);
      
        Task<ExpensePayableDTO> FindExpensePayableAsync(Guid expensePayableId, ServiceHeader serviceHeader = null);

        Task<ExpensePayableEntryDTO> AddExpensePayableEntryAsync(ExpensePayableEntryDTO expensePayableEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateExpensePayableAsync(ExpensePayableDTO expensePayableDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveExpensePayableEntriesAsync(ObservableCollection<ExpensePayableEntryDTO> expensePayableEntries, ServiceHeader serviceHeader = null);

        Task<bool> AuditExpensePayableAsync(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeExpensePayableAsync(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateExpensePayableEntryCollectionAsync(Guid expensePayableId, ObservableCollection<ExpensePayableEntryDTO> expensePayableEntryCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region MobileToBankRequestDTO

        Task<PageCollectionInfo<MobileToBankRequestDTO>> FindMobileToBankRequestsByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<MobileToBankRequestDTO>> FindMobileToBankRequestsByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<MobileToBankRequestDTO>> FindMobileToBankRequestsByStatusRecordStatusAndFilterInPageAsync(int status, int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<MobileToBankRequestDTO> AddMobileToBankRequestAsync(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> ReconcileMobileToBankRequestAsync(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> AuditMobileToBankRequestReconciliationAsync(MobileToBankRequestDTO mobileToBankRequestDTO, int requestAuthOption, ServiceHeader serviceHeader = null);

        Task<MobileToBankRequestDTO> FindMobileToBankRequestAsync(Guid mobileToBankRequestId, ServiceHeader serviceHeader = null);

        #endregion

        #region BankToMobileRequestDTO

        Task<PageCollectionInfo<BankToMobileRequestDTO>> FindBankToMobileRequestsByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BankToMobileRequestDTO>> FindThirdPartyNotifiableBankToMobileRequestsByFilterInPageAsync(string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader = null);

        Task<BankToMobileRequestDTO> AddBankToMobileRequestAsync(BankToMobileRequestDTO bankToMobileRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBankToMobileRequestResponseAsync(Guid bankToMobileRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBankToMobileRequestIPNStatusAsync(Guid bankToMobileRequestId, int ipnStatus, string ipnResponse, ServiceHeader serviceHeader = null);

        Task<bool> ResetBankToMobileRequestsIPNStatusAsync(Guid[] bankToMobileRequestIds, ServiceHeader serviceHeader = null);

        Task<BankToMobileRequestDTO> FindBankToMobileRequestAsync(Guid bankToMobileRequestId, ServiceHeader serviceHeader = null);

        #endregion

        #region BrokerRequestDTO

        Task<PageCollectionInfo<BrokerRequestDTO>> FindBrokerRequestsByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<BrokerRequestDTO>> FindThirdPartyNotifiableBrokerRequestsByFilterInPageAsync(string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader = null);

        Task<BrokerRequestDTO> AddBrokerRequestAsync(BrokerRequestDTO brokerRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBrokerRequestResponseAsync(Guid brokerRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, ServiceHeader serviceHeader = null);

        Task<bool> UpdateBrokerRequestIPNStatusAsync(Guid brokerRequestId, int ipnStatus, string ipnResponse, ServiceHeader serviceHeader = null);

        Task<bool> ResetBrokerRequestsIPNStatusAsync(Guid[] brokerRequestIds, ServiceHeader serviceHeader = null);

        Task<BrokerRequestDTO> FindBrokerRequestAsync(Guid brokerRequestId, ServiceHeader serviceHeader = null);

        #endregion

        #region TransactionThresholdDTO

        Task<ObservableCollection<TransactionThresholdDTO>> FindTransactionThresholdCollectionByDesignationIdAsync(Guid designationId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<TransactionThresholdDTO>> FindTransactionThresholdCollectionByDesignationIdAndTransactionThresholdTypeAsync(Guid designationId, int transactionThresholdType, ServiceHeader serviceHeader = null);

        Task<bool> UpdateTransactionThresholdCollectionByDesignationIdAsync(Guid designationId, ObservableCollection<TransactionThresholdDTO> transactionThresholdCollection, ServiceHeader serviceHeader = null);

        Task<bool> ValidateTransactionThresholdByDesignationIdAsync(Guid designationId, int transactionThresholdType, decimal transactionAmount, ServiceHeader serviceHeader = null);

        #endregion

        #region EducationVenueDTO

        Task<PageCollectionInfo<EducationVenueDTO>> FindEducationVenuesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EducationVenueDTO>> FindEducationVenuesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EducationVenueDTO> AddEducationVenueAsync(EducationVenueDTO educationVenueDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEducationVenueAsync(EducationVenueDTO educationVenueDTO, ServiceHeader serviceHeader = null);

        Task<EducationVenueDTO> FindEducationVenueAsync(Guid educationVenueId, ServiceHeader serviceHeader = null);

        #endregion

        #region EducationRegisterDTO

        Task<PageCollectionInfo<EducationRegisterDTO>> FindEducationRegistersByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EducationRegisterDTO> AddEducationRegisterAsync(EducationRegisterDTO educationRegisterDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEducationRegisterAsync(EducationRegisterDTO educationRegisterDTO, ServiceHeader serviceHeader = null);

        Task<EducationRegisterDTO> FindEducationRegisterAsync(Guid educationRegisterId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EducationAttendeeDTO>> FindEducationAttendeesByEducationRegisterIdAsync(Guid educationRegisterId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EducationAttendeeDTO>> FindEducationAttendeesByEducationRegisterIdInPageAsync(Guid educationRegisterId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EducationAttendeeDTO> AddEducationAttendeeAsync(EducationAttendeeDTO educationAttendeeDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveEducationAttendeesAsync(ObservableCollection<EducationAttendeeDTO> educationAttendeeDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateMicroCreditMemberGroupCollectionByEducationRegisterIdAsync(Guid educationRegisterId, ObservableCollection<EducationAttendeeDTO> educationAttendeeCollection, ServiceHeader serviceHeader = null);

        #endregion

        #region GeneralLedgerDTO

        Task<PageCollectionInfo<GeneralLedgerDTO>> FindGeneralLedgersByStatusAndFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerDTO>> FindGeneralLedgersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerDTO>> FindGeneralLedgersByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<GeneralLedgerEntryDTO>> FindGeneralLedgerEntriesByGeneralLedgerIdAsync(Guid generalLedgerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<GeneralLedgerEntryDTO>> FindGeneralLedgerEntriesByGeneralLedgerIdInPageAsync(Guid generalLedgerId, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<GeneralLedgerDTO> AddGeneralLedgerAsync(GeneralLedgerDTO generalLedgerDTO, ServiceHeader serviceHeader = null);

        Task<GeneralLedgerDTO> FindGeneralLedgerAsync(Guid generalLedgerId, ServiceHeader serviceHeader = null);

        Task<GeneralLedgerEntryDTO> AddGeneralLedgerEntryAsync(GeneralLedgerEntryDTO generalLedgerEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateGeneralLedgerAsync(GeneralLedgerDTO generalLedgerDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveGeneralLedgerEntriesAsync(ObservableCollection<GeneralLedgerEntryDTO> generalLedgerEntries, ServiceHeader serviceHeader = null);

        Task<bool> AuditGeneralLedgerAsync(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, ServiceHeader serviceHeader = null);

        Task<bool> AuthorizeGeneralLedgerAsync(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);

        Task<bool> UpdateGeneralLedgerEntryCollectionAsync(Guid generalLedgerId, ObservableCollection<GeneralLedgerEntryDTO> generalLedgerEntryCollection, ServiceHeader serviceHeader = null);

        Task<BatchImportParseInfo> ParseGeneralLedgerImportEntriesAsync(GeneralLedgerEntryDTO generalLedgerEntryDTO, string fileName, ServiceHeader serviceHeader = null);

        #endregion

        #region ConditionalLendingDTO

        Task<ObservableCollection<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ConditionalLendingDTO>> FindConditionalLendingsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdInPageAsync(Guid conditionalLendingId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ConditionalLendingDTO> AddConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO, ServiceHeader serviceHeader = null);

        Task<ConditionalLendingEntryDTO> AddConditionalLendingEntryAsync(ConditionalLendingEntryDTO conditionalLendingEntryDTO, ServiceHeader serviceHeader = null);

        Task<ConditionalLendingDTO> FindConditionalLendingAsync(Guid conditionalLendingId, ServiceHeader serviceHeader = null);

        Task<bool> RemoveConditionalLendingEntriesAsync(ObservableCollection<ConditionalLendingEntryDTO> conditionalLendingEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> UpdateConditionalLendingEntryCollectionByConditionalLendingIdAsync(Guid conditionalLendingId, ObservableCollection<ConditionalLendingEntryDTO> conditionalLendingEntryCollection, ServiceHeader serviceHeader = null);

        Task<bool> FetchCustomerConditionalLendingStatusAsync(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader = null);

        #endregion

        #region AlternateChannelReconciliationPeriodDTO

        Task<PageCollectionInfo<AlternateChannelReconciliationPeriodDTO>> FindAlternateChannelReconciliationPeriodsByFilterInPageAsync(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AlternateChannelReconciliationPeriodDTO>> FindAlternateChannelReconciliationPeriodsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<AlternateChannelReconciliationPeriodDTO> AddAlternateChannelReconciliationPeriodAsync(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAlternateChannelReconciliationPeriodAsync(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, ServiceHeader serviceHeader = null);

        Task<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriodAsync(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AlternateChannelReconciliationEntryDTO>> FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodIdAndFilterInPageAsync(Guid alternateChannelReconciliationPeriodId, int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<AlternateChannelReconciliationEntryDTO> AddAlternateChannelReconciliationEntryAsync(AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveAlternateChannelReconciliationEntriesAsync(ObservableCollection<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs, ServiceHeader serviceHeader = null);

        Task<bool> CloseAlternateChannelReconciliationPeriodAsync(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, int alternateChannelReconciliationPeriodAuthOption, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<BatchImportEntryWrapper>> ParseAlternateChannelReconciliationImportAsync(Guid alternateChannelReconciliationPeriodId, string fileName, ServiceHeader serviceHeader = null);

        #endregion

        #region AccountClosureRequestDTO

        Task<PageCollectionInfo<AccountClosureRequestDTO>> FindAccountClosureRequestsByStatusAndFilterInPageAsync(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AccountClosureRequestDTO>> FindAccountClosureRequestsInPageAsync(int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AccountClosureRequestDTO>> FindAccountClosureRequestsByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<AccountClosureRequestDTO> AddAccountClosureRequestAsync(AccountClosureRequestDTO accountClosureRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> ApproveAccountClosureRequestAsync(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureApprovalOption, ServiceHeader serviceHeader = null);

        Task<bool> AuditAccountClosureRequestAsync(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureAuditOption, ServiceHeader serviceHeader = null);

        Task<bool> SettleAccountClosureRequestAsync(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureSettlementOption, ServiceHeader serviceHeader = null);

        Task<AccountClosureRequestDTO> FindAccountClosureRequestAsync(Guid accountClosureRequestId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAccountClosureRequestAsync(AccountClosureRequestDTO accountClosureRequestDTO, ServiceHeader serviceHeader = null);
        #endregion

        #region FixedDepositTypeDTO

        Task<PageCollectionInfo<FixedDepositTypeDTO>> FindFixedDepositTypesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<FixedDepositTypeDTO>> FindFixedDepositTypesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<FixedDepositTypeDTO> AddFixedDepositTypeAsync(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, ServiceHeader serviceHeader = null);

        Task<bool> UpdateFixedDepositTypeAsync(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, ServiceHeader serviceHeader = null);

        Task<FixedDepositTypeDTO> FindFixedDepositTypeAsync(Guid chequeTypeId, ServiceHeader serviceHeader = null);

        Task<List<FixedDepositTypeDTO>> FindFixedDepositTypesByMonthsAsync(int months, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LevyDTO>> FindLeviesByFixedDepositTypeIdAsync(Guid chequeTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLeviesByFixedDepositTypeIdAsync(Guid chequeTypeId, ObservableCollection<LevyDTO> levies, ServiceHeader serviceHeader = null);

        Task<ProductCollectionInfo> FindAttachedProductsByFixedDepositTypeIdAsync(Guid chequeTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAttachedProductsByFixedDepositTypeIdAsync(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader = null);

        #endregion

        #region FixedDepositTypeGraduatedScaleDTO

        Task<ObservableCollection<FixedDepositTypeGraduatedScaleDTO>> FindGraduatedScalesByFixedDepositTypeIdAsync(Guid fixedDepositTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateGraduatedScalesByFixedDepositTypeIdAsync(Guid fixedDepositTypeId, ObservableCollection<FixedDepositTypeGraduatedScaleDTO> graduatedScales, ServiceHeader serviceHeader = null);

        #endregion

        #region WireTransferTypeDTO

        Task<PageCollectionInfo<WireTransferTypeDTO>> FindWireTransferTypesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<WireTransferTypeDTO>> FindWireTransferTypesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<WireTransferTypeDTO>> FindWireTransferTypesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<WireTransferTypeDTO> AddWireTransferTypeAsync(WireTransferTypeDTO wireTransferTypeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateWireTransferTypeAsync(WireTransferTypeDTO wireTransferTypeDTO, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<CommissionDTO>> FindCommissionsByWireTransferTypeIdAsync(Guid wireTransferTypeId, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCommissionsByWireTransferTypeIdAsync(Guid wireTransferTypeId, ObservableCollection<CommissionDTO> commissions, ServiceHeader serviceHeader = null);

        #endregion---------------------------------------

        #region ElectronicStatementOrderDTO

        Task<ObservableCollection<ElectronicStatementOrderDTO>> FindElectronicStatementOrdersByCustomerIdAsync(Guid customerId, int customerAccountTypeProductCode, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<ElectronicStatementOrderDTO>> FindElectronicStatementOrdersByCustomerAccountIdAsync(Guid customerAccountId, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ElectronicStatementOrderDTO>> FindElectronicStatementOrdersInPageAsync(int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ElectronicStatementOrderDTO>> FindElectronicStatementOrdersByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ElectronicStatementOrderDTO> AddElectronicStatementOrderAsync(ElectronicStatementOrderDTO electronicStatementOrderDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateElectronicStatementOrderAsync(ElectronicStatementOrderDTO electronicStatementOrderDTO, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<ElectronicStatementOrderHistoryDTO>> FindElectronicStatementOrderHistoryByElectronicStatementOrderIdInPageAsync(Guid electronicStatementOrderId, int pageIndex, int pageSize, bool includeProductDescription, ServiceHeader serviceHeader = null);

        Task<ElectronicStatementOrderHistoryDTO> FindElectronicStatementOrderHistoryAsync(Guid electronicStatementOrderHistoryId, ServiceHeader serviceHeader = null);

        Task<bool> FixSkippedElectronicStatementOrdersAsync(DateTime targetDate, ServiceHeader serviceHeader = null);

        #endregion

        #region EmployeeTypeDTO

        Task<PageCollectionInfo<EmployeeTypeDTO>> FindEmployeeTypesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<EmployeeTypeDTO>> FindEmployeeTypesAsync(ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<EmployeeTypeDTO>> FindEmployeeTypesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<EmployeeTypeDTO> AddEmployeeTypeAsync(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateEmployeeTypeAsync(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader = null);

        Task<EmployeeTypeDTO> FindEmployeeTypeAsync(Guid employeeTypeId, ServiceHeader serviceHeader = null);

        #endregion

        #region LoanRequestDTO

        Task<LoanRequestDTO> AddLoanRequestAsync(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> CancelLoanRequestAsync(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> RegisterLoanRequestAsync(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader = null);

        Task<bool> RemoveLoanRequestAsync(Guid loanRequestId, ServiceHeader serviceHeader = null);

        Task<LoanRequestDTO> FindLoanRequestAsync(Guid loanRequestId, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanRequestDTO>> FindLoanRequestsByFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanRequestDTO>> FindLoanRequestsByStatusAndFilterInPageAsync(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanRequestDTO>> FindLoanRequestsByCustomerIdInProcessAsync(Guid customerId, ServiceHeader serviceHeader = null);

        #endregion

        #region AdministrativeDivisionDTO

        Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<AdministrativeDivisionDTO> AddAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO, ServiceHeader serviceHeader = null);

        Task<AdministrativeDivisionDTO> FindAdministrativeDivisionAsync(Guid administrativeDivisionId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(bool updateDepth = false, bool traverseTree = true, ServiceHeader serviceHeader = null);

        #endregion

        #region DivisionDTO

        Task<PageCollectionInfo<DivisionDTO>> FindDivisionsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<DivisionDTO>> FindDivisionsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<DivisionDTO> AddDivisionAsync(DivisionDTO divisionDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateDivisionAsync(DivisionDTO divisionDTO, ServiceHeader serviceHeader = null);

        Task<DivisionDTO> FindDivisionAsync(Guid divisionId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<DivisionDTO>> FindDivisionsAsync(bool updateDepth = false, bool traverseTree = true, ServiceHeader serviceHeader = null);

        #endregion

        #region LoanProductAppraisalProductDTO

        Task<PageCollectionInfo<LoanProductAppraisalProductDTO>> FindLoanProductAppraisalProductsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<LoanProductAppraisalProductDTO>> FindLoanProductAppraisalProductsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<LoanProductAppraisalProductDTO> AddLoanProductAppraisalProductAsync(LoanProductAppraisalProductDTO LoanProductAppraisalProductDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateLoanProductAppraisalProductAsync(LoanProductAppraisalProductDTO LoanProductAppraisalProductDTO, ServiceHeader serviceHeader = null);

        Task<LoanProductAppraisalProductDTO> FindLoanProductAppraisalProductAsync(Guid LoanProductAppraisalProductId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<LoanProductAppraisalProductDTO>> FindLoanProductAppraisalProductsAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region SystemGeneralLedgerAccountMappingDTO
        Task<PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO>> FindSystemGeneralLedgerAccountMappingsByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO>> FindSystemGeneralLedgerAccountMappingsInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<SystemGeneralLedgerAccountMappingDTO> AddSystemGeneralLedgerAccountMappingAsync(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSystemGeneralLedgerAccountMappingAsync(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, ServiceHeader serviceHeader = null);

        Task<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappingAsync(Guid systemGeneralLedgerAccountMappingId, ServiceHeader serviceHeader = null);

        Task<ObservableCollection<SystemGeneralLedgerAccountMappingDTO>> FindSystemGeneralLedgerAccountMappingsAsync(ServiceHeader serviceHeader = null);

        Task<Guid> GetChartOfAccountMappingForSystemGeneralLedgerAccountCodeAsync(int systemGeneralLedgerAccountCode, ServiceHeader serviceHeader = null);

        Task<bool> MapSystemGeneralLedgerAccountCodeToChartOfAccountAsync(int systemGeneralLedgerAccountCode, Guid chartOfAccountId, ServiceHeader serviceHeader = null);



        #endregion


        //Added DTO
        #region CategoryDTO

        Task<PageCollectionInfo<CategoryDTO>> FindCategoriesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<CategoryDTO>> FindCategoryInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateCategoryAsync(CategoryDTO categoryDTO, ServiceHeader serviceHeader = null);

        Task<CategoryDTO> FindCategoryAsync(Guid categoryId, ServiceHeader serviceHeader = null);

        Task<List<CategoryDTO>> FindCategoriesAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region InventoryDTO

        Task<PageCollectionInfo<InventoryDTO>> FindInventoriesByFilterInPageAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<InventoryDTO>> FindInventoriesInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<InventoryDTO> AddInventoryAsync(InventoryDTO InventoryDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateInventoryAsync(InventoryDTO InventoryDTO, ServiceHeader serviceHeader = null);

        Task<InventoryDTO> FindInventoryAsync(Guid InventoryId, ServiceHeader serviceHeader = null);

        Task<List<InventoryDTO>> FindInventoriesAsync(ServiceHeader serviceHeader = null);

        #endregion

        #region PurchaseOrderDTO

        Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrderByFilterInPageAsync(int itemType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrderInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PurchaseOrderDTO> AddPurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdatePurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO, ServiceHeader serviceHeader = null);

        Task<PurchaseOrderDTO> FindPurchaseOrderAsync(Guid policyId, ServiceHeader serviceHeader = null);

        Task<List<PurchaseOrderDTO>> FindPurchaseOrderByCodeAsync(string code, ServiceHeader serviceHeader = null);

        Task<List<PurchaseOrderDTO>> FindPurchaseOrdersAsync(ServiceHeader serviceHeader = null);

        Task<PurchaseOrderEntryDTO> AddPurchaseOrderEntryAsync(PurchaseOrderEntryDTO purchaseOrderEntryDTO, ServiceHeader serviceHeader = null);

        Task<List<PurchaseOrderEntryDTO>> FindPurchaseOrderEntriesByPurchaseOrderIdAsync(Guid policyId, ServiceHeader serviceHeader = null);







        #endregion

        #region SalesOrderDTO

        Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersByFilterInPageAsync(int itemType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersInPageAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader = null);

        Task<SalesOrderDTO> AddSalesOrderAsync(SalesOrderDTO salesOrderDTO, ServiceHeader serviceHeader = null);

        Task<bool> UpdateSalesOrderAsync(SalesOrderDTO salesOrderDTO, ServiceHeader serviceHeader = null);

        Task<SalesOrderDTO> FindSalesOrderAsync(Guid salesOrderId, ServiceHeader serviceHeader = null);

        Task<List<SalesOrderDTO>> FindSalesOrderByCodeAsync(string code, ServiceHeader serviceHeader = null);

        Task<List<SalesOrderDTO>> FindSalesOrdersAsync(ServiceHeader serviceHeader = null);

        Task<SalesOrderEntryDTO> AddSalesOrderEntryAsync(SalesOrderEntryDTO salesOrderEntryDTO, ServiceHeader serviceHeader = null);

        Task<List<SalesOrderEntryDTO>> FindSalesOrderEntriesBySalesOrderIdAsync(Guid salesOrderId, ServiceHeader serviceHeader = null);

        #endregion


        #region PurchaseInvoiceDTO


        Task<List<PurchaseInvoiceDTO>> FindPurchaseInvoicesAsync(ServiceHeader serviceHeader);

        Task<List<PurchaseInvoiceLineDTO>> FindPurchaseInvoiceLinesAsync(ServiceHeader serviceHeader);
        Task<PurchaseInvoiceDTO> AddNewPurchaseInvoiceAsync(PurchaseInvoiceDTO purchaseInvoiceDTO, ServiceHeader serviceHeader);

        Task<bool> UpdatePurchaseInvoiceAsync(PurchaseInvoiceDTO purchaseInvoiceDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostPurchaseInvoiceAsync(PurchaseInvoiceDTO purchaseInvoiceDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        #endregion

        #region PurchaseCreditMemoDTO

        Task<List<PurchaseCreditMemoDTO>> FindPurchaseCreditMemosAsync(ServiceHeader serviceHeader);
        Task<PurchaseCreditMemoDTO> AddNewPurchaseCreditMemoAsync(PurchaseCreditMemoDTO purchaseCreditMemoDTO, ServiceHeader serviceHeader);

        Task<bool> UpdatePurchaseCreditMemoAsync(PurchaseCreditMemoDTO purchaseCreditMemoDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostPurchaseCreditMemoAsync(PurchaseCreditMemoDTO purchaseCreditMemoDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        #endregion


        #region SalesInvoiceDTO

        //Task<SalesInvoiceDTO> FindSalesInvoiceAsync(Guid salesInvoiceId, ServiceHeader serviceHeader);
        Task<List<SalesInvoiceDTO>> FindSalesInvoicesAsync(ServiceHeader serviceHeader);
        Task<SalesInvoiceDTO> AddNewSalesInvoiceAsync(SalesInvoiceDTO salesInvoiceDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateSalesInvoiceAsync(SalesInvoiceDTO salesInvoiceDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostSalesInvoiceAsync(SalesInvoiceDTO salesInvoiceDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        #endregion

        #region SalesCreditMemoDTO

        Task<List<SalesCreditMemoDTO>> FindSalesCreditMemosAsync(ServiceHeader serviceHeader);
        Task<SalesCreditMemoDTO> AddNewSalesCreditMemoAsync(SalesCreditMemoDTO salesCreditMemoDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateSalesCreditMemoAsync(SalesCreditMemoDTO salesCreditMemoDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostSalesCreditMemoAsync(SalesCreditMemoDTO salesCreditMemoDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);


        Task<JournalDTO> PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);
        #endregion


        //Payment Voucher payment



        //Task<JournalDTO> PayCustomerInvoice(PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader = null);


        #region PaymentDTO

        Task<List<PaymentDTO>> FindPaymentsAsync(ServiceHeader serviceHeader);
        Task<PaymentDTO> AddNewPaymentAsync(PaymentDTO paymentDTO, ServiceHeader serviceHeader);

        Task<bool> UpdatePaymentAsync(PaymentDTO paymentDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostPaymentAsync(PaymentDTO paymentDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        #endregion


        #region ARCustomerDTO

        Task<List<ARCustomerDTO>> FindARCustomersAsync(ServiceHeader serviceHeader);

        Task<ARCustomerDTO> AddARCustomerAsync(ARCustomerDTO arCustomerDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateARCustomerAsync(ARCustomerDTO arCustomerDTO, ServiceHeader serviceHeader);

        Task<ARCustomerDTO> FindARCustomerAsync(Guid arCustomerId, ServiceHeader serviceHeader);

        #endregion


        #region ReceiptDTO

        Task<List<ReceiptDTO>> FindReceiptsAsync(ServiceHeader serviceHeader);
        Task<ReceiptDTO> AddNewReceiptAsync(ReceiptDTO receiptDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateReceiptAsync(ReceiptDTO receiptDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostReceiptAsync(ReceiptDTO receiptDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);


        #endregion


        #region ImprestDTO


        Task<List<ImprestDTO>> FindImprestsAsync(ServiceHeader serviceHeader);

        Task<List<ImprestLineDTO>> FindImprestLinesAsync(ServiceHeader serviceHeader);
        Task<ImprestDTO> AddNewImprestAsync(ImprestDTO imprestDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateImprestAsync(ImprestDTO imprestDTO, ServiceHeader serviceHeader);


        Task<JournalDTO> PostImprestAsync(ImprestDTO imprestDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);


        Task<JournalDTO> PostSurrenderAsync(ImprestDTO imprestDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

       // Task<JournalDTO> PayImprestAsync(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader = null);
        #endregion

    }
}
