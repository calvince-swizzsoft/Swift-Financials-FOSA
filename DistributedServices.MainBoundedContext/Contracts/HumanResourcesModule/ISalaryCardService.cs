using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISalaryCardService
    {
        #region Salary Card

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryCardDTO AddSalaryCard(SalaryCardDTO salaryCardDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalaryCard(SalaryCardDTO salaryCardDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ResetSalaryCardEntries(SalaryCardDTO salaryCardDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalaryCardEntry(SalaryCardEntryDTO salaryCardEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalaryCardDTO> FindSalaryCards();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryCardDTO> FindSalaryCardsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryCardDTO> FindSalaryCardsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryCardDTO FindSalaryCard(Guid salaryCardId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalaryCardEntryDTO> FindSalaryCardEntriesBySalaryCardId(Guid salaryCardId);

        #endregion
    }
}
