using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Examination_System.Models.View_Models
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(255, ErrorMessage = "Department name cannot exceed 255 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public int BranchId { get; set; }

        public int? ManagerId { get; set; } // Nullable, as manager is optional

        public string? BranchName { get; set; } // For display

        public string? ManagerName { get; set; } // For display

        public bool IsActive { get; set; }

        public IEnumerable<SelectListItem>? Branches { get; set; } // Dropdown for branches

        public IEnumerable<SelectListItem>? Instructors { get; set; } // Dropdown for managers
    }
}