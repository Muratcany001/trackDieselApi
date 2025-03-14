﻿using BarMenu.Abstract;
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
        public ActionResult<Car> AddMenu(Car car)
        {
            {
                if (car == null)
                {
                    return BadRequest("Geçersiz menü verisi");
                }
                var AddCar = _carRepository.AddCar(car);
                return CreatedAtAction(nameof(AddCar), AddCar);
            }
        }
        [HttpGet("cars/GetAll")]
        public ActionResult<Car>  GetAllCar() {

            var cars = _carRepository.GetAllCars();
            return Ok(cars);
        }
        [HttpGet("cars/GetCarByPlate/{plate}")]
        public ActionResult<Car> GetCarByPlate(string plate) { 
            var car = _carRepository.GetCarByPlate(plate);
            if (car == null)
            {
                return NotFound("Araç bulunamadı");
            }
            return Ok(car);
        }
        [HttpPatch("cars/UpdateCar/{id}")]
        public async Task<ActionResult<Car>> UpdateMenu(int id, [FromBody] Car updateCar) { 
            var existingCar = await _carRepository.GetCarById(id);
            if (existingCar == null)
            {
                return NotFound("Menü bulunamadı");
            }
            existingCar.Name = updateCar.Name ?? existingCar.Name;  // Name varsa güncellenir
            existingCar.Model = updateCar.Model ?? existingCar.Model;  // Model varsa güncellenir
            existingCar.Plate = updateCar.Plate ?? existingCar.Plate;  // Plate varsa güncellenir
            existingCar.EngineType = updateCar.EngineType ?? existingCar.EngineType;  // EngineType varsa güncellenir
            existingCar.ErrorHistory = updateCar.ErrorHistory ?? existingCar.ErrorHistory;  // ErrorHistory varsa güncellenir
            existingCar.LastMaintenanceDate = updateCar.LastMaintenanceDate == default(DateTime) ? existingCar.LastMaintenanceDate : updateCar.LastMaintenanceDate;  // LastMaintenanceDate varsa güncellenir
            existingCar.ErrorDescription = updateCar.ErrorDescription ?? existingCar.ErrorDescription;  // ErrorDescription varsa güncellenir
            existingCar.PartsReplaced = updateCar.PartsReplaced ?? existingCar.PartsReplaced;  // PartsReplaced varsa güncellenir
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
    }
}
