using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/Auth")]
    [ApiVersionNeutral]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly ILogger<AuthApiController> _logger;
        private readonly IAuthRepository _repository;

        public AuthApiController(ILogger<AuthApiController> Logger, IAuthRepository repository)
        {
            _logger = Logger;
            _repository = repository;
            _response = new();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDTO loginRequestDTO)
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
                LoginResponseDTO loginResponse = await _repository.Login(loginRequestDTO);
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
        public async Task<ActionResult<ApiResponse>> Register([FromBody] UserDTO user)
        {
            if (null == user)
            {
                string errorMessage = "User required!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            var isExistingUser = await _repository.IsExistingUser(user.UserName!);
            if (isExistingUser)
            {
                string errorMessage = "User already exists!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                
                await _repository.Register(user);

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
