namespace EmployeePortal.Core.Helpers
{
    public class BaseResult
    {
        public bool IsSuccessfull { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
