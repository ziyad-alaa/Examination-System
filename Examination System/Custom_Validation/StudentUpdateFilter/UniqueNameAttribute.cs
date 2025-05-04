using Examination_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Custom_Validation.StudentUpdateFilter
{
    public class UniqueNameUpdateAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (studentupdate)validationContext.ObjectInstance;
            var dbContext = (Exam_sysContext)validationContext.GetService(typeof(Exam_sysContext));

            if (value != null)
            {
                string name = value?.ToString();
                var existingStudent = dbContext.Students
                    
                    .FirstOrDefault(s => s.std.name == name);

                if (existingStudent?.std != null && existingStudent.std.id != model.Id)
                {
                    return new ValidationResult(ErrorMessage ?? "Name already exists.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
