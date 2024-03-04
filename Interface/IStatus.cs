using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IStatus
    {
        List<EngStatusModel> GetStatuses();
        int GetLastStatusID();
        string CreateStatus(EngStatusModel status);
        string EditStatus(EngStatusModel status);
        string DeleteStatus(EngStatusModel status);
    }
}
