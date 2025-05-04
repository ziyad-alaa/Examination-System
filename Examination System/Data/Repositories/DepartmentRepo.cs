using Examination_System.Data.Interfaces;

namespace Examination_System.Data.Repositories
{
    public class DepartmentRepo : IService<Department>
    {
        public Exam_sysContext _dbContext;

        public DepartmentRepo(Exam_sysContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public void Create(Department entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Department> GetAll()
        {
            return _dbContext.Departments.Where(d=>d.isActive==true).ToList();
        }

        public List<Department> GetAllNotActive()
        {
            throw new NotImplementedException();
        }

        public Department GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Department GetNotactiveById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, Department entity)
        {
            throw new NotImplementedException();
        }
    }
}
