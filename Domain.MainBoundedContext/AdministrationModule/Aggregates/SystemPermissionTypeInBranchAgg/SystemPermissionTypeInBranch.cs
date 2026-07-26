using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInBranchAgg
{
    public class SystemPermissionTypeInBranch : Domain.Seedwork.Entity
    {
        [Index("IX_SystemPermissionTypeInBranch_SystemPermissionType")]
        public int SystemPermissionType { get; set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        

        
    }
}
