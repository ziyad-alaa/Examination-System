using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}