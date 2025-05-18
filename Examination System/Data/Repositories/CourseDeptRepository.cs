using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.Repositories
{
    public class CourseDeptRepository
    {
        private readonly Exam_sysContext _context;

        public CourseDeptRepository(Exam_sysContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetBranchesForInstructorAsync(int instructorId)
        {
            return await _context.course_depts
                .Where(cd => cd.insid == instructorId && cd.isActive)
                .Select(cd => cd.branch)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetDepartmentsForInstructorAsync(int instructorId, int branchId)
        {
            return await _context.course_depts
                .Where(cd => cd.insid == instructorId &&
                            cd.branch_id == branchId &&
                            cd.isActive)
                .Select(cd => cd.dept)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<course>> GetCoursesForInstructorAsync(int instructorId, int branchId, int deptId)
        {
            return await _context.course_depts
                .Where(cd => cd.insid == instructorId &&
                            cd.branch_id == branchId &&
                            cd.dept_id == deptId &&
                            cd.isActive)
                .Select(cd => cd.crs)
                .Where(c => c.isActive)
                .ToListAsync();
        }

        public async Task<bool> IsValidCourseAssignmentAsync(int instructorId, int courseId, int branchId, int deptId)
        {
            return await _context.course_depts
                .AnyAsync(cd => cd.insid == instructorId &&
                               cd.crsid == courseId &&
                               cd.dept_id == deptId &&
                               cd.branch_id == branchId &&
                               cd.isActive);
        }
    }
}
