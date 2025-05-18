using Examination_System.Data.UnitOfWorks;
using Examination_System.Model.Models;
using Examination_System.Models;
using Examination_System.Models.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace Examination_System.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly UnitOfWork _unit;

        public DepartmentController(UnitOfWork unit)
        {
            _unit = unit;
        }

        // GET: Department
        public IActionResult Index()
        {
            var departments = _unit.DepartmentRepo.GetAll();
            var departmentViewModels = departments.Select(d => new DepartmentViewModel
            {
                DepartmentId = d.dept_id,
                Name = d.name,
                BranchId = _unit._dbContext.branch_depts
                    .FirstOrDefault(bd => bd.dept_id == d.dept_id && bd.isActive)?.branch_id ?? 0,
                BranchName = _unit._dbContext.branch_depts
                    .Include(bd => bd.Branch)
                    .FirstOrDefault(bd => bd.dept_id == d.dept_id && bd.isActive)?.Branch.name ?? "Not assigned",
                ManagerId = _unit._dbContext.branch_depts
                    .FirstOrDefault(bd => bd.dept_id == d.dept_id && bd.isActive)?.ManagerId,
                ManagerName = _unit._dbContext.branch_depts
                    .Include(bd => bd.Manager)
                    .ThenInclude(m => m.ins)
                    .FirstOrDefault(bd => bd.dept_id == d.dept_id && bd.isActive)?.Manager?.ins.name ?? "Not assigned",
                IsActive = d.isActive
            }).ToList();

            return View(departmentViewModels);
        }

        // GET: Department/Details/id
        public IActionResult Details(int id)
        {
            var department = _unit.DepartmentRepo.GetById(id);
            if (department == null)
                return NotFound();

            var branchDept = _unit._dbContext.branch_depts
                .Include(bd => bd.Branch)
                .Include(bd => bd.Manager)
                .ThenInclude(m => m.ins)
                .FirstOrDefault(bd => bd.dept_id == id && bd.isActive);

            var departmentViewModel = new DepartmentViewModel
            {
                DepartmentId = department.dept_id,
                Name = department.name,
                BranchId = branchDept?.branch_id ?? 0,
                BranchName = branchDept?.Branch?.name ?? "Not assigned",
                ManagerId = branchDept?.ManagerId,
                ManagerName = branchDept?.Manager?.ins.name ?? "Not assigned",
                IsActive = department.isActive
            };

            return View(departmentViewModel);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            var branches = _unit._dbContext.Branches
                .Where(b => b.isActive)
                .Select(b => new SelectListItem
                {
                    Value = b.branch_id.ToString(),
                    Text = b.name
                }).ToList();

            var departmentViewModel = new DepartmentViewModel
            {
                Branches = branches,
                IsActive = true,
                Instructors = new List<SelectListItem>() 
            };

            return View(departmentViewModel);
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            //if (_unit._dbContext.Departments.Any(d => d.name == departmentViewModel.Name && d.isActive))
            //{
            //    ModelState.AddModelError("Name", "Department name already exists");
            //}

            if (_unit._dbContext.branch_depts.Any(bd => bd.dept_id == departmentViewModel.DepartmentId && bd.branch_id == departmentViewModel.BranchId && bd.isActive))
            {
                ModelState.AddModelError("BranchId", "Department already exists in this branch");
            }

            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    name = departmentViewModel.Name,
                    isActive = true
                };

                if (departmentViewModel.ManagerId.HasValue)
                {
                    var instructor = _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == departmentViewModel.ManagerId && i.isActive && i.ins.isActive && i.ins.branch_id == departmentViewModel.BranchId);
                    if (instructor == null)
                    {
                        ModelState.AddModelError("ManagerId", "Selected manager is not valid or not associated with this branch");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _unit.DepartmentRepo.Create(department);
                        _unit.Save();

                        var branchDept = new Branch_Dept
                        {
                            branch_id = departmentViewModel.BranchId,
                            dept_id = department.dept_id,
                            ManagerId = departmentViewModel.ManagerId,
                            isActive = true
                        };
                        _unit._dbContext.branch_depts.Add(branchDept);
                        _unit.Save();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error saving department: {ex.Message}");
                        ModelState.AddModelError("", "An error occurred while saving the department. Please try again.");
                    }
                }
            }

            departmentViewModel.Branches = _unit._dbContext.Branches
                .Where(b => b.isActive)
                .Select(b => new SelectListItem
                {
                    Value = b.branch_id.ToString(),
                    Text = b.name
                }).ToList();

            departmentViewModel.Instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive && i.ins.branch_id == departmentViewModel.BranchId)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            return View(departmentViewModel);
        }

        // GET: Department/Edit/id
        public IActionResult Edit(int id)
        {
            var department = _unit.DepartmentRepo.GetById(id);
            if (department == null)
                return NotFound();

            var branchDept = _unit._dbContext.branch_depts
                .FirstOrDefault(bd => bd.dept_id == id && bd.isActive);

            var branches = _unit._dbContext.Branches
                .Where(b => b.isActive)
                .Select(b => new SelectListItem
                {
                    Value = b.branch_id.ToString(),
                    Text = b.name
                }).ToList();

            var instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive && i.ins.branch_id ==branchDept.branch_id)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            var departmentViewModel = new DepartmentViewModel
            {
                DepartmentId = department.dept_id,
                Name = department.name,
                BranchId = branchDept?.branch_id ?? 0,
                ManagerId = branchDept?.ManagerId,
                IsActive = department.isActive,
                Branches = branches,
                Instructors = instructors
            };

            return View(departmentViewModel);
        }

        // POST: Department/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DepartmentViewModel departmentViewModel)
        {
            if (id != departmentViewModel.DepartmentId)
                return BadRequest();

            if (_unit._dbContext.Departments.Any(d => d.name == departmentViewModel.Name && d.isActive && d.dept_id != id))
            {
                ModelState.AddModelError("Name", "Department name already exists");
            }

            if (_unit._dbContext.branch_depts.Any(bd => bd.dept_id != id && bd.branch_id == departmentViewModel.BranchId && bd.isActive))
            {
                ModelState.AddModelError("BranchId", "Department already exists in this branch");
            }

            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    dept_id = departmentViewModel.DepartmentId,
                    name = departmentViewModel.Name,
                    isActive = departmentViewModel.IsActive
                };

                if (departmentViewModel.ManagerId.HasValue)
                {
                    var instructor = _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == departmentViewModel.ManagerId && i.isActive && i.ins.isActive && i.ins.branch_id == departmentViewModel.BranchId);
                    if (instructor == null)
                    {
                        ModelState.AddModelError("ManagerId", "Selected manager is not valid or not associated with this branch");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _unit.DepartmentRepo.Update(id, department);
                        _unit.Save();

                        var branchDept = _unit._dbContext.branch_depts
                            .FirstOrDefault(bd => bd.dept_id == id && bd.isActive);
                        if (branchDept != null)
                        {
                            branchDept.branch_id = departmentViewModel.BranchId;
                            branchDept.ManagerId = departmentViewModel.ManagerId;
                            branchDept.isActive = departmentViewModel.IsActive;
                        }
                        else
                        {
                            branchDept = new Branch_Dept
                            {
                                branch_id = departmentViewModel.BranchId,
                                dept_id = id,
                                ManagerId = departmentViewModel.ManagerId,
                                isActive = true
                            };
                            _unit._dbContext.branch_depts.Add(branchDept);
                        }
                        _unit.Save();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error updating department: {ex.Message}");
                        ModelState.AddModelError("", "An error occurred while updating the department. Please try again.");
                    }
                }
            }

            departmentViewModel.Branches = _unit._dbContext.Branches
                .Where(b => b.isActive)
                .Select(b => new SelectListItem
                {
                    Value = b.branch_id.ToString(),
                    Text = b.name
                }).ToList();

            departmentViewModel.Instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive && i.ins.branch_id == departmentViewModel.BranchId)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            return View(departmentViewModel);
        }

        // GET: Department/Delete/id
        public IActionResult Delete(int id)
        {
            var department = _unit.DepartmentRepo.GetById(id);
            if (department == null)
                return NotFound();

            var branchDept = _unit._dbContext.branch_depts
                .Include(bd => bd.Branch)
                .Include(bd => bd.Manager)
                .ThenInclude(m => m.ins)
                .FirstOrDefault(bd => bd.dept_id == id && bd.isActive);

            var departmentViewModel = new DepartmentViewModel
            {
                DepartmentId = department.dept_id,
                Name = department.name,
                BranchId = branchDept?.branch_id ?? 0,
                BranchName = branchDept?.Branch?.name ?? "Not assigned",
                ManagerId = branchDept?.ManagerId,
                ManagerName = branchDept?.Manager?.ins.name ?? "Not assigned",
                IsActive = department.isActive
            };

            return View(departmentViewModel);
        }

        // POST: Department/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Soft delete Department
                _unit.DepartmentRepo.Delete(id);

                // Soft delete Branch_Dept
                var branchDept = _unit._dbContext.branch_depts
                    .FirstOrDefault(bd => bd.dept_id == id && bd.isActive);
                if (branchDept != null)
                {
                    branchDept.isActive = false;
                }

                _unit.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting department: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetInstructorsByBranch(int branchId)
        {
            var instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive && i.ins.branch_id == branchId)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            return Json(instructors);
        }
    }
}