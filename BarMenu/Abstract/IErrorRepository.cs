using BarMenu.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;

namespace BarMenu.Abstract
{
    public interface IErrorRepository
    {
        Error GetErrorByName(string codeName); 
        Error AddError(Error error);

    }
}
