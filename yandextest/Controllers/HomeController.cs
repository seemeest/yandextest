using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace yandextest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult TestApi(string test)
        {
            return Ok(test);
        }
    }
}
