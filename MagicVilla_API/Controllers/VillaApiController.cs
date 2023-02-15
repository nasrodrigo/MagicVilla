using MagicVilla_API.Transformers;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOs;
using MagicVilla_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using MagicVilla_API.Persistence;
using MagicVilla_API.DataMappers;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/VillaApi")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly ILogger<VillaApiController> _logger;
        private readonly IUnitOfWork _db;
        private readonly IVillaRepository villaRepository;
        private readonly IVillaDataMapper villaDataMapper;

        private readonly VillaToVillaDTOTransformer villaToVillaDTOTransformer;
        private readonly VillaDTOToVillaTransformer villaDTOToVillaTransformer;

        public VillaApiController(ILogger<VillaApiController> Logger, IUnitOfWork db)
        {
            _logger = Logger;
            _db = db;
            villaRepository = db.VillaRepository;
            villaDataMapper = db.VillaDataMapper;
            _response = new();
            villaToVillaDTOTransformer = new();
            villaDTOToVillaTransformer = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseCache(CacheProfileName = "Default30")]
        public ActionResult<ApiResponse> GetVillas([FromQuery(Name = "SearchName")] string? name,
            [FromQuery(Name = "Search")] string? search, int pageSize = 3, int pageNumber = 1)
        {
            _logger.LogInformation("Fetching Villas");

            try
            {
                List<Villa> villaList;
                if (!string.IsNullOrEmpty(name))
                {
                    villaList = villaRepository.Find(villa => name.ToUpper() == villa.Name!.ToUpper(), pageSize, pageNumber).ToList();
                }
                else
                {
                    villaList = villaRepository.GetAll(pageSize, pageNumber).ToList();
                }

                if (null == villaList || 0 >= villaList.Count)
                {
                    string errorMessage = "No Villa found!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return NotFound(_response.NotFoundResponse(errorMessage));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(villa => villa.Name!.ToUpper().Contains(search.ToUpper())).ToList();
                }

                List<VillaDTO> villaDTOList = new();

                villaList.ForEach(villa =>
                {
                    villaDTOList.Add(villaToVillaDTOTransformer.CreateMap(villa));
                });

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(
                    new Pagination()
                    {
                        PageSize = pageSize,
                        PageNumber = pageNumber
                    })
                    );

                _response.Result = villaDTOList;
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

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ApiResponse> GetVilla(int id)
        {
            if (0 == id)
            {
                string errorMessage = id + " is not a valid id!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                Villa villa = villaRepository.Get(id);

                if (null == villa)
                {
                    string errorMessage = "No Villa found for id " + id + "!";
                    _logger.LogError("ERROR: {errorMessage}", errorMessage);
                    return NotFound(_response.NotFoundResponse(errorMessage));
                }

                _response.Result = villaToVillaDTOTransformer.CreateMap(villa);
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
        public ActionResult<ApiResponse> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            if (null == villaDTO)
            {
                string errorMessage = "Villa Required!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            if (null != villaRepository.Find(villa => villaDTO.Name!.ToUpper() == villa.Name!.ToUpper()).FirstOrDefault())
            {
                string errorMessage = "Villa already exists!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            try
            {
                Villa villa = villaDTOToVillaTransformer.CreateMap(villaDTO);
                villa.CreatedDate = DateTime.Now;

                villaRepository.Add(villa);
                _db.Complete();

                CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);

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

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public ActionResult<ApiResponse> DeleteVilla(int id)
        {

            if (0 == id)
            {
                string errorMessage = id + " is not a valid id!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            Villa villa = villaRepository.Get(id);

            if (null == villa)
            {
                string errorMessage = "No Villa found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                villaRepository.Remove(villa);

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

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public ActionResult<ApiResponse> UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (null == villaDTO || 0 == id)
            {
                string errorMessage = "";

                if (0 == id)
                {
                    errorMessage = id + " is not a valid id!";
                }

                if (null == villaDTO)
                {
                    errorMessage = "Villa required!";
                }

                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            Villa villa = villaRepository.Get(id);

            if (null == villa)
            {
                string errorMessage = "No Villa found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                villa = villaDTOToVillaTransformer.CreateMap(villaDTO);
                villa.Id = id;
                villa.UpdatedDate = DateTime.Now;

                villaDataMapper.Update(villa);
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

        [HttpPatch("{id:int}", Name = "PatchVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> PatchVilla(int id, JsonPatchDocument<Villa> villaPatch)
        {
            if (null == villaPatch || 0 == id)
            {
                string errorMessage = "";

                if (0 == id)
                {
                    errorMessage = id + " is not a valid id!";
                }

                if (null == villaPatch)
                {
                    errorMessage = "Value required!";
                }

                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return BadRequest(_response.BadRequestResponse(errorMessage));
            }

            Villa villa = villaRepository.Get(id);

            if (null == villa)
            {
                string errorMessage = "No Villa found for id " + id + "!";
                _logger.LogError("ERROR: {errorMessage}", errorMessage);
                return NotFound(_response.NotFoundResponse(errorMessage));
            }

            try
            {
                await villaDataMapper.PatchAsync(villa => id == villa.Id, villaPatch);
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
    }
}
