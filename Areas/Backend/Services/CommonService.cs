using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Data;
using mvc_andy.Models.com;

namespace mvc_andy.Services.Backend;

public class CommonService
{
    private static CommonService? _help;
    private MvcAndyContext? _context;

    public static CommonService CreateObject()
    {
        if (_help != null)
        {
            return _help;
        }
        _help = new CommonService();
        return _help;
    }

    public CommonService setDbContext(MvcAndyContext context)
    {
        _context = context;
        return this;
    }


    public string uniqid(string prefix, bool more_entropy)
    {
        if (string.IsNullOrEmpty(prefix))
            prefix = string.Empty;

        if (!more_entropy)
        {
            return (prefix + System.Guid.NewGuid().ToString()).PadLeft(13);
        }
        else
        {
            return (prefix + System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString()).PadLeft(23);
        }
    }


}
