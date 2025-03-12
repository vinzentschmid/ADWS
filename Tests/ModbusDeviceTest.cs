using DAL.Entities.Devices;
using Services.Drivers;

namespace Tests
{
    public class ModbusDeviceTest : BasisTest
    {

        [Test]
        public async Task ReadModbus()
        {
            ModbusDevice device = await UnitOfWork.ModbusDevices.Find(x => x.DeviceName.Equals("Pump"));
            List<ModbusDataPoint> datapoints = await UnitOfWork.ModbusDatapoint.Get(x => x.FK_Device == device.ID);

            List<ModbusDataPoint> listOfModbusDataPoints = datapoints.Cast<ModbusDataPoint>().ToList();

            ModbusDriver driver = new ModbusDriver(AquariumLogger, device, listOfModbusDataPoints);

            await driver.Connect();
            Assert.That(driver.IsConnected, Is.True);
            await driver.Read();
            Assert.That(driver.Measurements.Count, Is.GreaterThan(0));

        }







    }
}