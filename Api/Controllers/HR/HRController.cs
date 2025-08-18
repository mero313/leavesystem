using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using System.Security.Claims;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Migrations;

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
        [HttpPost("{departmentId:int}/assign-manager")]
        public async Task<IActionResult> AssignManager(int departmentId,  int manageruserid, CancellationToken ct)
        {
            var result = await _departmentService.AssignManagerAsync(departmentId, manageruserid, ct);
            return Ok(result);
        }



        [HttpPost("to-manager")]
        public async Task<IActionResult> Manager([FromBody] PromotionManagerDto dto, CancellationToken ct)
        {

            await _userService.Promotion2Manager(dto.UserId, ct);
            return Ok(new { message = "Updated" });
        }

        [HttpPost("to-employee")]
        public async Task<IActionResult> Employee([FromBody] PromotionManagerDto  dto, bool removeFromDepartment, CancellationToken ct)
        {

            await _userService.Demote2Employee(dto.UserId, removeFromDepartment, ct);
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
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentRequestDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _departmentService.CreateAsync(dto, ct);
            return Ok(result);
        }


        [HttpGet("departments/{id:int}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var dep = await _departmentService.GetDepByIdAsync(id);
            return Ok(dep);
        }

        [HttpGet("department-stats")]
        public async Task<IActionResult> GetDepartmentStats(int id, bool users, CancellationToken ct)
        {
            var stats = await _departmentService.GetDepartmentWithStatsAsync(id, users, ct);
            return Ok(stats);
        }

        [HttpGet("department-list")]
        public async Task<List<DepartmentWithStatsDto>> ListDepartmentsWithCount(CancellationToken ct)
        {
            var departments = await _departmentService.ListDepartmentsWithCountsAsync(ct);
            return departments;
        }


        // add employee to dep
        [HttpPost("{departmentId:int}/employees")]
        public async Task<IActionResult> AddEmployee(bool moveIfInAnotherDept,
            int departmentId,
            [FromBody] AddEmployeeToDepartmentDto dto,
            CancellationToken ct)
        {
            await _departmentService.AddEmployeeAsync(departmentId, dto.UserId, moveIfInAnotherDept, ct);
            return Ok(new { success = true });
        }

        // إضافة مجموعة موظفين
        [HttpPost("{departmentId:int}/employees/bulk")]
        public async Task<IActionResult> AddEmployeesBulk(
            int departmentId,
            [FromBody] AddEmployeesToDepartmentDto dto,
            CancellationToken ct)
        {
            await _departmentService.AddEmployeesAsync(departmentId, dto, ct);
            return Ok(new { success = true, added = dto.UserIds.Distinct().Count() });
        }

        // (اختياري) إزالة موظف من القسم
        [HttpDelete("{departmentId:int}/employees/{userId:int}")]
        public async Task<IActionResult> RemoveEmployee(int departmentId, int userId, CancellationToken ct)
        {
            await _departmentService.RemoveEmployeeAsync(departmentId, userId, ct);
            return Ok(new { success = true });
        }
    }
}
