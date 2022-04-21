using Microsoft.AspNetCore.Mvc;
using ocelot_securedApi.Models;

namespace ocelot_securedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecuredController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Private information");
        }

        [HttpGet("{number:int}")]
        public async Task<IActionResult> GetNumberSquare(int number)
        {
            return Ok($"Secured number square: {number * number}");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NamesModel model)
        {
            var firstName = model.FirstName ?? throw new ArgumentNullException();
            var lastName = model.LastName ?? throw new ArgumentNullException();

            return Ok($"Your secured name: {firstName} {lastName}");
        }
    }
}