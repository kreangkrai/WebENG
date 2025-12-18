using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.HRModel;

namespace WebENG.HRInterface
{
    public interface IDevice
    {
        List<DeviceModel> GetDevices();
    }
}
