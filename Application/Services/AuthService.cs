using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LeaveRequestSystem.Application.Mappers;
using LeaveRequestSystem.Domain.Enums;






namespace LeaveRequestSystem.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository UserRepository;
        private readonly IConfiguration config;

        public AuthService(IUserRepository _UserRepository, IConfiguration _config)
        {
            UserRepository = _UserRepository;
            config = _config;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {

            var user = await UserRepository.GetByUsernameAsync(dto.Username);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Email", user.Email ?? string.Empty),
                new Claim("IsActive", user.IsActive.ToString()),
                new Claim("ManagerId", user.ManagerId?.ToString() ?? string.Empty)
                
            };
            if (user.DepartmentId.HasValue)
                claims.Add(new Claim("DepartmentId", user.DepartmentId.Value.ToString()));

            if (!string.IsNullOrWhiteSpace(user.Department?.Name))
                claims.Add(new Claim("DepartmentName", user.Department!.Name));
            Console.WriteLine($"ManagerId عند تسجيل الدخول: {user.ManagerId}");


            var keyString = config["Jwt:Key"] ?? throw new Exception("JWT Key not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return AuthMapper.ToLoginResponseDto(user, tokenString);

        }

        public async Task<UserDto> Register(RegisterRequestDto dto)
        {

            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            {
                throw new ArgumentException("Username and password are required");
            }
            var existingUser = await UserRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                throw new Exception("Username already exists");

            }

            var email = dto.Email?.ToUpper() ?? "";

            if (email.EndsWith("@MANAGER"))
                dto.Role = Role.MANAGER;
            else if (email.EndsWith("@HR"))
                dto.Role = Role.HR;
            else
                dto.Role = Role.EMPLOYEE;

            var user = AuthMapper.ToUserEntity(dto);
            await UserRepository.AddAsync(user);


            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,

            };

        }
    }
}
