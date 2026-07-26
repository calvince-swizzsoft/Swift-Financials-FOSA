using Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemInRoleAgg
{
    public class ModuleNavigationItemInRole : Domain.Seedwork.Entity
    {
        public Guid ModuleNavigationItemId { get; set; }

        public virtual ModuleNavigationItem ModuleNavigationItem { get; private set; }

        [Index("IX_ModuleNavigationItemInRole_RoleName")]
        public string RoleName { get; set; }

        

        
    }
}
