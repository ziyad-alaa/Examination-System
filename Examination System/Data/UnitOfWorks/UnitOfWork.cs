using Examination_System.Data.Repositories;
using Examination_System.Models;
using Examination_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.UnitOfWorks
{
    public class UnitOfWork
    {
<<<<<<< HEAD
        DepartmentRepo DepartmentRepo { get; set; }
        BranchesREpo BranchrEpo { get; set; }

        StudentRepo StudentRepo { get; set; }

        Exam_sysContext _dbContext;
        public UnitOfWork(Exam_sysContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public DepartmentRepo DeptRepo
=======
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
>>>>>>> main
        {
            get
            {
                _branchRepo ??= new BranchRepo(_dbContext);
                return _branchRepo;
            }
        }
<<<<<<< HEAD
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
=======
>>>>>>> main

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
