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
public class OrderOutController : BaseController
{

    public OrderOutController(MvcAndyContext context) : base(context)
    {
    }

    // GET: OrderOut
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        var model = from m in _context.OrderOutModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        if (!String.IsNullOrEmpty(searchString))
        {
            model = model.Where(s => s.OrderNo!.Contains(searchString));
        }
        model = model.OrderByDescending(s => s.Id);

        int pageSize = 10;
        var list = await PaginatedList<OrderOutModel>.CreateAsync(model.AsNoTracking(), pageNumber ?? 1, pageSize);
        // ViewData["goodsList"] =[];
        return View(list);
    }

    // GET: OrderOut/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.OrderOutModels == null)
        {
            return NotFound();
        }

        var orderOutModel = await _context.OrderOutModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderOutModel == null || orderOutModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        await setOrderGoodsAsync(orderOutModel);
        return View(orderOutModel);
    }

    // GET: OrderOut/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: OrderOut/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,OrderNo,Mark")] OrderOutModel orderOutModel, int[] goodsIds, int[] goodsNums)
    {
        if (!goodsIds.Any() || !goodsNums.Any() || goodsIds.Length != goodsNums.Length)
        {

            ModelState.AddModelError("OrderNo", "提交参数不正确");
            return View(orderOutModel);
        }
        if (!string.IsNullOrEmpty(orderOutModel.OrderNo))
        {
            var modelTest = await _context.OrderOutModels
                .Where(m => m.OrderNo == orderOutModel.OrderNo).Where(m => m.Deleted == DeleteType.Enable)
                .FirstOrDefaultAsync();
            if (modelTest != null)
            {
                ModelState.AddModelError("OrderNo", "出库单已存在");
                return View(orderOutModel);
            }
        }
        if (goodsNums.Any())
        {
            foreach (var num in goodsNums)
            {
                if (num <= 0 || num > 100000000)
                {
                    ModelState.AddModelError("OrderNo", "商品数量必须大于0的合法数值");
                    return View(orderOutModel);
                }
            }
        }
        if (goodsIds.Any())
        {
            var modelTest = from m in _context.GoodsModels
                            where m.Deleted.Equals(DeleteType.Enable)
                            select m;
            modelTest = modelTest.Where(s => goodsIds.Contains(s.Id));
            var list = await modelTest.AsNoTracking().ToListAsync();
            if (list.ToArray().Length != goodsIds.Length)
            {
                ModelState.AddModelError("OrderNo", "选择的商品不合法。请重新选择。");
                return View(orderOutModel);
            }
            foreach (var item in list)
            {
                var i = Array.IndexOf(goodsIds, item.Id);
                if (item.Quantity <= 0 || goodsNums[i] > item.Quantity)
                {
                    ModelState.AddModelError("OrderNo", "选择的商品库存不合法。请重新设置。");
                    return View(orderOutModel);
                }
            }
        }

        if (ModelState.IsValid)
        {
            orderOutModel.GoodsNum = goodsIds.Length;
            orderOutModel.OrderStatus = OrderStatusType.Pending;
            orderOutModel.StockStatus = StockType.Default;
            orderOutModel.AuditTime = orderOutModel.StockTime = DateTime.MinValue;
            orderOutModel.TimeOfDay = int.Parse(DateTime.Today.ToString("yyyymmdd"));
            orderOutModel.TimeOfMonth = int.Parse(DateTime.Today.ToString("yyyymm"));
            orderOutModel.CreateAt = orderOutModel.UpdateAt = DateTime.Now;
            orderOutModel.Deleted = DeleteType.Enable;
            _context.OrderOutModels.Add(orderOutModel);
            await _context.SaveChangesAsync();

            int i = 0;
            foreach (var item in goodsIds)
            {
                var orderGoodsModel = new OrderGoodsModel();
                orderGoodsModel.OrderId = orderOutModel.Id;
                orderGoodsModel.Type = OrderInOutType.Out;
                orderGoodsModel.GoodsId = item;
                orderGoodsModel.Quantity = goodsNums[i];
                orderGoodsModel.CreateAt = orderGoodsModel.UpdateAt = DateTime.Now;
                orderGoodsModel.Deleted = DeleteType.Enable;
                _context.OrderGoodsModels.Add(orderGoodsModel);
                i++;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(orderOutModel);
    }

    // GET: OrderOut/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.OrderOutModels == null)
        {
            return NotFound();
        }

        var orderOutModel = await _context.OrderOutModels.FindAsync(id);
        if (orderOutModel == null)
        {
            return NotFound();
        }
        await setOrderGoodsAsync(orderOutModel);
        return View(orderOutModel);
    }

    // POST: OrderOut/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,OrderNo,Mark")] OrderOutModel orderOutModel, int[] goodsIds, int[] goodsNums)
    {
        if (id != orderOutModel.Id)
        {
            return NotFound();
        }

        var tmpModel = await _context.OrderOutModels.FindAsync(id);
        if (tmpModel == null || tmpModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        if (tmpModel.OrderStatus == OrderStatusType.Success)
        {
            //审核成功，不可再次编辑
            ModelState.AddModelError("OrderNo", "订单已审核通过,无法编辑");
            return View(orderOutModel);
        }

        if (!goodsIds.Any() || !goodsNums.Any() || goodsIds.Length != goodsNums.Length)
        {
            ModelState.AddModelError("OrderNo", "提交参数不正确");
            return View(orderOutModel);
        }
        if (!string.IsNullOrEmpty(orderOutModel.OrderNo))
        {
            var modelTest = await _context.OrderOutModels
                .Where(m => m.OrderNo == orderOutModel.OrderNo).Where(m => m.Deleted == DeleteType.Enable)
                .Where(m => m.Id != tmpModel.Id)
                .FirstOrDefaultAsync();
            if (modelTest != null)
            {
                ModelState.AddModelError("OrderNo", "出库单已存在");
                return View(orderOutModel);
            }
        }
        if (goodsNums.Any())
        {
            foreach (var num in goodsNums)
            {
                if (num <= 0 || num > 100000000)
                {
                    ModelState.AddModelError("OrderNo", "商品数量必须大于0的合法数值");
                    return View(orderOutModel);
                }
            }
        }
        if (goodsIds.Any())
        {
            var modelTest = from m in _context.GoodsModels
                            where m.Deleted.Equals(DeleteType.Enable)
                            select m;
            modelTest = modelTest.Where(s => goodsIds.Contains(s.Id));
            var list = await modelTest.AsNoTracking().ToListAsync();
            if (list.ToArray().Length != goodsIds.Length)
            {
                ModelState.AddModelError("OrderNo", "选择的商品不合法。请重新选择。");
                return View(orderOutModel);
            }
            foreach (var item in list)
            {
                var i = Array.IndexOf(goodsIds, item.Id);
                if (item.Quantity <= 0 || goodsNums[i] > item.Quantity)
                {
                    ModelState.AddModelError("OrderNo", "选择的商品库存不合法。请重新设置。");
                    return View(orderOutModel);
                }
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                tmpModel.OrderNo = orderOutModel.OrderNo;
                tmpModel.Mark = orderOutModel.Mark;
                tmpModel.GoodsNum = goodsIds.Length;
                tmpModel.UpdateAt = DateTime.Now;
                if (tmpModel.OrderStatus == OrderStatusType.Failed)
                {
                    tmpModel.OrderStatus = OrderStatusType.Pending;
                }
                _context.Update(tmpModel);
                await _context.SaveChangesAsync();

                //查询匹配orderGoods
                var goodsModels = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(tmpModel.Id, OrderInOutType.Out);
                var goodsMap = new Dictionary<int, OrderGoodsModel>();
                foreach (var item in goodsModels)
                {
                    goodsMap.Add(item.GoodsId, item);
                    if (!goodsIds.Contains(item.GoodsId))
                    {
                        item.Deleted = DeleteType.Disable;
                        item.UpdateAt = DateTime.Now;
                        _context.OrderGoodsModels.Update(item);
                    }
                }
                int i = 0;
                foreach (var item in goodsIds)
                {
                    if (goodsMap.ContainsKey(item))
                    {
                        var orderGoodsModel = goodsMap[item];
                        orderGoodsModel.Quantity = goodsNums[i];
                        orderGoodsModel.UpdateAt = DateTime.Now;
                        _context.OrderGoodsModels.Update(orderGoodsModel);
                    }
                    else
                    {
                        var orderGoodsModel = new OrderGoodsModel();
                        orderGoodsModel.OrderId = tmpModel.Id;
                        orderGoodsModel.Type = OrderInOutType.Out;
                        orderGoodsModel.GoodsId = item;
                        orderGoodsModel.Quantity = goodsNums[i];
                        orderGoodsModel.CreateAt = orderGoodsModel.UpdateAt = DateTime.Now;
                        orderGoodsModel.Deleted = DeleteType.Enable;
                        _context.OrderGoodsModels.Add(orderGoodsModel);
                    }
                    i++;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderOutModelExists(orderOutModel.Id))
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
        return View(orderOutModel);
    }

    // GET: OrderOut/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.OrderOutModels == null)
        {
            return NotFound();
        }

        var orderOutModel = await _context.OrderOutModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderOutModel == null || orderOutModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        await setOrderGoodsAsync(orderOutModel);
        return View(orderOutModel);
    }

    // POST: OrderOut/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.OrderOutModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.OrderOutModels'  is null.");
        }
        var orderOutModel = await _context.OrderOutModels.FindAsync(id);
        if (orderOutModel != null && orderOutModel.Deleted == DeleteType.Enable)
        {
            orderOutModel.UpdateAt = DateTime.Now;
            orderOutModel.Deleted = DeleteType.Disable;
            _context.Update(orderOutModel);
            //delete orderGoods
            _context.Database.ExecuteSqlRaw("UPDATE wms_order_goods SET Deleted = 1 WHERE Deleted=0 and type=2 AND OrderId=" + orderOutModel.Id);
            // _context.OrderOutModels.Remove(orderOutModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: OrderOut/Check/5
    public async Task<IActionResult> Check(int? id)
    {
        if (id == null || _context.OrderOutModels == null)
        {
            return NotFound();
        }

        var orderOutModel = await _context.OrderOutModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderOutModel == null || orderOutModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        await setOrderGoodsAsync(orderOutModel);
        return View(orderOutModel);
    }

    // POST: OrderOut/Check/5
    [HttpPost, ActionName("Check")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckConfirmed(int id, OrderStatusType orderStatus)
    {
        if (_context.OrderOutModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.OrderOutModels'  is null.");
        }
        var orderOutModel = await _context.OrderOutModels.FindAsync(id);

        if (orderOutModel != null && orderOutModel.Deleted == DeleteType.Enable)
        {
            if (orderOutModel.OrderStatus != OrderStatusType.Pending)
            {
                return NotFound();
            }
            if (orderStatus != OrderStatusType.Failed && orderStatus != OrderStatusType.Success)
            {
                return NotFound();
            }
            //更新审核
            orderOutModel.OrderStatus = orderStatus;
            orderOutModel.AuditTime = DateTime.Now;
            orderOutModel.UpdateAt = DateTime.Now;
            if (orderStatus == OrderStatusType.Success)
            {
                orderOutModel.StockStatus = StockType.Pending;
            }
            _context.Update(orderOutModel);
            //update goods quantity
            if (orderStatus == OrderStatusType.Success)
            {
                var orderGoodsList = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(orderOutModel.Id, OrderInOutType.Out);
                if (orderGoodsList != null && orderGoodsList.Any())
                {
                    foreach (var item in orderGoodsList)
                    {
                        _context.Database.ExecuteSqlRaw("UPDATE wms_goods SET Quantity = Quantity-" + item.Quantity + " WHERE Id=" + item.GoodsId + "");
                    }
                }
                orderOutModel.StockStatus = StockType.Success;
                orderOutModel.StockTime = DateTime.Now;
                _context.Update(orderOutModel);
            }
            await _context.SaveChangesAsync();
        }


        return RedirectToAction(nameof(Index));
    }

    private bool OrderOutModelExists(int id)
    {
        return (_context.OrderOutModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    private async Task setOrderGoodsAsync(OrderOutModel orderOutModel)
    {
        var orderGoodsList = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(orderOutModel.Id, OrderInOutType.Out);
        orderGoodsList = await GoodsService.CreateObject().formatOrderGoods(orderGoodsList);
        ViewData["list"] = orderGoodsList;
    }
}
