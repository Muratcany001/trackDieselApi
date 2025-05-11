using BarMenu.Abstract;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
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
            return await _context.Cars.Include(c=> c.ErrorHistory).ToListAsync();
        }
        public async Task<Car> GetCarById(int id)
        {
            var menu = await _context.Cars.Include(x => x.ErrorHistory).FirstOrDefaultAsync(m => m.Id == id);
            return  menu;

        }
        public async Task<Car> GetCarWithIssuesAsync(int carId)
        {
            return await _context.Cars
                .Where(car => car.Id == carId)
                .Include(car => car.ErrorHistory) 
                .FirstOrDefaultAsync();
        }
        public async Task<List<Issue>> GetAllIssues()
        {
                    return await _context.Issues
               .Select(i => new Issue
               {
                   Id = i.Id,
                   Model = i.Model,
                   EngineType = i.EngineType,
                   PartName = i.PartName,
                   Description = i.Description,
                   DateReported = i.DateReported,
                   IsReplaced = i.IsReplaced,
                   CarId = i.CarId
                            })
                           .ToListAsync();
         }
        public async Task<Car> UpdateCar(string plate, List<Issue> updatedIssues)
        {
            var existingCar = await _context.Cars
                .Include(c => c.ErrorHistory)
                .FirstOrDefaultAsync(c => c.Plate == plate);

            if (existingCar == null)
                throw new Exception("Araç bulunamadı");
            foreach (var updatedIssue in updatedIssues)
            {
                if (updatedIssue.Id == 0)
                {
                    updatedIssue.CarId = existingCar.Id;
                    updatedIssue.DateReported = DateTime.Now;
                    existingCar.ErrorHistory.Add(updatedIssue);
                }
                else
                {
                    var existingIssue = existingCar.ErrorHistory
                        .FirstOrDefault(i => i.Id == updatedIssue.Id);

                    if (existingIssue != null)
                    {
                        existingIssue.PartName = updatedIssue.PartName ?? existingIssue.PartName;
                        existingIssue.Description = updatedIssue.Description ?? existingIssue.Description;
                        existingIssue.IsReplaced = updatedIssue.IsReplaced;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return existingCar;
        }
        public async Task<bool> DeleteCar(string plate)
        {
            var existedCar = _context.Cars.Include(x=> x.ErrorHistory).FirstOrDefault(x => x.Plate == plate);
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
            var existedCar = await _context.Cars.Include(x=> x.ErrorHistory).FirstOrDefaultAsync(x => x.Plate == plate);
            
            return existedCar;
        }
        public async Task<List<dynamic>> GetModelsWithBrokenParts()
        {
            return await _context.Issues
                .Include(i => i.Car)
                .GroupBy(i => new {
                    i.Model,
                    i.EngineType,
                    i.PartName,
                    i.DateReported,
                    UserId = i.Car.UserId
                })
                .Select(g => new
                {
                    g.Key.Model,
                    g.Key.EngineType,
                    g.Key.PartName,
                    g.Key.DateReported,
                    Count = g.Count(),
                    g.Key.UserId
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync<dynamic>();
        }
        public async Task<List<dynamic>> MostCommonProblems()
        {
            return await _context.Issues
                .Include (i => i.Car)
                .GroupBy (i => new
                {
                    i.Description,
                    i.DateReported,
                    UserId = i.Car.UserId
                })
                .Select(g => new
                {
                    g.Key.Description,
                    g.Key.DateReported,
                    g.Key.UserId,
                    Count = g.Count(),
                })
                .OrderByDescending(x=> x.Count)
                .ToListAsync<dynamic>();
        }
    }
}