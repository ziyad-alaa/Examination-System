using Examination_System.Data.UnitOfWorks;
using Examination_System.Models;
using global::Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Examination_System.Data.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly Exam_sysContext _context;

        public DepartmentRepository(Exam_sysContext context)
        {
            _context = context;
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments
                .Where(d => d.isActive)
                .ToList();
        }

        public Department GetById(int id)
        {
            return _context.Departments
                .FirstOrDefault(d => d.dept_id == id && d.isActive);
        }

        public void Create(Department department)
        {
            _context.Departments.Add(department);
        }

        public void Update(int id, Department department)
        {
            var existing = _context.Departments.FirstOrDefault(d => d.dept_id == id && d.isActive);
            if (existing != null)
            {
                existing.name = department.name;
                existing.isActive = department.isActive;
            }
        }

        public void Delete(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.dept_id == id && d.isActive);
            if (department != null)
            {
                department.isActive = false;
            }
        }
    }

    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetAll();
        Department GetById(int id);
        void Create(Department department);
        void Update(int id, Department department);
        void Delete(int id);
    }
}
