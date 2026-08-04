using System;
using Infrastructure.Crosscutting.Framework.Utils;
//using Application.MainBoundedContext.ControlModule.Services;
using System.Threading.Tasks;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class WorkflowProcessorAppService : IWorkflowProcessorAppService
    {
        //private readonly IRequisitionAppService _requisitionAppService;
        //private readonly IPurchaseOrderAppService _purchaseOrderAppService;
        //private readonly IGoodsReceivedNoteAppService _goodsReceivedNoteAppService;
        //private readonly IInvoiceAppService _invoiceAppService;
        private readonly int _moduleNavigationCode;

        private readonly ICashDepositRequestAppService _cashDepositRequestAppService;

        private readonly ICashWithdrawalRequestAppService _cashWithdrawalRequestAppService;

        private readonly IExpensePayableAppService _expensePayableAppService;

        private readonly ISuperSaverPayableAppService _superSaverPayableAppService;

        private readonly IFuneralRiderClaimAppService _funeralRiderClaimAppService;

        private readonly IWorkflowAppService _workflowAppService;

        private readonly ICustomerAccountAppService _customerAccountAppService;

        public WorkflowProcessorAppService(

            ICashDepositRequestAppService cashDepositRequestAppService,

            ICashWithdrawalRequestAppService cashWithdrawalRequestAppService,

            IExpensePayableAppService expensePayableAppService,

            ISuperSaverPayableAppService superSaverPayableAppService,

            IFuneralRiderClaimAppService funeralRiderClaimAppService,

            IWorkflowAppService workflowAppService,

            ICustomerAccountAppService customerAccountAppService
            )
        {
            if (cashDepositRequestAppService == null)
                throw new ArgumentNullException(nameof(cashDepositRequestAppService));

            if (cashWithdrawalRequestAppService == null)
                throw new ArgumentNullException(nameof(cashWithdrawalRequestAppService));

            if (expensePayableAppService == null)
                throw new ArgumentNullException(nameof(expensePayableAppService));

            if (superSaverPayableAppService == null)
                throw new ArgumentNullException(nameof(superSaverPayableAppService));

            if (funeralRiderClaimAppService == null)
                throw new ArgumentNullException(nameof(funeralRiderClaimAppService));

            if (workflowAppService == null)
                throw new ArgumentNullException(nameof(workflowAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            _cashDepositRequestAppService = cashDepositRequestAppService;

            _cashWithdrawalRequestAppService = cashWithdrawalRequestAppService;

            _expensePayableAppService = expensePayableAppService;

            _superSaverPayableAppService = superSaverPayableAppService;

            _funeralRiderClaimAppService = funeralRiderClaimAppService;

            _workflowAppService = workflowAppService;

            _customerAccountAppService = customerAccountAppService;

            _moduleNavigationCode = 0;
        }

        public async Task<bool> ProcessWorkflowQueueAsync(Guid recordId, int workflowRecordType, int workflowRecordStatus, ServiceHeader serviceHeader)
        {
            var _workflowRecordType = (SystemPermissionType)workflowRecordType;

            var result = default(bool);

            var approved = (WorkflowApprovalOption)workflowRecordStatus == WorkflowApprovalOption.Approved;

            switch (_workflowRecordType)
            {
                #region Requisition

                //case SystemPermissionType.RequisitionOrigination:

                //    result = await _requisitionAppService.AuthorizeRequisitionAsync(recordId, workflowRecordStatus, serviceHeader);

                //    break;

                //#endregion

                //#region Purchase Order

                //case SystemPermissionType.PurchaseOrderOrigination:

                //    result = await _purchaseOrderAppService.AuthorizePurchaseOrderAsync(recordId, workflowRecordStatus, serviceHeader);

                //    break;

                //#endregion

                //#region Goods Received Note

                //case SystemPermissionType.GoodsReceivedNoteOrigination:

                //    result = await _goodsReceivedNoteAppService.AuthorizeGoodsReceivedNoteAsync(recordId, workflowRecordStatus, serviceHeader);

                //    break;

                //#endregion

                //#region Invoice

                //case SystemPermissionType.InvoiceOrigination:

                //    result = await _invoiceAppService.AuthorizeInvoiceAsync(recordId, workflowRecordStatus, _moduleNavigationCode, serviceHeader);

                //    break;
                #endregion

                case SystemPermissionType.CashDepositRequestAuthorization:

                    var cashDepositRequestDTO = _cashDepositRequestAppService.FindCashDepositRequest(recordId, serviceHeader);

                    if (cashDepositRequestDTO != null)
                    {
                        var customerTransactionAuthOption = (int)(approved
                            ? CustomerTransactionAuthOption.Authorize
                            : CustomerTransactionAuthOption.Reject);

                        result = _cashDepositRequestAppService.AuthorizeCashDepositRequest(cashDepositRequestDTO, customerTransactionAuthOption, serviceHeader);

                        if (result)
                            result = _workflowAppService.MarkWorkflowMatched(recordId, workflowRecordType, serviceHeader);
                    }

                    break;

                case SystemPermissionType.CashWithdrawalRequestAuthorization:

                    var cashWithdrawalRequestDTO = _cashWithdrawalRequestAppService.FindCashWithdrawalRequest(recordId, serviceHeader);

                    if (cashWithdrawalRequestDTO != null)
                    {
                        var customerTransactionAuthOption = (int)(approved
                            ? CustomerTransactionAuthOption.Authorize
                            : CustomerTransactionAuthOption.Reject);

                        result = _cashWithdrawalRequestAppService.AuthorizeCashWithdrawalRequest(cashWithdrawalRequestDTO, customerTransactionAuthOption, serviceHeader);

                        if (result)
                            result = _workflowAppService.MarkWorkflowMatched(recordId, workflowRecordType, serviceHeader);
                    }

                    break;

                case SystemPermissionType.ExpensePayablesAuthorization:

                    var expensePayableDTO = _expensePayableAppService.FindExpensePayable(recordId, serviceHeader);

                    if (expensePayableDTO != null)
                    {
                        var expensePayableAuthOption = (int)(approved
                            ? ExpensePayableAuthOption.Post
                            : ExpensePayableAuthOption.Reject);

                        result = _expensePayableAppService.AuthorizeExpensePayable(expensePayableDTO, expensePayableAuthOption, _moduleNavigationCode, serviceHeader);

                        if (result)
                            result = _workflowAppService.MarkWorkflowMatched(recordId, workflowRecordType, serviceHeader);
                    }

                    break;

                case SystemPermissionType.SuperSaverPayableAuthorization:

                    var superSaverPayableDTO = _superSaverPayableAppService.FindSuperSaverPayable(recordId, serviceHeader);

                    if (superSaverPayableDTO != null)
                    {
                        var superSaverPayableAuthOption = (int)(approved
                            ? SuperSaverPayableAuthOption.Posted
                            : SuperSaverPayableAuthOption.Rejected);

                        result = _superSaverPayableAppService.AuthorizeSuperSaverPayable(superSaverPayableDTO, superSaverPayableAuthOption, _moduleNavigationCode, serviceHeader);

                        if (result)
                            result = _workflowAppService.MarkWorkflowMatched(recordId, workflowRecordType, serviceHeader);
                    }

                    break;

                case SystemPermissionType.CustomerAccountVerification:

                    var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(recordId, serviceHeader);

                    if (customerAccountDTO != null)
                    {
                        var recordAuthOption = (int)(approved
                            ? RecordAuthOption.Approve
                            : RecordAuthOption.Reject);

                        result = _customerAccountAppService.AuthorizeCustomerAccount(customerAccountDTO, recordAuthOption, serviceHeader);

                        if (result)
                            result = _workflowAppService.MarkWorkflowMatched(recordId, workflowRecordType, serviceHeader);
                    }

                    break;

                case SystemPermissionType.FuneralRiderClaimPayableAuthorization:

                    var funeralRiderClaimPayableDTO = await _funeralRiderClaimAppService.FindFuneralRiderClaimPayableAsync(recordId, serviceHeader);

                    if (funeralRiderClaimPayableDTO != null)
                    {
                        var authorizationOption = (int)(approved
                            ? AuthorizationOption.Posted
                            : AuthorizationOption.Rejected);

                        result = _funeralRiderClaimAppService.AuthorizeFuneralRiderClaimPayable(funeralRiderClaimPayableDTO, authorizationOption, _moduleNavigationCode, serviceHeader);

                        if (result)
                            result = _workflowAppService.MarkWorkflowMatched(recordId, workflowRecordType, serviceHeader);
                    }

                    break;
            }

            return result;
        }
    }
}
