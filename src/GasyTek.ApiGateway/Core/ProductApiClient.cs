using GasyTek.ApiGateway.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GasyTek.ApiGateway.Core
{
    public class ProductApiClient
    {
        private const string ProductServiceUrl = "http://localhost:5002";
        private const string CartServiceUrl = "http://localhost:5001";
        private readonly HttpClient httpClient;

        public ProductApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ProductData[]> GetProductListAsync()
        {
            using (var response = await this.httpClient.GetAsync($"{ProductServiceUrl}/products"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductData[]>();
                }
                else
                {
                    throw new Exception($"Call to Product Service failed with status {response.StatusCode}");
                }
            }
        }

        public async Task<ProductData> GetProductAsync(int id)
        {
            using (var response = await this.httpClient.GetAsync($"{ProductServiceUrl}/products/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductData>();
                }
                else
                {
                    return null;
                }
            };
        }
    }
}
