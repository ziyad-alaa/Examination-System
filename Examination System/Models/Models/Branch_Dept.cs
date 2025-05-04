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


        public bool isActive { get; set; }

        
        public virtual Branch Branch { get; set; }
        

        public virtual Department Department { get; set; }
    }
}
