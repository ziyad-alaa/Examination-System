using Examination_System.Custom_Validation.StudentUpdateFilter;
using Examination_System.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.View_Models.StudentView_Models
{
    public class studentupdate
    {
        public int Id { get; set; }
        [Required]
        [UniqueNameUpdate]
        public string name { get; set; }

        [Required, StringLength(255)]

        [DisplayName("City")]
        public string st_city { get; set; }



        [DataType(DataType.Password)]
        [Required, RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain uppercase, lowercase letters, and a number.")]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]

        public string CPassword { get; set; }
        [Required, EmailAddress]
        [UniqueEmailUpdate]
        public string email { get; set; }

        [StringLength(255)]

        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Egyptian phone number.")]
        [UniquePhoneUpdate]

        public string phone { get; set; }



        public List<Department>? Departments { get; set; }
        public List<Branch>? Branches { get; set; }

        public int dept_id { get; set; } // Make sure this is defined to hold the selected department ID.
        public int branch_id { get; set; }
    }
}
