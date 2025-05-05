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
                    : "غير محدد",
                IsActive = b.isActive
            }).ToList();

            return View(branchViewModels);
        }

        // GET: Branch/Details/5
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
                    : "غير محدد",
                IsActive = branch.isActive
            };

            return View(branchViewModel);
        }

        // GET: Branch/Create
        public IActionResult Create()
        {
            // جلب المدربين النشطين
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
            // التحقق من تفرّد Name وLocation
            if (_unit._dbContext.Branches.Any(b => b.name == branchViewModel.Name && b.isActive))
            {
                ModelState.AddModelError("Name", "اسم الفرع موجود بالفعل.");
            }
            if (_unit._dbContext.Branches.Any(b => b.location == branchViewModel.Location && b.isActive))
            {
                ModelState.AddModelError("Location", "موقع الفرع موجود بالفعل.");
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

                // التحقق من أن ManagerId صالح (نشط)
                if (branch.ManagerId.HasValue)
                {
                    var instructor = _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == branch.ManagerId && i.isActive && i.ins.isActive);
                    if (instructor == null)
                    {
                        ModelState.AddModelError("ManagerId", "المدرب المختار غير صالح أو غير نشط.");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _unit.BranchRepo.Create(branch);
                        _unit.Save();

                        // تحديث branch_id للمستخدم اللي بقى مدير
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
                        ModelState.AddModelError("", "حدث خطأ أثناء حفظ الفرع. حاول مرة أخرى.");
                    }
                }
            }

            // طباعة أخطاء ModelState للتحقق
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            // إعادة تحميل قائمة المدربين في حالة وجود خطأ
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

        // GET: Branch/Edit/5
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

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BranchViewModel branchViewModel)
        {
            if (id != branchViewModel.BranchId)
                return BadRequest();

            // التحقق من تفرّد Name وLocation
            if (_unit._dbContext.Branches.Any(b => b.name == branchViewModel.Name && b.isActive && b.branch_id != branchViewModel.BranchId))
            {
                ModelState.AddModelError("Name", "اسم الفرع موجود بالفعل.");
            }
            if (_unit._dbContext.Branches.Any(b => b.location == branchViewModel.Location && b.isActive && b.branch_id != branchViewModel.BranchId))
            {
                ModelState.AddModelError("Location", "موقع الفرع موجود بالفعل.");
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

                // التحقق من أن ManagerId تابع للفرع
                if (branch.ManagerId.HasValue)
                {
                    var instructor = _unit._dbContext.Instructors
                        .Include(i => i.ins)
                        .FirstOrDefault(i => i.insid == branch.ManagerId && i.isActive && i.ins.isActive && i.ins.branch_id == id);
                    if (instructor == null)
                    {
                        ModelState.AddModelError("ManagerId", "المدرب المختار غير صالح أو غير مرتبط بهذا الفرع.");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _unit.BranchRepo.Update(id, branch);
                        _unit.Save();

                        // تحديث branch_id للمستخدم اللي بقى مدير
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
                        ModelState.AddModelError("", "حدث خطأ أثناء تحديث الفرع. حاول مرة أخرى.");
                    }
                }
            }

            // طباعة أخطاء ModelState للتحقق
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            // إعادة تحميل قائمة المدربين
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

        // GET: Branch/Delete/5
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
                    : "غير محدد",
                IsActive = branch.isActive
            };

            return View(branchViewModel);
        }

        // POST: Branch/Delete/5
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