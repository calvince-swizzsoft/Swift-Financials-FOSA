using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILevyService
    {
        #region Levy

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LevyDTO AddLevy(LevyDTO levyDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLevy(LevyDTO levyDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LevyDTO> FindLevies();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LevyDTO FindLevy(Guid levyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LevyDTO> FindLeviesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LevyDTO> FindLeviesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LevySplitDTO> FindLevySplitsByLevyId(Guid levyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLevySplitsByLevyId(Guid levyId, List<LevySplitDTO> levySplits);

        #endregion
    }
}
