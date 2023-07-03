using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IScore
    {
        List<EngineerScoreModel> GetScores(string user_id, string year);
    }
}
