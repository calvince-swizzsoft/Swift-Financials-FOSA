using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.Aggregates.EnumerationAgg
{
    public static class EnumerationSpecifications
    {
        public static Specification<Enumeration> DefaultSpec()
        {
            Specification<Enumeration> specification = new TrueSpecification<Enumeration>();

            return specification;
        }

        public static Specification<Enumeration> EnumerationFullText(string text)
        {
            Specification<Enumeration> specification = new TrueSpecification<Enumeration>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var keySpec = new DirectSpecification<Enumeration>(c => c.Key.Contains(text));

                var descriptionSpec = new DirectSpecification<Enumeration>(c => c.Description.Contains(text));

                specification &= (keySpec | descriptionSpec);
            }

            return specification;
        }
    }
}
