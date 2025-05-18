using Examination_System.Models;

namespace Examination_System.Data.Repositories
{
    public class CourseRepository : IService<course>
    {
        public Exam_sysContext _dbContext;

        public CourseRepository(Exam_sysContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public Task<course> Create(course entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<course>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<course> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<course> Update(int id, course entity)
        {
            throw new NotImplementedException();
        }
    }
}
