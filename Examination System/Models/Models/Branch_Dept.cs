using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Model.Models
{
    [Table("Branch_Dept")] // Explicit table name
    [Index(nameof(branch_id), nameof(dept_id), IsUnique = true)]
    public class Branch_Dept
    {
        [ForeignKey(nameof(Branch))]
        public int branch_id { get; set; }


        [ForeignKey(nameof(Department))]
        public int dept_id { get; set; }

        public int? ManagerId { get; set; } // Foreign key (nullable)

        [ForeignKey("ManagerId")]
        [InverseProperty("ManagedDepartments")]
        public virtual Instructor? Manager { get; set; } // Navigation property
        public bool isActive { get; set; }


        [ForeignKey("ManagerId")]
        [InverseProperty("ManagedBranchDepts")]
        public virtual Instructor? Manager { get; set; } // Navigation property
        public virtual Branch Branch { get; set; }
      

        [InverseProperty("Branch_Depts")]
        public virtual Branch Branch { get; set; }

        [InverseProperty("Branch_Depts")]

        public virtual Department Department { get; set; }
    }
}
