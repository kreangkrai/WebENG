using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;
using WebENG.Models;

namespace WebENG.LeaveInterfaces
{
    public interface INotification
    {
        List<NotificationModel> GetNotificationsByEMPID(string emp_id);
        string Insert(NotificationModel notification);
        string Update(NotificationModel notification);
        string UpdateStatus(string emp_id,string status);
    }
}
