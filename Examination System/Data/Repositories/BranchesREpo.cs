using Examination_System.Data.Interfaces;

namespace Examination_System.Repositories
{
    public class BranchesREpo : IService<Branch>
    {
        public Exam_sysContext Context { get; set; }
        public BranchesREpo(Exam_sysContext Context)
        {
            this.Context = Context;
        }
        public void Create(Branch entity)
        {
            throw new NotImplementedException();
        }

        public List<Branch> GetAll()
        {
            return Context.Branches.Where(s=>s.isActive==true).ToList();
        }

        public Branch GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, Branch entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Branch> GetAllNotActive()
        {
            throw new NotImplementedException();
        }

        public Branch GetNotactiveById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
