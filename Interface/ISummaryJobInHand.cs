using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ISummaryJobInHand
    {
        List<SummaryJobInHandModel> GetsSummaryJobInHand(int year, string type);
        List<SummaryJobInHandModel> GetsProjectJobInHand(int year, string type);
        List<SummaryJobInHandModel> GetsAccJobInHand(int year, string type);
        List<SummaryJobInHandModel> GetsServiceJobInHand(int year, string type);
    }
}
