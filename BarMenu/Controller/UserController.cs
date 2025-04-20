using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace BarMenu.Controller
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("users/GetUserByName")]
        public async Task<ActionResult<User>> GetUserByName(string name)
        {
            var existedUser = await _userRepository.GetUserByName(name);
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
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        [HttpGet("users/getUser/{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            if (id == 0)
            {
                return BadRequest("Geçersiz id girildi");
            }
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }
            return Ok(user);
        }

        [HttpPatch("users/updateUser/{id}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User updateUser)
        {
            if (updateUser == null)
            {
                return BadRequest("Geçersiz kullanıcı verisi");
            }
            var user = _userRepository.UpdateUser(id, updateUser);
            return Ok(user);
        }

        [HttpDelete("users/delete/{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return BadRequest("Kullanıcı id'si bulunamadı");
            }
            _userRepository.DeleteUser(id);
            return Ok(new { message = "Kullanıcı başarıyla silindi" });
        }

        [HttpGet("users/getUser")]
        public ActionResult<List<User>> GetUser()
        {
            var users = _userRepository.GetAllUsers();
            return Ok(users);
        }
    }
}
