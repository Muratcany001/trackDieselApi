using BarMenu.Entities.AppEntities;

namespace BarMenu.Abstract
{
    public interface ICarRepository
    {
        Car AddCar (Car car);
        Task<Car> UpdateCar (Car car);
        Task<bool> DeleteCar (int id);
        Task<List<Car>> GetAllCars ();
        Task<Car> GetCarById (int id);
        Task<Car> GetCarByPlate (string plate);
        Task<int> GetCarCountAsync();
        Task<Car> GetCarWithIssuesAsync(int carId);
        //Task<List<Car>> GetCarsWithPartNames();
        Task<List<Issue>> GetAllIssues();
    }
}
