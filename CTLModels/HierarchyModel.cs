using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.CTLModels
{
    public class HierarchyPersonalModel
    {
        public string emp_id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public int level { get; set; }
        //public string location { get; set; }
        //public string gender { get; set; }
        public List<EmployeeModel> managers { get; set; }
        public List<EmployeeModel> directors { get; set; }
        public List<EmployeeModel> auditors { get; set; }
    }

    public class HierarchyDepartmentModel
    {
        public string department { get; set; }
        //public string location { get; set; }
        public List<EmployeeModel> operations { get; set; }
        public List<EmployeeModel> managers { get; set; }
        public List<EmployeeModel> directors { get; set; }
        public List<EmployeeModel> auditors { get; set; }
    }
}
