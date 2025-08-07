using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using LeaveRequestSystem.Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LeaveRequestSystem.Api.Controllers.EMPLOYEE
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
        public async Task<IActionResult> Create([FromBody] LeaveRequestRequestDto dto )
        {
            // فرضاً تجيب userId من التوكن $أو ثابت مؤقتاً (نجربه)
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized(new { message = "User not authenticated" });
            var userId = int.Parse(userIdString);
            var response = await _service.CreateLeaveRequestAsync(dto, userId);
            return Ok (response);

        }

        // جلب طلبات الإجازة للمستخدم
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRequestsForUser(int userId)
        {
            var requests = await _service.GetRequestsForUserAsync(userId);
            return Ok(requests);
        }

        // أضف أي Endpoint تحتاجه: GetById, Delete, Update...
    }
}
