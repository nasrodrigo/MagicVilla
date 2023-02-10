using MagicVilla_API.Mapper;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;
using MagicVilla_API.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/VillaNumberApi")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class VillaNumberApiController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly ILogger<VillaNumberApiController> _logger;
        private readonly IVillaNumberRepository _repository;
        private readonly IVillaRepository _repositoryVilla;

        private readonly VillaNumberToVillaNumberDTOMapper villaNumberToVillaNumberDTOMapper;
        private readonly VillaNumberDTOToVillaNumberMapper villaNumberDTOToVillaNumberMapper;

        public VillaNumberApiController(ILogger<VillaNumberApiController> Logger, IVillaNumberRepository repository, IVillaRepository repositoryVilla)
        {
            _logger = Logger;
            _repository = repository;
            _repositoryVilla = repositoryVilla;
            _response = new();
            villaNumberToVillaNumberDTOMapper = new();
            villaNumberDTOToVillaNumberMapper = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumbers()
        {
            _logger.LogInformation("Fetching VillaNumbers");

            try
            {
                List<VillaNumber> VillaNumberList = await _repository.GetAsync();

                if (null == VillaNumberList || 0 >= VillaNumberList.Count)
                {
                    string errorMessage = "No VillaNumber found!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return NotFound(_response.NotFoundResponse(errorMessage));
                }

                List<VillaNumberDTO> VillaNumberDTOList = new();

                VillaNumberList.ForEach(VillaNumber =>
                {
                    VillaNumberDTOList.Add(villaNumberToVillaNumberDTOMapper.CreateMap(VillaNumber));
                });

                _response.Result = VillaNumberDTOList;
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

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumber(int id)
        {
            if (0 == id)
            {
                string errorMessage = id + " is not a valid id!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

                if (null == VillaNumber)
                {
                    string errorMessage = "No VillaNumber found for id " + id + "!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return NotFound(_response.NotFoundResponse(errorMessage));
                }

                _response.Result = villaNumberToVillaNumberDTOMapper.CreateMap(VillaNumber);
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] VillaNumberDTO VillaNumberDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            if (null == VillaNumberDTO)
            {
                string errorMessage = "VillaNumber Required!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            if (null != await _repository.GetByAsync(villaNumber => VillaNumberDTO.VillaId == villaNumber.VillaId))
            {
                string errorMessage = "VillaNumber already exists!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                VillaNumber VillaNumber = villaNumberDTOToVillaNumberMapper.CreateMap(VillaNumberDTO);
                VillaNumber.CreatedDate = DateTime.Now;

                await _repository.CreateAsync(VillaNumber);

                CreatedAtRoute("GetVillaNumber", new { id = VillaNumber.Id }, VillaNumber);

                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _logger.LogError("ERROR: Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError, _response.InternalServerErrorResponse(new List<string>() { e.ToString() })); ;
            }


            return Ok(_response);
        }

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> DeleteVillaNumber(int id)
        {

            if (0 == id)
            {
                string errorMessage = id + " is not a valid id!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

            if (null == VillaNumber)
            {
                string errorMessage = "No VillaNumber found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                await _repository.RemoveAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _logger.LogError("ERROR: Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError, _response.InternalServerErrorResponse(new List<string>() { e.ToString() }));
            }

            return Ok(_response);
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberDTO VillaNumberDTO)
        {
            if (null == VillaNumberDTO || 0 == id)
            {
                string errorMessage = "";

                if (0 == id)
                {
                    errorMessage = id + " is not a valid id!";
                }

                if (null == VillaNumberDTO)
                {
                    errorMessage = "VillaNumber required!";
                }

                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

            if (null == VillaNumber)
            {
                string errorMessage = "No VillaNumber found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            if (null == await _repositoryVilla.GetByAsync(villa => VillaNumberDTO.VillaId == villa.Id))
            {
                string errorMessage = "No Villa found for VillaId " + VillaNumberDTO.VillaId + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                VillaNumber = villaNumberDTOToVillaNumberMapper.CreateMap(VillaNumberDTO);
                VillaNumber.Id = id;
                VillaNumber.UpdatedDate = DateTime.Now;

                await _repository.UpdateAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _logger.LogError("ERROR: Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError, _response.InternalServerErrorResponse(new List<string>() { e.ToString() }));
            }

            return Ok(_response);
        }

        [HttpPatch("{id:int}", Name = "PatchVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> PatchVillaNumber(int id, JsonPatchDocument<VillaNumber> VillaNumberPatch)
        {
            if (null == VillaNumberPatch || 0 == id)
            {
                string errorMessage = "";

                if (0 == id)
                {
                    errorMessage = id + " is not a valid id!";
                }

                if (null == VillaNumberPatch)
                {
                    errorMessage = "Value required!";
                }

                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

            if (null == VillaNumber)
            {
                string errorMessage = "No VillaNumber found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            var operationVillaId = VillaNumberPatch.Operations.FirstOrDefault(operation => "/villaId" == operation.path);
            if (null != operationVillaId && null == await _repositoryVilla.GetByAsync(villa => (int)operationVillaId.value == villa.Id))
            {
                string errorMessage = "No Villa found for VillaId " + operationVillaId.value + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                await _repository.PatchAsync(VillaNumber => id == VillaNumber.Id, VillaNumberPatch);

                _response.StatusCode = HttpStatusCode.NoContent;
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
