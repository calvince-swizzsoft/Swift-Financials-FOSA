using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IFuneralRiderClaimAppService
    {
        #region FuneralRiderClaimDTO

        FuneralRiderClaimDTO AddNewFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO, ServiceHeader serviceHeader);

        bool UpdateFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO, ServiceHeader serviceHeader);

        PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaims(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaims(int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<FuneralRiderClaimDTO> FindFuneralRiderClaims(ServiceHeader serviceHeader);

        List<FuneralRiderClaimDTO> FindFuneralRiderClaimsByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        FuneralRiderClaimDTO FindFuneralRiderClaim(Guid funeralRiderClaimId, ServiceHeader serviceHeader);

        PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaims(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        #endregion

        #region FuneralRiderClaimPayableDTO

        FuneralRiderClaimPayableDTO AddNewFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, ServiceHeader serviceHeader);

        bool UpdateFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, ServiceHeader serviceHeader);

        bool AuditFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int verificationoption, ServiceHeader serviceHeader);

        bool AuthorizeFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int authorizationOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool PostFuneralRiderClaimPayable(Guid funeralRiderClaimPayableId, ServiceHeader serviceHeader);

        PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayables(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayables(int recordStatus, int paymentStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayables(int recordStatus, int paymentStatus, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayableAsync(Guid funeralRiderClaimPayableId, ServiceHeader serviceHeader);

        #endregion
    }
}
