
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Custom_Validation.StudentUpdateFilter
{
    public class UniqueEmailUpdateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (studentupdate)validationContext.ObjectInstance;
            var dbContext = (Exam_sysContext)validationContext.GetService(typeof(Exam_sysContext));

            // ابحث عن طالب بنفس الإيميل
            var existingStudent = dbContext.Students
                .FirstOrDefault(s => s.std.email == value.ToString());

            if (existingStudent?.std != null && existingStudent.std.id != model.Id)
            {
                throw new InvalidOperationException("A student with this identifier already exists.");
            }


            return ValidationResult.Success;
        }
    }
}
