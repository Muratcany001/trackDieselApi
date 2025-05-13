using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarMenu.Controller
{
    [ApiController]
    [Authorize] 
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
        public async Task<ActionResult<Car>> AddCar(Car car)
        {
            if (car == null)
            {
                return BadRequest("Lütfen araç bilgilerini doldur...");
            }
            var existingCar = _carRepository.GetCarByPlate(car.Plate);
            if (existingCar == null) { 
                return BadRequest("Plaka sistemde mevcut");
            }
            car.UserId = GetCurrentUserId();

            foreach (var issue in car.ErrorHistory)
            {
                issue.Id = 0;
                issue.Car = null;
            }

            var AddCar = await _carRepository.AddCar(car);
            return Ok(AddCar);
        }

        [HttpGet("cars/GetAll")]
        public async Task<ActionResult<List<Car>>> GetAllCar()
        {
            var userId = GetCurrentUserId();
            var cars = await _carRepository.GetAllCars();
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
            if (car.UserId != GetCurrentUserId())
            {
                return Forbid("Bu araca erişim yetkiniz yok");
            }

            return Ok(car);
        }

        [HttpPatch("cars/UpdateCar/{plate}")]
        public async Task<IActionResult> UpdateCarIssues(string plate, [FromBody] List<Issue> updatedIssues)
        {
            try
            { 
                var car = await _carRepository.UpdateCar(plate, updatedIssues);
                return Ok(car);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("cars/DeleteCar/{plate}")]
        public async Task<ActionResult<Car>> DeleteCar(string plate)
        {
            var selectedCar = await _carRepository.GetCarByPlate(plate);
            if (selectedCar == null)
            {
                return NotFound("Araç bulunamadı");
            }
            if (selectedCar.UserId != GetCurrentUserId())
            {
                return Forbid("Bu aracı silme yetkiniz yok");
            }

            var isDeleted = await _carRepository.DeleteCar(plate);
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
            var userCarCount = cars.Count(c => c.UserId == userId);
            return Ok(userCarCount);
        }

        [HttpGet("cars/getAllIssues")]
        public async Task<IActionResult> GetAllIssues()
        {
            var userId = GetCurrentUserId();
            var Issues = await _carRepository.GetAllIssues();
            var userIssues = Issues.Where(i => i.Car.UserId == userId).ToList();
            if (userIssues == null || !userIssues.Any())    
            {
                return NotFound("Kayıtlı arıza parçası bulunamadı");
            }
            return Ok(userIssues);
        }
        [HttpGet("cars/GetModelsWithBrokenParts")]
        public async Task<IActionResult> GetModelsWithBrokenParts()
        {
           var userId = GetCurrentUserId();
           var models = await _carRepository.GetModelsWithBrokenParts();
           var userModels = models.Where(i=> (string)i.UserId == userId).ToList();
            if (userModels == null || !userModels.Any())
            {
                return NotFound("Araç kayıtları bulunamadı");
            }
            return Ok(userModels);
        }
        [HttpGet("cars/MostCommonProblems")]
        public async Task<IActionResult> MostCommonProblems()
        {
            var userId = GetCurrentUserId();
            var problems = await _carRepository.MostCommonProblems();
            var userProblems = problems.Where(i=> (string)i.UserId == userId).ToList();
            if(userProblems == null || !userProblems.Any())
            {
                return NotFound("Araç kayıtları bulunamadı");
            }
            return Ok(userProblems);
        }
    }
}
