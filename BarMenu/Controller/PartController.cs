using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarMenu.Controller
{
    [ApiController]
    [Authorize]
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
        [HttpGet("parts/GetPartByName/{name}")]
        public async Task<ActionResult<Part>> GetPartByName(string name)
        {
            if (name == null)
            {
                return BadRequest("Geçerli ad giriniz");
            }
            var existedPart = await _partRepository.GetPartByName(name);
            if (existedPart == null) {
                return BadRequest("Araç bulunamadı");
            }
            return Ok(existedPart);
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
        [HttpPatch("parts/UpdateStockAsync/{id}")]
        public async Task<ActionResult<Part>> UpdateStockAsync(int id, [FromBody] Part updatedPart)
        {
            var userId = GetCurrentUserId();
            var existedPart = await _partRepository.GetPartByIdAsync(id);
            if (existedPart == null || existedPart.UserId != userId)
            {
                return NotFound("Araç bulunamadı");
            }
            updatedPart.Id = id;
            updatedPart.UserId = userId;
            var result = await _partRepository.UpdateStockAsync(updatedPart);
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
        [HttpPost("parts/AddBulkParts")]
        public async Task<IActionResult> AddBulkParts([FromBody] List<Part> parts)
        {
            if (parts == null || !parts.Any())
            {
                return BadRequest("En az bir parça bilgisi gereklidir.");
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi alınamadı.");
            }

            // Tüm parçalara userId ataması yap
            parts.ForEach(p => p.UserId = userId);

            try
            {
                await _partRepository.AddBulkPart(parts);
                return Ok($"{parts.Count} adet parça başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                // Hata detayını loglayabilirsiniz
                return StatusCode(500, $"Toplu parça ekleme sırasında hata oluştu: {ex.Message}");
            }
        }

    } 
}
