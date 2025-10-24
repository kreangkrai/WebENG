using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.Service;

namespace WebENG.CTLServices
{
    public class HierarchyService : IHierarchy
    {
        readonly IEmployee Employees;
        public HierarchyService()
        {
            Employees = new EmployeeService();
        }

        public List<HierarchyPersonalModel> GetPersonalHierarchies()
        {
            List<HierarchyPersonalModel> hierarchies = new List<HierarchyPersonalModel>();
            List<HierarchyPersonalModel> hierarchies_operation = new List<HierarchyPersonalModel>();
            List<HierarchyPersonalModel> hierarchies_manager = new List<HierarchyPersonalModel>();
            List<HierarchyPersonalModel> hierarchies_director = new List<HierarchyPersonalModel>();
            List<EmployeeModel> employees = new List<EmployeeModel>();
            employees = Employees.GetEmployees();
            employees = employees.Where(w => w.active).ToList();
            List<string> managements = new List<string>()
            {
                "Manager",
                "Director",
                "Manager/Director",
            };
            List<EmployeeModel> operations = employees.Where(w => !managements.Contains(w.position)).ToList();
            List<EmployeeModel> managers = employees.Where(w => w.position.Contains("Manager")).ToList();
            List<EmployeeModel> directors = employees.Where(w => w.position.Contains("Director")).ToList();

            hierarchies_operation = operations.GroupBy(g => g.emp_id).Select(s => new HierarchyPersonalModel()
            {
                emp_id = s.Key,
                name = operations.Where(w => w.emp_id == s.Key).Select(c => c.name).FirstOrDefault(),
                gender = operations.Where(w => w.emp_id == s.Key).Select(c => c.gender).FirstOrDefault(),
                department = operations.Where(w => w.emp_id == s.Key).Select(c => c.department).FirstOrDefault(),
                position = operations.Where(w => w.emp_id == s.Key).Select(c => c.position).FirstOrDefault(),
                location = operations.Where(w => w.emp_id == s.Key).Select(c => c.location).FirstOrDefault(),
                managers = managers.Where(w => w.department == operations.Where(t => t.emp_id == s.Key).Select(c => c.department).FirstOrDefault()).ToList(),
                directors = directors
            }).ToList();

            hierarchies_manager = managers.GroupBy(g => g.emp_id).Select(s => new HierarchyPersonalModel()
            {
                emp_id = s.Key,
                name = managers.Where(w => w.emp_id == s.Key).Select(c => c.name).FirstOrDefault(),
                gender = managers.Where(w => w.emp_id == s.Key).Select(c => c.gender).FirstOrDefault(),
                department = managers.Where(w => w.emp_id == s.Key).Select(c => c.department).FirstOrDefault(),
                position = managers.Where(w => w.emp_id == s.Key).Select(c => c.position).FirstOrDefault(),
                location = managers.Where(w => w.emp_id == s.Key).Select(c => c.location).FirstOrDefault(),
                managers = new List<EmployeeModel>(),
                directors = directors
            }).ToList();

            hierarchies_director = directors.GroupBy(g => g.emp_id).Select(s => new HierarchyPersonalModel()
            {
                emp_id = s.Key,
                name = directors.Where(w => w.emp_id == s.Key).Select(c => c.name).FirstOrDefault(),
                gender = directors.Where(w => w.emp_id == s.Key).Select(c => c.gender).FirstOrDefault(),
                department = directors.Where(w => w.emp_id == s.Key).Select(c => c.department).FirstOrDefault(),
                position = directors.Where(w => w.emp_id == s.Key).Select(c => c.position).FirstOrDefault(),
                location = directors.Where(w => w.emp_id == s.Key).Select(c => c.location).FirstOrDefault(),
                managers = new List<EmployeeModel>(),
                directors = new List<EmployeeModel>(),
            }).ToList();

            hierarchies.AddRange(hierarchies_operation);
            hierarchies.AddRange(hierarchies_manager);
            hierarchies.AddRange(hierarchies_director);
            return hierarchies;
        }

        List<HierarchyDepartmentModel> IHierarchy.GetDepartmentHierarchies()
        {
            List<HierarchyDepartmentModel> hierarchies = new List<HierarchyDepartmentModel>();
            List<HierarchyDepartmentModel> hierarchies_manager = new List<HierarchyDepartmentModel>();
            List<HierarchyDepartmentModel> hierarchies_director = new List<HierarchyDepartmentModel>();
            List<EmployeeModel> employees = new List<EmployeeModel>();
            employees = Employees.GetEmployees();
            employees = employees.Where(w => w.active).ToList();
            List<string> managements = new List<string>()
            {
                "Manager",
                "Director",
                "Manager/Director",
            };
            List<EmployeeModel> operations = employees.Where(w => !managements.Contains(w.position)).ToList();
            List<EmployeeModel> managers = employees.Where(w => w.position.Contains("Manager")).ToList();
            List<EmployeeModel> directors = employees.Where(w => w.position.Contains("Director")).ToList();

            hierarchies_manager = managers.GroupBy(g => g.department).Select(s => new HierarchyDepartmentModel()
            {
                department = s.Key,
                location = managers.Where(w=>w.department == s.Key).Select(x=>x.location).FirstOrDefault(),
                operations = operations.Where(x=>x.department == s.Key).ToList(),
                managers = managers.Where(w => w.department == s.Key).ToList(),
                directors = directors
            }).ToList();

            hierarchies_director = directors.Where(w=>w.department == "CEO").GroupBy(g => g.department).Select(s => new HierarchyDepartmentModel()
            {
                department = s.Key,
                location = directors.Where(w => w.department == s.Key).Select(x => x.location).FirstOrDefault(),
                operations = operations,
                managers = managers,
                directors = new List<EmployeeModel> ()
            }).ToList();

            hierarchies.AddRange(hierarchies_manager);
            hierarchies.AddRange(hierarchies_director);
            return hierarchies;
        }
    }
}
