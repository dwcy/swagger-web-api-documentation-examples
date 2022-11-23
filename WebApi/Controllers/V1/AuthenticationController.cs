using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ITHS.Webapi.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        Authentication _auth;
        public AuthenticationController()
        {
            _auth = new Authentication();
        }
        
        [HttpPost]
        public IActionResult Login(AuthenticationRequest authRequest)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var serverHostUrl = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
                
                var token = _auth.CreateJWTBearerToken(serverHostUrl);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Check if the user is authorized
        /// </summary>
        [HttpGet, Authorize]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Logged in successfull")]
        public IActionResult CheckAccess() 
        {
            var user = HttpContext.Items["User"];
            return Ok(user);
        }
    }
}
