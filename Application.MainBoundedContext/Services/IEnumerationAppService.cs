using Application.MainBoundedContext.DTO;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.Services
{
    public interface IEnumerationAppService
    {
        bool SeedEnumerations(ServiceHeader serviceHeader);

        EnumerationDTO FindEnumeration(Guid enumerationId, ServiceHeader serviceHeader);

        List<EnumerationDTO> FindEnumerations(ServiceHeader serviceHeader);

        PageCollectionInfo<EnumerationDTO> FindEnumerations(int pageIndex, int pageSize, string text, ServiceHeader serviceHeader);

        List<EnumerationDTO> FindEnumerations(string text, ServiceHeader serviceHeader);
    }
}
