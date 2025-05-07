using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOS
{
    public class UserEditDto : UserCreateDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int id { get; set; }
    }
}