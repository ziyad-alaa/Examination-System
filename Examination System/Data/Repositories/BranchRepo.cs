using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.Repositories
{
    public class BranchRepo : IService<Branch>
    {
        private readonly Exam_sysContext _context;

        public BranchRepo(Exam_sysContext context)
        {
            _context = context;
        }

        public List<Branch> GetAll()
        {
            return _context.Branches
                .Include(b => b.Manager)
                .Where(b => b.isActive)
                .ToList();
        }

        public Branch GetById(int id)
        {
            return _context.Branches
                .Include(b => b.Manager)
                .FirstOrDefault(b => b.branch_id == id && b.isActive);
        }

        public Branch Create(Branch entity)
        {
            entity.isActive = true; 
            _context.Branches.Add(entity);
            return entity;
        }

        public Branch Update(int id, Branch entity)
        {
            var existingBranch = _context.Branches.Find(id);
            if (existingBranch == null || !existingBranch.isActive)
                return null;

            existingBranch.name = entity.name;
            existingBranch.location = entity.location;
            existingBranch.ManagerId = entity.ManagerId;
            existingBranch.isActive = entity.isActive;

            _context.Branches.Update(existingBranch);
            return existingBranch;
        }

        public void Delete(int id)
        {
            var branch = _context.Branches.Find(id);
            if (branch != null && branch.isActive)
            {
                branch.isActive = false; 
                _context.Branches.Update(branch);
            }
        }
    }
}