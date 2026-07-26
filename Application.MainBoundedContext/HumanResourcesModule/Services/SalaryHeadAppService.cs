using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryHeadAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class SalaryHeadAppService : ISalaryHeadAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalaryHead> _salaryHeadRepository;

        public SalaryHeadAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SalaryHead> salaryHeadRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (salaryHeadRepository == null)
                throw new ArgumentNullException(nameof(salaryHeadRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _salaryHeadRepository = salaryHeadRepository;
        }

        public SalaryHeadDTO AddNewSalaryHead(SalaryHeadDTO salaryHeadDTO, ServiceHeader serviceHeader)
        {
            if (salaryHeadDTO != null && Enum.IsDefined(typeof(SalaryHeadType), salaryHeadDTO.Type))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var proceed = true;

                    switch ((SalaryHeadType)salaryHeadDTO.Type)
                    {
                        case SalaryHeadType.FullTimeBasicPayEarning:
                        case SalaryHeadType.PartTimeBasicPayEarning:
                        case SalaryHeadType.ContractBasicPayEarning:
                        case SalaryHeadType.NSSFDeduction:
                        case SalaryHeadType.NHIFDeduction:
                        case SalaryHeadType.PAYEDeduction:
                        case SalaryHeadType.StatutoryProvidentFundDeduction:

                            // get the specification
                            var filter = SalaryHeadSpecifications.SalaryHeadWithType(salaryHeadDTO.Type);

                            ISpecification<SalaryHead> spec = filter;

                            //Query this criteria
                            var salaryHeads = _salaryHeadRepository.AllMatching(spec, serviceHeader);

                            if (salaryHeads != null && salaryHeads.Any())
                            {
                                proceed = false;
                            }

                            break;
                        default:
                            break;
                    }

                    if (proceed)
                    {
                        var customerAccountType = new CustomerAccountType(salaryHeadDTO.CustomerAccountTypeProductCode, salaryHeadDTO.CustomerAccountTypeTargetProductId, salaryHeadDTO.CustomerAccountTypeTargetProductCode);

                        var salaryHead = SalaryHeadFactory.CreateSalaryHead(salaryHeadDTO.ChartOfAccountId, salaryHeadDTO.Description, salaryHeadDTO.Type, customerAccountType);

                        switch ((SalaryHeadType)salaryHeadDTO.Type)
                        {
                            case SalaryHeadType.FullTimeBasicPayEarning:
                            case SalaryHeadType.PartTimeBasicPayEarning:
                            case SalaryHeadType.ContractBasicPayEarning:
                            case SalaryHeadType.OtherEarning:
                                salaryHead.Category = (int)SalaryHeadCategory.Earning;
                                salaryHead.IsOneOff = salaryHeadDTO.IsOneOff;
                                break;
                            case SalaryHeadType.NSSFDeduction:
                            case SalaryHeadType.NHIFDeduction:
                            case SalaryHeadType.PAYEDeduction:
                            case SalaryHeadType.StatutoryProvidentFundDeduction:
                            case SalaryHeadType.LoanDeduction:
                            case SalaryHeadType.InvestmentDeduction:
                            case SalaryHeadType.OtherDeduction:
                            case SalaryHeadType.VoluntaryProvidentFundDeduction:
                                salaryHead.Category = (int)SalaryHeadCategory.Deduction;
                                break;
                            default:
                                break;
                        }

                        _salaryHeadRepository.Add(salaryHead, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return salaryHead.ProjectedAs<SalaryHeadDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateSalaryHead(SalaryHeadDTO salaryHeadDTO, ServiceHeader serviceHeader)
        {
            if (salaryHeadDTO == null || salaryHeadDTO.Id == Guid.Empty || !Enum.IsDefined(typeof(SalaryHeadType), salaryHeadDTO.Type))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salaryHeadRepository.Get(salaryHeadDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var customerAccountType = new CustomerAccountType(salaryHeadDTO.CustomerAccountTypeProductCode, salaryHeadDTO.CustomerAccountTypeTargetProductId, salaryHeadDTO.CustomerAccountTypeTargetProductCode);

                    var current = SalaryHeadFactory.CreateSalaryHead(salaryHeadDTO.ChartOfAccountId, salaryHeadDTO.Description, salaryHeadDTO.Type, customerAccountType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                    switch ((SalaryHeadType)salaryHeadDTO.Type)
                    {
                        case SalaryHeadType.FullTimeBasicPayEarning:
                        case SalaryHeadType.PartTimeBasicPayEarning:
                        case SalaryHeadType.ContractBasicPayEarning:
                        case SalaryHeadType.OtherEarning:
                            current.Category = (int)SalaryHeadCategory.Earning;
                            current.IsOneOff = salaryHeadDTO.IsOneOff;
                            break;
                        case SalaryHeadType.NSSFDeduction:
                        case SalaryHeadType.NHIFDeduction:
                        case SalaryHeadType.PAYEDeduction:
                        case SalaryHeadType.StatutoryProvidentFundDeduction:
                        case SalaryHeadType.LoanDeduction:
                        case SalaryHeadType.InvestmentDeduction:
                        case SalaryHeadType.OtherDeduction:
                        case SalaryHeadType.VoluntaryProvidentFundDeduction:
                            current.Category = (int)SalaryHeadCategory.Deduction;
                            break;
                        default:
                            break;
                    }

                    _salaryHeadRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<SalaryHeadDTO> FindSalaryHeads(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var salaryHeads = _salaryHeadRepository.GetAll(serviceHeader);

                if (salaryHeads != null && salaryHeads.Any())
                {
                    return salaryHeads.ProjectedAsCollection<SalaryHeadDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryHeadDTO> FindSalaryHeads(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryHeadSpecifications.DefaultSpec();

                ISpecification<SalaryHead> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryHeadPagedCollection = _salaryHeadRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryHeadPagedCollection != null)
                {
                    var pageCollection = salaryHeadPagedCollection.PageCollection.ProjectedAsCollection<SalaryHeadDTO>();

                    var itemsCount = salaryHeadPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryHeadDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryHeadDTO> FindSalaryHeads(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryHeadSpecifications.SalaryHeadFullText(text);

                ISpecification<SalaryHead> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryHeadCollection = _salaryHeadRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryHeadCollection != null)
                {
                    var pageCollection = salaryHeadCollection.PageCollection.ProjectedAsCollection<SalaryHeadDTO>();

                    var itemsCount = salaryHeadCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryHeadDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public SalaryHeadDTO FindSalaryHead(Guid salaryHeadId, ServiceHeader serviceHeader)
        {
            if (salaryHeadId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var salaryHead = _salaryHeadRepository.Get(salaryHeadId, serviceHeader);

                    if (salaryHead != null)
                    {
                        return salaryHead.ProjectedAs<SalaryHeadDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
