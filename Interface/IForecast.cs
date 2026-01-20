using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    public interface IForecast
    {
        List<ForecastModel> GetForecasts(int year);
        List<InvoicesModel> GetInvoice(int year);
        double GetJonInHand(int year, string department, string responsible);
        //double GetInvoice(int year, string department, string responsible);
    }
}
