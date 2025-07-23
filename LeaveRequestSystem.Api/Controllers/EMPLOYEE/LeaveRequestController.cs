using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Data;
using LeaveRequestSystem.Models;
using LeaveRequestSystem.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;


namespace LeaveRequestSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class LeaveRequestController : ControllerBase
    {
        private readonly AppData _db;

        public LeaveRequestController(AppData db)
        {
            _db = db;
        }

        // GET: api/LeaveRequest/my-requests
        // Employee gets their own leave requests
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var userId = GetCurrentUserId();
            
            var requests = await _db.LeaveRequests
                .Where(lr => lr.UserId == userId)
                .Include(lr => lr.User)
                .Include(lr => lr.ApprovedByManager)
                .Include(lr => lr.ApprovedByHR)
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestResponse
                {
                    Id = lr.Id,
                    FromDate = lr.FromDate,
                    ToDate = lr.ToDate,
                    LeaveType = lr.LeaveType,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    CreatedAt = lr.CreatedAt,
                    UpdatedAt = lr.UpdatedAt,
                    ManagerComments = lr.ManagerComments,
                    HRComments = lr.HRComments,
                    ManagerApprovalDate = lr.ManagerApprovalDate,
                    HRApprovalDate = lr.HRApprovalDate,
                    ApprovedByManagerName = lr.ApprovedByManager != null ? lr.ApprovedByManager.Name : null,
                    ApprovedByHRName = lr.ApprovedByHR != null ? lr.ApprovedByHR.Name : null
                })
                .ToListAsync();

            return Ok(requests);
        }

        // POST: api/LeaveRequest
        // Employee creates a new leave request
        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();

            // Validate dates
            if (dto.FromDate >= dto.ToDate)
                return BadRequest("From date must be before to date.");

            if (dto.FromDate <= DateTime.Now.Date)
                return BadRequest("Leave request must be for future dates.");

            // Get current user to check department
            var currentUser = await _db.Users.FindAsync(userId);
            if (currentUser == null)
                return BadRequest("User not found.");

            // Check leave settings for this department and leave type
            var leaveSettings = await _db.LeaveSettings
                .FirstOrDefaultAsync(ls => ls.Department == currentUser.Department && 
                                         ls.LeaveType == dto.LeaveType);

            if (leaveSettings != null)
            {
                // Calculate requested days
                var requestedDays = (dto.ToDate - dto.FromDate).Days + 1;

                // Validate against settings
                if (requestedDays < leaveSettings.MinDaysPerRequest)
                    return BadRequest($"Minimum days for {dto.LeaveType} is {leaveSettings.MinDaysPerRequest}.");

                if (requestedDays > leaveSettings.MaxDaysPerRequest)
                    return BadRequest($"Maximum days for {dto.LeaveType} is {leaveSettings.MaxDaysPerRequest}.");

                // Check annual limit
                var currentYearRequests = await _db.LeaveRequests
                    .Where(lr => lr.UserId == userId && 
                               lr.LeaveType == dto.LeaveType &&
                               lr.FromDate.Year == DateTime.Now.Year &&
                               (lr.Status == "Approved" || lr.Status == "Pending"))
                    .SumAsync(lr => (lr.ToDate - lr.FromDate).Days + 1);

                if (currentYearRequests + requestedDays > leaveSettings.MaxDaysPerYear)
                    return BadRequest($"Annual limit for {dto.LeaveType} is {leaveSettings.MaxDaysPerYear} days. You have {leaveSettings.MaxDaysPerYear - currentYearRequests} days remaining.");
            }

            // Create the leave request
            var leaveRequest = new LeaveRequest
            {
                UserId = userId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                LeaveType = dto.LeaveType,
                Reason = dto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.LeaveRequests.Add(leaveRequest);
            await _db.SaveChangesAsync();

            // Create history entry
            var history = new LeaveRequestHistory
            {
                LeaveRequestId = leaveRequest.Id,
                FromStatus = "",
                ToStatus = "Pending",
                ActionByUserId = userId,
                ActionDate = DateTime.UtcNow,
                Comments = "Leave request created"
            };

            _db.LeaveRequestHistories.Add(history);
            await _db.SaveChangesAsync();

            // Return the created request
            var response = new LeaveRequestResponse
            {
                Id = leaveRequest.Id,
                FromDate = leaveRequest.FromDate,
                ToDate = leaveRequest.ToDate,
                LeaveType = leaveRequest.LeaveType,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                CreatedAt = leaveRequest.CreatedAt
            };

            return CreatedAtAction(nameof(GetLeaveRequestById), new { id = leaveRequest.Id }, response);
        }

        // GET: api/LeaveRequest/{id}
        // Get specific leave request details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(int id)
        {
            var userId = GetCurrentUserId();
            
            var request = await _db.LeaveRequests
                .Include(lr => lr.User)
                .Include(lr => lr.ApprovedByManager)
                .Include(lr => lr.ApprovedByHR)
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.UserId == userId);

            if (request == null)
                return NotFound("Leave request not found.");

            var response = new LeaveRequestResponse
            {
                Id = request.Id,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                LeaveType = request.LeaveType,
                Reason = request.Reason,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                ManagerComments = request.ManagerComments,
                HRComments = request.HRComments,
                ManagerApprovalDate = request.ManagerApprovalDate,
                HRApprovalDate = request.HRApprovalDate,
                ApprovedByManagerName = request.ApprovedByManager?.Name,
                ApprovedByHRName = request.ApprovedByHR?.Name
            };

            return Ok(response);
        }

        // PUT: api/LeaveRequest/{id}
        // Employee can update their pending leave request
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(int id, [FromBody] UpdateLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            
            var request = await _db.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.UserId == userId);

            if (request == null)
                return NotFound("Leave request not found.");

            // Only allow updates if status is Pending
            if (request.Status != "Pending")
                return BadRequest("Can only update pending leave requests.");

            // Validate dates
            if (dto.FromDate >= dto.ToDate)
                return BadRequest("From date must be before to date.");

            if (dto.FromDate <= DateTime.Now.Date)
                return BadRequest("Leave request must be for future dates.");

            var oldStatus = request.Status;

            // Update the request
            request.FromDate = dto.FromDate;
            request.ToDate = dto.ToDate;
            request.LeaveType = dto.LeaveType;
            request.Reason = dto.Reason;
            request.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Create history entry
            var history = new LeaveRequestHistory
            {
                LeaveRequestId = request.Id,
                FromStatus = oldStatus,
                ToStatus = request.Status,
                ActionByUserId = userId,
                ActionDate = DateTime.UtcNow,
                Comments = "Leave request updated"
            };

            _db.LeaveRequestHistories.Add(history);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Leave request updated successfully." });
        }

        // DELETE: api/LeaveRequest/{id}
        // Employee can cancel their pending leave request
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelLeaveRequest(int id)
        {
            var userId = GetCurrentUserId();
            
            var request = await _db.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.UserId == userId);

            if (request == null)
                return NotFound("Leave request not found.");

            // Only allow cancellation if status is Pending
            if (request.Status != "Pending")
                return BadRequest("Can only cancel pending leave requests.");

            var oldStatus = request.Status;
            request.Status = "Cancelled";
            request.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Create history entry
            var history = new LeaveRequestHistory
            {
                LeaveRequestId = request.Id,
                FromStatus = oldStatus,
                ToStatus = "Cancelled",
                ActionByUserId = userId,
                ActionDate = DateTime.UtcNow,
                Comments = "Leave request cancelled by employee"
            };

            _db.LeaveRequestHistories.Add(history);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Leave request cancelled successfully." });
        }

        // GET: api/LeaveRequest/{id}/history
        // Get history of a specific leave request
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetLeaveRequestHistory(int id)
        {
            var userId = GetCurrentUserId();
            
            // First check if the request belongs to the current user
            var request = await _db.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.UserId == userId);

            if (request == null)
                return NotFound("Leave request not found.");

            var history = await _db.LeaveRequestHistories
                .Where(h => h.LeaveRequestId == id)
                .Include(h => h.ActionByUser)
                .OrderBy(h => h.ActionDate)
                .Select(h => new LeaveRequestHistoryResponse
                {
                    Id = h.Id,
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ActionDate = h.ActionDate,
                    Comments = h.Comments,
                    ActionByUserName = h.ActionByUser.Name
                })
                .ToListAsync();

            return Ok(history);
        }

        // Helper method to get current user ID from JWT token
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");
            
            return int.Parse(userIdClaim.Value);
        }
    }
}