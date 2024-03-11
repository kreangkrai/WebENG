using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Interface
{
    interface ITask
    {
        List<TaskModel> GetAllTasks();
        List<TaskModel> GetOfficeTasks();
        List<TaskModel> GetSiteTasks();
        List<TaskModel> GetOtherTasks();
        string CreateTask(TaskModel task);
        string UpdateTask(TaskModel task);

    }
}
