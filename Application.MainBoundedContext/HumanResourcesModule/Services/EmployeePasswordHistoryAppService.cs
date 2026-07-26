using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeePasswordHistoryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeePasswordHistoryAppService : IEmployeePasswordHistoryAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeePasswordHistory> _employeePasswordHistoryRepository;

        public EmployeePasswordHistoryAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EmployeePasswordHistory> employeePasswordHistoryRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeePasswordHistoryRepository == null)
                throw new ArgumentNullException(nameof(employeePasswordHistoryRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeePasswordHistoryRepository = employeePasswordHistoryRepository;
        }

        public EmployeePasswordHistoryDTO AddNewEmployeePasswordHistory(EmployeePasswordHistoryDTO employeePasswordHistoryDTO, ServiceHeader serviceHeader)
        {
            if (employeePasswordHistoryDTO == null)
                return null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                employeePasswordHistoryDTO.PasswordHash = PasswordHash.CreateHash(string.Format("{0}", employeePasswordHistoryDTO.Password));

                var employeePasswordHistory = EmployeePasswordHistoryFactory.CreateEmployeePasswordHistory(employeePasswordHistoryDTO.EmployeeId, employeePasswordHistoryDTO.PasswordHash);

                employeePasswordHistory.CreatedBy = serviceHeader.ApplicationUserName;

                _employeePasswordHistoryRepository.Add(employeePasswordHistory, serviceHeader);

                dbContextScope.SaveChanges(serviceHeader);

                return employeePasswordHistory.ProjectedAs<EmployeePasswordHistoryDTO>();
            }
        }

        public List<EmployeePasswordHistoryDTO> FindEmployeePasswordHistories(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<EmployeePasswordHistory> spec = EmployeePasswordHistorySpecifications.DefaultSpec();

                var employeePasswordHistorys = _employeePasswordHistoryRepository.AllMatching(spec, serviceHeader);

                if (employeePasswordHistorys != null && employeePasswordHistorys.Any())
                {
                    return employeePasswordHistorys.ProjectedAsCollection<EmployeePasswordHistoryDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeePasswordHistoryDTO> FindEmployeePasswordHistories(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeePasswordHistorySpecifications.DefaultSpec();

                ISpecification<EmployeePasswordHistory> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeePasswordHistoryPagedCollection = _employeePasswordHistoryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeePasswordHistoryPagedCollection != null)
                {
                    var pageCollection = employeePasswordHistoryPagedCollection.PageCollection.ProjectedAsCollection<EmployeePasswordHistoryDTO>();

                    var itemsCount = employeePasswordHistoryPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeePasswordHistoryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EmployeePasswordHistoryDTO FindEmployeePasswordHistory(Guid employeePasswordHistoryId, ServiceHeader serviceHeader)
        {
            if (employeePasswordHistoryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var employeePasswordHistory = _employeePasswordHistoryRepository.Get(employeePasswordHistoryId, serviceHeader);

                    if (employeePasswordHistory != null)
                    {
                        return employeePasswordHistory.ProjectedAs<EmployeePasswordHistoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<EmployeePasswordHistoryDTO> FindEmployeePasswordHistories(Guid employeeId, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EmployeePasswordHistorySpecifications.EmployeePasswordHistoryWithEmployeeId(employeeId);

                    ISpecification<EmployeePasswordHistory> spec = filter;

                    var employeeCollection = _employeePasswordHistoryRepository.AllMatching(spec, serviceHeader);

                    if (employeeCollection != null)
                    {
                        return employeeCollection.ProjectedAsCollection<EmployeePasswordHistoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool ValidatePasswordHistory(Guid employeeId, string proposedPassword, int passwordHistoryPolicy, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var passwordHistories = FindEmployeePasswordHistories(employeeId, serviceHeader);

            if (passwordHistories != null && passwordHistories.Any())
            {
                var targetList = passwordHistories.OrderBy(x => x.CreatedDate).TakeLast(passwordHistoryPolicy);

                result = !targetList.Any(x => PasswordHash.ValidatePassword(proposedPassword, x.PasswordHash));
            }
            else result = true;

            return result;
        }
    }
}
