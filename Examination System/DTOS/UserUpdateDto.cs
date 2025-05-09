using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class UserUpdateDto
    {
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string? Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$",
         ErrorMessage = "Password must contain: 8+ chars, 1 uppercase, 1 lowercase, 1 number")]
        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public int? DeptId { get; set; }

        public int? BranchId { get; set; }

        [Display(Name = "Role")]
        public int? RoleId { get; set; }

        public bool? isActive { get; set; }

        // Additional properties for validation
        public bool ShouldValidatePassword => !string.IsNullOrEmpty(Password);
    }
}