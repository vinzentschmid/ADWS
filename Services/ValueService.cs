using DAL.Entities;
using DAL.UnitOfWork;
using DataCollector.ReturnModels;

namespace Services
{
    public class ValueService
    {
        IUnitOfWork UnitOfWork = null;
        public ValueService(IUnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }

        public async Task<ValueReturnModelSingle> GetLastValue(String aquarium, String device, String datapoint)
        {
            Device dev = await UnitOfWork.Devices.GetDeviceForAquarium(aquarium, device);
            if (dev != null)
            {
                DataPoint dp = await UnitOfWork.DataPoints.GetDatapointForDeviceAndAquarium(dev.ID, datapoint);

                if (dp != null)
                {
                    ValueReturnModelSingle ret = null;
                    if (dp.DataType != DataType.Boolean)
                    {
                        ValueReturnNumericModel returnval = new ValueReturnNumericModel(); ;

                        returnval.Sample = await UnitOfWork.Numeric.GetLastValue(dp.ID);
                        returnval.Unit = dp.Unit;
                        returnval.Icon = dp.Icon;
                        ret = returnval;
                    }
                    else
                    {

                        ValueReturnBinaryModel returnval = new ValueReturnBinaryModel(); ;

                        returnval.Sample = await UnitOfWork.Binary.GetLastValue(dp.ID);
                        returnval.TextForTrue = "On";
                        returnval.TextForFalse = "Off";

                        ret = returnval;
                    }
                    ret.DataPoint = dp.Name;
                    ret.DataType = dp.DataType;

                    return ret;
                }
            }

            return null;
        }

        public async Task<ValueReturnModelMultiple> GetLastValues(String aquarium, String device)
        {
            ValueReturnModelMultiple returnval = new ValueReturnModelMultiple();
            Device dev = await UnitOfWork.Devices.GetDeviceForAquarium(aquarium, device);
            if (dev != null)
            {
                List<DataPoint> dps = await UnitOfWork.DataPoints.GetDatapointsForDevice(dev.ID);


                foreach (DataPoint dp in dps)
                {
                    if (dp != null)
                    {
                        ValueReturnModelSingle val = await GetLastValue(aquarium, device, dp.Name);

                        if (val.GetType() == typeof(ValueReturnNumericModel))
                        {
                            returnval.Numeric.Add((ValueReturnNumericModel)val);
                        }
                        else if (val.GetType() == typeof(ValueReturnBinaryModel))
                        {
                            returnval.Binary.Add((ValueReturnBinaryModel)val);
                        }
                    }
                }
            }

            return returnval;
        }

        public async Task<ValueReturnModelMultiple> GetLastValues(String aquarium)
        {
            ValueReturnModelMultiple returnval = new ValueReturnModelMultiple();
            List<Device> dev = await UnitOfWork.Devices.GetDevicesForAquarium(aquarium);
            if (dev != null)

                foreach (Device d in dev)
                {
                    ValueReturnModelMultiple val = await GetLastValues(aquarium, d.DeviceName);
                    returnval.Numeric.AddRange(val.Numeric);
                    returnval.Binary.AddRange(val.Binary);
                }


            return returnval;
        }






    }
}
