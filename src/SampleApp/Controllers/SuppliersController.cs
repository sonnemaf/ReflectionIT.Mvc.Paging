using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;
using SampleApp.ViewModels;

namespace SampleApp.Controllers {

    public class SuppliersController : Controller {

        private readonly NorthwindContext _context;

        public SuppliersController(NorthwindContext context) {
            _context = context;
        }

        [AcceptVerbs("Get", "Post")]
        public JsonResult IsCompanyNameAvailable(int? supplierId, string companyName) {

            return !_context.Suppliers.Any(sup => (!supplierId.HasValue || sup.SupplierId != supplierId)
                                            && sup.CompanyName == companyName)
                ? Json(true)
                : Json($"{companyName} is already used");
        }


        // GET: Suppliers
        public async Task<IActionResult> Index(int pageindex = 1, string sort = "CompanyName") {

            var qry = _context.Suppliers.AsNoTracking();

            var model = new SuppliersIndexViewModel {
                Title = "Suppliers",
                Suppliers = await PagingList.CreateAsync(qry, 10, pageindex, sort, nameof(Suppliers.CompanyName)),
            };

            return View(model);
        }



        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var suppliers = await _context.Suppliers.SingleOrDefaultAsync(m => m.SupplierId == id);
            return suppliers == null ? NotFound() : (IActionResult)View(suppliers);
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
            return suppliers == null ? NotFound() : (IActionResult)View(suppliers);
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
            return suppliers == null ? NotFound() : (IActionResult)View(suppliers);
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
