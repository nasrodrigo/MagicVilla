using MagicVilla_API.Mapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/Auth")]
    [ApiVersionNeutral]
    [ApiController]
    public class LocalUserApiController : ControllerBase
    {

        protected ApiResponse _response;
        private readonly ILogger<LocalUserApiController> _logger;
        private readonly ILocalUserRepository _repository;
        private readonly ILocalUserRoleRepository _roleRepository;

        private LocalUserDTOToLocalUserMapper localUserDTOToLocalUserMapper = new LocalUserDTOToLocalUserMapper();
        private LocalUserRoleDTOToLocalUserRoleMapper localUserRoleDTOToLocalUserRoleMapper = new LocalUserRoleDTOToLocalUserRoleMapper();

        public LocalUserApiController(ILogger<LocalUserApiController> Logger, ILocalUserRepository repository, ILocalUserRoleRepository roleRepository)
        {
            _logger = Logger;
            _repository = repository;
            _roleRepository = roleRepository;
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
                return BadRequest();
            }

            try
            {
                LoginResponse loginResponse = await _repository.Login(loginRequestDTO);
                if (loginResponse is null ||
                (loginResponse.User is null && string.IsNullOrEmpty(loginResponse.Token)))
                {
                    _response.ErrorMessage = new List<string> {
                        "UserName or Password incorrect!"
                    };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                _response.Result = loginResponse;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] LocalUserDTO localUserDTO)
        {
            var isExistingUser = await _repository.isExistingUser(localUserDTO.UserName);
            if (isExistingUser)
            {
                ModelState.AddModelError("CustomError", "User already exists!");
                return BadRequest(ModelState);
            }

            if (null == localUserDTO)
            {
                return BadRequest();
            }

            try
            {
                LocalUser localUser = localUserDTOToLocalUserMapper.CreateMap(localUserDTO);
                await _repository.Register(localUser);

                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

    }
}
