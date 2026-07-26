using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SystemGeneralLedgerAccountMappingAgg
{
    public static class SystemGeneralLedgerAccountMappingSpecifications
    {
        public static Specification<SystemGeneralLedgerAccountMapping> DefaultSpec()
        {
            Specification<SystemGeneralLedgerAccountMapping> specification = new TrueSpecification<SystemGeneralLedgerAccountMapping>();

            return specification;
        }

        public static Specification<SystemGeneralLedgerAccountMapping> SystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode)
        {
            Specification<SystemGeneralLedgerAccountMapping> specification =
                new DirectSpecification<SystemGeneralLedgerAccountMapping>(m => m.SystemGeneralLedgerAccountCode == systemGeneralLedgerAccountCode);

            return specification;
        }

        public static Specification<SystemGeneralLedgerAccountMapping> SystemGeneralLedgerAccountMappingFullText(string text)
        {
            Specification<SystemGeneralLedgerAccountMapping> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<SystemGeneralLedgerAccountMapping>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);

                int number = default(int);
                {
                    var systemGeneralLedgerAccountCodeSpec = new DirectSpecification<SystemGeneralLedgerAccountMapping>(x => x.SystemGeneralLedgerAccountCode == number);

                    specification |= systemGeneralLedgerAccountCodeSpec;
                }
            }

            return specification;
        }
    }
}
