using MagicVilla_API.Transformers;
using MagicVilla_API.Models.DTOs;
using MagicVilla_API.Models;
using MagicVilla_API.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MagicVilla_API.DataMappers;
using MagicVilla_API.Persistence;

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
        private readonly IUnitOfWork _db;
        private readonly IVillaNumberRepository villaNumberRepository;
        private readonly IVillaNumberDataMapper villaNumberDataMapper;
        private readonly IVillaRepository villaRepository;

        private readonly VillaNumberToVillaNumberDTOTransformer villaNumberToVillaNumberDTOTransformer;
        private readonly VillaNumberDTOToVillaNumberTransformer villaNumberDTOToVillaNumberTransformer;

        public VillaNumberApiController(ILogger<VillaNumberApiController> Logger, IUnitOfWork db)
        {
            _logger = Logger;
            _db = db;
            villaNumberRepository = _db.VillaNumberRepository;
            villaNumberDataMapper = _db.VillaNumberDataMapper;
            villaRepository = _db.VillaRepository;
            _response = new();
            villaNumberToVillaNumberDTOTransformer = new();
            villaNumberDTOToVillaNumberTransformer = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ApiResponse> GetVillaNumbers()
        {
            _logger.LogInformation("Fetching VillaNumbers");

            try
            {
                List<VillaNumber> VillaNumberList = villaNumberRepository.GetAll().ToList();

                if (null == VillaNumberList || 0 >= VillaNumberList.Count)
                {
                    string errorMessage = "No VillaNumber found!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return NotFound(_response.NotFoundResponse(errorMessage));
                }

                List<VillaNumberDTO> VillaNumberDTOList = new();

                VillaNumberList.ForEach(VillaNumber =>
                {
                    VillaNumberDTOList.Add(villaNumberToVillaNumberDTOTransformer.CreateMap(VillaNumber));
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
        public ActionResult<ApiResponse> GetVillaNumber(int id)
        {
            if (0 == id)
            {
                string errorMessage = id + " is not a valid id!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                VillaNumber VillaNumber = villaNumberRepository.Get(id);

                if (null == VillaNumber)
                {
                    string errorMessage = "No VillaNumber found for id " + id + "!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return NotFound(_response.NotFoundResponse(errorMessage));
                }

                _response.Result = villaNumberToVillaNumberDTOTransformer.CreateMap(VillaNumber);
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
        public ActionResult<ApiResponse> CreateVillaNumber([FromBody] VillaNumberDTO VillaNumberDTO)
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

            if (null != villaNumberRepository.Find(villaNumber => VillaNumberDTO.VillaId == villaNumber.VillaId).FirstOrDefault())
            {
                string errorMessage = "VillaNumber already exists!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                VillaNumber VillaNumber = villaNumberDTOToVillaNumberTransformer.CreateMap(VillaNumberDTO);
                VillaNumber.CreatedDate = DateTime.Now;

                villaNumberRepository.Add(VillaNumber);
                _db.Complete();

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
        public ActionResult<ApiResponse> DeleteVillaNumber(int id)
        {

            if (0 == id)
            {
                string errorMessage = id + " is not a valid id!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            VillaNumber VillaNumber = villaNumberRepository.Get(id);

            if (null == VillaNumber)
            {
                string errorMessage = "No VillaNumber found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                villaNumberRepository.Remove(VillaNumber);
                _db.Complete();

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
        public ActionResult<ApiResponse> UpdateVillaNumber(int id, [FromBody] VillaNumberDTO VillaNumberDTO)
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

            VillaNumber VillaNumber = villaNumberRepository.Get(id);

            if (null == VillaNumber)
            {
                string errorMessage = "No VillaNumber found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            if (null == villaRepository.Get(VillaNumberDTO.VillaId))
            {
                string errorMessage = "No Villa found for VillaId " + VillaNumberDTO.VillaId + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                VillaNumber = villaNumberDTOToVillaNumberTransformer.CreateMap(VillaNumberDTO);
                VillaNumber.Id = id;
                VillaNumber.UpdatedDate = DateTime.Now;

                villaNumberDataMapper.Update(VillaNumber);
                _db.Complete();

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

            VillaNumber VillaNumber = villaNumberRepository.Get(id);

            if (null == VillaNumber)
            {
                string errorMessage = "No VillaNumber found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            var operationVillaId = VillaNumberPatch.Operations.FirstOrDefault(operation => "/villaId" == operation.path);
            if (null != operationVillaId && null == villaRepository.Get((int)operationVillaId.value))
            {
                string errorMessage = "No Villa found for VillaId " + operationVillaId.value + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                await villaNumberDataMapper.PatchAsync(VillaNumber => id == VillaNumber.Id, VillaNumberPatch);

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
