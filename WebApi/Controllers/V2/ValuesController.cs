using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Mime;

namespace ITHS.Webapi.Controllers.V2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [ApiExplorerSettings(GroupName = "v2")]

    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {
        }

        public static List<string> values = new List<string>();

        public struct Payload
        {
            public string TextV1 { get; set; }
        }

        [HttpPost, Authorize]
        
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [Consumes(MediaTypeNames.Application.Json)]
        public IActionResult Post([FromBody] Payload payload)
        {
            if (payload.TextV1 == null || payload.TextV1 == string.Empty)
            {
                return BadRequest("No text was provided");
            }

            if (values.Where(x => x == payload.TextV1).Any())
            {
                return BadRequest("Text already exists");
            }

            values.Add(payload.TextV1);

            return Ok(values);
        }
    }
}
