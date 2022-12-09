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
public class GoodsController : BaseController
{

    public GoodsController(MvcAndyContext context) : base(context)
    {
    }

    // GET: Goods
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        var model = from m in _context.GoodsModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        if (!String.IsNullOrEmpty(searchString))
        {
            model = model.Where(s => s.Title!.Contains(searchString));
        }
        model = model.OrderByDescending(s => s.Id);

        int pageSize = 10;
        var list = await PaginatedList<GoodsModel>.CreateAsync(model.AsNoTracking(), pageNumber ?? 1, pageSize);
        List<int> ids = GoodsService.CreateObject().getListIds(list);
        ViewData["cateList"] = await GoodsService.CreateObject().setDbContext(_context)
            .getGoodsCateList(ids);

        return View(list);

    }

    // GET: Goods/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.GoodsModels == null)
        {
            return NotFound();
        }

        var goodsModel = await _context.GoodsModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (goodsModel == null || goodsModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        ViewData["list"] = await GoodsService.CreateObject().setDbContext(_context)
            .getGoodsCateList(new List<int> { goodsModel.CateId });

        return View(goodsModel);
    }

    // GET: Goods/Create
    public async Task<IActionResult> CreateAsync()
    {
        ViewData["list"] = await GoodsService.CreateObject().setDbContext(_context).getGoodsCateList();
        return View();
    }

    // POST: Goods/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,SubTitle,CateId,SerialNumber,Sku,Logo,Content,IsOnSale")] GoodsModel goodsModel)
    {
        if (!string.IsNullOrEmpty(goodsModel.SerialNumber))
        {
            //判断sn不能重复.
            var modelTest = await _context.GoodsModels
                   .FirstOrDefaultAsync(m => m.SerialNumber == goodsModel.SerialNumber);
            if (modelTest != null)
            {
                ModelState.AddModelError("SerialNumber", "货号已存在");
                return View(goodsModel);
            }
        }
        if (ModelState.IsValid)
        {
            goodsModel.SortBy = 100;
            goodsModel.Quantity = goodsModel.TotalQuantity = 0;
            goodsModel.UniqueId = CommonService.CreateObject().uniqid("gd", false);
            goodsModel.TimeOfDay = int.Parse(DateTime.Today.ToString("yyyymmdd"));//20221207
            goodsModel.TimeOfMonth = int.Parse(DateTime.Today.ToString("yyyymm"));//202212

            goodsModel.CreateAt = goodsModel.UpdateAt = DateTime.Now;
            goodsModel.Deleted = DeleteType.Enable;
            _context.Add(goodsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(goodsModel);
    }

    // GET: Goods/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.GoodsModels == null)
        {
            return NotFound();
        }

        var goodsModel = await _context.GoodsModels.FindAsync(id);
        if (goodsModel == null || goodsModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        ViewData["list"] = await GoodsService.CreateObject().setDbContext(_context).getGoodsCateList();
        return View(goodsModel);
    }

    // POST: Goods/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,SubTitle,Sku,CateId,Logo,Content,IsOnSale")] GoodsModel goodsModel)
    {
        if (id != goodsModel.Id)
        {
            return NotFound();
        }
        var tmpModel = await _context.GoodsModels.FindAsync(id);
        if (tmpModel == null || tmpModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                tmpModel.Title = goodsModel.Title;
                tmpModel.SubTitle = goodsModel.SubTitle;
                tmpModel.CateId = goodsModel.CateId;
                // tmpModel.Logo = goodsModel.Logo;
                tmpModel.Content = goodsModel.Content;
                tmpModel.IsOnSale = goodsModel.IsOnSale;
                tmpModel.Sku = goodsModel.Sku;

                tmpModel.UpdateAt = DateTime.Now;
                _context.Update(tmpModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodsModelExists(goodsModel.Id))
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
        return View(goodsModel);
    }

    // GET: Goods/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.GoodsModels == null)
        {
            return NotFound();
        }

        var goodsModel = await _context.GoodsModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (goodsModel == null || goodsModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }

        ViewData["list"] = await GoodsService.CreateObject().setDbContext(_context)
                    .getGoodsCateList(new List<int> { goodsModel.CateId });
        return View(goodsModel);
    }

    // POST: Goods/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.GoodsModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.GoodsModels'  is null.");
        }
        var goodsModel = await _context.GoodsModels.FindAsync(id);
        if (goodsModel != null && goodsModel.Deleted == DeleteType.Enable)
        {
            goodsModel.UpdateAt = DateTime.Now;
            goodsModel.Deleted = DeleteType.Disable;
            _context.Update(goodsModel);
            // _context.GoodsModels.Remove(goodsModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GoodsModelExists(int id)
    {
        return (_context.GoodsModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }


    // GET: Goods
    public async Task<IActionResult> ListJson(string goodsSn, string goodsTitle, int? start, int? length)
    {
        var model = _context.GoodsModels.Where(m => m.Deleted.Equals(DeleteType.Enable));
        if (!String.IsNullOrEmpty(goodsSn))
        {
            model = model.Where(s => s.SerialNumber!.Contains(goodsSn));
        }
        if (!String.IsNullOrEmpty(goodsTitle))
        {
            model = model.Where(s => s.Title!.Contains(goodsTitle));
        }
        model = model.OrderByDescending(s => s.Id);

        if (start == null || start <= 0)
        {
            start = 0;
        }
        if (length == null || length <= 0 || length > 100)
        {
            length = 10;
        }
        var list = await model.AsNoTracking().Skip((int)start).Take((int)length)
        .Select(m => new
        {
            id = m.Id,
            title = m.Title,
            sn = m.SerialNumber,
            sku = m.Sku,
            CateId = m.CateId,
            quantity = m.Quantity,
            totalQuantity = m.TotalQuantity,
        })
        .ToListAsync();
        List<int> ids = new List<int>();
        foreach (var item in list)
        {
            if (!ids.Contains(item.CateId))
            {
                ids.Add(item.CateId);
            }
        }
        var cateList = await GoodsService.CreateObject().setDbContext(_context).getGoodsCateList(ids);
        // foreach (var item in list)
        // {
        //     if (cateList.ContainsKey(item.CateId))
        //     {
        //         item.CateName = cateList[item.CateId];
        //     }
        // }
        // return Json(list);
        return Json(CommonService.CreateObject().jsonFormat(0, "success",
            new Dictionary<string, object>() { { "list", list }, { "cateList", cateList } }
        ));
        // return Json(CommonService.CreateObject().jsonFormat(0, "success", list));
    }

}
