using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.TripModels;

namespace WebENG.TripInterface
{
    public interface INEWTripExpense
    {
        List<TripExpenseModel> GetData(DateTime start, DateTime stop);
        List<TripExpenseModel> GetDataByEmployee(string emp_id, DateTime start, DateTime stop);
    }
}
