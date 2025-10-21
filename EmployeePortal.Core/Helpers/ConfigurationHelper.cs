using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;

namespace EmployeePortal.Core.Helpers
{
    public class ConfigurationHelper
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }
    }
}
