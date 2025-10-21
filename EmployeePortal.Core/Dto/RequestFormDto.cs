namespace EmployeePortal.Core.Dto
{
    public class RequestFormDto
    {
        public Guid EmployeeId { get; set; }
        public string? Role { get; set; } = "noRole";
    }
}