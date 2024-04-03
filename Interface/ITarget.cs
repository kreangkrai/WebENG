using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ITarget
    {
        List<TargetModel> getData(string year,string type);
        string Insert(List<TargetModel> targets, string type);
        string Update(List<TargetModel> targets, string type);
    }
}
