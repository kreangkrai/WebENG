using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLModels;

namespace WebENG.CTLInterfaces
{
    public interface IHierarchy
    {
        List<HierarchyPersonalModel> GetPersonalHierarchies();
        List<HierarchyDepartmentModel> GetDepartmentHierarchies();
    }
}
