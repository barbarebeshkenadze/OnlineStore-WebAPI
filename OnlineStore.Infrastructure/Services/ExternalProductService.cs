using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Infrastructure.Services
{
    public class ExternalProductService : IExternalProductService
    {
        private readonly HttpClient _httpClient;

        public ExternalProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync(
                "https://dummyjson.com/products?limit=10");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}