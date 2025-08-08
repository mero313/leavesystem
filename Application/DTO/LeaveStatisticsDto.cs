 public class LeaveStatisticsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ManagerApproved { get; set; }
        public int HRApproved { get; set; }
        public int Rejected { get; set; }
        public int Cancelled { get; set; }
    }