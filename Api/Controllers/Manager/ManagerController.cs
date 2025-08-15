using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using System.Security.Claims;

namespace LeaveRequestSystem.Api.Controllers
{
    [ApiController]
    [Route("api/manager")]
    [Authorize(Roles = "MANAGER")]
    public class ManagerController : ControllerBase
    {
        private readonly ByManagerService _byManager;

        public ManagerController(ByManagerService byManager)
        {
            _byManager = byManager;
        }

        [HttpPost("approve/{leaveId:int}")]
        public async Task<IActionResult> Approve(int leaveId, [FromBody] DecisionDto dto)
        {
            var managerIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(managerIdStr)) return Unauthorized();
            var managerId = int.Parse(managerIdStr);

            var res = await _byManager.ApproveByManagerAsync(leaveId, managerId, dto?.Reason);
            return Ok(res);
        }

        [HttpPost("reject/{leaveId:int}")]
        public async Task<IActionResult> Reject(int leaveId, [FromBody] DecisionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Reason))
                return BadRequest(new { message = "Reason is required to reject" });

            var managerIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(managerIdStr)) return Unauthorized();
            var managerId = int.Parse(managerIdStr);

            var res = await _byManager.RejectByManagerAsync(leaveId, managerId, dto.Reason!);
            return Ok(res);
        }
    }
}
