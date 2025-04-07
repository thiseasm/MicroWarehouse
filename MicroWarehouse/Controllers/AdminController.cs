using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Interfaces;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    public class AdminController(IMongoInitializationService mongoInitializationService) : ControllerBase
    {
        [HttpPost]
        [Route("initialize-mongo")]
        public async Task<IActionResult> InitializeMongoDbAsync(CancellationToken cancellationToken)
        {
            await mongoInitializationService.InitializeAsync(cancellationToken);
            return Ok();
        }

    }
}
