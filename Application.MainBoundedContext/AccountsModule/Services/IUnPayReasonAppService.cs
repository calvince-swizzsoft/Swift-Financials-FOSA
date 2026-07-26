using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IUnPayReasonAppService
    {
        UnPayReasonDTO AddNewUnPayReason(UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader);

        bool UpdateUnPayReason(UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader);

        List<UnPayReasonDTO> FindUnPayReasons(ServiceHeader serviceHeader);

        PageCollectionInfo<UnPayReasonDTO> FindUnPayReasons(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<UnPayReasonDTO> FindUnPayReasons(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        UnPayReasonDTO FindUnPayReason(Guid unPayReasonId, ServiceHeader serviceHeader);

        UnPayReasonDTO FindCachedUnPayReason(Guid unPayReasonId, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid unPayReasonId, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid unPayReasonId, List<CommissionDTO> commissions, ServiceHeader serviceHeader);
    }
}
