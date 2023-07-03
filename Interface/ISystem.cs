using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ISystem
    {
        List<EngSystemModel> GetSystems();
        int GetLastSystemID();
        string CreateSystem(EngSystemModel system);
        string EditSystem(EngSystemModel system);
        string DeleteSystem(EngSystemModel system);
    }
}
