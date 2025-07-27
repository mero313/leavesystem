using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            // فرضاً تجيب userId من التوكن أو ثابت مؤقتاً (نجربه)
            int userId = 1; // لازم تعدله بالواقع عال JWT
            var response = await _service.CreateLeaveRequestAsync(dto, userId);
            return Ok(response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRequestsForUser(int userId)
        {
            var requests = await _service.GetRequestsForUserAsync(userId);
            return Ok(requests);
        }

        // أضف أي Endpoint تحتاجه: GetById, Delete, Update...
    }
}
