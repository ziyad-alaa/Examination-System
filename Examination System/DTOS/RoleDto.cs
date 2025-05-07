using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class RoleDto
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role title is required")]
        [StringLength(255, ErrorMessage = "Role title cannot exceed 255 characters")]
        public string RoleTitle { get; set; }

        public bool isActive { get; set; } = true;
    }
}