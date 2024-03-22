using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.CustomLogging;
using VillaAPI.Data;
using VillaAPI.Models;



namespace VillaAPI.Controllers
{
    // Stopped at the video 2:16:36 after creating a custom logger

    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VillaController> _logger;

        public VillaController(ApplicationDbContext context, ILogger<VillaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Villa>> GetVillas()
        {
            _logger.Log(LogLevel.Information, "Getting All Villas");

            var villas = _context.Villas.ToList();
            if (villas == null || villas.Count == 0)
            {
                _logger.LogError("No Villas Found");
                return StatusCode(StatusCodes.Status404NotFound, "No villas Found");
            }
            return StatusCode(StatusCodes.Status200OK, villas);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        //[HttpGet("GetVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Villa> Get(int id)
        {
            if(id == 0 || id.ToString() == null)
            {
                _logger.LogError("Invalid villa Id");
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid Id");
            }
            var villa = _context.Villas.FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                _logger.LogError("Villa not found");
                return NotFound();
            }
            return StatusCode(StatusCodes.Status200OK, villa);
        }

        [HttpPost("CreateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody]Villa villa)
        {
            if (villa == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Villa should not be null");
            }
            if(villa.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _context.Villas.Add(villa);
            _context.SaveChanges();
            return CreatedAtRoute("GetVilla", new {id = villa.Id}, villa);
            //return StatusCode(StatusCodes.Status201Created, "Villa Created Successfully!");
        }

        [HttpDelete("DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            if (id.ToString() == null || id == 0) return BadRequest("Invalid Id");
            var villa = _context.Villas.FirstOrDefault(x => x.Id == id);
            if(villa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Villa Not Found");
            }
            _context.Villas.Remove(villa);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent, "Villa Deleted Successfully");

        }



        [HttpPut("UpdateVilla")]        // Update complete record
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateVilla(int id, [FromBody] Villa villa)
        {
            if (villa == null || id != villa.Id)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            if (id == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid Id");
            }

            _context.Update(villa);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status200OK, villa);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<Villa> patchDocument)
        {
            if(id == 0 || patchDocument == null)
            {
                return BadRequest();
            }
            var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
            if(villa == null)
            {
                return BadRequest();
            }
            patchDocument.ApplyTo(villa, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Villas.Update(villa);
            _context.SaveChanges();

            return NoContent();

        }   // Patch Request is not working 1:23 in course REST API

    }
}
