using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.TripModels;

namespace WebENG.TripInterface
{
    public interface ITripExpense
    {
        List<TripExpenseModel> GetData(DateTime start, DateTime stop);
    }
}
