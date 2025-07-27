using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LeaveRequestSystem.Data;
using LeaveRequestSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Application.DTOs;  
using LeaveRequestSystem.Application.Mappers;
using LeaveRequestSystem.Entities;




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













    
}