using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Data;
using mvc_andy.Models.com;
using mvc_andy.Services.Backend;

namespace mvc_andy.Controllers.Backend;

public class RoleController : BaseController
{

    public RoleController(MvcAndyContext context) : base(context)
    {
    }

    // GET: Role
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        var role = from m in _context.RoleModels
                       where m.Deleted.Equals(0)
                       select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                role = role.Where(s => s.Name!.Contains(searchString));
            }
            role = role.OrderByDescending(s => s.Id);

            int pageSize = 10;
            return View(await PaginatedList<RoleModel>.CreateAsync(role.AsNoTracking(), pageNumber ?? 1, pageSize));
            
        // return _context.RoleModels != null ?
        //             View(await _context.RoleModels.ToListAsync()) :
        //             Problem("Entity set 'MvcAndyContext.RoleModels'  is null.");
    }

    // GET: Role/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.RoleModels == null)
        {
            return NotFound();
        }

        var roleModel = await _context.RoleModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (roleModel == null || roleModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        return View(roleModel);
    }

    // GET: Role/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Role/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Mark")] RoleModel roleModel)
    {
        if (ModelState.IsValid)
        {
            roleModel.CreateAt = roleModel.UpdateAt = DateTime.Now;
            roleModel.Deleted = DeleteType.Enable;
            _context.Add(roleModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(roleModel);
    }

    // GET: Role/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.RoleModels == null)
        {
            return NotFound();
        }

        var roleModel = await _context.RoleModels.FindAsync(id);
        if (roleModel == null || roleModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        return View(roleModel);
    }

    // POST: Role/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Mark")] RoleModel roleModel)
    {
        if (id != roleModel.Id)
        {
            return NotFound();
        }

        var tmpRoleModel = await _context.RoleModels.FindAsync(id);
        if (tmpRoleModel == null || tmpRoleModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                tmpRoleModel.Name = roleModel.Name;
                tmpRoleModel.Mark = roleModel.Mark;
                tmpRoleModel.UpdateAt = DateTime.Now;
                _context.Update(tmpRoleModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleModelExists(roleModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(roleModel);
    }

    // GET: Role/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.RoleModels == null)
        {
            return NotFound();
        }

        var roleModel = await _context.RoleModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (roleModel == null || roleModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        return View(roleModel);
    }

    // POST: Role/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.RoleModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.RoleModels'  is null.");
        }
        var roleModel = await _context.RoleModels.FindAsync(id);
        if (roleModel != null && roleModel.Deleted == DeleteType.Enable)
        {
            roleModel.UpdateAt = DateTime.Now;
            roleModel.Deleted = DeleteType.Disable;
            _context.Update(roleModel);
            // _context.RoleModels.Remove(roleModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool RoleModelExists(int id)
    {
        return (_context.RoleModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}

