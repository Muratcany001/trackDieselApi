using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarMenu.Controller
{
    [ApiController]
    public class PartController : ControllerBase
    {

        private readonly IPartRepository _partRepository;
        public PartController(IPartRepository partRepository)
        {
            _partRepository = partRepository;
        }
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost("parts/AddParts")]
        public async Task<ActionResult<Part>> AddPartAsync(Part part)
        {
            if (part == null)
            {
                return BadRequest("Lütfen geçerli değer girin.");
            }
            part.UserId = GetCurrentUserId();
            await _partRepository.AddPartAsync(part);
            return CreatedAtAction(nameof(GetPartById), new { id= part.Id},part);
        }



        [HttpGet("parts/GetAllParts/")]
        public async Task<ActionResult<List<Part>>> GetPartsAsync()
        {
            var parts = await _partRepository.GetAllPartsAsync(GetCurrentUserId());
            return Ok(parts);
        }
        [HttpGet("parts/GetPartById/{id}")]
        public async Task<ActionResult<Part>> GetPartById(int id)
        {
            var userId = GetCurrentUserId();
            var existedParts = await _partRepository.GetPartByIdAsync(id);
            if (existedParts == null || existedParts.UserId != userId)
            {
                return NotFound("Araç bulunamadı");
            }
            return Ok(existedParts);
        }
        [HttpPatch("parts/UpdatePart/{id}")]
        public async Task<ActionResult<Part>> UpdatePart(int id, [FromBody] Part updatedPart)
        {
            var userId = GetCurrentUserId();
            var existedPart = await _partRepository.GetPartByIdAsync(id);
            if (existedPart == null || existedPart.UserId != userId)
            {
                return NotFound("Araç bulunamadı");
            }
            updatedPart.Id = id;
            updatedPart.UserId = userId;
            var result = await _partRepository.UpdatePartAsync(updatedPart);
            return Ok(result);
        }
        [HttpDelete("parts/DeletePart/{id}")]
        public async Task<ActionResult<Part>> DeletePart(int id)
        {
            var userId = GetCurrentUserId();
            var existedPart = await _partRepository.GetPartByIdAsync(id);
            if (existedPart == null)
            {
                return NotFound("Parça bulunamadı");
            }
            await _partRepository.DeletePartAsync(id);
            return NoContent();
        }
        

    } 
}
