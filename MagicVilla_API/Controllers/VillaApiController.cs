using MagicVilla_API.Data;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/VillaApi")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            List<VillaDTO> villaList = VillaStore.VillaList;

            if (null == villaList || 0 >= villaList.Count)
            {
                return NotFound();
            }

            return Ok(villaList);

        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (0 == id)
            {
                return BadRequest();
            }

            VillaDTO villa = VillaStore.VillaList.FirstOrDefault(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            if(null != VillaStore.VillaList.FirstOrDefault(villa => villaDTO.Name.ToUpper() == villa.Name.ToUpper()))
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }

            if (null == villaDTO)
            {
                return BadRequest();
            }

            if (0 < villaDTO.Id)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDTO.Id = VillaStore.VillaList.OrderByDescending(villa => villa.Id).FirstOrDefault().Id + 1;
            VillaStore.VillaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (0 == id)
            {
                return BadRequest();
            }

            VillaDTO villa = VillaStore.VillaList.FirstOrDefault(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            VillaStore.VillaList.Remove(villa);

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDto)
        {
            if (0 == id)
            {
                return BadRequest();
            }

            VillaDTO villa = VillaStore.VillaList.FirstOrDefault(villa => id == villa.Id);

            if (null == villa)
            {
                return NotFound();
            }

            villa.Name = villaDto.Name;

            return NoContent();
        }
    }
}
