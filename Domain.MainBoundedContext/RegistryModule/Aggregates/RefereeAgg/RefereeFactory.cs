using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.RefereeAgg
{
    public static class RefereeFactory
    {
        public static Referee CreateReferee(Guid customerId, Guid witnessId, string remarks)
        {
            var referee = new Referee();

            referee.GenerateNewIdentity();

            referee.CustomerId = customerId;

            referee.WitnessId = witnessId;

            referee.Remarks = remarks;

            referee.CreatedDate = DateTime.Now;

            return referee;
        }
    }
}
