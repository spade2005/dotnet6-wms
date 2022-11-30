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

public class UserController : BaseController
{

    public UserController(MvcAndyContext context) : base(context)
    {
    }

    // GET: User
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {

        var model = from m in _context.UserModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        if (!String.IsNullOrEmpty(searchString))
        {
            model = model.Where(s => s.UserName!.Contains(searchString));
        }
        model = model.OrderByDescending(s => s.Id);

        int pageSize = 10;
        var list = await PaginatedList<UserModel>.CreateAsync(model.AsNoTracking(), pageNumber ?? 1, pageSize);
        List<int> ids = UserService.CreateObject().getListIds(list);

        ViewData["roleList"] = await UserService.CreateObject().setDbContext(_context).getRoleList(ids);

        return View(list);

        // return _context.UserModels != null ?
        //             View(await _context.UserModels.ToListAsync()) :
        //             Problem("Entity set 'MvcAndyContext.UserModels'  is null.");
    }

    // GET: User/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.UserModels == null)
        {
            return NotFound();
        }

        var userModel = await _context.UserModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (userModel == null || userModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        ViewData["roleList"] = await UserService.CreateObject().setDbContext(_context).getRoleList(new List<int> { userModel.RoleId });

        return View(userModel);
    }

    // GET: User/Create
    public async Task<IActionResult> Create()
    {
        ViewData["list"] = await UserService.CreateObject().setDbContext(_context).getRoleList();
        return View();
    }

    // POST: User/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,UserName,UserPass,Phone,Email,NickName,RoleId,Status")] UserModel userModel)
    {
        if (!string.IsNullOrEmpty(userModel.UserName))
        {
            var modelTest = await _context.UserModels
                   .FirstOrDefaultAsync(m => m.UserName == userModel.UserName);
            if (modelTest != null)
            {
                ModelState.AddModelError("UserName", "账号已存在");
                return View(userModel);
            }
        }
        if (ModelState.IsValid)
        {
            userModel.UserPass = BCrypt.Net.BCrypt.HashPassword(userModel.UserPass, BCrypt.Net.BCrypt.GenerateSalt(10, 'a'));
            userModel.CreateAt = userModel.UpdateAt = DateTime.Now;
            userModel.Deleted = DeleteType.Enable;
            _context.Add(userModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(userModel);
    }

    // GET: User/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.UserModels == null)
        {
            return NotFound();
        }

        var userModel = await _context.UserModels.FindAsync(id);
        if (userModel == null || userModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        ViewData["list"] = await UserService.CreateObject().setDbContext(_context).getRoleList();
        return View(userModel);
    }

    // POST: User/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,UserPass,Phone,Email,NickName,RoleId,Status")] UserModel userModel)
    {
        if (id != userModel.Id)
        {
            return NotFound();
        }
        var tmpModel = await _context.UserModels.FindAsync(id);
        if (tmpModel == null || tmpModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (!string.IsNullOrEmpty(userModel.UserPass))
                {
                    tmpModel.UserPass = BCrypt.Net.BCrypt.HashPassword(userModel.UserPass, BCrypt.Net.BCrypt.GenerateSalt(10, 'a'));
                }
                tmpModel.Phone = userModel.Phone;
                tmpModel.Email = userModel.Email;
                tmpModel.NickName = userModel.NickName;
                tmpModel.RoleId = userModel.RoleId;
                tmpModel.Status = userModel.Status;
                _context.Update(tmpModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserModelExists(userModel.Id))
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
        return View(userModel);
    }

    // GET: User/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.UserModels == null)
        {
            return NotFound();
        }

        var userModel = await _context.UserModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (userModel == null || userModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        ViewData["roleList"] = await UserService.CreateObject().setDbContext(_context).getRoleList(new List<int> { userModel.RoleId });
        return View(userModel);
    }

    // POST: User/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.UserModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.UserModels'  is null.");
        }
        var userModel = await _context.UserModels.FindAsync(id);
        if (userModel != null && userModel.Deleted == DeleteType.Enable)
        {
            userModel.Deleted = DeleteType.Disable;
            userModel.UpdateAt = DateTime.Now;
            _context.Update(userModel);
            // _context.UserModels.Remove(userModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserModelExists(int id)
    {
        return (_context.UserModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}

