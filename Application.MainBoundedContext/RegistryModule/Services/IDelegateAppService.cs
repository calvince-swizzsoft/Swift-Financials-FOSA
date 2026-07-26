using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IDelegateAppService
    {
        DelegateDTO AddNewDelegate(DelegateDTO delegateDTO, ServiceHeader serviceHeader);

        bool UpdateDelegate(DelegateDTO delegateDTO, ServiceHeader serviceHeader);

        List<DelegateDTO> FindDelegates(ServiceHeader serviceHeader);

        PageCollectionInfo<DelegateDTO> FindDelegates(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DelegateDTO> FindDelegates(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DelegateDTO FindDelegate(Guid delegateId, ServiceHeader serviceHeader);
    }
}
