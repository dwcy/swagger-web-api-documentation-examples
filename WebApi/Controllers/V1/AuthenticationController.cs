using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ITHS.Webapi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public class AuthenticationRequest
        {
            [Required]
            public string UserName { get; set; } = "dawid";
            
            [Required]
            public string Password { get; set; } = "iths";
        }

        [HttpPost]
        public IActionResult Login(AuthenticationRequest authRequest)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ITHSsuperSecretKey"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                string host = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: "ITHS",
                    audience: "https://localhost:7261/",
                    claims: new List<Claim>() { 
                        new Claim("School", "ITHS"),
                        new Claim("Teacher", "Dawid")
                    },
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signinCredentials
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Unauthorized();
        }
    }
}
