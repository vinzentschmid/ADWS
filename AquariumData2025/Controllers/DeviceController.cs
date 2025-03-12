using DAL.Entities;
using DAL.Entities.Devices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Response.Basis;
using System.ComponentModel.DataAnnotations;

namespace AquariumManagementAPI.Controllers
{
    [ApiController]
    [Route("data/[controller]")]
    public class DeviceController : ControllerBase
    {
        ModbusDeviceService ModbusService;
        IGlobalService Service;
        MQTTDeviceService MQTTService;
        public DeviceController(IGlobalService service)
        {
            ModbusService = service.ModbusDeviceService;
            MQTTService = service.MQTTDeviceService;
            this.Service = service;
            service.ModbusDeviceService.SetModelState(this.ModelState);
            service.MQTTDeviceService.SetModelState(this.ModelState);
        }




        [HttpGet]
        [Route("{Aquarium}/Devices")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Device>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public virtual async Task<ActionResult<List<Device>>> Get([Required] String Aquarium)
        {
            List<Device> respobse = await Service.DeviceService.GetAll(Aquarium);

            return new OkObjectResult(respobse); ;
        }


        [HttpPost]
        [Route("{Aquarium}/ModbusDevice")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemResponseModel<ModbusDevice>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<ItemResponseModel<ModbusDevice>>> ModbusDevice([Required] String Aquarium, [Required][FromBody] ModbusDevice modbus)
        {
            modbus.Aquarium = Aquarium;
            ItemResponseModel<ModbusDevice> response = await ModbusService.AddModbusDevice(modbus);

            ActionResult<ItemResponseModel<ModbusDevice>> actresp = response.ToResponse();

            return actresp;
        }

        [HttpPost]
        [Route("{Aquarium}/MQTTDevice")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemResponseModel<MQTTDevice>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<ItemResponseModel<MQTTDevice>>> MQTTDevice([Required] String Aquarium, [Required][FromBody] MQTTDevice modbus)
        {
            modbus.Aquarium = Aquarium;
            ItemResponseModel<MQTTDevice> response = await MQTTService.AddMQTTDevice(modbus);

            ActionResult<ItemResponseModel<MQTTDevice>> actresp = response.ToResponse();

            return actresp;
        }


        [HttpPut]
        [Route("{Aquarium}/ModbusDevice/{ID}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemResponseModel<Device>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<ItemResponseModel<Device>>> ModbusDevice([Required] String Aquarium, [Required] int ID, [Required][FromBody] ModbusDevice modbus)
        {

            ItemResponseModel<Device> response = await ModbusService.UpdateHandler(ID, modbus);

            ActionResult<ItemResponseModel<Device>> actresp = response.ToResponse();

            return actresp;
        }


        [HttpPut]
        [Route("{Aquarium}/MQTTDevice/{ID}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemResponseModel<Device>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<ItemResponseModel<Device>>> MQTTDevice([Required] String Aquarium, [Required] int ID, [Required][FromBody] MQTTDevice mqtt)
        {

            ItemResponseModel<Device> response = await ModbusService.UpdateHandler(ID, mqtt);

            ActionResult<ItemResponseModel<Device>> actresp = response.ToResponse();

            return actresp;
        }



    }
}
