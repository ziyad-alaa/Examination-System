using Examination_System.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.Repositories
{
    public class StudentRepo// : IService<Student>
    {
        Exam_sysContext _iti;
        public StudentRepo(Exam_sysContext _iti)
        {
            this._iti = _iti;
        }
        public void Create(Student entity)
        {
            _iti.Students.Add(entity);

        }

        public void Delete(int id)
        {
            Student student = GetById(id);
            student.std.isActive= false;
        }

        public List<Student> GetAll()
        {
            List<Student> Students = _iti.Students.Where(s=>s.std.isActive==true).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).ToList();
            return Students;
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Student> GetAllNotActive()
        {
            List<Student> StudentNotactive= _iti.Students.Where(s => s.std.isActive == false).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).ToList();
            return StudentNotactive;
        }

        Task<Student> IService<Student>.GetById(int id)
        {
            Student student = _iti.Students.Where(s => s.std.isActive == true).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).FirstOrDefault(s => s.stdid == id);
            return student;
        }

        public Student GetNotactiveById(int id)
        {
            Student student = _iti.Students.Where(s => s.std.isActive == false).Include(s => s.std).Include(b => b.std.branch).Include(d => d.std.dept).FirstOrDefault(s => s.stdid == id);
            return student;
        }

        public void Update(int id, Student entity)
        {
            Student std = GetById(id);
            if (std != null)
            {
                _iti.Students.Update(std);
            }

        }

        Task<Student> IService<Student>.Create(Student entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Student>> IService<Student>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<Student> IService<Student>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        Task<Student> IService<Student>.Update(int id, Student entity)
        {
            throw new NotImplementedException();
        }
    }
}
