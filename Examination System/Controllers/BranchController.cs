using Examination_System.Data.UnitOfWorks;
using Examination_System.Models;
using Examination_System.Models.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Examination_System.Controllers
{
    public class BranchController : Controller
    {
        private readonly UnitOfWork _unit;

        public BranchController(UnitOfWork unit)
        {
            _unit = unit;
        }

        // GET: Branch
        public IActionResult Index()
        {
            var branches = _unit.BranchRepo.GetAll();
            var branchViewModels = branches.Select(b => new BranchViewModel
            {
                BranchId = b.branch_id,
                Name = b.name,
                Location = b.location,
                ManagerId = b.ManagerId,
                ManagerName = b.ManagerId.HasValue
                    ? _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == b.ManagerId)?.ins.name
                    : "not assigned",
                IsActive = b.isActive
            }).ToList();

            return View(branchViewModels);
        }

        // GET: Branch/Details/id
        public IActionResult Details(int id)
        {
            var branch = _unit.BranchRepo.GetById(id);
            if (branch == null)
                return NotFound();

            var branchViewModel = new BranchViewModel
            {
                BranchId = branch.branch_id,
                Name = branch.name,
                Location = branch.location,
                ManagerId = branch.ManagerId,
                ManagerName = branch.ManagerId.HasValue
                    ? _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == branch.ManagerId)?.ins.name
                    : "not assigned",
                IsActive = branch.isActive
            };

            return View(branchViewModel);
        }

        // GET: Branch/Create
        public IActionResult Create()
        {
            var instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            var branchViewModel = new BranchViewModel
            {
                Instructors = instructors,
                IsActive = true
            };

            return View(branchViewModel);
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BranchViewModel branchViewModel)
        {
            if (_unit._dbContext.Branches.Any(b => b.name == branchViewModel.Name && b.isActive))
            {
                ModelState.AddModelError("Name", "this branch already exists");
            }
            if (_unit._dbContext.Branches.Any(b => b.location == branchViewModel.Location && b.isActive))
            {
                ModelState.AddModelError("Location", "this Location already exists");
            }

            if (ModelState.IsValid)
            {
                var branch = new Branch
                {
                    name = branchViewModel.Name,
                    location = branchViewModel.Location,
                    ManagerId = branchViewModel.ManagerId,
                    isActive = true
                };

                if (branch.ManagerId.HasValue)
                {
                    var instructor = _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == branch.ManagerId && i.isActive && i.ins.isActive);
                    if (instructor == null)
                    {
                        ModelState.AddModelError("ManagerId", "not valid.");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _unit.BranchRepo.Create(branch);
                        _unit.Save();

                        if (branch.ManagerId.HasValue)
                        {
                            var user = _unit._dbContext.Users.FirstOrDefault(u => u.id == branch.ManagerId && u.isActive);
                            if (user != null)
                            {
                                user.branch_id = branch.branch_id;
                                _unit.Save();
                            }
                            else
                            {
                                Debug.WriteLine($"No active user found for ManagerId: {branch.ManagerId}");
                            }
                        }

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error saving branch: {ex.Message}");
                        ModelState.AddModelError("", "something went wrong");
                    }
                }
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            branchViewModel.Instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            return View(branchViewModel);
        }

        // GET: Branch/Edit/id
        public IActionResult Edit(int id)
        {
            var branch = _unit.BranchRepo.GetById(id);
            if (branch == null)
                return NotFound();

            var instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive && i.ins.branch_id == id)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            var branchViewModel = new BranchViewModel
            {
                BranchId = branch.branch_id,
                Name = branch.name,
                Location = branch.location,
                ManagerId = branch.ManagerId,
                IsActive = branch.isActive,
                Instructors = instructors
            };

            return View(branchViewModel);
        }

        // POST: Branch/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BranchViewModel branchViewModel)
        {
            if (id != branchViewModel.BranchId)
                return BadRequest();

            if (_unit._dbContext.Branches.Any(b => b.name == branchViewModel.Name && b.isActive && b.branch_id != branchViewModel.BranchId))
            {
                ModelState.AddModelError("Name", "this branch already exists");
            }
            if (_unit._dbContext.Branches.Any(b => b.location == branchViewModel.Location && b.isActive && b.branch_id != branchViewModel.BranchId))
            {
                ModelState.AddModelError("Location", "this Location already exists");
            }

            if (ModelState.IsValid)
            {
                var branch = new Branch
                {
                    branch_id = branchViewModel.BranchId,
                    name = branchViewModel.Name,
                    location = branchViewModel.Location,
                    ManagerId = branchViewModel.ManagerId,
                    isActive = branchViewModel.IsActive
                };

                if (branch.ManagerId.HasValue)
                {
                    var instructor = _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == branch.ManagerId && i.isActive && i.ins.isActive && i.ins.branch_id == id);
                    if (instructor == null)
                    {
                        ModelState.AddModelError("ManagerId", "not valid");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _unit.BranchRepo.Update(id, branch);
                        _unit.Save();

                        if (branch.ManagerId.HasValue)
                        {
                            var user = _unit._dbContext.Users.FirstOrDefault(u => u.id == branch.ManagerId && u.isActive);
                            if (user != null)
                            {
                                user.branch_id = branch.branch_id;
                                _unit.Save();
                            }
                            else
                            {
                                Debug.WriteLine($"No active user found for ManagerId: {branch.ManagerId}");
                            }
                        }

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error updating branch: {ex.Message}");
                        ModelState.AddModelError("", "something went wrong");
                    }
                }
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            branchViewModel.Instructors = _unit._dbContext.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive && i.ins.isActive && i.ins.branch_id == id)
                .Select(i => new SelectListItem
                {
                    Value = i.insid.ToString(),
                    Text = i.ins.name
                }).ToList();

            return View(branchViewModel);
        }

        // GET: Branch/Delete/id
        public IActionResult Delete(int id)
        {
            var branch = _unit.BranchRepo.GetById(id);
            if (branch == null)
                return NotFound();

            var branchViewModel = new BranchViewModel
            {
                BranchId = branch.branch_id,
                Name = branch.name,
                Location = branch.location,
                ManagerId = branch.ManagerId,
                ManagerName = branch.ManagerId.HasValue
                    ? _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == branch.ManagerId)?.ins.name
                    : "not assigned",
                IsActive = branch.isActive
            };

            return View(branchViewModel);
        }

        // POST: Branch/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _unit.BranchRepo.Delete(id);
                _unit.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting branch: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}