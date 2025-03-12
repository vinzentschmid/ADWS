using DAL.Entities;
using DAL.Entities.Devices;
using Modbus.Device;
using Modbus.Utility;
using System.Net.Sockets;
using Utilities.Logging;
using ModbusDevice = DAL.Entities.Devices.ModbusDevice;

namespace Services.Drivers
{
    public class ModbusDriver : Driver
    {
        TcpClient Client;
        ModbusIpMaster Master;



        private ModbusDevice Source;
        String FinalUrl;
        List<ModbusDataPoint> ModbusDataPoints = new List<ModbusDataPoint>();

        System.Timers.Timer FetchTimer = new System.Timers.Timer();


        public ModbusDriver(IAquariumLogger logger, ModbusDevice src, List<ModbusDataPoint> datapoints) : base(logger, src.DeviceName)
        {
            this.Source = src;
            ModbusDataPoints = datapoints;
        }


        public async override Task Connect()
        {
            try
            {
              

                log.Information("Created Client - trying to connect to {Url}", FinalUrl);

            //Connect Here

                FetchTimer = new System.Timers.Timer(10000);
                FetchTimer.Enabled = true;
                FetchTimer.Elapsed += FetchTimer_Elapsed;

                await Read();

            }
            catch (Exception ex)
            {
                log.Fatal("Could not create Master ", ex);
            }

           

            foreach (ModbusDataPoint dpi in ModbusDataPoints)
            {
                AddDataPoint(dpi.Name, dpi);
            }

        }

        private async void FetchTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await Read();
        }

        public async override Task Disconnect()
        {
            try
            {

                log.Information("Stopping Client");
                if (Client != null)
                {
                    Client.Dispose();
                }

                if (FetchTimer != null)
                {
                    FetchTimer.Dispose();
                    FetchTimer = null;
                }
            }
            catch (Exception e)
            {
                log.Warning("Stopping failed " + e.ToString());
            }
        }

        public async Task Read()
        {

            if (IsConnected)
            {
                foreach (ModbusDataPoint pt in ModbusDataPoints)
                {

                    if (pt.RegisterCount > 0 && pt.Register >= 0)
                    {
                        HyperEntity mn = await GetModbusVal(pt);

                        if (mn != null)
                        {

                            if (mn.GetType() == typeof(NumericSample))
                            {
                                AddNumericMeasurement(pt.Name, mn as NumericSample);
                            }
                            else
                            {
                                AddBinaryMeasurement(pt.Name, mn as BinarySample);
                            }


                            //log.Debug($"Got {mn.Value} from {pt.Name} ");
                        }
                    }
                    else
                    {
                        log.Error("Cannot Read {Datapoint} Register or Offset 0", pt.Name);
                    }
                }
            }
        }


        private NumericSample DecodeNumeric(ushort[] register, ModbusDataPoint pt)
        {
            NumericSample sample = new NumericSample();
            sample.FK_Datapoint = pt.ID;
            if (pt.DataType == DataType.Float)
            {

                if (register.Count() > 1)
                {
                    if (register.Count() == 2)
                    {
                        ushort register0 = register[0];
                        ushort register1 = register[1];

                        if (pt.ReadingType == ReadingType.HighToLow)
                        {
                            register0 = register[1];
                            register1 = register[0];
                        }

                        sample.Value = ModbusUtility.GetSingle(register0, register1);


                        log.Verbose("Reading {Datapoint} Quantity 2 - Address {Address}/{Count} returned {Value}", this.Name, pt.Register, pt.RegisterCount, sample.Value);
                    }
                    else
                    {
                        sample.Value = 0;
                        log.Verbose("Reading {Datapoint} Quantity 2 - Address {Address}/{Count} not found", this.Name, pt.Register, pt.RegisterCount);
                    }
                }
                else if (register.Count() == 1)
                {
                    ushort register0 = register[0];
                    sample.Value = Convert.ToSingle(register0);
                }
                else
                {
                    sample.Value = 0;
                }


                if (pt.Offset > 0)
                {
                    sample.Value = Convert.ToSingle(sample.Value) / pt.Offset;
                }
            }

            return sample;

        }

        private async Task<HyperEntity> GetModbusVal(ModbusDataPoint pt)
        {
            HyperEntity rsample = null;


            ushort start = Convert.ToUInt16(pt.Register);
            ushort offset = Convert.ToUInt16(pt.RegisterCount);
            try
            {
                if (pt.RegisterType == RegisterType.Coil)
                {
                  // Read Coil


                }
                else if (pt.RegisterType == RegisterType.InputStatus)
                {
                   // Read Input Status

                }
                else if (pt.RegisterType == RegisterType.InputRegister)
                {
                   //Read Input Register
                }
                else if (pt.RegisterType == RegisterType.HoldingRegister)
                {

                    // Read Holding Register

                }


            }
            catch (Exception e)
            {
                //  log.Error($"Could not read register {pt.Register} and quantity {start}", e);
                log.Error("Could not Read {Datapoint} - {Register}/{Offset} ", this.Name, start, offset);
                rsample = null;
            }


            return rsample;
        }




    }
}
