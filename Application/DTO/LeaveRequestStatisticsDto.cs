namespace LeaveRequestSystem.Application.DTOs
{
    public class LeaveRequestStatisticsDto
{
    public int Pending { get; set; }
    public int ManagerApproved { get; set; }
    public int HRApproved { get; set; }
    public int Rejected { get; set; }
}

}