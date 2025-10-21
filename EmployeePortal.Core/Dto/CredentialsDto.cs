using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class CredentialsDto
    {
        public string EmployeeId { get; set; }
    
        public string UserName { get; set; }
  
        public string? PasswordOld { get; set; }
   
        public string? PasswordNew { get; set; }
    }
}
