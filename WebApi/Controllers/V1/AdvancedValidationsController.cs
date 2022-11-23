using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ITHS.Webapi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AdvancedValidationsController : ControllerBase
    {
        public AdvancedValidationsController()
        {
        }

        public class ProductRequestModel {

            [Required]
            [StringLength(100, ErrorMessage = "Max length is 100 characters")]
            public string ProductName { get; set; } = null!;


            [Range(0, 999.99)]
            public decimal Price { get; set; }

            [Range(0, 10)]
            public int Quantity { get; set; }

            [EmailAddress]
            public string CompanySupportMail { get; set; } = null!;

            [Required]
            [DataType(DataType.Date)]
            public DateTime OrderDate { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime OrderDateTime { get; set; }
        }

        /// <summary>
        /// Add new products to order system
        /// </summary>
        [HttpPost()]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public IActionResult Put([FromBody] ProductRequestModel payload)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(payload);
        }
    }

}
