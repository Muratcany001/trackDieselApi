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
            // Veritabanında aynı "Code" değerine sahip bir hata var mı diye kontrol ediyoruz
            var existedError = _context.Errors.FirstOrDefault(x => x.Code == error.Code);

            if (existedError != null)
            {
                // Eğer böyle bir hata kodu varsa, kullanıcıya mesaj veriyoruz
                throw new Exception("Arıza kodu zaten mevcut");
            }

            // Yeni hatayı veritabanına ekliyoruz
            var newError = new Error { Code = error.Code, Description = error.Description };
            _context.Errors.Add(newError);
            _context.SaveChanges();

            return newError;
        }


        public Error GetErrorByName(string errorName)
        {
            // Veritabanından hata kodunu arıyoruz
            var existedError = _context.Errors.FirstOrDefault(x => x.Code == errorName);

            // Hata bulamazsak, null döndürüyoruz.
            return existedError;
        }


    }
}
