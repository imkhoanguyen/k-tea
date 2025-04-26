using Microsoft.AspNetCore.Mvc;
using Tea.Infrastructure.Interfaces;

namespace Tea.Api.Controllers
{
    public class GeminiAIController(IGeminiService geminiService) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string input)
        {
            return Ok(await geminiService.GetDrinkRecommendationAsync(input));
        }
    }
}
