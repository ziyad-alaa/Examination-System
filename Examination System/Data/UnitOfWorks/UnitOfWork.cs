using Examination_System.Data.Repositories;
using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.UnitOfWorks
{
    public class UnitOfWork
    {
        DepartmentRepo DepartmentRepo { get; set; }

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
