using BarMenu.Abstract;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BarMenu.Concrete
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly Context _context;
        public ErrorRepository(Context context)
        {
            _context = context;
        }
        public Error AddError(Error error)
        {
            var newError = new Error { Code = error.Code, Description = error.Description };
            _context.Errors.Add(newError);
            _context.SaveChanges();
            return newError;
        }
        public Error GetErrorByName(string errorName)
        {
            var existedError = _context.Errors.FirstOrDefault(x => x.Code == errorName);
            return existedError;
        }


    }
}
