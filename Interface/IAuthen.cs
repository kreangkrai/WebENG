using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IAuthen
    {
        List<AuthenModel> GetAuthens();
        string Insert(AuthenModel authen);
        string UpdateRole(AuthenModel authen);
        string UpdateLevel(AuthenModel authen);

    }
}
