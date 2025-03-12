using DataCollector.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AquariumDataAPI.Controllers
{
    [ApiController]
    [Route("data/[controller]")]
    public class ValueController : ControllerBase
    {
        IGlobalService Service;
        public ValueController(IGlobalService Service)
        {
            this.Service = Service;
        }

        [HttpGet("{Aquarium}/GetLastValue/{Device}/{Datapoint}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValueReturnModelSingle))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValueReturnModelSingle>> GetLastValue([Required] String Aquarium, [Required] String Device, [Required] String Datapoint)
        {

            ValueReturnModelSingle valueReturnModelSingle = await Service.ValueService.GetLastValue(Aquarium, Device, Datapoint);

            if (valueReturnModelSingle != null)
            {
                return valueReturnModelSingle;
            }
            else
            {
                return NotFound();
            }
        }



        [HttpGet("{Aquarium}/GetLastValues/{Device}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValueReturnModelMultiple))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValueReturnModelMultiple>> GetLastValues([Required] String Aquarium, [Required] String Device)
        {

            ValueReturnModelMultiple valueReturnModelSingle = await Service.ValueService.GetLastValues(Aquarium, Device);

            if (valueReturnModelSingle != null)
            {
                return valueReturnModelSingle;
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{Aquarium}/GetAllValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValueReturnModelMultiple))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValueReturnModelMultiple>> GetAllValues([Required] String Aquarium)
        {

            ValueReturnModelMultiple valueReturnModelSingle = await Service.ValueService.GetLastValues(Aquarium);

            if (valueReturnModelSingle != null)
            {
                return valueReturnModelSingle;
            }
            else
            {
                return NotFound();
            }
        }





    }
}
