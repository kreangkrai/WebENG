using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.HRModel;

namespace WebENG.HRInterface
{
    public interface IDeviceGroup
    {
        List<DeviceGroupModel> GetData();
        List<DeviceGroupModel> GetDataByDevice(string device);
    }
}
