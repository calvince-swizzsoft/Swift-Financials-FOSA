using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public interface ICategoryAppService
    {
        Task<CategoryDTO> AddNewCategoryAsync(CategoryDTO categoryDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateCategoryAsync(CategoryDTO categoryDTO, ServiceHeader serviceHeader);

        Task<List<CategoryDTO>> FindCategoriesAsync(ServiceHeader serviceHeader);

        Task<List<CategoryDTO>> FindCategoriesAsync(int level,ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CategoryDTO>> FindCategoriesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CategoryDTO>> FindCategoriesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<CategoryDTO> FindCategoryAsync(Guid categoryId, ServiceHeader serviceHeader);
    }
}
