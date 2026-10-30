using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            ValidateCategory(category);
            await _categoryRepository.AddAsync(category);
            return category;
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            ValidateCategory(category);

            var existing = await _categoryRepository.GetByIdAsync(id);

            if (existing == null)
                return false;

            existing.Name = category.Name;
            existing.Description = category.Description;

            await _categoryRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return false;

            if (await _categoryRepository.HasProductsAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete a category that contains products.");

            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        private static void ValidateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name is required.");

            if (category.Name.Length > 100)
                throw new ArgumentException(
                    "Category name cannot exceed 100 characters.");

            if (category.Description?.Length > 300)
                throw new ArgumentException(
                    "Category description cannot exceed 300 characters.");
        }
    }
}
