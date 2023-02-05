using MagicVilla_API.Mapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/VillaApi")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        private readonly ILogger<VillaApiController> _logger;
        private readonly IVillaRepository _repository;

        private VillaToVillaDTOMapper villaToVillaDTOMapper = new VillaToVillaDTOMapper();
        private VillaDTOToVillaMapper villaDTOToVillaMapper = new VillaDTOToVillaMapper();

        public VillaApiController(ILogger<VillaApiController> Logger, IVillaRepository repository)
        {
            _logger = Logger;
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Fetching Villas");

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

            return Ok(villaDTOList);

        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (0 == id)
            {
                _logger.LogError("Id can not be " + id);
                return BadRequest();
            }

            Villa villa = await _repository.GetByAsync(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            return Ok(villaToVillaDTOMapper.CreateMap(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaDTO villaDTO)
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

            Villa villa = villaDTOToVillaMapper.CreateMap(villaDTO);
            villa.CreatedDate = DateTime.Now;

            await _repository.CreateAsync(villa);

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
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

            await _repository.RemoveAsync(villa);

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
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

            villa = villaDTOToVillaMapper.CreateMap(villaDTO);
            villa.Id = id;
            villa.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(villa);

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "PatchVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchVilla(int id, JsonPatchDocument<Villa> villaPatch)
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

            await _repository.PatchAsync(villa => id == villa.Id, villaPatch);

            return NoContent();
        }
    }
}
