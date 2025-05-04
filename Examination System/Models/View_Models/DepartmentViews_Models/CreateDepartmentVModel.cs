using Examination_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Model.View_Models.DepartmentViews_Models
{
    public class CreateDepartmentVModel
    {
        public int dept_id { get; set; }

        public string name { get; set; }

        
        
        public int? managerId { get; set; }

        public virtual ICollection<User>? Users { get; set; } = new List<User>();

        [InverseProperty("dept")]
        public virtual ICollection<course_dept>? course_depts { get; set; } = new List<course_dept>();

        [ForeignKey("dept_id")]
        [InverseProperty("depts")]
        public virtual ICollection<Branch>? branches { get; set; } = new List<Branch>();
    }
}
