using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Data;
using mvc_andy.Models.com;

namespace mvc_andy.Services.Backend;

public class UserService
{
    private static UserService? _myUserHelp;
    private MvcAndyContext? _context;

    public static UserService CreateObject()
    {
        if (_myUserHelp != null)
        {
            return _myUserHelp;
        }
        _myUserHelp = new UserService();
        return _myUserHelp;
    }

    public UserService setDbContext(MvcAndyContext context)
    {
        _context = context;
        return this;
    }

    public List<int> getListIds(PaginatedList<UserModel> list)
    {
        List<int> ids = new List<int>();
        foreach (var item in list)
        {
            if (!ids.Contains(item.RoleId))
            {
                ids.Add(item.RoleId);
            }
        }
        return ids;
    }

    public async Task<Dictionary<int, String>> getRoleList(List<int> ids)
    {
        Dictionary<int, String> tmp = new Dictionary<int, string>();
        if (_context == null) return tmp;
        var role = from m in _context.RoleModels
                   where m.Deleted.Equals(DeleteType.Enable)
                   select m;
        role = role.Where(s => ids.Contains(s.Id));
        var roleList = await role.ToListAsync();

        foreach (var item in roleList)
        {
            tmp.Add(item.Id, item.Name ?? "");
        }
        return tmp;
    }



    public async Task<List<RoleModel>> getRoleList()
    {
        if (_context == null) return new List<RoleModel>();
        var role = from m in _context.RoleModels
                   where m.Deleted.Equals(DeleteType.Enable)
                   select m;
        role = role.OrderByDescending(s => s.Id);
        return await role.ToListAsync();
    }


    public UserModel getSessionUser(int? uid)
    {
        if (_context == null) return new UserModel();
        //填充 user 和 menu。
        var user = from m in _context.UserModels
                   where m.Deleted.Equals(DeleteType.Enable)
                   where m.Id.Equals(uid)
                   select m;
        return  user.FirstOrDefault();
    }



}
