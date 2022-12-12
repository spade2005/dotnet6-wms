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
public class OrderInController : BaseController
{

    public OrderInController(MvcAndyContext context) : base(context)
    {
    }

    // GET: OrderIn
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {

        var model = from m in _context.OrderInModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        if (!String.IsNullOrEmpty(searchString))
        {
            model = model.Where(s => s.OrderNo!.Contains(searchString));
        }
        model = model.OrderByDescending(s => s.Id);

        int pageSize = 10;
        var list = await PaginatedList<OrderInModel>.CreateAsync(model.AsNoTracking(), pageNumber ?? 1, pageSize);
        // ViewData["goodsList"] =[];
        return View(list);

    }

    // GET: OrderIn/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.OrderInModels == null)
        {
            return NotFound();
        }

        var orderInModel = await _context.OrderInModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderInModel == null || orderInModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        var orderGoodsList = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(orderInModel);
        orderGoodsList = await GoodsService.CreateObject().formatOrderGoods(orderGoodsList);
        ViewData["list"] = orderGoodsList;

        return View(orderInModel);
    }

    // GET: OrderIn/Create
    public IActionResult Create()
    {
        // ViewData["list"] = await GoodsService.CreateObject().setDbContext(_context).getGoodsCateList();
        return View();
    }

    // POST: OrderIn/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,OrderNo,Mark")] OrderInModel orderInModel, int[] goodsIds, int[] goodsNums)
    {
        if (!goodsIds.Any() || !goodsNums.Any() || goodsIds.Length != goodsNums.Length)
        {

            ModelState.AddModelError("OrderNo", "提交参数不正确");
            return View(orderInModel);
        }
        if (!string.IsNullOrEmpty(orderInModel.OrderNo))
        {
            var modelTest = await _context.OrderInModels
                .Where(m => m.OrderNo == orderInModel.OrderNo).Where(m => m.Deleted == DeleteType.Enable)
                .FirstOrDefaultAsync();
            if (modelTest != null)
            {
                ModelState.AddModelError("OrderNo", "入库单已存在");
                return View(orderInModel);
            }
        }
        if (goodsNums.Any())
        {
            foreach (var num in goodsNums)
            {
                if (num <= 0 || num > 100000000)
                {
                    ModelState.AddModelError("OrderNo", "商品数量必须大于0的合法数值");
                    return View(orderInModel);
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
                return View(orderInModel);
            }
        }

        if (ModelState.IsValid)
        {
            // orderInModel.OrderNo = CommonService.CreateObject().uniqid("in", false);
            orderInModel.GoodsNum = goodsIds.Length;
            orderInModel.OrderStatus = OrderStatusType.Pending;
            orderInModel.StockStatus = StockType.Default;
            orderInModel.AuditTime = orderInModel.StockTime = DateTime.MinValue;

            orderInModel.TimeOfDay = int.Parse(DateTime.Today.ToString("yyyymmdd"));//20221207
            orderInModel.TimeOfMonth = int.Parse(DateTime.Today.ToString("yyyymm"));//202212
            orderInModel.CreateAt = orderInModel.UpdateAt = DateTime.Now;
            orderInModel.Deleted = DeleteType.Enable;
            _context.OrderInModels.Add(orderInModel);
            await _context.SaveChangesAsync();

            int i = 0;
            foreach (var item in goodsIds)
            {
                var orderGoodsModel = new OrderGoodsModel();
                orderGoodsModel.OrderId = orderInModel.Id;
                orderGoodsModel.Type = OrderInOutType.In;
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
        return View(orderInModel);
    }

    // GET: OrderIn/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.OrderInModels == null)
        {
            return NotFound();
        }

        var orderInModel = await _context.OrderInModels.FindAsync(id);
        if (orderInModel == null || orderInModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        var orderGoodsList = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(orderInModel);
        orderGoodsList = await GoodsService.CreateObject().formatOrderGoods(orderGoodsList);
        ViewData["list"] = orderGoodsList;
        return View(orderInModel);
    }

    // POST: OrderIn/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Mark,OrderNo")] OrderInModel orderInModel, int[] goodsIds, int[] goodsNums)
    {
        if (id != orderInModel.Id)
        {
            return NotFound();
        }

        var tmpModel = await _context.OrderInModels.FindAsync(id);
        if (tmpModel == null || tmpModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        if (tmpModel.OrderStatus == OrderStatusType.Success)
        {
            //审核成功，不可再次编辑
            ModelState.AddModelError("OrderNo", "订单已审核通过,无法编辑");
            return View(orderInModel);
        }

        if (!goodsIds.Any() || !goodsNums.Any() || goodsIds.Length != goodsNums.Length)
        {

            ModelState.AddModelError("OrderNo", "提交参数不正确");
            return View(orderInModel);
        }

        if (!string.IsNullOrEmpty(orderInModel.OrderNo))
        {
            var modelTest = await _context.OrderInModels
                .Where(m => m.OrderNo == orderInModel.OrderNo).Where(m => m.Deleted == DeleteType.Enable)
                .Where(m => m.Id != tmpModel.Id)
                .FirstOrDefaultAsync();
            if (modelTest != null)
            {
                ModelState.AddModelError("OrderNo", "入库单已存在");
                return View(orderInModel);
            }
        }
        if (goodsNums.Any())
        {
            foreach (var num in goodsNums)
            {
                if (num <= 0 || num > 100000000)
                {
                    ModelState.AddModelError("OrderNo", "商品数量必须大于0的合法数值");
                    return View(orderInModel);
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
                return View(orderInModel);
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                tmpModel.OrderNo = orderInModel.OrderNo;
                tmpModel.Mark = orderInModel.Mark;
                tmpModel.GoodsNum = goodsIds.Length;
                tmpModel.UpdateAt = DateTime.Now;
                if (tmpModel.OrderStatus == OrderStatusType.Failed)
                {
                    tmpModel.OrderStatus = OrderStatusType.Pending;
                }
                _context.Update(tmpModel);
                await _context.SaveChangesAsync();

                //查询匹配orderGoods
                var modelGoods = from m in _context.OrderGoodsModels
                                 where m.Deleted.Equals(DeleteType.Enable)
                                 select m;
                modelGoods = modelGoods.Where(s => s.Type.Equals(OrderInOutType.In));
                modelGoods = modelGoods.Where(s => s.OrderId.Equals(tmpModel.Id));
                var goodsModels = await modelGoods.ToListAsync();
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
                        orderGoodsModel.OrderId = orderInModel.Id;
                        orderGoodsModel.Type = OrderInOutType.In;
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
                if (!OrderInModelExists(orderInModel.Id))
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
        return View(orderInModel);
    }

    // GET: OrderIn/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.OrderInModels == null)
        {
            return NotFound();
        }

        var orderInModel = await _context.OrderInModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderInModel == null || orderInModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        var orderGoodsList = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(orderInModel);
        orderGoodsList = await GoodsService.CreateObject().formatOrderGoods(orderGoodsList);
        ViewData["list"] = orderGoodsList;
        return View(orderInModel);
    }

    // POST: OrderIn/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.OrderInModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.OrderInModels'  is null.");
        }
        var orderInModel = await _context.OrderInModels.FindAsync(id);
        if (orderInModel != null && orderInModel.Deleted == DeleteType.Enable)
        {
            orderInModel.UpdateAt = DateTime.Now;
            orderInModel.Deleted = DeleteType.Disable;
            _context.Update(orderInModel);
            //delete orderGoods
            _context.Database.ExecuteSqlRaw("UPDATE wms_order_goods SET Deleted = 1 WHERE OrderId=" + orderInModel.Id + "");
            // _context.OrderInModels.Remove(orderInModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: OrderIn/Check/5
    public async Task<IActionResult> Check(int? id)
    {
        if (id == null || _context.OrderInModels == null)
        {
            return NotFound();
        }

        var orderInModel = await _context.OrderInModels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderInModel == null || orderInModel.Deleted != DeleteType.Enable)
        {
            return NotFound();
        }
        var orderGoodsList = await GoodsService.CreateObject().setDbContext(_context).getOrderGoods(orderInModel);
        orderGoodsList = await GoodsService.CreateObject().formatOrderGoods(orderGoodsList);
        ViewData["list"] = orderGoodsList;
        return View(orderInModel);
    }

    // POST: OrderIn/Check/5
    [HttpPost, ActionName("Check")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckConfirmed(int id, OrderStatusType orderStatus)
    {
        if (_context.OrderInModels == null)
        {
            return Problem("Entity set 'MvcAndyContext.OrderInModels'  is null.");
        }
        var orderInModel = await _context.OrderInModels.FindAsync(id);

        if (orderInModel != null && orderInModel.Deleted == DeleteType.Enable)
        {
            if (orderInModel.OrderStatus != OrderStatusType.Pending)
            {
                return NotFound();
            }
            if (orderStatus != OrderStatusType.Failed && orderStatus != OrderStatusType.Success)
            {
                return NotFound();
            }
            //更新审核
            orderInModel.OrderStatus = orderStatus;
            orderInModel.AuditTime = DateTime.Now;
            orderInModel.UpdateAt = DateTime.Now;
            _context.Update(orderInModel);
            // _context.OrderInModels.Remove(orderInModel);
            await _context.SaveChangesAsync();
            //update goods quantity
        }


        return RedirectToAction(nameof(Index));
    }

    private bool OrderInModelExists(int id)
    {
        return (_context.OrderInModels?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
