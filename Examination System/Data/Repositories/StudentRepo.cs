using Examination_System.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Examination_System.Data.Repositories
{
    public class StudentRepo:IService2<Student>
    {
        Exam_sysContext _iti;
        public StudentRepo(Exam_sysContext _iti)
        {
            this._iti = _iti;
        }
        public async Task Create(Student entity)
        {
             _iti.Students.Add(entity);

        }

        public async Task Delete(int id)
        {
            Student student = await GetById(id);
            student.std.isActive= false;
        }

        public async Task<List<Student>> GetAll()
        {
            List<Student> Students = await _iti.Students.Where(s=>s.std.isActive==true).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).ToListAsync();
            return Students;
        }

        public async Task<List<Student>> GetAllNotActive()
        {
            List<Student> StudentNotactive= await  _iti.Students.Where(s => s.std.isActive == false).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).ToListAsync();
            return StudentNotactive;
        }

        public async Task<List<Exam>> GetAvailableExams(int studentId)
        {
            var student = await GetById(studentId);
            if (student?.std == null)
                return new List<Exam>();

            // Adjust time zone if necessary
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); // Example
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            var filteredExams = await _iti.Exams
                .Where(e =>
                    e.deptid == student.std.dept_id &&
                    e.branchid == student.std.branch_id &&
                    e.isActive &&
                    e.startat <= now &&
                    e.endat >= now)
                .ToListAsync();

            return filteredExams;
        }

        public async Task<Student> GetById(int id)
        {
            return await _iti.Students
            .Where(s => s.std.isActive && s.stdid == id).Include(s => s.std)
            .Include(x => x.std.branch).Include(s => s.std.dept).FirstOrDefaultAsync();
        }

        public async Task<Student> GetNotactiveById(int id)
        {
            Student student =await _iti.Students.Where(s => s.std.isActive == false).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).FirstOrDefaultAsync(s => s.stdid == id);
            return student;
        }

        public async Task<List<Student_course>> GetStudentCourses(int studentId)
        {
         return await _iti.Student_courses.Include(s=>s.crs).Where(sc=>sc.stdid==studentId).ToListAsync();
        }

        public async Task<bool> HasStudentTakenCourseExam(int studentId, int courseId)
        {
            return await _iti.Student_Exams
            .AnyAsync(se => se.stdid == studentId &&
                   se.Exam.crsid == courseId);
        }

        public async Task Update(int id, Student entity)
        {
            Student std =await GetById(id);
            if (std != null)
            {
                _iti.Students.Update(std);
            }

        }
        public async Task<Student_course> GetStudentCourse(int studentId, int courseId)
        {
            return await _iti.Student_courses
                .Include(sc => sc.crs)  // Include course details if needed
                .FirstOrDefaultAsync(sc => sc.stdid == studentId &&
                                      sc.crsid == courseId &&
                                      sc.isActive);
        }

        public async Task UpdateStudentCourse(Student_course studentCourse)
        {
            var existing = await _iti.Student_courses
               .FirstOrDefaultAsync(sc => sc.stdid == studentCourse.stdid &&
                                      sc.crsid == studentCourse.crsid);

            if (existing != null)
                {
                    existing.grade+= studentCourse.grade;
                    existing.comments = studentCourse.comments;
                    existing.isActive = studentCourse.isActive;
                    await _iti.SaveChangesAsync();
                }

        }
    }
}
