using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface INextOfKinAppService
    {
        List<NextOfKinDTO> FindNextOfKins(Guid customerId, ServiceHeader serviceHeader);

        NextOfKinDTO FindNextOfKin(Guid nextOfKinId, ServiceHeader serviceHeader);

        NextOfKinDTO AddNewNextOfKin(NextOfKinDTO nextOfKinDTO, ServiceHeader serviceHeader);

        bool UpdateNextOfKin(NextOfKinDTO nextOfKinDTO, ServiceHeader serviceHeader);

        bool RemoveNextOfKin(Guid nextOfKinId, ServiceHeader serviceHeader);
    }
}
