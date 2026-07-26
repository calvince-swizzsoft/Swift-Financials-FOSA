using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeExitAppService : IEmployeeExitAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeExit> _employeeExitRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeeExitAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EmployeeExit> employeeExitRepository,
           IRepository<Employee> employeeRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeExitRepository == null)
                throw new ArgumentNullException(nameof(employeeExitRepository));

            if (employeeRepository == null)
                throw new ArgumentNullException(nameof(employeeRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeExitRepository = employeeExitRepository;
            _employeeRepository = employeeRepository;
        }

        public EmployeeExitDTO AddNewEmployeeExit(EmployeeExitDTO employeeExitDTO, ServiceHeader serviceHeader)
        {
            var employeeExitBindingModel = employeeExitDTO.ProjectedAs<EmployeeExitBindingModel>();

            employeeExitBindingModel.ValidateAll();

            if (employeeExitBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, employeeExitBindingModel.ErrorMessages));

            var existingEmployeeExits = FindEmployeeExitByEmployeeId(employeeExitDTO.EmployeeId, serviceHeader);

            if (existingEmployeeExits != null && existingEmployeeExits.Any())
            {
                foreach (var existingEmployeeExit in existingEmployeeExits)
                {
                    switch ((EmployeeExitStatus)existingEmployeeExit.Status)
                    {
                        case EmployeeExitStatus.Pending:
                        case EmployeeExitStatus.Verified:
                        case EmployeeExitStatus.Accepted:
                            throw new InvalidOperationException(string.Format("Exit process of {0} already in progress.", employeeExitDTO.EmployeeCustomerFullName));
                        case EmployeeExitStatus.Rejected:
                        default:
                            break;

                    }
                }
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var employeeExit = EmployeeExitFactory.CreateEmployeeExit(employeeExitDTO.EmployeeId, employeeExitDTO.BranchId, employeeExitDTO.Type, employeeExitDTO.Status, employeeExitDTO.Reason, employeeExitDTO.FileName, employeeExitDTO.FileTitle, employeeExitDTO.FileDescription, employeeExitDTO.FileMIMEType);

                employeeExit.CreatedBy = serviceHeader.ApplicationUserName;

                _employeeExitRepository.Add(employeeExit, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? employeeExit.ProjectedAs<EmployeeExitDTO>() : null;
            }
        }

        public bool VerifyEmployeeExit(EmployeeExitDTO employeeExitDTO, int auditOption, ServiceHeader serviceHeader)
        {
            var employeeExitBindingModel = employeeExitDTO.ProjectedAs<EmployeeExitBindingModel>();

            employeeExitBindingModel.ValidateAll();

            if (!Enum.IsDefined(typeof(VerificationOption), auditOption)) return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeExitRepository.Get(employeeExitDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)OriginationVerificationAuthorizationStatus.Pending) return false;

                switch ((VerificationOption)auditOption)
                {
                    case VerificationOption.Verified:

                        persisted.Status = (int)OriginationVerificationAuthorizationStatus.Verified;
                        persisted.AuditRemarks = employeeExitDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;
                        break;

                    case VerificationOption.Rejected:

                        persisted.Status = (int)OriginationVerificationAuthorizationStatus.Rejected;
                        persisted.AuditRemarks = employeeExitDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;
                        break;

                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool ApproveEmployeeExit(EmployeeExitDTO employeeExitDTO, int authorizationOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var employeeExitBindingModel = employeeExitDTO.ProjectedAs<EmployeeExitBindingModel>();

            employeeExitBindingModel.ValidateAll();

            if (!Enum.IsDefined(typeof(AuthorizationOption), authorizationOption)) return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeExitRepository.Get(employeeExitDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)OriginationVerificationAuthorizationStatus.Verified) return false;

                switch ((AuthorizationOption)authorizationOption)
                {
                    case AuthorizationOption.Posted:

                        #region Lock Employee

                        var persistedEmployee = _employeeRepository.Get(employeeExitDTO.EmployeeId, serviceHeader);
                        persistedEmployee.Remarks = string.Format("{0} Notice Placed", employeeExitDTO.TypeDescription);
                        persistedEmployee.Lock();

                        #endregion

                        persisted.Status = (int)OriginationVerificationAuthorizationStatus.Posted;
                        persisted.AuthorizationRemarks = employeeExitDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;

                    case AuthorizationOption.Rejected:

                        persisted.Status = (int)OriginationVerificationAuthorizationStatus.Rejected;
                        persisted.AuthorizationRemarks = employeeExitDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;
                        break;

                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;

            }
        }

        private List<EmployeeExitDTO> FindEmployeeExitByEmployeeId(Guid employeeId, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EmployeeExitSpecifications.EmployeeExitByEmployeeId(employeeId);

                    ISpecification<EmployeeExit> spec = filter;

                    var employeeExits = _employeeExitRepository.AllMatching<EmployeeExitQueryableDTO>(spec, serviceHeader);

                    if (employeeExits != null && employeeExits.Any())
                    {
                        return employeeExits.ProjectedAs<List<EmployeeExitDTO>>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<EmployeeExitDTO> FindEmployeeExits(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeExitSpecifications.EmployeeExitsByStatusWithDateRange(status, text, startDate, endDate);

                ISpecification<EmployeeExit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeExitPageCollection = _employeeExitRepository.AllMatchingPaged<EmployeeExitQueryableDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                return employeeExitPageCollection.ProjectedAs<PageCollectionInfo<EmployeeExitDTO>>();
            }
        }

        public bool UpdateEmployeeExit(EmployeeExitDTO employeeExitDTO, ServiceHeader serviceHeader)
        {
            var employeeExitBindingModel = employeeExitDTO.ProjectedAs<EmployeeExitBindingModel>();

            employeeExitBindingModel.ValidateAll();

            if (employeeExitBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, employeeExitBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeExitRepository.Get(employeeExitDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EmployeeExitFactory.CreateEmployeeExit(employeeExitDTO.EmployeeId, employeeExitDTO.BranchId, employeeExitDTO.Type, employeeExitDTO.Status, employeeExitDTO.Reason, employeeExitDTO.FileName, employeeExitDTO.FileTitle, employeeExitDTO.FileDescription, employeeExitDTO.FileMIMEType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _employeeExitRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }
    }
}
