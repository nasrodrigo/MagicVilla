using MagicVilla_API.Mapper;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;
using MagicVilla_API.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/VillaNumberApi")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class VillaNumberApiController: ControllerBase
    {
        protected APIResponse _response;
        private readonly ILogger<VillaNumberApiController> _logger;
        private readonly IVillaNumberRepository _repository;
        private readonly IVillaRepository _repositoryVilla;

        private VillaNumberToVillaNumberDTOMapper VillaNumberToVillaNumberDTOMapper = new VillaNumberToVillaNumberDTOMapper();
        private VillaNumberDTOToVillaNumberMapper VillaNumberDTOToVillaNumberMapper = new VillaNumberDTOToVillaNumberMapper();

        public VillaNumberApiController(ILogger<VillaNumberApiController> Logger, IVillaNumberRepository repository, IVillaRepository repositoryVilla)
        {
            _logger = Logger;
            _repository = repository;
            _repositoryVilla = repositoryVilla;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            _logger.LogInformation("Fetching VillaNumbers");

            try
            {
                List<VillaNumber> VillaNumberList = await _repository.GetAsync();

                if (null == VillaNumberList || 0 >= VillaNumberList.Count)
                {
                    return NotFound();
                }

                List<VillaNumberDTO> VillaNumberDTOList = new List<VillaNumberDTO>();

                VillaNumberList.ForEach(VillaNumber =>
                {
                    VillaNumberDTOList.Add(VillaNumberToVillaNumberDTOMapper.CreateMap(VillaNumber));
                });

                _response.Result = VillaNumberDTOList;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
            }

            return Ok(_response);

        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            if (0 == id)
            {
                _logger.LogError("INVALID ID: " + id + " is not a valid id");
                return BadRequest();
            }

            try
            {
                VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

                if (null == VillaNumber)
                {
                    return NotFound();
                }

                _response.Result = VillaNumberToVillaNumberDTOMapper.CreateMap(VillaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
            }

            return Ok(_response);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberDTO VillaNumberDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            if (null == VillaNumberDTO)
            {
                return BadRequest();
            }

            try
            {

                if (null == await _repositoryVilla.GetByAsync(villa => VillaNumberDTO.VillaId == villa.Id))
                {
                    ModelState.AddModelError("CustomError", "Villa Id is invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber VillaNumber = VillaNumberDTOToVillaNumberMapper.CreateMap(VillaNumberDTO);
                VillaNumber.CreatedDate = DateTime.Now;

                await _repository.CreateAsync(VillaNumber);

                CreatedAtRoute("GetVillaNumber", new { id = VillaNumber.Id }, VillaNumber);

                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
            }


            return Ok(_response);
        }

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {

            if (0 == id)
            {
                return BadRequest();
            }

            VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

            if (null == VillaNumber)
            {
                return NotFound();
            }

            try
            {
                await _repository.RemoveAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
            }

            return Ok(_response);
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberDTO VillaNumberDTO)
        {
            if (null == VillaNumberDTO || 0 == id)
            {
                return BadRequest();
            }

            VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

            if (null == VillaNumber)
            {
                return NotFound();
            }

            try
            {
                if (null == await _repositoryVilla.GetByAsync(villa => VillaNumberDTO.VillaId == villa.Id))
                {
                    ModelState.AddModelError("CustomError", "Villa Id is invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber = VillaNumberDTOToVillaNumberMapper.CreateMap(VillaNumberDTO);
                VillaNumber.Id = id;
                VillaNumber.UpdatedDate = DateTime.Now;

                await _repository.UpdateAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
            }

            return Ok(_response);
        }

        [HttpPatch("{id:int}", Name = "PatchVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> PatchVillaNumber(int id, JsonPatchDocument<VillaNumber> VillaNumberPatch)
        {
            if (null == VillaNumberPatch || 0 == id)
            {
                return BadRequest();
            }

            VillaNumber VillaNumber = await _repository.GetByAsync(VillaNumber => id == VillaNumber.Id);

            if (null == VillaNumber)
            {
                return NotFound();
            }

            try
            {
                var operationVillaId = VillaNumberPatch.Operations.FirstOrDefault(operation => "/villaId" == operation.path);
                if ( null != operationVillaId && null != await _repositoryVilla.GetByAsync(villa => (int)operationVillaId.value == villa.Id))
                {
                    ModelState.AddModelError("CustomError", "Villa Id is invalid!");
                    return BadRequest(ModelState);
                }

                await _repository.PatchAsync(VillaNumber => id == VillaNumber.Id, VillaNumberPatch);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

            }
            catch (Exception e)
            {
                _response.ErrorMessage = new List<string>() { e.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
            }

            return Ok(_response);
        }
    }
}
