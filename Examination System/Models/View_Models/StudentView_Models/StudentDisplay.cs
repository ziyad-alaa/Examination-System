using Examination_System.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.View_Models.StudentView_Models
{
    public class StudentDisplay
    {
        public string? name { get; set; }
        public string? st_city { get; set; }

        public string? email { get; set; }

        public string? phone { get; set; }
        public string? Branch { get; set; }
        public string? Department { get; set; }
    }
    
}
