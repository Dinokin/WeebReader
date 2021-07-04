using Microsoft.AspNetCore.Mvc;

namespace WeebReader.Web.API.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    public abstract class ApiController : ControllerBase
    { }
}