using Examination_System.Data.Repositories;
using Examination_System.Models;
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


        Exam_sysContext _dbContext;
        public UnitOfWork(Exam_sysContext _dbContext) {
            this._dbContext = _dbContext; 
        }

        public DepartmentRepo repo
        {
            get
            {
                DepartmentRepo = new DepartmentRepo(_dbContext);
                return DepartmentRepo;
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
