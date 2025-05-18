using Examination_System.Data.Repositories;
using Examination_System.Models;
using Examination_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.UnitOfWorks
{
    public class UnitOfWork
    {
        DepartmentRepo DepartmentRepo { get; set; }
        UsersRepo usersRepo { get; set; }
        ExamRepository examRepository { get; set; }
        CourseRepository courseRepository { get; set; }

        CourseDeptRepository courseDeptRepository { get; set; }


        DepartmentRepo DepartmentRepos { get; set; }
        BranchesREpo BranchrEpos { get; set; }

        StudentRepo StudentRepo { get; set; }

        //Exam_sysContext _dbContext;
        //public UnitOfWork(Exam_sysContext _dbContext)
        //{
        //    this._dbContext = _dbContext;
        //}

        public DepartmentRepo DeptRepo;

        //private readonly Exam_sysContext _context;
        public  DepartmentRepository DepartmentRepo { get; set; }
        //DepartmentRepo DepartmentRepo { get; set; }
        private BranchRepo _branchRepo;



        public Exam_sysContext _dbContext { get; }
        public UnitOfWork(Exam_sysContext context) {
            _dbContext = context;
            DepartmentRepo = new DepartmentRepository(context);
            _dbContext = context; 
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

        public UsersRepo userRepo
        {
            get
            {
                usersRepo= new UsersRepo(_dbContext);
                return usersRepo;


            }
        }

        public ExamRepository Exam
        {
            get 
            {   examRepository = new ExamRepository(_dbContext);
                return examRepository; 
            }
        }
        public CourseRepository Course
        {
            get
            {
                courseRepository = new CourseRepository(_dbContext);
                return courseRepository;
            }
        }
        public CourseDeptRepository Course_Dept 
        {
            get
            { 
                courseDeptRepository=new CourseDeptRepository(_dbContext);
                return courseDeptRepository;
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
                BranchrEpos = new BranchesREpo(_dbContext);
                return BranchrEpos;
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
