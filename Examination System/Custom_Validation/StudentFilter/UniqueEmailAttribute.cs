using System.ComponentModel.DataAnnotations;
using Examination_System.Models; // حسب مكان DbContext بتاعك
using System.Linq;

public class UniqueEmail : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (studentupdate)validationContext.ObjectInstance;
        var dbContext = (Exam_sysContext)validationContext.GetService(typeof(Exam_sysContext));

        // ابحث عن طالب بنفس الإيميل
        var existingStudent = dbContext.Students
            .FirstOrDefault(s => s.std.email == value.ToString());

        if (existingStudent != null && existingStudent.std.id != model.Id)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}


