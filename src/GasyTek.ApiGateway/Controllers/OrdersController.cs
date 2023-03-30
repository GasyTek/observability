using GasyTek.ApiGateway.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GasyTek.ApiGateway.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private const string ProductServiceUrl = "http://localhost:5002";
        private const string CartServiceUrl = "http://localhost:5001";

        private readonly ILogger<OrdersController> logger;
        private readonly HttpClient httpClient;

        public OrdersController(ILogger<OrdersController> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }

        [HttpGet("/products", Name = nameof(GetProductList))]
        public async Task<IActionResult> GetProductList()
        {
            using (var response = await this.httpClient.GetAsync($"{ProductServiceUrl}/products"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<ProductData[]>();
                    return this.Ok(products);
                }
                else
                {
                    return this.StatusCode(500);
                }
            };
        }
    }
}