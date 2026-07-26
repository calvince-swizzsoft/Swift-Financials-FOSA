using Domain.MainBoundedContext.AccountsModule.Aggregates.NumberSeriesAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Application.MainBoundedContext.Services
{
    // Specification for finding NumberSeries by Code
    public class NumberSeriesByCodeSpec : Specification<NumberSeries>
    {
        private readonly string _code;

      
        public NumberSeriesByCodeSpec(string code)
        {
            _code = code;
        }

        public override Expression<Func<NumberSeries, bool>> SatisfiedBy()
        {
            return ns => ns.Code == _code;
        }
    }

    public class NumberSeriesGenerator : INumberSeriesGenerator
    {
        private readonly IRepository<NumberSeries> _numberSeriesRepository;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public NumberSeriesGenerator(IRepository<NumberSeries> numberSeriesRepository, IDbContextScopeFactory dbContextScopeFactory)
        {
            _numberSeriesRepository = numberSeriesRepository;
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        public string GetNextNumber(string code, ServiceHeader serviceHeader)
        {
            var spec = new NumberSeriesByCodeSpec(code);



            using (var dbContextScope = _dbContextScopeFactory.Create())
            {

                var series = _numberSeriesRepository.AllMatching(spec, serviceHeader).FirstOrDefault();

                if (series == null)
                    throw new ArgumentException($"Number series code '{code}' not found.");

                // Simply increment - EF change tracking will detect this
                series.LastUsedNumber += 1;

                dbContextScope.SaveChanges(serviceHeader);

                // The change will be saved when SaveChanges is called on the context
                // (usually handled by your Unit of Work pattern)

                string nextNumber = $"{series.Prefix}{series.LastUsedNumber.ToString().PadLeft(series.Padding, '0')}";
                return nextNumber;
            }
        }

        private NumberSeries CloneNumberSeries(NumberSeries source)
        {
            return new NumberSeries
            {
                //Id = source.Id,
                Code = source.Code,
                Prefix = source.Prefix,
                LastUsedNumber = source.LastUsedNumber,
                Padding = source.Padding
            };
        }
    }
}