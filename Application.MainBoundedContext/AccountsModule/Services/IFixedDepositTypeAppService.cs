using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IFixedDepositTypeAppService
    {
        FixedDepositTypeDTO AddNewFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, ServiceHeader serviceHeader);

        bool UpdateFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, ServiceHeader serviceHeader);

        List<FixedDepositTypeDTO> FindFixedDepositTypes(ServiceHeader serviceHeader);

        List<FixedDepositTypeDTO> FindFixedDepositTypesByMonths(int months, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        FixedDepositTypeDTO FindFixedDepositType(Guid fixedDepositTypeId, ServiceHeader serviceHeader);

        List<LevyDTO> FindLevies(Guid fixedDepositTypeId, ServiceHeader serviceHeader);

        bool UpdateLevies(Guid fixedDepositTypeId, List<LevyDTO> levies, ServiceHeader serviceHeader);

        ProductCollectionInfo FindAttachedProducts(Guid fixedDepositTypeId, ServiceHeader serviceHeader, bool useCache = true);

        ProductCollectionInfo FindCachedAttachedProducts(Guid fixedDepositTypeId, ServiceHeader serviceHeader);

        bool UpdateAttachedProducts(Guid fixedDepositTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader);

        List<TariffWrapper> ComputeTariffs(Guid fixedDepositTypeId, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName, ServiceHeader serviceHeader);

        List<FixedDepositTypeGraduatedScaleDTO> FindGraduatedScales(Guid fixedDepositTypeId, ServiceHeader serviceHeader);

        bool UpdateGraduatedScales(Guid fixedDepositTypeId, List<FixedDepositTypeGraduatedScaleDTO> graduatedScales, ServiceHeader serviceHeader);
    }
}
