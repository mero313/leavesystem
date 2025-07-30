using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;





namespace LeaveRequestSystem.Application.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {

            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Email", user.Email ?? string.Empty),
                new Claim("Department", user.Department ?? string.Empty),
            };

            var keyString = _config["Jwt:Key"] ?? throw new Exception("JWT Key not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiry
            };
        }
    }
}
