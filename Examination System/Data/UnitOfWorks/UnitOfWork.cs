using Examination_System.Data.Repositories;
using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.UnitOfWorks
{
    public class UnitOfWork
    {
        private readonly Exam_sysContext _context;
        public readonly DepartmentRepository DepartmentRepo;

        //DepartmentRepo DepartmentRepo { get; set; }
        private BranchRepo _branchRepo;



        public Exam_sysContext _dbContext { get; }
        public UnitOfWork(Exam_sysContext context) {
            _context = context;
            DepartmentRepo = new DepartmentRepository(context);
            this._dbContext = context; 
        }

        //public DepartmentRepo repo
        //{
        //    get
        //    {
        //        DepartmentRepo = new DepartmentRepo(_dbContext);
        //        return DepartmentRepo;
        //    }
        //}
        public BranchRepo BranchRepo
        {
            get
            {
                _branchRepo ??= new BranchRepo(_dbContext);
                return _branchRepo;
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
