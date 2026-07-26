using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.PopulationRegisterQueryAgg
{
    public static class PopulationRegisterQuerySpecifications
    {
        public static Specification<PopulationRegisterQuery> DefaultSpec()
        {
            Specification<PopulationRegisterQuery> specification = new TrueSpecification<PopulationRegisterQuery>();

            return specification;
        }

        public static Specification<PopulationRegisterQuery> PopulationRegisterQueryFullText(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<PopulationRegisterQuery> specification = new DirectSpecification<PopulationRegisterQuery>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((PopulationRegisterFilter)populationRegisterFilter)
                {
                    case PopulationRegisterFilter.IDNumber:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.IDNumber) > 0);
                        break;
                    case PopulationRegisterFilter.SerialNumber:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.SerialNumber) > 0);
                        break;
                    case PopulationRegisterFilter.Gender:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Gender) > 0);
                        break;
                    case PopulationRegisterFilter.FirstName:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.FirstName) > 0);
                        break;
                    case PopulationRegisterFilter.OtherName:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.OtherName) > 0);
                        break;
                    case PopulationRegisterFilter.Surname:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Surname) > 0);
                        break;
                    case PopulationRegisterFilter.Pin:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Pin) > 0);
                        break;
                    case PopulationRegisterFilter.Citizenship:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Citizenship) > 0);
                        break;
                    case PopulationRegisterFilter.Family:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Family) > 0);
                        break;
                    case PopulationRegisterFilter.Clan:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Clan) > 0);
                        break;
                    case PopulationRegisterFilter.EthnicGroup:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.EthnicGroup) > 0);
                        break;
                    case PopulationRegisterFilter.Occupation:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Occupation) > 0);
                        break;
                    case PopulationRegisterFilter.PlaceOfBirth:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.PlaceOfBirth) > 0);
                        break;
                    case PopulationRegisterFilter.PlaceOfDeath:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.PlaceOfDeath) > 0);
                        break;
                    case PopulationRegisterFilter.PlaceOfLive:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.PlaceOfLive) > 0);
                        break;
                    case PopulationRegisterFilter.RegOffice:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.RegOffice) > 0);
                        break;
                    case PopulationRegisterFilter.Remarks:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<PopulationRegisterQuery> ThirdPartyNotifiablePopulationRegisterQueries(string text, int populationRegisterFilter, int daysCap)
        {
            Specification<PopulationRegisterQuery> specification = new DirectSpecification<PopulationRegisterQuery>(c => (SqlFunctions.DateDiff("DD", c.CreatedDate, SqlFunctions.GetDate()) <= daysCap)
            && c.Status == (int)PopulationRegisterQueryStatus.Authorized);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((PopulationRegisterFilter)populationRegisterFilter)
                {
                    case PopulationRegisterFilter.IDNumber:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.IDNumber) > 0);
                        break;
                    case PopulationRegisterFilter.SerialNumber:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.SerialNumber) > 0);
                        break;
                    case PopulationRegisterFilter.Gender:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Gender) > 0);
                        break;
                    case PopulationRegisterFilter.FirstName:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.FirstName) > 0);
                        break;
                    case PopulationRegisterFilter.OtherName:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.OtherName) > 0);
                        break;
                    case PopulationRegisterFilter.Surname:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Surname) > 0);
                        break;
                    case PopulationRegisterFilter.Pin:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Pin) > 0);
                        break;
                    case PopulationRegisterFilter.Citizenship:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Citizenship) > 0);
                        break;
                    case PopulationRegisterFilter.Family:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Family) > 0);
                        break;
                    case PopulationRegisterFilter.Clan:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Clan) > 0);
                        break;
                    case PopulationRegisterFilter.EthnicGroup:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.EthnicGroup) > 0);
                        break;
                    case PopulationRegisterFilter.Occupation:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Occupation) > 0);
                        break;
                    case PopulationRegisterFilter.PlaceOfBirth:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.PlaceOfBirth) > 0);
                        break;
                    case PopulationRegisterFilter.PlaceOfDeath:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.PlaceOfDeath) > 0);
                        break;
                    case PopulationRegisterFilter.PlaceOfLive:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.PlaceOfLive) > 0);
                        break;
                    case PopulationRegisterFilter.RegOffice:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.RegOffice) > 0);
                        break;
                    case PopulationRegisterFilter.Remarks:
                        specification &= new DirectSpecification<PopulationRegisterQuery>(c => SqlFunctions.PatIndex(text, c.Remarks) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
