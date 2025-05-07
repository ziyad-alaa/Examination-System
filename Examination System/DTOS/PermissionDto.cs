using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class PermissionDto
    {
        public int PeriD { get; set; }

        [Required(ErrorMessage = "Permission title is required")]
        [StringLength(255, ErrorMessage = "Permission title cannot exceed 255 characters")]
        public string PerTitle { get; set; }

        public bool isActive { get; set; } = true;
    }
}