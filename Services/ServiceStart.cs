using DAL.Entities;
using DAL.Entities.Devices;
using DAL.UnitOfWork;
using Serilog;
using Services.Drivers;
using System.Collections.Concurrent;
using Utilities.Logging;

namespace Services
{
    public class ServiceStart : IServiceStart
    {
        protected ILogger log = null;
        IUnitOfWork UnitOfWork = null;
        Dictionary<String, List<Driver>> Drivers = new Dictionary<String, List<Driver>>();
        System.Timers.Timer timer = null;
        IAquariumLogger Logger;
        public ServiceStart(IUnitOfWork unitOfWork, IAquariumLogger logger)
        {
            UnitOfWork = unitOfWork;
            this.Logger = logger;
            log = logger.ContextLog<ServiceStart>();
        }

        public async Task Start()
        {
            log.Information("Starting Scheduler");
            timer = new System.Timers.Timer(10000);
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
            log.Information("Trying to get MQTT Devices");
            List<MQTTDevice> mqtdevices = await UnitOfWork.MQTTDevices.Get(x => x.Active == true);
            foreach (MQTTDevice device in mqtdevices)
            {
                List<MQTTDataPoint> datapoints = await UnitOfWork.MQTTDatapoint.GetForDevice(device.ID);


                MQTTDriver mqtt = new MQTTDriver(Logger, device, datapoints);


                if (!Drivers.ContainsKey(device.Aquarium))
                {
                    Drivers.Add(device.Aquarium, new List<Driver>());
                }
                Drivers[device.Aquarium].Add(mqtt);

                Task.Run(() => mqtt.Connect());
            }


            log.Information("Trying to get Modbus Devices");

            List<ModbusDevice> moddevices = await UnitOfWork.ModbusDevices.Get(x => x.Active == true);


            foreach (ModbusDevice device in moddevices)
            {
                List<ModbusDataPoint> datapoints = await UnitOfWork.ModbusDatapoint.GetForDevice(device.ID);

                ModbusDriver modbus = new ModbusDriver(Logger, device, datapoints);

                if (!Drivers.ContainsKey(device.Aquarium))
                {
                    Drivers.Add(device.Aquarium, new List<Driver>());
                }
                Drivers[device.Aquarium].Add(modbus);

                Task.Run(() => modbus.Connect());
            }

            await Save();
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await Save();
        }

        public async Task Save()
        {

            foreach (KeyValuePair<String, List<Driver>> driver in Drivers)
            {

                ConcurrentBag<BinarySample> binarysamples = new ConcurrentBag<BinarySample>();
                ConcurrentBag<NumericSample> numericSamples = new ConcurrentBag<NumericSample>();
                foreach (Driver dr in driver.Value)
                {
                    foreach (KeyValuePair<String, ConcurrentBag<HyperEntity>> smp in dr.Measurements)
                    {
                        foreach (HyperEntity ent in smp.Value)
                        {
                            if (ent.GetType() == typeof(BinarySample))
                            {
                                BinarySample bs = (BinarySample)ent;
                                await UnitOfWork.Binary.CreateAsync(bs);
                            }
                            else
                            {
                                NumericSample bs = (NumericSample)ent;
                                await UnitOfWork.Numeric.CreateAsync(bs);
                            }
                        }
                    }
                }
                //await InfluxUnitOfWork.Influx.InsertManyAsync(driver.Key, samples);

                await UnitOfWork.SaveChangesAsync();

                foreach (Driver dr in driver.Value)
                {
                    await dr.Clear();
                }
            }


        }



        public async Task Stop()
        {
            foreach (KeyValuePair<String, List<Driver>> driver in Drivers)
            {


                foreach (Driver dr in driver.Value)
                {
                    await dr.Disconnect();
                }
            }

        }


    }
}
