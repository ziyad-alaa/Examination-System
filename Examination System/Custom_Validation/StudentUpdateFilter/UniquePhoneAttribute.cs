using Examination_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Custom_Validation.StudentUpdateFilter
{
    public class UniquePhoneUpdateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (studentupdate)validationContext.ObjectInstance;
            var dbContext = (Exam_sysContext)validationContext.GetService(typeof(Exam_sysContext));

            if (value != null)
            {
                string phone = value.ToString();
                var existingStudent = dbContext.Students

                    .FirstOrDefault(s => s.std.phone == phone);

                if (existingStudent?.std != null && existingStudent.std.id != model.Id)
                {
                    return new ValidationResult(ErrorMessage ?? "Phone already exists.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
