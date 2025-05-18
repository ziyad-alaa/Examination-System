using Examination_System.Models;

namespace Examination_System.Data.Interfaces
{
    public interface ICourseDeptRepository
    {
        Task<IEnumerable<Branch>> GetBranchesForInstructorAsync(int instructorId);
        Task<IEnumerable<Department>> GetDepartmentsForInstructorAsync(int instructorId, int branchId);
        Task<IEnumerable<course>> GetCoursesForInstructorAsync(int instructorId, int branchId, int deptId);
        Task<bool> IsValidCourseAssignmentAsync(int instructorId, int courseId, int branchId, int deptId);
    }
}
