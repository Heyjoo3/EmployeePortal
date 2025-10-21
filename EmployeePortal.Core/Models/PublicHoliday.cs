using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Core.Models
{
    public class PublicHoliday
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Type { get; set; }
    }
}
