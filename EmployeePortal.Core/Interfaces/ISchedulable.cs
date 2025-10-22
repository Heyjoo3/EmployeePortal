using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Interfaces
{
    public interface ISchedulable
    {
        public Task<TimeSpan?> CalculateDuration();
    }
}
