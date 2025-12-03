using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveInterfaces
{
    public interface IMail
    {
        string SendCreate(string form, List<string> to, string description);
    }
}
