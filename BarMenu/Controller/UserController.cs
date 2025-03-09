using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;

namespace BarMenu.Controller
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("users/register")]
        public ActionResult<User> Register(User user) {
            if (user == null)
            {
                return BadRequest("Geçersiz kullanıcı verisi");
                
               
            }
            var createdUser = _userRepository.CreateUser(user);
            return CreatedAtAction(nameof(Register), createdUser);
        }
        [HttpGet("users/{id}")]
        public ActionResult<User> Get(int id)
        {
            if (id == null) {
                return BadRequest("Geçersiz id girildi");
            }
            var User = _userRepository.GetUserById(id);
            return Ok(User);
        }
        [HttpPatch("users/updateUser/{id}")]
        public ActionResult<User> UpdateUser(int id, User updateUser) {
            var user = _userRepository.GetUserById(id);
            if (updateUser == null) { 
                return BadRequest("Geçersiz kullanıcı Id'si");
            }
            _userRepository.UpdateUser(id, updateUser);
            return Ok(user);
        }
        [HttpDelete("users/delete/{id}")]
        public ActionResult<User> DeleteUser(int id) {
            var user = _userRepository.GetUserById(id);
            if (user == null) { 
                return BadRequest("Kullanıcı id'si bulunamadı");
            }
            _userRepository.DeleteUser(id);
            return Ok(new {message = "Kullanıcı başarıyla silindi"});
        }
    }
}
