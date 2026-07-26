using Domain.MainBoundedContext.AccountsModule.Aggregates.NumberSeriesAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.MainBoundedContext.Seeders
{
    /// <summary>
    /// Seeds initial NumberSeries data into the database.
    /// </summary>
    public class NumberSeriesSeeder
    {
        private readonly IRepository<NumberSeries> _numberSeriesRepository;

        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public NumberSeriesSeeder(IRepository<NumberSeries> numberSeriesRepository, IDbContextScopeFactory dbContextScopeFactory)
        {
            _numberSeriesRepository = numberSeriesRepository;
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
        }

        public async Task SeedAsync(ServiceHeader serviceHeader)
        {
            using (var scope = _dbContextScopeFactory.Create())
            {
                var existingSeries = _numberSeriesRepository.GetAll(serviceHeader);

                    //Define the default number series configurations inline
                    var defaultSeries = new List<NumberSeries>
                    {

                          new NumberSeries
                     {

                    Code = "PCM", // Purchase Credit Memo
                    Prefix = "PCM-",
                    LastUsedNumber = 0,
                    Padding = 5
                },

     new NumberSeries
                     {

                    Code = "SCM", // Sales Credit Memo
                    Prefix = "SCM-",
                    LastUsedNumber = 0,
                    Padding = 5
                },
                new NumberSeries {

                    Code = "RCPT", // Receipt
                    Prefix = "RCPT-",
                    LastUsedNumber = 0,
                    Padding = 5
                },

                new NumberSeries
                {
                    
                    Code = "SI", // Sales Invoice
                    Prefix = "SI-",
                    LastUsedNumber = 0,
                    Padding = 5
                },


                new NumberSeries
                {

                    Code = "ARC", // AR Cystomer
                    Prefix = "ARC-",
                    LastUsedNumber = 0,
                    Padding = 5
                },

                new NumberSeries
                {
                    
                    Code = "PV", // Payment Voucher
                    Prefix = "PV-",
                    LastUsedNumber = 0,
                    Padding = 5
                },
                new NumberSeries
                {
                    Code = "PR", // Purchase Receipt
                    Prefix = "PR-",
                    LastUsedNumber = 0,
                    Padding = 5
                },
                new NumberSeries
                {
                    Code = "PI", // Purchase Invoice
                    Prefix = "PI-",
                    LastUsedNumber = 0,
                    Padding = 5
                },
                new NumberSeries
                {
                    Code = "PC", // Purchase Credit Memo
                    Prefix = "PC-",
                    LastUsedNumber = 0,
                    Padding = 5
                },
                new NumberSeries
                {
                    Code = "JV", // Journal Voucher
                    Prefix = "JV-",
                    LastUsedNumber = 0,
                    Padding = 5
                },

                  new NumberSeries
                {
                    Code = "IM", // Imprest
                    Prefix = "IM-",
                    LastUsedNumber = 0,
                    Padding = 5
                },


                    new NumberSeries
                {
                    Code = "SURR", // Surrender
                    Prefix = "SURR-",
                    LastUsedNumber = 0,
                    Padding = 5
                }


                    };

                    foreach (var config in defaultSeries)
                    {
                        var exists = existingSeries.Any(s => s.Code == config.Code);
                        if (!exists)
                        {
                        var series = NumberSeriesFactory.CreateNumberSeries(config.Code, config.Prefix, config.LastUsedNumber, config.Padding);
                            _numberSeriesRepository.Add(series, serviceHeader);
               
                        }
                        else
                        {
                            Console.WriteLine($"Skipped existing NumberSeries: {config.Code}");
                        }
                    }
          
                await scope.SaveChangesAsync(serviceHeader);

            }

           
        }
    }
}
