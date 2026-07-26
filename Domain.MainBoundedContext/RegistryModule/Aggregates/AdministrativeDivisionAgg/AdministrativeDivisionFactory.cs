using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg
{
    public static class AdministrativeDivisionFactory
    {
        public static AdministrativeDivision CreateAdministrativeDivision(Guid? parentId, string description, byte type, string remarks)
        {
            var administrativeDivision = new AdministrativeDivision();

            administrativeDivision.GenerateNewIdentity();

            administrativeDivision.ParentId = parentId;

            administrativeDivision.Type = type;

            administrativeDivision.Description = description;

            administrativeDivision.Remarks = remarks;

            administrativeDivision.CreatedDate = DateTime.Now;

            return administrativeDivision;
        }
    }
}
