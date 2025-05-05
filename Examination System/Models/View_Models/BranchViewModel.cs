using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Examination_System.Models.View_Models
{
    public class BranchViewModel
    {
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(255)]
        public string Location { get; set; }

        public int? ManagerId { get; set; } // Nullable لأنه اختياري
        public string? ManagerName { get; set; }
        public IEnumerable<SelectListItem>? Instructors { get; set; } // لعرض المدربين في القائمة المنسدلة

        public bool IsActive { get; set; }
    }
}