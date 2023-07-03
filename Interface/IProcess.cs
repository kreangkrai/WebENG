using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IProcess
    {
        List<EngProcessModel> GetProcesses();
        int GetLastProcessID();
        string CreateProcess(EngProcessModel process);
        string EditProcess(EngProcessModel process);
        string DeleteProcess(EngProcessModel process);
    }
}
