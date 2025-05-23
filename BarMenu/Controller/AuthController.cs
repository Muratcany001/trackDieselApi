﻿using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

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

        // Kullanıcı bulunamazsa ya da şifre uyuşmazsa hata döndürüyoruz
        if (user == null || HashPasswordSHA256(loginUser.Password) != user.Password)
        {
            return Unauthorized("Geçersiz kullanıcı adı veya şifre");
        }

        // JWT token oluşturma
        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }
    private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
    [HttpPost("logout")]
    [Authorize] // Sadece giriş yapmış kullanıcılar çıkış yapabilir
    public IActionResult Logout()
    {
        
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) {
            return BadRequest("Geçersiz token");
        }
        var token = authHeader.Substring("Bearer ".Length).Trim();
        _blacklistedTokens.Add(token);
        return Ok(new { message = "Başarıyla çıkış yapıldı" });
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

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };

        // Şimdi, secret key'i doğru uzunlukta (256-bit) kullanıyoruz
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "your-issuer",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
