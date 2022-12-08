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
        var list = await model.ToListAsync();

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



}
