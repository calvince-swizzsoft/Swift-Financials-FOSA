using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.InventoryModule.Services;
using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.UnderwritingModule.Services
{
    public class CategoryAppService : ICategoryAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Category> _categoryRepository;

        public CategoryAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<Category> categoryRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<CategoryDTO> AddNewCategoryAsync(CategoryDTO categoryDTO, ServiceHeader serviceHeader)
        {
            var categoryBindingModel = categoryDTO.ProjectedAs<CategoryBindingModel>();

            categoryBindingModel.ValidateAll();

            if (categoryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, categoryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var category = CategoryFactory.CreateCategory(categoryDTO.Description);

                _categoryRepository.Add(category, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? category.ProjectedAs<CategoryDTO>() : null;
            }
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDTO categoryDTO, ServiceHeader serviceHeader)
        {
            var categoryBindingModel = categoryDTO.ProjectedAs<CategoryBindingModel>();

            categoryBindingModel.ValidateAll();

            if (categoryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, categoryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _categoryRepository.GetAsync(categoryDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = CategoryFactory.CreateCategory(categoryDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _categoryRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<CategoryDTO>> FindCategoriesAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _categoryRepository.GetAllAsync<CategoryDTO>(serviceHeader);
            }
        }

        public async Task<List<CategoryDTO>> FindCategoriesAsync(int level, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CategorySpecifications.DefaultSpec();

                ISpecification<Category> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _categoryRepository.AllMatchingAsync<CategoryDTO>(spec, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CategoryDTO>> FindCategoriesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CategorySpecifications.DefaultSpec();

                ISpecification<Category> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _categoryRepository.AllMatchingPagedAsync<CategoryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CategoryDTO>> FindCategoriesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CategorySpecifications.CategoryFullText(text);

                ISpecification<Category> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _categoryRepository.AllMatchingPagedAsync<CategoryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<CategoryDTO> FindCategoryAsync(Guid categoryId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _categoryRepository.GetAsync<CategoryDTO>(categoryId, serviceHeader);
            }
        }
    }
}