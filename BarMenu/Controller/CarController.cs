using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;

namespace BarMenu.Controller
{
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        public CarController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }
        [HttpPost("cars/AddCar")]
        public IActionResult AddCar(Car car)
        {    
                if (car == null)
                {
                return BadRequest("Lütfen araç bilgilerini doldur...");
                }
            foreach (var issue in car.ErrorHistory)
            {
                issue.Id = 0;
                issue.Car = null;
            }

            var AddCar = _carRepository.AddCar(car);
            return Ok(AddCar);
        }
        [HttpGet("cars/GetAll")]
        public async Task<ActionResult<List<Car>>>  GetAllCar() {
            var cars = await _carRepository.GetAllCars();
            return Ok(cars);
        }
        [HttpGet("cars/GetCarByPlate/{plate}")]
        public async Task<ActionResult<Car>> GetCarByPlate(string plate) { 
            var car = await _carRepository.GetCarByPlate(plate);
            if (car == null)
            {
                return NotFound("Araç bulunamadı");
            }
            return Ok(car);
        }
        [HttpPatch("cars/UpdateCar/{plate}")]
        public async Task<ActionResult<Car>> UpdateMenu(string plate, [FromBody] Car updateCar) {
            var existingCar = await _carRepository.GetCarByPlate(plate);
            if (existingCar == null)
            {
                return NotFound("Menü bulunamadı");
            }
            existingCar.Name = updateCar.Name ?? existingCar.Name;  // Name varsa güncellenir
            existingCar.Plate = updateCar.Plate ?? existingCar.Plate;  // Plate varsa güncellenir
            existingCar.ErrorHistory = updateCar.ErrorHistory ?? existingCar.ErrorHistory;  // ErrorHistory varsa güncellenir
            existingCar.LastMaintenanceDate = updateCar.LastMaintenanceDate == default(DateTime) ? existingCar.LastMaintenanceDate : updateCar.LastMaintenanceDate;  // LastMaintenanceDate varsa güncellenir
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
                return NotFound("Menü bulunamadı");
            }

            var isDeleted = await _carRepository.DeleteCar(id);
            if (!isDeleted) {
                return BadRequest("Araç silinmedi");
            }
            return NoContent();
        }
        [HttpGet("cars/GetCarCount")]
        public async Task<ActionResult<int>> GetCarCount()
        {
            var carCount = await _carRepository.GetCarCountAsync();
            return Ok(carCount);
        }
        //[HttpGet("GetAllCarsWithParts")]
        //public async Task<IActionResult> GetAllCarsWithParts()
        //{
        //    var carsWithParts = await _carRepository.GetCarsWithPartNames();

        //    if (carsWithParts == null || !carsWithParts.Any())
        //    {
        //        return NotFound("Araç veya arızalı parça bulunamadı.");
        //    }

        //    return Ok(carsWithParts);  // JSON formatında tüm araçları ve arızalı parçaları döndürüyoruz
        //}
        [HttpGet("cars/getAllIssues")]
        public async Task<IActionResult> GetAllIssues()
        {
            var Issues = await _carRepository.GetAllIssues();
            if (Issues == null || !Issues.Any()) { return NotFound("Kayıtlı arıza parçası bulunamadı"); }
            return Ok(Issues);
        }

    }
}
