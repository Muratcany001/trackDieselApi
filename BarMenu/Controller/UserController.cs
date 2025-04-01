using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

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
        [HttpGet("users/GetUserByName")]
        public ActionResult<User> GetUserByName(string name)
        {
            var existedUser = _userRepository.GetUserByName(name);
            if (existedUser == null)
            {
                return NotFound("Kullanıcı adı bulunamadı");
            }
            return Ok(existedUser);
        }

        [HttpPost("users/register")]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Geçersiz kullanıcı verisi");
            }
            if (string.IsNullOrEmpty(user.Name))
            {
                return BadRequest("Kullanıcı adı boş olamaz");
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Şifre boş olamaz");
            }
            var existedUser = await _userRepository.GetUserByName(user.Name);
            if (existedUser != null)
            {
                return BadRequest("Kullanıcı adı zaten mevcut");
            }

            // Şifreyi SHA256 ile hash'leme
            user.Password = HashPasswordSHA256(user.Password);

            var createdUser = await _userRepository.CreateUser(user);
            return CreatedAtAction(nameof(GetUserByName), new { name = createdUser.Name }, createdUser);
        }

        private string HashPasswordSHA256(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Şifreyi byte dizisine çeviriyoruz
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash'li şifreyi oluşturuyoruz
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Hash'li şifreyi Base64 string olarak geri döndürüyoruz
                return Convert.ToBase64String(hashBytes);
            }
        }

        [HttpGet("users/getUser/{id}")]
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
        [HttpGet("users/getUser")]
        public ActionResult<User> GetUser()
        {
            var users = _userRepository.GetAllUsers().ToList();
            return Ok(users);
        }
    }
}
