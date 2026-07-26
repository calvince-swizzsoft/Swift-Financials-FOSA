using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewQuestionAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class ExitInterviewQuestionAppService : IExitInterviewQuestionAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ExitInterviewQuestion> _exitInterviewQuestionRepository;

        public ExitInterviewQuestionAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ExitInterviewQuestion> exitInterviewQuestionRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (exitInterviewQuestionRepository == null)
                throw new ArgumentNullException(nameof(exitInterviewQuestionRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _exitInterviewQuestionRepository = exitInterviewQuestionRepository;
        }

        public ExitInterviewQuestionDTO AddNewExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO, ServiceHeader serviceHeader)
        {
            var exitInterviewQuestionBindingModel = exitInterviewQuestionDTO.ProjectedAs<ExitInterviewQuestionBindingModel>();

            exitInterviewQuestionBindingModel.ValidateAll();

            if (exitInterviewQuestionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, exitInterviewQuestionBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var exitInterviewQuestion = ExitInterviewQuestionFactory.CreateExitInterviewQuestion(exitInterviewQuestionDTO.Description);

                _exitInterviewQuestionRepository.Add(exitInterviewQuestion, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? exitInterviewQuestion.ProjectedAs<ExitInterviewQuestionDTO>() : null;
            }
        }

        public ExitInterviewQuestionDTO FindExitInterviewQuestion(Guid exitInterviewQuestionId, ServiceHeader serviceHeader)
        {
            if (exitInterviewQuestionId == Guid.Empty) return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _exitInterviewQuestionRepository.Get<ExitInterviewQuestionDTO>(exitInterviewQuestionId, serviceHeader);
            }
        }

        public List<ExitInterviewQuestionDTO> FindExitInterviewQuestions(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _exitInterviewQuestionRepository.GetAll<ExitInterviewQuestionDTO>(serviceHeader);
            }
        }

        public List<ExitInterviewQuestionDTO> FindUnlockedExitInterviewQuestions(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExitInterviewQuestionSpecifications.UnlockedQuestions();

                ISpecification<ExitInterviewQuestion> spec = filter;

                return _exitInterviewQuestionRepository.AllMatching<ExitInterviewQuestionDTO>(spec, serviceHeader);
            }
        }

        public PageCollectionInfo<ExitInterviewQuestionDTO> FindExitInterviewQuestions(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExitInterviewQuestionSpecifications.ExitInterviewQuestionFullText(text);

                ISpecification<ExitInterviewQuestion> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _exitInterviewQuestionRepository.AllMatchingPaged<ExitInterviewQuestionDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public bool UpdateExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO, ServiceHeader serviceHeader)
        {
            var exitInterviewQuestionBindingModel = exitInterviewQuestionDTO.ProjectedAs<ExitInterviewQuestionBindingModel>();

            exitInterviewQuestionBindingModel.ValidateAll();

            if (exitInterviewQuestionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, exitInterviewQuestionBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _exitInterviewQuestionRepository.Get(exitInterviewQuestionDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ExitInterviewQuestionFactory.CreateExitInterviewQuestion(exitInterviewQuestionDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (exitInterviewQuestionDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _exitInterviewQuestionRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }
    }
}
