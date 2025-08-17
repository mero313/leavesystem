using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using System.Security.Claims;
using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Api.Controllers
{
    [ApiController]
    [Route("api/hr")]
    [Authorize(Roles = "HR")]
    public class HRController : ControllerBase
    {
        private readonly HRService _hrService;
        private readonly UserService _userService;
        private readonly DepartmentService _departmentService;

        public HRController(HRService hrService, UserService userService, DepartmentService departmentService)
        {
            _hrService = hrService;
            _userService = userService;
            _departmentService = departmentService;
        }

        [HttpPost("approve/{leaveId:int}")]
        public async Task<IActionResult> Approve(int leaveId, [FromBody] DecisionDto dto)
        {
            var hrIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hrIdStr)) return Unauthorized();
            var hrId = int.Parse(hrIdStr);

            var res = await _hrService.ApproveByHRAsync(leaveId, hrId, dto?.Reason);
            return Ok(res);
        }

        [HttpPost("reject/{leaveId:int}")]
        public async Task<IActionResult> Reject(int leaveId, [FromBody] DecisionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Reason))
                return BadRequest(new { message = "Reason is required to reject" });

            var hrIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hrIdStr)) return Unauthorized();
            var hrId = int.Parse(hrIdStr);

            var res = await _hrService.RejectByHRAsync(leaveId, hrId, dto.Reason!);
            return Ok(res);
        }

        [HttpPost("assign-manager")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerDto dto, CancellationToken ct)
        {

            await _userService.AssignManagerAsync(dto.UserId, dto.DepartmentId, dto.PromoteToManager);
            return Ok(new { message = "Updated" });
        }

        [HttpPost("toggle-active")]
        public async Task<IActionResult> ToggleActive([FromBody] ToggleActiveDto dto)
        {
            await _userService.ToggleActiveAsync(dto.UserId, dto.IsActive);
            return Ok(new { message = "Updated" });
        }

        [HttpGet("user-managment")]
        public async Task<IActionResult> UserManagement()
        {
            var allusers = await _userService.GetallUsersAsync();
            return Ok(new
            {
                message = "seccses",
                count = allusers.Count(),
                users = allusers
            });
        }

        [HttpPost("departments")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentRequestDto dto , CancellationToken ct )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _departmentService.AddDep(dto , ct);
            return Ok(result);
        }


        [HttpGet("departments/{id:int}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var dep = await _departmentService.GetDepByIdAsync(id);
            return Ok(dep);
        }
    }
}
