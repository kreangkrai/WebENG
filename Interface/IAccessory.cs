using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IAccessory
    {
        List<UserModel> getAllUser();
        List<UserModel> getWorkingUser();
    }
}
