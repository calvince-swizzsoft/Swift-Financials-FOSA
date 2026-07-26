using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg
{
    public static class LevyFactory
    {
        public static Levy CreateLevy(string description, Charge charge)
        {
            var levy = new Levy();

            levy.GenerateNewIdentity();

            levy.Description = description;

            levy.Charge = charge;

            levy.CreatedDate = DateTime.Now;

            return levy;
        }
    }
}
