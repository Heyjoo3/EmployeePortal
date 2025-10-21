    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Core.Dto;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Serilog;
using EmployeePortal.Core.Services;
using EmployeePortal.Core.Helpers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using EmployeePortal.Core.Models;
using Microsoft.Data.SqlClient.Server;

namespace EmployeePortal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;

        public EmployeeController(IEmployeeService employeeService, IConfiguration configuration, IRoleService roleService)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            _roleService = roleService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login(IFormCollection logindata)
        {
            if (logindata.TryGetValue("loginData", out var someString))
            {
                var employeeDto = JsonConvert.DeserializeObject<EmployeeDto>(someString);
                var result = await _employeeService.Authenticateemployee(employeeDto);

                if (result.IsSuccessfull)
                {

                    var token = GenerateJwtToken(employeeDto.UserName);

                    return Ok(new { Token = token, Data = result.Data, IsSuccessfull = true, });
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("CreateEmployee")]
        public async Task<ActionResult> CreateEmployee(IFormCollection employeeFormData)
        {
            if (employeeFormData.TryGetValue("employeeFormData", out var someString))
            {
                var employeeCreateDto = JsonConvert.DeserializeObject<EmployeeDto>(someString);
                var result = await _employeeService.CreateEmployeeAsync(employeeCreateDto);
                if (result.Succeeded)
                {
                    return Ok(new BaseResult { Data = employeeCreateDto.Id, IsSuccessfull = true, });
                }
                else
                {
                    return Ok(new BaseResult { IsSuccessfull = false });

                }
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("UpdateCredentials")]
        public async Task<ActionResult> UpdateCredentials(IFormCollection credentialsForm)
        {
            if (credentialsForm.TryGetValue("credentialsForm", out var someString))
            {
                    var credentialsDto = JsonConvert.DeserializeObject<CredentialsDto>(someString);
                var result = await _employeeService.UpdateCredentialsAsync(credentialsDto);

                    if (result.IsSuccessfull)
                    {

                        var token = GenerateJwtToken(credentialsDto.UserName);

                        return Ok(new { Token = token, Data = result.Data, IsSuccessfull = true, });
                    }
                    else
                    {
                        return Ok(result);
                    }
            }
            else
            { 
                return BadRequest(ModelState.IsValid); 
            }    
        }

        [HttpPost("UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee(IFormCollection employeeFormData)
        {
            if (employeeFormData.TryGetValue("employeeFormData", out var someString))
            {
                employeeFormData.TryGetValue("forcePasswordUpdate", out var booleanString);
                bool.TryParse(booleanString, out var forcePasswordUpdate);

                var employeeUpdateDto = JsonConvert.DeserializeObject<EmployeeUpdateDto>(someString);

                var result = await _employeeService.UpdateEmployeeAsync(employeeUpdateDto, forcePasswordUpdate);
                if (result.Succeeded)
                {
                    return Ok(new BaseResult { Data = employeeUpdateDto, IsSuccessfull = true, });
                }
                else
                {
                    return Ok(new BaseResult { IsSuccessfull = false });

                }
            }
            else
                { return BadRequest(ModelState.IsValid); }
        }

        [HttpPost("GetEmployeeById")]
        public async Task<ActionResult> GetEmployeeById(IFormCollection formData)
        {
            if (formData.TryGetValue("employeeID", out var guidString))
            {
                var guid = JsonConvert.DeserializeObject(guidString).ToString();
                var employee = await _employeeService.GetEmployeeByIdAsync(guid);
                return Ok(new BaseResult { Data = employee, IsSuccessfull = true });
            }
            else
                return BadRequest(ModelState.IsValid);

        }

        [HttpPost("GetAllEmployees")]
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployees()
        {
            return await _employeeService.GetAllEmployees();
        }


        [HttpPost("GetAllEmployeesReduced")]
        public async Task<ActionResult> GetAllEmployeesReduced()
        {
            try
            {
               var employees = await _employeeService.GetAllEmployeesReduced();
                return Ok(new BaseResult { Data = employees, IsSuccessfull = true});
            }
            catch
            {
                return BadRequest(ModelState.IsValid);
            }
        }

        [HttpGet("GetEmployeeRoles")]
        public async Task<IEnumerable<string>> GetEmployeeRoles()
        {
            return await _roleService.GetAllRoles();

        }


        private string GenerateJwtToken(string username)
        {
            var appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
            var secretkey = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretkey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        [HttpPost("RenewJwtToken")]
        public async Task<ActionResult> RenewJwtToken(IFormCollection usernameData)
        {
            usernameData.TryGetValue("Username", out var usernameString);
            usernameData.TryGetValue("Token", out var token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
            var secretKey = Encoding.ASCII.GetBytes(appSettings.Secret);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false, 
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false 
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken)
                    throw new SecurityTokenException("Invalid token");

                var username = principal.FindFirst(ClaimTypes.Name)?.Value;
                if (username == null)
                    throw new SecurityTokenException("Invalid token claims");

                var tokenResult = GenerateJwtToken(usernameString);
                return Ok(new { Token = tokenResult, IsSuccessfull = true });
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Token refresh failed", ex);
            }
            }
        }
}
