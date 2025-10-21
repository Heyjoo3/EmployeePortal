namespace EmployeePortal.Core.Dto
{
    public class SideNavDto
    {
        public string Department { get; set; }
        public IEnumerable<SideNavEmployeeDto> Employees { get; set; }
    }
}
