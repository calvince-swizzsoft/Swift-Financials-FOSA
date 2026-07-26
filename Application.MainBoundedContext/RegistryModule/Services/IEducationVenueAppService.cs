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
    public interface IEducationVenueAppService
    {
        EducationVenueDTO AddNewEducationVenue(EducationVenueDTO educationVenueDTO, ServiceHeader serviceHeader);

        bool UpdateEducationVenue(EducationVenueDTO educationVenueDTO, ServiceHeader serviceHeader);

        List<EducationVenueDTO> FindEducationVenues(ServiceHeader serviceHeader);

        PageCollectionInfo<EducationVenueDTO> FindEducationVenues(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EducationVenueDTO> FindEducationVenues(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EducationVenueDTO FindEducationVenue(Guid educationVenueId, ServiceHeader serviceHeader);
    }
}
