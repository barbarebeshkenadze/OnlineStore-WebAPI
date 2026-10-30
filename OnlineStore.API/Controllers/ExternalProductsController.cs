using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalProductsController : ControllerBase
    {
        private readonly IExternalProductService _service;

        public ExternalProductsController(
            IExternalProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var json = await _service.GetProductsAsync();

            return Content(json, "application/json");
        }
    }
}
