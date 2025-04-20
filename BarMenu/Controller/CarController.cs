using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarMenu.Controller
{
    [ApiController]
    [Authorize] // Tüm endpoint'ler için yetkilendirme gerekli
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        public CarController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost("cars/AddCar")]
        public IActionResult AddCar(Car car)
        {
            if (car == null)
            {
                return BadRequest("Lütfen araç bilgilerini doldur...");
            }

            // Kullanıcı ID'sini otomatik olarak set et
            car.UserId = GetCurrentUserId();

            foreach (var issue in car.ErrorHistory)
            {
                issue.Id = 0;
                issue.Car = null;
            }

            var AddCar = _carRepository.AddCar(car);
            return Ok(AddCar);
        }

        [HttpGet("cars/GetAll")]
        public async Task<ActionResult<List<Car>>> GetAllCar()
        {
            var userId = GetCurrentUserId();
            var cars = await _carRepository.GetAllCars();
            // Sadece kullanıcıya ait araçları filtrele
            var userCars = cars.Where(c => c.UserId == userId).ToList();
            return Ok(userCars);
        }

        [HttpGet("cars/GetCarByPlate/{plate}")]
        public async Task<ActionResult<Car>> GetCarByPlate(string plate)
        {
            var car = await _carRepository.GetCarByPlate(plate);
            if (car == null)
            {
                return NotFound("Araç bulunamadı");
            }

            // Kullanıcı kontrolü
            if (car.UserId != GetCurrentUserId())
            {
                return Forbid("Bu araca erişim yetkiniz yok");
            }

            return Ok(car);
        }

        [HttpPatch("cars/UpdateCar/{plate}")]
        public async Task<ActionResult<Car>> UpdateMenu(string plate, [FromBody] Car updateCar)
        {
            var existingCar = await _carRepository.GetCarByPlate(plate);
            if (existingCar == null)
            {
                return NotFound("Araç bulunamadı");
            }

            // Kullanıcı kontrolü
            if (existingCar.UserId != GetCurrentUserId())
            {
                return Forbid("Bu aracı güncelleme yetkiniz yok");
            }

            existingCar.Name = updateCar.Name ?? existingCar.Name;
            existingCar.Plate = updateCar.Plate ?? existingCar.Plate;
            existingCar.ErrorHistory = updateCar.ErrorHistory ?? existingCar.ErrorHistory;
            existingCar.LastMaintenanceDate = updateCar.LastMaintenanceDate == default(DateTime) ? existingCar.LastMaintenanceDate : updateCar.LastMaintenanceDate;
            existingCar.Age = updateCar.Age != 0 ? updateCar.Age : existingCar.Age;

            _carRepository.UpdateCar(existingCar);
            return Ok(existingCar);
        }

        [HttpDelete("cars/DeleteCar/{id}")]
        public async Task<ActionResult<Car>> DeleteCar(int id)
        {
            var selectedCar = await _carRepository.GetCarById(id);
            if (selectedCar == null)
            {
                return NotFound("Araç bulunamadı");
            }

            // Kullanıcı kontrolü
            if (selectedCar.UserId != GetCurrentUserId())
            {
                return Forbid("Bu aracı silme yetkiniz yok");
            }

            var isDeleted = await _carRepository.DeleteCar(id);
            if (!isDeleted)
            {
                return BadRequest("Araç silinmedi");
            }
            return NoContent();
        }

        [HttpGet("cars/GetCarCount")]
        public async Task<ActionResult<int>> GetCarCount()
        {
            var userId = GetCurrentUserId();
            var cars = await _carRepository.GetAllCars();
            // Sadece kullanıcıya ait araçların sayısını döndür
            var userCarCount = cars.Count(c => c.UserId == userId);
            return Ok(userCarCount);
        }

        [HttpGet("cars/getAllIssues")]
        public async Task<IActionResult> GetAllIssues()
        {
            var userId = GetCurrentUserId();
            var Issues = await _carRepository.GetAllIssues();
            // Sadece kullanıcıya ait araçların sorunlarını filtrele
            var userIssues = Issues.Where(i => i.Car?.UserId == userId).ToList();
            if (userIssues == null || !userIssues.Any())
            {
                return NotFound("Kayıtlı arıza parçası bulunamadı");
            }
            return Ok(userIssues);
        }
    }
}
