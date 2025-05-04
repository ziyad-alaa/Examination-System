using System.ComponentModel.DataAnnotations;
using Examination_System.Models; // أو حسب مكان DbContext بتاعك
using System.Linq;

public class UniqueNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var _iti = (Exam_sysContext)validationContext.GetService(typeof(Exam_sysContext));

        if (_iti == null)
        {
            throw new Exception("Database context is not available.");
        }

        // Check if the name already exists
        var entity = _iti.Users.FirstOrDefault(e => e.name == value.ToString());

        if (entity != null)
        {
            return new ValidationResult("Name already exists. Please choose another one.");
        }

        return ValidationResult.Success;
    }
}
