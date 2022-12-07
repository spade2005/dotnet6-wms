using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Data;
using mvc_andy.Models.wms;
using mvc_andy.Models.com;
using mvc_andy.Services.Backend;

namespace mvc_andy.Controllers.Backend;
public class GoodsCateController : BaseController
{

    public GoodsCateController(MvcAndyContext context) : base(context)
    {
    }

    // GET: GoodsCate
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        var model = from m in _context.GoodsCateModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        if (!String.IsNullOrEmpty(searchString))
        {
            model = model.Where(s => s.Name!.Contains(searchString));
        }
        model = model.OrderByDescending(s => s.Id);

        int pageSize = 10;
        return View(await PaginatedList<GoodsCateModel>.CreateAsync(model.AsNoTracking(), pageNumber ?? 1, pageSize));


        // return _context.GoodsCateModels != null ?
        // View(await _context.GoodsCateModels.ToListAsync()) :
        // Problem("Entity set 'MvcAndyContext.GoodsCateModels'  is null.");
    }

    // GET: GoodsCate/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.GoodsCateModels == null)
        {
            return NotFound();
        }

        var goodsCateModel = await _context.GoodsCateModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (goodsCateModel == null || goodsCateModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        return View(goodsCateModel);
    }

    // GET: GoodsCate/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: GoodsCate/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Mark")] GoodsCateModel goodsCateModel)
    {
        if (ModelState.IsValid)
        {
            goodsCateModel.CreateAt = goodsCateModel.UpdateAt = DateTime.Now;
            goodsCateModel.Deleted = DeleteType.Enable;
            _context.Add(goodsCateModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(goodsCateModel);
    }

    // GET: GoodsCate/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.GoodsCateModels == null)
        {
            return NotFound();
        }

        var goodsCateModel = await _context.GoodsCateModels.FindAsync(id);
        if (goodsCateModel == null || goodsCateModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        return View(goodsCateModel);
    }

    // POST: GoodsCate/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Mark")] GoodsCateModel goodsCateModel)
    {
        if (id != goodsCateModel.Id)
        {
            return NotFound();
        }

        var tmpModel = await _context.GoodsCateModels.FindAsync(id);
        if (tmpModel == null || tmpModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                tmpModel.Name = goodsCateModel.Name;
                tmpModel.Mark = goodsCateModel.Mark;
                tmpModel.UpdateAt = DateTime.Now;
                _context.Update(tmpModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodsCateModelExists(goodsCateModel.Id))
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
        return View(goodsCateModel);
    }

    // GET: GoodsCate/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.GoodsCateModels == null)
        {
            return NotFound();
        }

        var goodsCateModel = await _context.GoodsCateModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (goodsCateModel == null || goodsCateModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        return View(goodsCateModel);
    }

    // POST: GoodsCate/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.GoodsCateModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.GoodsCateModels'  is null.");
        }
        var goodsCateModel = await _context.GoodsCateModels.FindAsync(id);
        if (goodsCateModel != null && goodsCateModel.Deleted == DeleteType.Enable)
        {
            goodsCateModel.UpdateAt = DateTime.Now;
            goodsCateModel.Deleted = DeleteType.Disable;
            _context.Update(goodsCateModel);
            // _context.GoodsCateModels.Remove(goodsCateModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GoodsCateModelExists(int id)
    {
        return (_context.GoodsCateModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
