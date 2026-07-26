using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GraduatedScaleAgg
{
    public static class GraduatedScaleFactory
    {
        public static GraduatedScale CreateGraduatedScale(Guid commissionId, Range range, Charge charge)
        {
            var graduatedScale = new GraduatedScale();

            graduatedScale.GenerateNewIdentity();

            graduatedScale.CommissionId = commissionId;

            graduatedScale.Range = range;

            graduatedScale.Charge = charge;

            graduatedScale.CreatedDate = DateTime.Now;

            return graduatedScale;
        }
    }
}
