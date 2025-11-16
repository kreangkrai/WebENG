using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.LeaveInterfaces;
using WebENG.LeaveServices;
using WebENG.Service;

namespace WebENG.CTLServices
{
    public class HierarchyService : IHierarchy
    {
        readonly CTLInterfaces.IEmployee Employees;
        readonly IPosition Position;
        public HierarchyService()
        {
            Employees = new EmployeeService();
            Position = new PositionService();
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

            List<LeaveModels.PositionModel> managements = Position.GetManagerPositions();
            List<LeaveModels.PositionModel> directors = Position.GetDirectorPositions();
            List<LeaveModels.PositionModel> auditors = Position.GetAuditorPositions();

            List<EmployeeModel> operations = employees.Where(w => w.position == "Operation").ToList();

            hierarchies_operation = operations.GroupBy(g => g.emp_id).Select(s => new HierarchyPersonalModel()
            {
                emp_id = s.Key,
                name = operations.Where(w => w.emp_id == s.Key).Select(c => c.name_en).FirstOrDefault(),
                department = operations.Where(w => w.emp_id == s.Key).Select(c => c.department).FirstOrDefault(),
                position = operations.Where(w => w.emp_id == s.Key).Select(c => c.position).FirstOrDefault(),
                managers = managements.Where(w => w.department == operations.Where(t => t.emp_id == s.Key).Select(c => c.department).FirstOrDefault()).
                Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList(),
                directors = directors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList(),
                auditors = auditors.Select(x=> new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList()
            }).ToList();

            hierarchies_manager = managements.GroupBy(g => g.emp_id).Select(s => new HierarchyPersonalModel()
            {
                emp_id = s.Key,
                name = managements.Where(w => w.emp_id == s.Key).Select(c => c.emp_name).FirstOrDefault(),
                department = managements.Where(w => w.emp_id == s.Key).Select(c => c.department).FirstOrDefault(),
                position = managements.Where(w => w.emp_id == s.Key).Select(c => c.position).FirstOrDefault(),
                managers = new List<EmployeeModel>(),
                directors = directors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList(),
                auditors = auditors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList()
            }).ToList();

            hierarchies_director = directors.GroupBy(g => g.emp_id).Select(s => new HierarchyPersonalModel()
            {
                emp_id = s.Key,
                name = directors.Where(w => w.emp_id == s.Key).Select(c => c.emp_name).FirstOrDefault(),
                department = directors.Where(w => w.emp_id == s.Key).Select(c => c.department).FirstOrDefault(),
                position = directors.Where(w => w.emp_id == s.Key).Select(c => c.position).FirstOrDefault(),
                managers = new List<EmployeeModel>(),
                directors = new List<EmployeeModel>(),
                auditors = auditors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList()
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
          
            List<EmployeeModel> operations = employees.Where(w => w.position == "Operation").ToList();
            List<LeaveModels.PositionModel> managements = Position.GetManagerPositions();
            List<LeaveModels.PositionModel> directors = Position.GetDirectorPositions();
            List<LeaveModels.PositionModel> auditors = Position.GetAuditorPositions();

            hierarchies_manager = managements.GroupBy(g => g.department).Select(s => new HierarchyDepartmentModel()
            {
                department = s.Key,
                //location = managements.Where(w=>w.department == s.Key).Select(x=>x.location).FirstOrDefault(),
                operations = operations.Where(x => x.department == s.Key).ToList(),
                managers = managements.Where(w => w.department == s.Key).Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList(),
                directors = directors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList(),
                auditors = auditors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList()
            }).ToList();

            hierarchies_director = directors.GroupBy(g => g.department).Select(s => new HierarchyDepartmentModel()
            {
                department = s.Key,
                //location = directors.Where(w => w.department == s.Key).Select(x => x.location).FirstOrDefault(),
                operations = operations,
                managers = managements.Where(w => w.department == s.Key).Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList(),
                directors = new List<EmployeeModel> (),
                auditors = auditors.Select(x => new EmployeeModel()
                {
                    emp_id = x.emp_id,
                    name_en = x.emp_name,
                    department = x.department,
                    active = x.is_active
                }).ToList()
            }).ToList();

            hierarchies.AddRange(hierarchies_manager);
            hierarchies.AddRange(hierarchies_director);
            return hierarchies;
        }
    }
}
