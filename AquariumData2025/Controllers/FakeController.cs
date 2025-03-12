using DAL.Entities;
using DAL.Entities.Devices;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AquariumDataAPI.Controllers
{
    [ApiController]
    [Route("data/[controller]")]
    public class FakeController : ControllerBase
    {
        IGlobalService Service;
        IUnitOfWork UnitOfWork = null;
        public FakeController(IGlobalService service)
        {
            UnitOfWork = service.UnitOfWork;
        }

        [HttpGet("Seed")]
        public async Task<ActionResult<Boolean>> Seed()
        {
            await Aquarium();
            return true;
        }


        private async Task Aquarium()
        {
            MQTTDevice device = new MQTTDevice();
            device.Active = true;
            device.Aquarium = "SchiScho";
            device.DeviceName = "WaterQuality";
            device.DeviceDescription = "Water Quality Measurement";
            device.Host = "127.0.0.1";
            device.Port = 1883;


            await UnitOfWork.Devices.CreateAsync(device);
            await UnitOfWork.SaveChangesAsync();


            List<DataPoint> pts = new List<DataPoint>
            {
                await CreateFloatMQTTDP("WaterQuality", "Calcium", 1),
                await CreateFloatMQTTDP("WaterQuality","Alkalinity", 1),
                 await CreateFloatMQTTDP("WaterQuality","Magnesium", 1),
                   await CreateFloatMQTTDP("WaterQuality","WaterTemp", 1),
            };

            await UnitOfWork.DataPoints.CreateAsync(pts);
            await UnitOfWork.SaveChangesAsync();

            ModbusDevice mdevice = new ModbusDevice();
            mdevice.Active = true;
            mdevice.Aquarium = "SchiScho";
            mdevice.DeviceName = "Pump";
            mdevice.DeviceDescription = "Water Pump";
            mdevice.SlaveID = 1;
            mdevice.Host = "127.0.0.1";
            mdevice.Port = 502;


            await UnitOfWork.Devices.CreateAsync(mdevice);
            await UnitOfWork.SaveChangesAsync();


            List<DataPoint> mpts = new List<DataPoint>
            {
                await CreateFloatModbusDP("Pump", "Pump Current",  0, 1),
                 await CreateFloatModbusDP("Pump", "Pump Voltage",  2, 1),
                 await CreateBooleanModbusDP("Pump", "Pump Status", 1 )
            };


            await UnitOfWork.DataPoints.CreateAsync(mpts);
            await UnitOfWork.SaveChangesAsync();


        }


        private async Task<ModbusDataPoint> CreateFloatModbusDP(String devicename, String name, int register, int offset = 1)
        {

            Device dev = await UnitOfWork.Devices.Find(x => x.DeviceName.Equals(devicename));


            ModbusDataPoint pt = new ModbusDataPoint();
            pt.FK_Device = dev.ID;
            pt.Name = name;
            pt.RegisterCount = 2;
            pt.ReadingType = ReadingType.LowToHigh;
            pt.Description = name;
            pt.Register = register;
            pt.RegisterType = RegisterType.HoldingRegister;
            pt.Offset = offset;
            pt.DataType = DataType.Float;
            pt.Icon = "sun";
            return pt;

        }

        private async Task<ModbusDataPoint> CreateBooleanModbusDP(String devicename, String name, int register)
        {
            Device dev = await UnitOfWork.Devices.Find(x => x.DeviceName.Equals(devicename));
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.Name = name;
            pt.Icon = "sun";
            pt.FK_Device = dev.ID;
            pt.Description = name;
            pt.RegisterCount = 1;
            pt.Register = register;
            pt.RegisterType = RegisterType.Coil;
            pt.DataType = DataType.Boolean;
            return pt;

        }

        private async Task<MQTTDataPoint> CreateFloatMQTTDP(String devicename, String dbname, int offset = 1)
        {
            Device dev = await UnitOfWork.Devices.Find(x => x.DeviceName.Equals(devicename));


            MQTTDataPoint pt = new MQTTDataPoint();
            pt.FK_Device = dev.ID;
            pt.Name = dbname;
            pt.TopicName = dbname;
            pt.Offset = offset;
            pt.Icon = "sun";
            pt.DataType = DataType.Float;
            return pt;

        }


    }
}
