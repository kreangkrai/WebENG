using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{

    public class DeviceGroupModel
    {
        public int id { get; set; }
        public string device { get; set; }
        public string groupname { get; set; }
        public TimeSpan starttime { get; set; }
    }
}
