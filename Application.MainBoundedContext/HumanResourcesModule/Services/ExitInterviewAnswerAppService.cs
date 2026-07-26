using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewAnswerAgg;
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
    public class ExitInterviewAnswerAppService : IExitInterviewAnswerAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ExitInterviewAnswer> _exitInterviewAnswerRepository;

        public ExitInterviewAnswerAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<ExitInterviewAnswer> exitInterviewAnswerRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (exitInterviewAnswerRepository == null)
                throw new ArgumentNullException(nameof(exitInterviewAnswerRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _exitInterviewAnswerRepository = exitInterviewAnswerRepository;
        }

        public bool AddNewExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs, ServiceHeader serviceHeader)
        {
            if (exitInterviewAnswerDTOs != null && exitInterviewAnswerDTOs.Any())
            {
                foreach (var exitInterviewAnswerDTO in exitInterviewAnswerDTOs)
                {
                    var exitInterviewAnswerBindingModel = exitInterviewAnswerDTO.ProjectedAs<ExitInterviewAnswerBindingModel>();

                    exitInterviewAnswerBindingModel.ValidateAll();

                    if (exitInterviewAnswerBindingModel.HasErrors) return false;
                }

                var prepopulatedQuestions = FindExitInterviewAnswers(exitInterviewAnswerDTOs.FirstOrDefault().EmployeeId, serviceHeader);

                if (prepopulatedQuestions == null || !prepopulatedQuestions.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        exitInterviewAnswerDTOs.ForEach(exitInterviewAnswerDTO =>
                        {
                            var exitInterviewAnswer = ExitInterviewAnswerFactory.CreateExitInterviewAnswer(exitInterviewAnswerDTO.ExitInterviewQuestionId, exitInterviewAnswerDTO.EmployeeId, exitInterviewAnswerDTO.Answer);

                            exitInterviewAnswer.CreatedBy = serviceHeader.ApplicationUserName;

                            _exitInterviewAnswerRepository.Add(exitInterviewAnswer, serviceHeader);
                        });

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
            else return false;
        }

        public List<ExitInterviewAnswerDTO> FindExitInterviewAnswers(Guid employeeId, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ExitInterviewAnswerSpecifications.ExitInterviewAnswerWithEmployeeId(employeeId);

                    ISpecification<ExitInterviewAnswer> spec = filter;

                    return _exitInterviewAnswerRepository.AllMatching<ExitInterviewAnswerDTO>(spec, serviceHeader);
                }
            }
            else return null;
        }

        public PageCollectionInfo<ExitInterviewAnswerDTO> FindExitInterviewAnswers(Guid employeeId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExitInterviewAnswerSpecifications.ExitInterviewAnswerWithEmployeeId(employeeId);

                ISpecification<ExitInterviewAnswer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _exitInterviewAnswerRepository.AllMatchingPaged<ExitInterviewAnswerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public bool LockExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs, ServiceHeader serviceHeader)
        {
            if (exitInterviewAnswerDTOs != null && exitInterviewAnswerDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    exitInterviewAnswerDTOs.ForEach(exitInterviewAnswerDTO =>
                    {
                        var persisted = _exitInterviewAnswerRepository.Get(exitInterviewAnswerDTO.Id, serviceHeader);

                        if (persisted != null)
                        {
                            persisted.Lock();
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool UpdateExitInterviewAnswer(ExitInterviewAnswerDTO exitInterviewAnswerDTO, ServiceHeader serviceHeader)
        {
            var exitInterviewAnswerBindingModel = exitInterviewAnswerDTO.ProjectedAs<ExitInterviewAnswerBindingModel>();

            exitInterviewAnswerBindingModel.ValidateAll();

            if (exitInterviewAnswerBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, exitInterviewAnswerBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _exitInterviewAnswerRepository.Get(exitInterviewAnswerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ExitInterviewAnswerFactory.CreateExitInterviewAnswer(exitInterviewAnswerDTO.ExitInterviewQuestionId, exitInterviewAnswerDTO.EmployeeId, exitInterviewAnswerDTO.Answer);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _exitInterviewAnswerRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }
    }
}
