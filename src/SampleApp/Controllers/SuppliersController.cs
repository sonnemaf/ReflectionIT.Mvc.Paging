using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;

namespace SampleApp.Controllers {
    public class SuppliersController : Controller {

        private readonly NorthwindContext _context;

        public SuppliersController(NorthwindContext context) {
            _context = context;
        }

        [AcceptVerbs("Get", "Post")]
        public JsonResult IsCompanyNameAvailable(int? supplierId, string companyName) {

            if (!_context.Suppliers.Any(sup => (!supplierId.HasValue || sup.SupplierId != supplierId)
                                            && sup.CompanyName == companyName)) {
                return Json(true);
            }

            return Json($"{companyName} is already used");
        }


        // GET: Suppliers
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Suppliers.ToListAsync());
        //}
        public async Task<IActionResult> Index(int page = 1) {

            var qry = _context.Suppliers.AsNoTracking().OrderBy(p => p.CompanyName);

            var model = await PagingList<Suppliers>.CreateAsync(qry, 10, page);

            return View(model);
        }


        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var suppliers = await _context.Suppliers.SingleOrDefaultAsync(m => m.SupplierId == id);
            if (suppliers == null) {
                return NotFound();
            }

            return View(suppliers);
        }

        // GET: Suppliers/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,Address,City,CompanyName,ContactName,ContactTitle,Country,Fax,HomePage,Phone,PostalCode,Region")] Suppliers suppliers) {
            if (ModelState.IsValid) {
                _context.Add(suppliers);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var suppliers = await _context.Suppliers.SingleOrDefaultAsync(m => m.SupplierId == id);
            if (suppliers == null) {
                return NotFound();
            }
            return View(suppliers);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,Address,City,CompanyName,ContactName,ContactTitle,Country,Fax,HomePage,Phone,PostalCode,Region")] Suppliers suppliers) {
            if (id != suppliers.SupplierId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(suppliers);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!SuppliersExists(suppliers.SupplierId)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var suppliers = await _context.Suppliers.SingleOrDefaultAsync(m => m.SupplierId == id);
            if (suppliers == null) {
                return NotFound();
            }

            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var suppliers = await _context.Suppliers.SingleOrDefaultAsync(m => m.SupplierId == id);
            _context.Suppliers.Remove(suppliers);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SuppliersExists(int id) {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
