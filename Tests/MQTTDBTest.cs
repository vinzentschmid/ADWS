using DAL.Entities;
using DAL.Entities.Devices;

namespace Tests
{
    public class MQTTDBTest : BasisTest
    {
        [Test]
        public async Task CreateDevice()
        {
            MQTTDevice device = new MQTTDevice();
            device.Active = true;
            device.Aquarium = "SchiScho2";
            device.DeviceName = "WaterQuality";
            device.DeviceDescription = "Water Quality Measurement";
            device.Host = "127.0.0.1";
            device.Port = 1883;

            await UnitOfWork.Devices.CreateAsync(device);
            await UnitOfWork.SaveChangesAsync();
        }



        [Test]
        public async Task CreateDatapoints()
        {
            ModbusDevice device = new ModbusDevice();

            List<DataPoint> pts = new List<DataPoint>
            {
                await CreateFloatMQTTDP("WaterQuality", "Calcium", 1),
                await CreateFloatMQTTDP("WaterQuality","Alkalinity", 1),
                 await CreateFloatMQTTDP("WaterQuality","Magnesium", 1),
                   await CreateFloatMQTTDP("WaterQuality","WaterTemp", 1),
            };

            await UnitOfWork.DataPoints.CreateAsync(pts);
            await UnitOfWork.SaveChangesAsync();

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

        private async Task<MQTTDataPoint> CreateBooleanModbusDP(String devicename, String dbname, String visual = "")
        {
            Device dev = await UnitOfWork.Devices.Find(x => x.DeviceName.Equals(devicename));


            MQTTDataPoint pt = new MQTTDataPoint();
            pt.FK_Device = dev.ID;
            pt.Icon = "sun";
            pt.Name = dbname;
            pt.TopicName = dbname;
            pt.DataType = DataType.Boolean;
            return pt;

        }



    }
}
