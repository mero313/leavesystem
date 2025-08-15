using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using System.Security.Claims;

namespace LeaveRequestSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly LeaveRequestService _service;

        public LeaveRequestController(LeaveRequestService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveRequestRequestDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new { message = "No user in token" });

      
            var userId = int.Parse(userIdStr);
            var res = await _service.CreateLeaveRequestAsync(dto, userId);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> My()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new { message = "No user in token" });

            var userId = int.Parse(userIdStr);
            var res = await _service.GetRequestsForUserAsync(userId);
            return Ok(res);
        }
    }
}
