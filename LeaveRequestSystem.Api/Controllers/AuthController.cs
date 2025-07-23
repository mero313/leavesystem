using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LeaveRequestSystem.Data;
using LeaveRequestSystem.Models;
using LeaveRequestSystem.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;



namespace LeaveRequestSystem.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppData _db;
    private readonly PasswordHasher<User> hasher;
    private readonly string _jwtKey;
    public AuthController(IConfiguration config, AppData db)
    {
        _db = db;
        hasher = new PasswordHasher<User>();
        _jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
        
    }

   
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
        return BadRequest(ModelState);

        // 2️⃣ تأكد ماكو يوزر بنفس الاسم
        if (_db.Users.Any(u => u.Username == req.Username))
            return Conflict("Username already exists.");

        if (!string.IsNullOrEmpty(req.Email) && _db.Users.Any(u => u.Email == req.Email))
            return Conflict("Email already exists.");

        // 3️⃣ جهّز كائن User جديد
        var user = new User
        {
            Username = req.Username,
            Role = req.Role,
            Name = req.Name,
            Email = string.IsNullOrEmpty(req.Email) ? null : req.Email,
            Department = req.Department,
            CreatedAt = DateTime.UtcNow,


        };

        // 4️⃣ شفر كلمة المرور
        user.PasswordHash = hasher.HashPassword(user, req.Password);


        // 5️⃣ خزّن بالكاش وفّر التغييرات
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // 6️⃣ رجّع 201 Created بدون باسورد
        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            }
        );
    }


    [Authorize]
    // 1️⃣ جيب اليوزر من الداتا
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // 1️⃣ جيب اليوزر من الداتا
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found.");

        // 2️⃣ رجّع اليوزر بدون باسورد
        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {

        if(!ModelState.IsValid)
            return BadRequest(ModelState);



        // 1️⃣ جيب اليوزر من الداتا
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        // 2️⃣ شيك على الباسورد
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password.");

        var claims = new[]
   {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)); // use the same as in Program.cs
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            user = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            }
        });
    }


    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        // 1️⃣ جيب كل اليوزرز من الداتا
        var users = await _db.Users.ToListAsync();

        // 2️⃣ رجّعهم بدون باسورد

        var userResponses = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role
        }).ToList();

        return Ok(userResponses);
    }
}
