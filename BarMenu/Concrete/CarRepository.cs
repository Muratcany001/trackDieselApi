using BarMenu.Abstract;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BarMenu.Concrete
{
    public class CarRepository : ICarRepository
    {
        private readonly Context _context;
        public CarRepository(Context context) { 
        _context = context;
        }

       public async Task<Car> AddCar(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChangesAsync();
            return car;
        }
        public async Task<List<Car>> GetAllCars()
        {
            return await _context.Cars.ToListAsync();
        }
        public async Task<Car> GetCarById(int id)
        {
            var menu = _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            return await menu;

        }
        public async Task<Car> UpdateCar(Car car)
        {
            var existingCar = _context.Cars.FirstOrDefault(m => m.Id == car.Id);
            if (existingCar == null) {
                throw new Exception("Araç bulunamadı");
            }
            existingCar.Id = car.Id;
            existingCar.Name = car.Name;
            existingCar.ErrorHistory = car.ErrorHistory;
            existingCar.ErrorDescription = car.ErrorDescription;
            existingCar.LastMaintenanceDate = car.LastMaintenanceDate;
            existingCar.PartsReplaced = car.PartsReplaced;
            existingCar.Plate = car.Plate;
            _context.Cars.Update(existingCar);
            await _context.SaveChangesAsync();
            return existingCar;
        }
        public async Task<Car> DeleteCar(int id)
        {
            var existedCar = _context.Cars.FirstOrDefault(x => x.Id == id);
            if (existedCar != null)
            {
                _context.Cars.Remove(existedCar);
                await _context.SaveChangesAsync();
            }
            return existedCar;
        }
        public async Task<List<Car>> GetCarsByPlate(string plate)
        {
            var cars = await _context.Cars.Where(x => x.Plate == plate).ToListAsync();
            return cars;
        }
        public async Task<int> GetCarCountAsync()
        {
            return await _context.Cars.CountAsync();
        }
        public async Task<Car> GetCarByPlate(string plate)
        {
            var existedCar = await _context.Cars.FirstOrDefaultAsync(x => x.Plate == plate);
            if (existedCar == null) {
                throw new Exception("Belirtilen plakaya ait araç bulunamadı");
            }
            return existedCar;
        }

    }
}
