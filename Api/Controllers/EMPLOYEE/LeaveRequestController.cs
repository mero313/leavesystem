using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LeaveRequestSystem.Api.Controllers
{
    [Authorize(Roles = "EMPLOYEE")]
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly LeaveRequestService _leaveservice;

        public LeaveRequestController(LeaveRequestService service)
        {
            _leaveservice = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveRequestRequestDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new { message = "No user in token" });


            var userId = int.Parse(userIdStr);
            var res = await _leaveservice.CreateLeaveRequestAsync(dto, userId);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> My()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new { message = "No user in token" });

            var userId = int.Parse(userIdStr);
            var res = await _leaveservice.GetRequestsForUserAsync(userId);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("my/count")]
        
        public async Task<IActionResult> MyCount()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new { message = "No user in token" });

            var userId = int.Parse(userIdStr);
            var res = await _leaveservice.GetAllRequestsCountAsync(userId);
            return Ok(new { count = res });
        }
    }
}
