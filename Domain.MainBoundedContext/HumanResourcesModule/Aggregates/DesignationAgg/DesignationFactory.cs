using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg
{
    public static class DesignationFactory
    {
        public static Designation CreateDesignation(Guid? parentId, string description, string remarks)
        {
            var designation = new Designation();

            designation.ParentId = parentId;

            designation.GenerateNewIdentity();

            designation.Description = description;

            designation.Remarks = remarks;

            designation.CreatedDate = DateTime.Now;

            return designation;
        }
    }
}
