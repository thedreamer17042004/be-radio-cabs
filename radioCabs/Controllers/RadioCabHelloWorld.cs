using Microsoft.AspNetCore.Mvc;

namespace radioCabs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RadioCabHelloWorld : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Welcome to RadioCab.in";
        }
    }
}
