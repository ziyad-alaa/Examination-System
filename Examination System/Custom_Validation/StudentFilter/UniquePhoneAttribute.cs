using System.ComponentModel.DataAnnotations;
using Examination_System.Models;
using System.Linq;

public class UniquePhoneAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var _iti = (Exam_sysContext)validationContext.GetService(typeof(Exam_sysContext));

        if (_iti == null)
        {
            throw new Exception("Database context is not available.");
        }

        var entity = _iti.Users.FirstOrDefault(e => e.phone == value.ToString());

        if (entity != null)
        {
            return new ValidationResult("Phone number already exists. Please choose another one.");
        }

        return ValidationResult.Success;
    }
}
