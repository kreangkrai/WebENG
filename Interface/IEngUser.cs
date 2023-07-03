using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Interface
{
    interface IEngUser
    {
        bool CheckAllowEditable(string user_name);
        List<EngUserModel> GetUsers();
        List<EngUserModel> GetEngineerUsers();
        EngUserModel GetEngineerUser(string user_name);
        string CreateEngineerUser(EngUserModel engineer);
        string UpdateEngineerUser(EngUserModel engineer);
    }
}
