using MagicVilla_API.Mapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/VillaApi")]
    [ApiController]
    [Authorize]
    public class VillaApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ILogger<VillaApiController> _logger;
        private readonly IVillaRepository _repository;

        private VillaToVillaDTOMapper villaToVillaDTOMapper = new VillaToVillaDTOMapper();
        private VillaDTOToVillaMapper villaDTOToVillaMapper = new VillaDTOToVillaMapper();

        public VillaApiController(ILogger<VillaApiController> Logger, IVillaRepository repository)
        {
            _logger = Logger;
            _repository = repository;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            _logger.LogInformation("Fetching Villas");

            try
            {
                List<Villa> villaList = await _repository.GetAsync();

                if (null == villaList || 0 >= villaList.Count)
                {
                    return NotFound();
                }

                List<VillaDTO> villaDTOList = new List<VillaDTO>();

                villaList.ForEach(villa =>
                {
                    villaDTOList.Add(villaToVillaDTOMapper.CreateMap(villa));
                });

                _response.Result = villaDTOList;
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

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            if (0 == id)
            {
                _logger.LogError("INVALID ID: " + id + " is not a valid id");
                return BadRequest();
            }

            try
            {
                Villa villa = await _repository.GetByAsync(villa => id == villa.Id);

                if (null == villa)
                {
                    return NotFound();
                }

                _response.Result = villaToVillaDTOMapper.CreateMap(villa);
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
        [Authorize (Roles ="ADMIN")]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            if (null != await _repository.GetByAsync(villa => villaDTO.Name.ToUpper() == villa.Name.ToUpper()))
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }

            if (null == villaDTO)
            {
                return BadRequest();
            }

            try
            {
                Villa villa = villaDTOToVillaMapper.CreateMap(villaDTO);
                villa.CreatedDate = DateTime.Now;

                await _repository.CreateAsync(villa);

                CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);

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

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {

            if (0 == id)
            {
                return BadRequest();
            }

            Villa villa = await _repository.GetByAsync(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            try
            {
                await _repository.RemoveAsync(villa);

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

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (null == villaDTO || 0 == id)
            {
                return BadRequest();
            }

            Villa villa = await _repository.GetByAsync(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            try
            {
                villa = villaDTOToVillaMapper.CreateMap(villaDTO);
                villa.Id = id;
                villa.UpdatedDate = DateTime.Now;

                await _repository.UpdateAsync(villa);

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

        [HttpPatch("{id:int}", Name = "PatchVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<APIResponse>> PatchVilla(int id, JsonPatchDocument<Villa> villaPatch)
        {
            if (null == villaPatch || 0 == id)
            {
                return BadRequest();
            }

            Villa villa = await _repository.GetByAsync(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            try
            {
                await _repository.PatchAsync(villa => id == villa.Id, villaPatch);

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
