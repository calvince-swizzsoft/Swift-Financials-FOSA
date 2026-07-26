using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg;
using Domain.Seedwork;
using Numero3.EntityFramework.Interfaces;
using System;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Crosscutting.Framework.Adapter;
using Domain.Seedwork.Specification;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class CashTransferRequestAppService : ICashTransferRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CashTransferRequest> _cashTransferRequestRepository;

        public CashTransferRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CashTransferRequest> cashTransferRequestRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (cashTransferRequestRepository == null)
                throw new ArgumentNullException(nameof(cashTransferRequestRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _cashTransferRequestRepository = cashTransferRequestRepository;
        }

        public async Task<CashTransferRequestDTO> AddNewCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, ServiceHeader serviceHeader)
        {
            var cashTransferRequestBindingModel = cashTransferRequestDTO.ProjectedAs<CashTransferRequestBindingModel>();

            cashTransferRequestBindingModel.ValidateAll();

            if (cashTransferRequestBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, cashTransferRequestBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var cashTransferRequest = CashTransferRequestFactory.CreateCashTransferRequest(cashTransferRequestDTO.EmployeeId.Value, cashTransferRequestDTO.Amount, cashTransferRequestDTO.Reference);

                cashTransferRequest.Status = (int)CashTransferRequestStatus.Pending;

                cashTransferRequest.CreatedBy = serviceHeader.ApplicationUserName;

                _cashTransferRequestRepository.Add(cashTransferRequest, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? cashTransferRequest.ProjectedAs<CashTransferRequestDTO>() : null;
            }
        }

        public async Task<bool> AcknowledgeCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _cashTransferRequestRepository.GetAsync(cashTransferRequestDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)CashTransferRequestStatus.Pending) return result;

                switch ((CashTransferRequestAcknowledgeOption)cashTransferRequestAcknowledgeOption)
                {
                    case CashTransferRequestAcknowledgeOption.Acknowledge:

                        persisted.Status = (int)CashTransferRequestAcknowledgeOption.Acknowledge;
                        persisted.AcknowledgedDate = DateTime.Now;
                        persisted.Remarks = cashTransferRequestDTO.Remarks;
                        persisted.AcknowledgedBy = serviceHeader.ApplicationUserName;

                        break;

                    case CashTransferRequestAcknowledgeOption.Reject:

                        persisted.Status = (int)CashTransferRequestAcknowledgeOption.Reject;
                        persisted.AcknowledgedDate = DateTime.Now;
                        persisted.Remarks = cashTransferRequestDTO.Remarks;
                        persisted.AcknowledgedBy = serviceHeader.ApplicationUserName;

                        break;
                    default:
                        break;
                }

                result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }

            return result;
        }

        public async Task<List<CashTransferRequestDTO>> FindCashTransferRequestsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _cashTransferRequestRepository.GetAllAsync<CashTransferRequestDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsAsync(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.CashTransferRequestWithEmployeeId(employeeId, startDate, endDate, status);

                ISpecification<CashTransferRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _cashTransferRequestRepository.AllMatchingPagedAsync<CashTransferRequestDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }


        public async Task<PageCollectionInfo<CashTransferRequestDTO>> FindAllCashTransferRequestsAsync(DateTime startDate, DateTime endDate, string text, int status, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.CashTransferRequestWithDateRangeStatusAndFullText(startDate, endDate, status, text, customerFilter);

                ISpecification<CashTransferRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _cashTransferRequestRepository.AllMatchingPagedAsync<CashTransferRequestDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<CashTransferRequestDTO> FindCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _cashTransferRequestRepository.GetAsync<CashTransferRequestDTO>(cashTransferRequestId, serviceHeader);
            }
        }

        public async Task<List<CashTransferRequestDTO>> FindMatureCashTransferRequestsAsync(Guid employeeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.ActionableCashTransferRequestWithEmployeeId(employeeId);

                ISpecification<CashTransferRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _cashTransferRequestRepository.AllMatchingAsync<CashTransferRequestDTO>(spec, serviceHeader);
            }
        }

        public async Task<bool> UtilizeCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (cashTransferRequestId != null && cashTransferRequestId != Guid.Empty)
                {
                    var persisted = await _cashTransferRequestRepository.GetAsync(cashTransferRequestId, serviceHeader);

                    if (persisted != null && persisted.Status != (int)CashTransferRequestStatus.Pending)
                    {
                        persisted.Utilized = true;
                        persisted.Status = (int)CashTransferRequestStatus.Utilized;
                    }
                }
                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }
    }
}
