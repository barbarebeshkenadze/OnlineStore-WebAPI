using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;



namespace OnlineStore.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            ValidateProduct(product);

            await _productRepository.AddAsync(product);

            return product;
        }

        public async Task<bool> UpdateAsync(int id, Product product)
        {
            ValidateProduct(product);

            var existingProduct =
                await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return false;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(product);

            return true;
        }

        private static void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException(
                    "Product name is required.");
            }

            if (product.Name.Length > 100)
            {
                throw new ArgumentException(
                    "Product name cannot exceed 100 characters.");
            }

            if (product.Description?.Length > 500)
            {
                throw new ArgumentException(
                    "Product description cannot exceed 500 characters.");
            }

            if (product.Price <= 0)
            {
                throw new ArgumentException(
                    "Product price must be greater than zero.");
            }

            if (product.StockQuantity < 0)
            {
                throw new ArgumentException(
                    "Stock quantity cannot be negative.");
            }

            if (product.CategoryId <= 0)
            {
                throw new ArgumentException(
                    "A valid category ID is required.");
            }
        }
    }
}