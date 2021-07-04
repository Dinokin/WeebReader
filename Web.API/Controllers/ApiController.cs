using Microsoft.AspNetCore.Mvc;

namespace WeebReader.Web.API.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    public abstract class ApiController : ControllerBase
    { }
}