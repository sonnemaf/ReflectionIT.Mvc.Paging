using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using SampleWithIdentity.Data;

namespace SampleWithIdentity.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var qry = _context.Products.AsNoTracking().OrderBy(u => u.Id);
            var model = await PagingList.CreateAsync(qry, 6, pageIndex);
            return View(model);
        }
    }
}