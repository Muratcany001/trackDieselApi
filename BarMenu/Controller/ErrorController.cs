using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;

namespace BarMenu.Controller
{
    public class ErrorController : ControllerBase
    {
        private readonly IErrorRepository _errorRepository;
        public ErrorController(IErrorRepository errorRepository)
        {
            _errorRepository = errorRepository;
        }
        [HttpGet("errors/GetErrorByName")]
        public ActionResult GetErrorByName(string errorName)
        {
            var existedError = _errorRepository.GetErrorByName(errorName);

            if (existedError == null)
            {
                // Hata kodu bulunamadıysa 404 Not Found döndürüyoruz
                return NotFound($"Arıza kodu {errorName} bulunamadı");
            }

            // Hata kodu bulunduysa, 200 OK yanıtı döndürüyoruz
            return Ok(existedError);
        }

        [HttpPost("errors/AddError")]
        public ActionResult AddError(Error error) { 
        
            var existedError = _errorRepository.GetErrorByName(error.Code);
            if (existedError == null)
            {
                return Conflict("Arıza kodu zaten mevcut");
            }
            var newError = _errorRepository.AddError(error);
            return Ok(newError);
        }
    }
}
