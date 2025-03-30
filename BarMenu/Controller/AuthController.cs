using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    // _secretKey'i 256-bit'e çıkarıyoruz
    private readonly string _secretKey = "your-256-bit-long-secret-key-that-you-need-to-ensure-is-256-bits-long";

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] User loginUser)
    {
        var user = await _userRepository.GetUserByName(loginUser.Name);

        if (user == null || user.Password != loginUser.Password)
        {
            return Unauthorized("Geçersiz kullanıcı adı veya şifre");
        }

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, "User")
        };

        // Şimdi, secret key'i doğru uzunlukta (256-bit) kullanıyoruz
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "yourIssuer", // İssuer bilgisi
            audience: "yourAudience", // Audience bilgisi
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
    