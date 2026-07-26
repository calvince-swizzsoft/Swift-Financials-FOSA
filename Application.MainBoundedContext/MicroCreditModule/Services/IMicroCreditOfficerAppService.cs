using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.MicroCreditModule.Services
{
    public interface IMicroCreditOfficerAppService
    {
        MicroCreditOfficerDTO AddNewMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO, ServiceHeader serviceHeader);

        bool UpdateMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO, ServiceHeader serviceHeader);

        List<MicroCreditOfficerDTO> FindMicroCreditOfficers(ServiceHeader serviceHeader);

        PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficers(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficers(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        MicroCreditOfficerDTO FindMicroCreditOfficer(Guid microCreditOfficerId, ServiceHeader serviceHeader);
    }
}
