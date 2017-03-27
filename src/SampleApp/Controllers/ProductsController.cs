using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;

namespace SampleApp.Controllers {
    public class ProductsController : Controller {
        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context) {
            _context = context;
        }

        //// GET: Products
        //public async Task<IActionResult> Index() {
        //    var qry = _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Supplier);
        //    return View(await qry.ToListAsync());
        //}

        // GET: Products
        //public async Task<IActionResult> Index(string filter) {
        //    var qry = _context.Products.AsNoTracking()
        //                        .Include(p => p.Category)
        //                        .Include(p => p.Supplier)
        //                        .OrderBy(p => p.ProductName).AsQueryable();

        //    if (!string.IsNullOrEmpty(filter)) {
        //        qry = qry.Where(p => p.ProductName.StartsWith(filter));
        //    }

        //    return View(await qry.ToListAsync());
        //}

        public async Task<IActionResult> Index(string filter, int page = 1, string sortExpression = "ProductName") {

            var qry = _context.Products.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter)) {
                qry = qry.Where(p => p.ProductName.Contains(filter));
            }

            var model = await PagingList<Products>.CreateAsync(qry, 10, page, sortExpression, "ProductName");

            model.RouteValue = new RouteValueDictionary {
                { "filter", filter}
            };

            return View(model);
        }



        //[HttpPost]
        //public async Task<IActionResult> Index(string filter) {
        //    ViewData["Filter"] = filter;
        //    var qry = _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Supplier)
        //                                .Where(p => p.ProductName.Contains(filter));
        //    return View(await qry.ToListAsync());
        //}


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var products = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (products == null) {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create() {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,CategoryId,Discontinued,ProductName,QuantityPerUnit,ReorderLevel,SupplierId,UnitPrice,UnitsInStock,UnitsOnOrder")] Products products) {
            if (ModelState.IsValid) {
                _context.Add(products);
                //await _context.SaveChangesAsync();
                if (await TrySaveChangesAsync()) {
                    return RedirectToAction("Index");
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", products.SupplierId);
            return View(products);
        }

        private async Task<bool> TrySaveChangesAsync() {
            try {
                await _context.SaveChangesAsync();
                return true;
            } catch (DbUpdateException ex) when ((ex.InnerException as SqlException).Number == 2601) {
                this.ModelState.AddModelError(string.Empty, "ProductName must be unique");
                return false;
            } catch (Exception ex) {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return false;
            }
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var products = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (products == null) {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", products.SupplierId);
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,CategoryId,Discontinued,ProductName,QuantityPerUnit,ReorderLevel,SupplierId,UnitPrice,UnitsInStock,UnitsOnOrder")] Products products) {
            if (id != products.ProductId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                _context.Update(products);

                if (await TrySaveChangesAsync()) {
                    return RedirectToAction("Index");
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", products.SupplierId);
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var products = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (products == null) {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var products = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductsExists(int id) {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
