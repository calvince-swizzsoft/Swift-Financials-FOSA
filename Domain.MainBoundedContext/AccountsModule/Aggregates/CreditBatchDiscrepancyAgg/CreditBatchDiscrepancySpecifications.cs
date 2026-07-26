using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchDiscrepancyAgg
{
    public static class CreditBatchDiscrepancySpecifications
    {
        public static Specification<CreditBatchDiscrepancy> DefaultSpec()
        {
            Specification<CreditBatchDiscrepancy> specification = new TrueSpecification<CreditBatchDiscrepancy>();

            return specification;
        }

        public static Specification<CreditBatchDiscrepancy> CreditBatchDiscrepancyWithCreditBatchId(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter)
        {
            Specification<CreditBatchDiscrepancy> specification = new DirectSpecification<CreditBatchDiscrepancy>(c => c.CreditBatchId == creditBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CreditBatchDiscrepancyFilter)creditBatchDiscrepancyFilter)
                {
                    case CreditBatchDiscrepancyFilter.BatchNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => c.CreditBatch.BatchNumber == number);
                        }

                        break;
                    case CreditBatchDiscrepancyFilter.Column1:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column1) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column2:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column2) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column3:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column3) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column4:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column4) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column5:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column5) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column6:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column6) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column7:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column7) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column8:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column8) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Remarks:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<CreditBatchDiscrepancy> CreditBatchDiscrepancyWithDateRange(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CreditBatchDiscrepancy> specification = new DirectSpecification<CreditBatchDiscrepancy>(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate && c.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CreditBatchDiscrepancyFilter)creditBatchDiscrepancyFilter)
                {
                    case CreditBatchDiscrepancyFilter.BatchNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => c.CreditBatch.BatchNumber == number);
                        }

                        break;
                    case CreditBatchDiscrepancyFilter.Column1:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column1) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column2:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column2) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column3:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column3) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column4:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column4) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column5:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column5) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column6:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column6) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column7:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column7) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Column8:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Column8) > 0);
                        break;
                    case CreditBatchDiscrepancyFilter.Remarks:
                        specification &= new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<CreditBatchDiscrepancy> CreditBatchDiscrepancyWithCreditBatchTypeAndStatusAndProductCode(int creditBatchType, int status, int productCode, string text)
        {
            Specification<CreditBatchDiscrepancy> specification = new DirectSpecification<CreditBatchDiscrepancy>(c => c.Status == status && c.CreditBatch.Type == creditBatchType);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var productCodeToString = Convert.ToString(productCode).SanitizePatIndexInput();

                var buffer = text.Split(new char[] { '|', ';', ':' });

                switch ((CreditBatchType)creditBatchType)
                {
                    case CreditBatchType.Payout:

                        var column4Spec = new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(productCodeToString, c.Column4) > 0);
                        specification &= (column4Spec);

                        var column1Specs = new List<Specification<CreditBatchDiscrepancy>>();

                        if (buffer != null)
                        {
                            Array.ForEach(buffer, (item) =>
                            {
                                item = item.Trim().SanitizePatIndexInput();

                                var column1Spec = new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(item, c.Column1) > 0);

                                column1Specs.Add(column1Spec);
                            });

                            if (column1Specs.Any())
                            {
                                var spec0 = column1Specs[0];

                                for (int i = 1; i < column1Specs.Count; i++)
                                {
                                    spec0 |= column1Specs[i];
                                }

                                specification &= (spec0);
                            }
                        }

                        break;
                    case CreditBatchType.CheckOff:

                        var column6Spec = new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(productCodeToString, c.Column6) > 0);
                        specification &= (column6Spec);

                        var column2Specs = new List<Specification<CreditBatchDiscrepancy>>();

                        if (buffer != null)
                        {
                            Array.ForEach(buffer, (item) =>
                            {
                                item = item.Trim().SanitizePatIndexInput();

                                var column2Spec = new DirectSpecification<CreditBatchDiscrepancy>(c => SqlFunctions.PatIndex(item, c.Column2) > 0);

                                column2Specs.Add(column2Spec);
                            });

                            if (column2Specs.Any())
                            {
                                var spec0 = column2Specs[0];

                                for (int i = 1; i < column2Specs.Count; i++)
                                {
                                    spec0 |= column2Specs[i];
                                }

                                specification &= (spec0);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
