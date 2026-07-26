using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface ISuperSaverPayableAppService
    {
        SuperSaverPayableDTO AddNewSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, ServiceHeader serviceHeader);

        SuperSaverPayableDTO FindSuperSaverPayable(Guid superSaverPayableId, ServiceHeader serviceHeader);

        PageCollectionInfo<SuperSaverPayableDTO> FindSuperSaverPayablesByStatus(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool AuditSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuditOption, ServiceHeader serviceHeader);
        
        bool AuthorizeSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);
    }
}
