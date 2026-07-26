using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DirectorAgg
{
    public static class DirectorFactory
    {
        public static Director CreateDirector(Guid divisionId, Guid customerId, string remarks)
        {
            var director = new Director();

            director.GenerateNewIdentity();

            director.DivisionId = divisionId;

            director.CustomerId = customerId;

            director.Remarks = remarks;

            director.CreatedDate = DateTime.Now;

            return director;
        }
    }
}
