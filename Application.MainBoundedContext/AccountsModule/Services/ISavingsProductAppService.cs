using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ISavingsProductAppService
    {
        SavingsProductDTO AddNewSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader);

        bool UpdateSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader);

        List<SavingsProductDTO> FindSavingsProducts(ServiceHeader serviceHeader);

        List<SavingsProductDTO> FindMandatorySavingsProducts(bool isMandatory, ServiceHeader serviceHeader);

        List<SavingsProductDTO> FindCachedSavingsProducts(ServiceHeader serviceHeader);

        List<SavingsProductDTO> FindSavingsProducts(int code, ServiceHeader serviceHeader);

        PageCollectionInfo<SavingsProductDTO> FindSavingsProducts(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SavingsProductDTO> FindSavingsProducts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        SavingsProductDTO FindSavingsProduct(Guid savingsProductId, Guid exemptionsBranchId, ServiceHeader serviceHeader);

        SavingsProductDTO FindCachedSavingsProduct(Guid savingsProductId, Guid exemptionsBranchId, ServiceHeader serviceHeader);

        SavingsProductDTO FindDefaultSavingsProduct(ServiceHeader serviceHeader);

        SavingsProductDTO FindCachedDefaultSavingsProduct(ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid savingsProductId, int savingsProductKnownChargeType, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCachedCommissions(Guid savingsProductId, int savingsProductKnownChargeType, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid savingsProductId, List<CommissionDTO> commissionDTOs, int savingsProductKnownChargeType, int savingsProductChargeBenefactor, ServiceHeader serviceHeader);

        List<SavingsProductExemptionDTO> FindSavingsProductExemptions(Guid savingsProductId, ServiceHeader serviceHeader);

        bool UpdateSavingsProductExemptions(Guid savingsProductId, List<SavingsProductExemptionDTO> savingsProductExemptions, ServiceHeader serviceHeader);

        List<SavingsProductDTO> FindSavingsProductsWithAutomatedLedgerFeeCalculation(ServiceHeader serviceHeader);
    }
}
