using MagicVilla_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MagicVilla_API.Persistence;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/Auth")]
    [ApiVersionNeutral]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly ILogger<AuthApiController> _logger;
        private readonly IUnitOfWork _db;

        public AuthApiController(ILogger<AuthApiController> Logger, IUnitOfWork db)
        {
            _logger = Logger;
            _db = db;
            _response = new();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequest loginRequestDTO)
        {
            if (loginRequestDTO is null ||
                (string.IsNullOrEmpty(loginRequestDTO.UserName) && string.IsNullOrEmpty(loginRequestDTO.Password)))
            {
                string errorMessage = "UserName and Password are required!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                LoginResponse loginResponse = await _db.AuthDataMapper.Login(loginRequestDTO);
                if (loginResponse is null ||
                (loginResponse.User is null && string.IsNullOrEmpty(loginResponse.Token)))
                {
                    string errorMessage = "UserName or Password incorrect!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return BadRequest(_response.BadRequestResponse(errorMessage));
                }

                _response.Result = loginResponse;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _logger.LogError("ERROR: Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError, _response.InternalServerErrorResponse(new List<string>() { e.ToString() }));
            }

            return Ok(_response);
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] User user)
        {
            if (null == user)
            {
                string errorMessage = "User required!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            var isExistingUser = await _db.AuthDataMapper.IsExistingUser(user.UserName!);
            if (isExistingUser)
            {
                string errorMessage = "User already exists!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                
                await _db.AuthDataMapper.Register(user);
                _db.Complete();

                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _logger.LogError("ERROR: Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError, _response.InternalServerErrorResponse(new List<string>() { e.ToString() }));
            }

            return Ok(_response);
        }

    }
}
