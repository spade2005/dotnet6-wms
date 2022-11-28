using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Data;
using mvc_andy.Models.com;

namespace mvc_andy.Controllers.Backend;

public class UserController : BaseController
{

     public UserController(MvcAndyContext context) : base(context)
    {
    }

    // GET: User
    public async Task<IActionResult> Index()
    {
        return _context.UserModels != null ?
                    View(await _context.UserModels.ToListAsync()) :
                    Problem("Entity set 'MvcAndyContext.UserModels'  is null.");
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
        if (userModel == null)
        {
            return NotFound();
        }

        return View(userModel);
    }

    // GET: User/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,UserName,UserPass,Phone,Email,NickName,RoleId,Status,CreateAt,UpdateAt,Deleted")] UserModel userModel)
    {
        if (ModelState.IsValid)
        {
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
        if (userModel == null)
        {
            return NotFound();
        }
        return View(userModel);
    }

    // POST: User/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,UserPass,Phone,Email,NickName,RoleId,Status,CreateAt,UpdateAt,Deleted")] UserModel userModel)
    {
        if (id != userModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(userModel);
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
        if (userModel == null)
        {
            return NotFound();
        }

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
        if (userModel != null)
        {
            _context.UserModels.Remove(userModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserModelExists(int id)
    {
        return (_context.UserModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}

