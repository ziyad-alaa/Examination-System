using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$",
         ErrorMessage = "Password must contain: 8+ chars, 1 uppercase, 1 lowercase, 1 number")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        public bool isActive { get; set; } = true;
    }
}