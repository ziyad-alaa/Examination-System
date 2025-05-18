// PermissionAssignmentDto.cs
using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class PermissionAssignmentDto
    {
        [Required]
        public int InstructorId { get; set; }

        [Required]
        public int BranchId { get; set; }

        public int? DepartmentId { get; set; } // Null for branch manager
    }
}