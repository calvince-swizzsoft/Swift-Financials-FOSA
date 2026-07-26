using System;
using Infrastructure.Crosscutting.Framework.Utils;
using Application.MainBoundedContext.ControlModule.Services;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class WorkflowProcessorAppService : IWorkflowProcessorAppService
    {
        private readonly IRequisitionAppService _requisitionAppService;
        private readonly IPurchaseOrderAppService _purchaseOrderAppService;
        private readonly IGoodsReceivedNoteAppService _goodsReceivedNoteAppService;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly int _moduleNavigationCode;


        public WorkflowProcessorAppService(
            IRequisitionAppService requisitionAppService,
            IPurchaseOrderAppService purchaseOrderAppService,
            IGoodsReceivedNoteAppService goodsReceivedNoteAppService,
            IInvoiceAppService invoiceAppService)
        {
            if (requisitionAppService == null)
                throw new ArgumentNullException(nameof(requisitionAppService));

            if (purchaseOrderAppService == null)
                throw new ArgumentNullException(nameof(purchaseOrderAppService));

            if (goodsReceivedNoteAppService == null)
                throw new ArgumentNullException(nameof(goodsReceivedNoteAppService));

            if (invoiceAppService == null)
                throw new ArgumentNullException(nameof(invoiceAppService));

            _requisitionAppService = requisitionAppService;

            _purchaseOrderAppService = purchaseOrderAppService;

            _goodsReceivedNoteAppService = goodsReceivedNoteAppService;

            _invoiceAppService = invoiceAppService;

            _moduleNavigationCode = 0x9999;
        }

        public async Task<bool> ProcessWorkflowQueueAsync(Guid recordId, int workflowRecordType, int workflowRecordStatus, ServiceHeader serviceHeader)
        {
            var _workflowRecordType = (SystemPermissionType)workflowRecordType;

            var result = default(bool);

            switch (_workflowRecordType)
            {
                #region Requisition

                case SystemPermissionType.RequisitionOrigination:

                    result = await _requisitionAppService.AuthorizeRequisitionAsync(recordId, workflowRecordStatus, serviceHeader);

                    break;

                #endregion

                #region Purchase Order

                case SystemPermissionType.PurchaseOrderOrigination:

                    result = await _purchaseOrderAppService.AuthorizePurchaseOrderAsync(recordId, workflowRecordStatus, serviceHeader);

                    break;

                #endregion

                #region Goods Received Note

                case SystemPermissionType.GoodsReceivedNoteOrigination:

                    result = await _goodsReceivedNoteAppService.AuthorizeGoodsReceivedNoteAsync(recordId, workflowRecordStatus, serviceHeader);

                    break;

                #endregion

                #region Invoice

                case SystemPermissionType.InvoiceOrigination:

                    result = await _invoiceAppService.AuthorizeInvoiceAsync(recordId, workflowRecordStatus, _moduleNavigationCode, serviceHeader);

                    break;
                    #endregion
            }

            return result;
        }
    }
}
