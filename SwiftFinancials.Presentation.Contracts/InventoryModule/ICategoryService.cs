using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System.ServiceModel;


namespace SwiftFinancials.Presentation.Contracts.InventoryModule
{
    [ServiceContract(Name = "ICategoryService")]
    public interface ICategoryService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCategory(CategoryDTO categoryDTO, AsyncCallback callback, Object state);
        CategoryDTO EndAddCategory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCategory(CategoryDTO categoryDTO, AsyncCallback callback, Object state);
        bool EndUpdateCategory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCategoriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CategoryDTO> EndFindCategoriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCategoriesByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CategoryDTO> EndFindCategoriesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCategory(Guid categoryId, AsyncCallback callback, Object state);
        CategoryDTO EndFindCategory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCategories(AsyncCallback callback, Object state);
        List<CategoryDTO> EndFindCategories(IAsyncResult result);
    }
}