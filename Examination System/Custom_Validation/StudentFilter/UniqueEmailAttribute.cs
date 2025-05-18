using System.ComponentModel.DataAnnotations;
using Examination_System.Models; // حسب مكان DbContext بتاعك
using System.Linq;

public class UniqueEmail : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        studentcreate model = (studentcreate)validationContext.ObjectInstance;
        var dbContext = new Exam_sysContext();
        string newemail = value.ToString();

        // ابحث عن طالب بنفس الإيميل
        var std = dbContext.Students
            .FirstOrDefault(s => s.std.email == newemail);

        if (std != null)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}


