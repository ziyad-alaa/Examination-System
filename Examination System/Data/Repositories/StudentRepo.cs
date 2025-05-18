using Examination_System.Models;

namespace Examination_System.Data.Repositories
{
    public class StudentRepo : IService<Student>
    {
        public Student Create(Student entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Student GetAll()
        {
            throw new NotImplementedException();
        }

        public Student GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Student Update(int id, Student entity)
        {
            throw new NotImplementedException();
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
