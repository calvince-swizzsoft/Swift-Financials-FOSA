using Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg;
using Domain.Seedwork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemInRoleAgg
{
    public class NavigationItemInRole : Entity
    {
        public Guid NavigationItemId { get; set; }

        public virtual NavigationItem NavigationItem { get; private set; }

        [Index("IX_NavigationItemInRole_RoleName")]
        public string RoleName { get; set; }
    }
}