using Examination_System.Models;

namespace Examination_System.Data.Repositories
{
    public class DepartmentRepo : IService<Department>
    {
        public Exam_sysContext _dbContext;

        public DepartmentRepo(Exam_sysContext _dbContext)
        {
           this._dbContext = _dbContext;
        }
        public Department Create(Department entity)
        {
            throw new NotImplementedException();
        }

        public Department GetAll()
        {
            throw new NotImplementedException();
        }

        public Department GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Department Update(int id, Department entity)
        {
            throw new NotImplementedException();
        }
    }
}
