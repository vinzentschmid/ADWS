using DAL.Entities.Devices;
using Services.Drivers;

namespace Tests
{
    public class MQTTDeviceTest : BasisTest
    {
        
        [Test]
        public async Task ReadMQTT()
        {
            
            MQTTDevice device = await UnitOfWork.MQTTDevices.Find(x => x.DeviceName.Equals("WaterQuality"));
            List<MQTTDataPoint> datapoints = await UnitOfWork.MQTTDatapoint.Get(x => x.FK_Device == device.ID);

            MQTTDriver driver = new MQTTDriver(AquariumLogger, device, datapoints);

            await driver.Connect();
            await Task.Delay(500);
            Assert.That(driver.IsConnected, Is.True);

            await Task.Delay(5000);
            Assert.That(driver.Measurements.Count, Is.GreaterThan(0));

        }

    }
}