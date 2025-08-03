using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LeaveRequestSystem.Infrastructure.Data;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;




namespace LeaveRequestSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService authRepository;

    public AuthController(AuthService authRepository)
    {
        this.authRepository = authRepository;
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var response = await authRepository.Login(dto);
        return Ok(response); // يطلع الـ token والمعلومات كلها بشكل صحيح
    }




    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var user = await authRepository.Register(dto);
        return Ok(new
        {
            message = "User registered successfully",
            user = user.Id,
            username = user.Username,
            Role = user.Role.ToString(),
            

    });
      

    }









}