using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.InventoryModule.Services;
using Application.MainBoundedContext.UnderwritingModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftInventory.Presentation.Contracts.InventoryModule;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoryService(ICategoryAppService categoryAppService)
        {
            Guard.ArgumentNotNull(categoryAppService, nameof(categoryAppService));

            _categoryAppService = categoryAppService;
        }

        #region Category

        public async Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _categoryAppService.AddNewCategoryAsync(categoryDTO, serviceHeader);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _categoryAppService.UpdateCategoryAsync(categoryDTO, serviceHeader);
        }

        public async Task<List<CategoryDTO>> FindCategoriesAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _categoryAppService.FindCategoriesAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<CategoryDTO>> FindCategoriesInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _categoryAppService.FindCategoriesAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CategoryDTO>> FindCategoriesByFilterInPageAsync(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _categoryAppService.FindCategoriesAsync(filter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<CategoryDTO> FindCategoryAsync(Guid categoryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _categoryAppService.FindCategoryAsync(categoryId, serviceHeader);
        }

        public IAsyncResult BeginAddCategory(CategoryDTO categoryDTO, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public CategoryDTO EndAddCategory(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginUpdateCategory(CategoryDTO categoryDTO, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public bool EndUpdateCategory(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginFindCategoriesInPage(int pageIndex, int pageSize, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public PageCollectionInfo<CategoryDTO> EndFindCategoriesInPage(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginFindCategoriesByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public PageCollectionInfo<CategoryDTO> EndFindCategoriesByFilterInPage(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginFindCategory(Guid categoryId, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public CategoryDTO EndFindCategory(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginFindCategories(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public List<CategoryDTO> EndFindCategories(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
