using GasyTek.ApiGateway.Contracts;
using GasyTek.ApiGateway.Core;
using Microsoft.AspNetCore.Mvc;

namespace GasyTek.ApiGateway.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> logger;
        private readonly ProductApiClient productApiClient;

        public OrdersController(ILogger<OrdersController> logger, ProductApiClient productApiClient)
        {
            this.logger = logger;
            this.productApiClient = productApiClient;
        }

        [HttpGet("/products", Name = nameof(GetProductList))]
        public async Task<IActionResult> GetProductList()
        {
            this.logger.LogInformation("Get product list");

            try
            {
                var products = await this.productApiClient.GetProductListAsync();
                return this.Ok(products);
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex);
            }
        }

        [HttpGet("/products/{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            this.logger.LogInformation("Get single product");

            var product = await this.productApiClient.GetProductAsync(id);
            return product is null ? this.NotFound() : this.Ok(product);
        }
    }
}