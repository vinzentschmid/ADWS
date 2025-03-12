using DAL.Entities;
using DAL.Entities.Devices;
using DAL.Repository.Impl;
using DAL.UnitOfWork;
using Services.Response.Basis;
using Utilities.Logging;

namespace Services
{
    public class ModbusDeviceService : DeviceService
    {
        public ModbusDeviceService(IAquariumLogger Logger, IUnitOfWork uoww, IDeviceRepository repo, IGlobalService service) : base(Logger, uoww, repo, service)
        {
        }

        public override async Task<bool> Validate(Device entry)
        {
            if (entry.GetType() == typeof(ModbusDevice))
            {
                Boolean result = await base.Validate(entry);

                if (!result)
                {
                    return false;
                }
                else
                {
                    ModbusDevice device = (ModbusDevice)entry;
                    if (!String.IsNullOrEmpty(device.Host))
                    {
                        validationDictionary.AddError("NotValid", "Host is not valid");
                    }

                    if (device.Port <= 0 | device.Port >= 65536)
                    {
                        validationDictionary.AddError("NotValid", "Port is not valid");
                    }

                    if (device.SlaveID <= 0 | device.SlaveID >= 65536)
                    {
                        validationDictionary.AddError("NotValid", "Port is not valid");
                    }
                }
            }
            else
            {
                validationDictionary.AddError("NotValid", "Item is no Modbus Device");
            }

            return validationDictionary.IsValid;
        }


        public async Task<ItemResponseModel<ModbusDevice>> AddModbusDevice(ModbusDevice entry)
        {
            ItemResponseModel<ModbusDevice> coralresp = new ItemResponseModel<ModbusDevice>();
            ItemResponseModel<Device> resp = await base.Create(entry);

            if (resp.HasError == false)
            {
                coralresp.Data = resp.Data as ModbusDevice;
            }
            else
            {
                coralresp.AddErrorMessageRange(resp.ErrorMessages);
                coralresp.AddWarningMessageRange(resp.WarningMessages);
                coralresp.HasError = true;
            }
            return coralresp;
        }
    }
}
