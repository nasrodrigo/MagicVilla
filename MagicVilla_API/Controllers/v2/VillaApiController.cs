using MagicVilla_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaApi")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize]
    public class VillaApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ILogger<VillaApiController> _logger;

        public VillaApiController(ILogger<VillaApiController> Logger)
        {
            _logger = Logger;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<APIResponse> GetVillas()
        {
            _logger.LogInformation("Test Versioning");

            _response.Result = "Test Versioning result";
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;

            return Ok(_response);

        }

    }
}