using BarMenu.Abstract;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;

namespace BarMenu.Concrete
{
    public class PartRepository : IPartRepository
    {
        public readonly Context _context;
        public PartRepository(Context context)
        {
            _context = context;
        }

        public async Task<Part> AddPartAsync(Part part)
        {
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task<Part> DeletePartAsync(int id)
        {
            var existedPart =await _context.Parts.FirstOrDefaultAsync(x => x.Id == id);
            _context.Parts.Remove(existedPart);
            await _context.SaveChangesAsync();
            return existedPart;
        }

        public async Task<List<Part>> GetAllPartsAsync(string userId)
        {
            return await _context.Parts.Where(c=> c.UserId == userId).ToListAsync();
        }
        public async Task<Part> GetPartByIdAsync(int id)
        {
            var existedPart = await _context.Parts.FindAsync(id);
            return existedPart;
        }
        public async Task<Part> GetPartByName(string name)
        {
            return await _context.Parts
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.Name, name));
        }
        public async Task AddBulkPart(List<Part> parts)
        {
            await _context.Parts.AddRangeAsync(parts);
            await _context.SaveChangesAsync();
        }
        public async Task<Part> UpdateStockAsync(Part part)
        {
            var existedPart =await _context.Parts.FirstOrDefaultAsync(x => x.Id == part.Id);
            existedPart.count = part.count;
            _context.Update(existedPart);   
            await _context.SaveChangesAsync();
            return existedPart;
        }
    }
}
