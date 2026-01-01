using Medinova.Models;
using System.Collections.Generic;

namespace Medinova.Areas.Admin.Data
{
    public class DashboardViewModel
    {
        public IEnumerable<Medinova.Models.Doctor> LastDoctors { get; set; }
        public IEnumerable<Department> AllDepartments { get; set; }
    }
}