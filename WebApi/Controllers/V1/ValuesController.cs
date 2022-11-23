using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace ITHS.Webapi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {
        }

        public static List<string> values = new List<string>();

        public struct Payload
        {/// <summary>
         /// The sizes the product is available in
         /// </summary>
         /// <example>["Small", "Medium", "Large"]</example>
            public string Text { get; set; }
        }
        public class PutPayload
        {
            [Required]
            [MinLength(3)]
            public string ValueToUpdate { get; set; }

            [Required]
            public string NewValue { get; set; }
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public IEnumerable<string> Get()
        {
            return values;
        }

        [HttpGet("{id}", Name = "GetById")]
        public string Get([FromQuery] int id)
        {
            return values[id];
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid input")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(Payload), 200)]
        [Consumes(MediaTypeNames.Application.Json)]
        public IActionResult Post([FromBody] Payload payload)
        {
            if (payload.Text == null || payload.Text == string.Empty)
            {
                return BadRequest("No text was provided");
            }

            if (values.Where(x => x == payload.Text).Any())
            {
                return BadRequest("Text already exists");
            }

            values.Add(payload.Text);

            return Ok();
        }


        /// <summary>
        /// updates a specific item in list
        /// </summary>
        [HttpPut()]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public IActionResult Put([FromBody] PutPayload payload)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                values.Remove(payload.ValueToUpdate);
                values.Add(payload.NewValue);
            }
            catch (Exception)
            {
                return NotFound("Id not found");
            }

            return Ok(values);
        }

        /// <summary>
        /// Deletes a specific item from list
        /// </summary>
        [HttpDelete]
        public IActionResult Delete(Payload payload)
        {
            try
            {
                var value = values.Remove(payload.Text);

            }
            catch (Exception)
            {
                return NotFound($"{payload.Text} not found");
            }

            return Ok();
        }

    }
}
