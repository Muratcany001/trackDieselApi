using BarMenu.Entities.AppEntities;

namespace BarMenu.Abstract
{
    public interface IPartRepository
    {
        Task<List<Part>> GetAllPartsAsync(string id);
        Task<Part> GetPartByIdAsync(int id);
        Task<Part> AddPartAsync(Part part);
        Task<Part> UpdatePartAsync(Part part);
        Task<Part> DeletePartAsync(int id);
        Task<Part> GetPartByName(string name);
        Task AddBulkPart(List<Part> parts);
    }
}
