using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Model.View_Models.DepartmentViews_Models
{
    public class DisplayDepartment
    {
        public int dept_id { get; set; }

        
        public string name { get; set; }

        public int? ManagerId { get; set; } // Foreign key (nullable)

        public string ManagerName { get; set; }
       
        public virtual ICollection<course> course_depts { get; set; } = new List<course>();

    }
}
