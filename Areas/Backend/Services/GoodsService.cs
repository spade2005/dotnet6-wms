using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Data;
using mvc_andy.Models.com;
using mvc_andy.Models.wms;

namespace mvc_andy.Services.Backend;

public class GoodsService
{
    private static GoodsService? _help;
    private MvcAndyContext? _context;

    public static GoodsService CreateObject()
    {
        if (_help != null)
        {
            return _help;
        }
        _help = new GoodsService();
        return _help;
    }

    public GoodsService setDbContext(MvcAndyContext context)
    {
        _context = context;
        return this;
    }

    public List<int> getListIds(PaginatedList<GoodsModel> list)
    {
        List<int> ids = new List<int>();
        foreach (var item in list)
        {
            if (!ids.Contains(item.CateId))
            {
                ids.Add(item.CateId);
            }
        }
        return ids;
    }

    public async Task<Dictionary<int, String>> getGoodsCateList(List<int> ids)
    {
        Dictionary<int, String> tmp = new Dictionary<int, string>();
        if (_context == null) return tmp;
        var model = from m in _context.GoodsCateModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        model = model.Where(s => ids.Contains(s.Id));
        var list = await model.AsNoTracking().ToListAsync();

        foreach (var item in list)
        {
            tmp.Add(item.Id, item.Name ?? "");
        }
        return tmp;
    }

    public async Task<List<GoodsCateModel>> getGoodsCateList()
    {
        if (_context == null) return new List<GoodsCateModel>();
        var role = from m in _context.GoodsCateModels
                   where m.Deleted.Equals(DeleteType.Enable)
                   select m;
        role = role.OrderByDescending(s => s.Id);
        return await role.ToListAsync();
    }



    // 获取入库单商品明细
    public async Task<List<OrderGoodsModel>> getOrderGoods(OrderInModel orderInModel)
    {
        if (_context == null) return new List<OrderGoodsModel>();
        var model = from m in _context.OrderGoodsModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        model = model.Where(s => s.OrderId.Equals(orderInModel.Id));
        var orderGoodsList = await model.ToListAsync();
        if (orderGoodsList == null || !orderGoodsList.Any())
        {
            return orderGoodsList;
        }
        var goodsIds = getGoodsIds(orderGoodsList);
        var goodsList = await getGoodsList(goodsIds);
        if (goodsList != null && goodsList.Any())
        {
            Dictionary<int, GoodsModel> tmp = new Dictionary<int, GoodsModel>();
            foreach (var item in goodsList)
            {
                if (!tmp.ContainsKey(item.Id))
                {
                    tmp.Add(item.Id, item);
                }
            }

            foreach (var item in orderGoodsList)
            {
                if (tmp != null && tmp.ContainsKey(item.GoodsId))
                {
                    item.goods = tmp[item.GoodsId];
                }
            }
        }

        return orderGoodsList;


    }

    // 获取list下的goodsId
    public List<int> getGoodsIds(List<OrderGoodsModel> list)
    {
        List<int> ids = new List<int>();
        foreach (var item in list)
        {
            if (!ids.Contains(item.GoodsId))
            {
                ids.Add(item.GoodsId);
            }
        }
        return ids;
    }

    public async Task<List<GoodsModel>?> getGoodsList(List<int> ids)
    {
        if (ids != null && ids.Any())
        {
            return null;
        }
        if (_context == null) return null;

        var model = from m in _context.GoodsModels
                    where m.Deleted.Equals(DeleteType.Enable)
                    select m;
        model = model.Where(s => ids.Contains(s.Id));
        return await model.ToListAsync();
    }

}
