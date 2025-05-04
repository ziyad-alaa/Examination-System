using Examination_System.Data.Repositories;
using Examination_System.Models;
using Examination_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.UnitOfWorks
{
    public class UnitOfWork
    {
        DepartmentRepo DepartmentRepo { get; set; }
        BranchesREpo BranchrEpo { get; set; }

        StudentRepo StudentRepo { get; set; }

        Exam_sysContext _dbContext;
        public UnitOfWork(Exam_sysContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public DepartmentRepo DeptRepo
        {
            get
            {
                DepartmentRepo = new DepartmentRepo(_dbContext);
                return DepartmentRepo;
            }
        }
        public StudentRepo StdRepo
        {
            get
            {
                StudentRepo = new StudentRepo(_dbContext);
                return StudentRepo;
            }
        }
        public BranchesREpo brancRepo
        {
            get
            {
                BranchrEpo = new BranchesREpo(_dbContext);
                return BranchrEpo;
            }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }



    }
}
