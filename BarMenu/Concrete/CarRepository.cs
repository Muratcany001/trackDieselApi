﻿using BarMenu.Abstract;
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

       public Car AddCar(Car car)
        {
            foreach (var issue in car.ErrorHistory) {
                issue.Car = null;
                issue.CarId = 0;
            }
            _context.Cars.Add(car);
            _context.SaveChanges();
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
        public async Task<Car> GetCarWithIssuesAsync(int carId)
        {
            return await _context.Cars
                .Where(car => car.Id == carId)
                .Include(car => car.ErrorHistory) 
                .FirstOrDefaultAsync();
        }
        public async Task<List<Car>> GetCarsWithPartNames()
        {
            var carsWithIssues = await _context.Cars
                .Where(car => car.ErrorHistory.Any()) // Arızalı parçası olan araçları filtreliyoruz
                .Select(car => new
                {
                    car.Name,
                    car.Model,
                    car.Age,
                    Issues = car.ErrorHistory.Select(issue => new
                    {
                        issue.PartName,  // Sadece PartName alıyoruz
                        issue.DateReported,
                        issue.IsReplaced
                    }).ToList()
                })
                .ToListAsync();

            // Veriyi dönüşümünü yapıyoruz
            var result = carsWithIssues.Select(car => new Car
            {
                Name = car.Name,
                Model = car.Model,
                Age = car.Age,
                ErrorHistory = car.Issues.Select(issue => new Issue
                {
                    PartName = issue.PartName,  // Arızalı parçaların adlarını ErrorHistory'ye ekliyoruz
                    DateReported = issue.DateReported,
                    IsReplaced = issue.IsReplaced
                }).ToList()
            }).ToList();

            return result;
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
            existingCar.LastMaintenanceDate = car.LastMaintenanceDate;
            existingCar.Plate = car.Plate;
            _context.Cars.Update(existingCar);
            await _context.SaveChangesAsync();
            return existingCar;
        }
        public async Task<bool> DeleteCar(int id)
        {
            var existedCar = _context.Cars.FirstOrDefault(x => x.Id == id);
            if (existedCar != null)
            {
                _context.Cars.Remove(existedCar);
                await _context.SaveChangesAsync();
            }
            return true;
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
