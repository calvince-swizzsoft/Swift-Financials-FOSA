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
    public interface IDirectorAppService
    {
        DirectorDTO AddNewDirector(DirectorDTO directorDTO, ServiceHeader serviceHeader);

        bool UpdateDirector(DirectorDTO directorDTO, ServiceHeader serviceHeader);

        List<DirectorDTO> FindDirectors(ServiceHeader serviceHeader);

        PageCollectionInfo<DirectorDTO> FindDirectors(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DirectorDTO> FindDirectors(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DirectorDTO FindDirector(Guid directorId, ServiceHeader serviceHeader);
    }
}
