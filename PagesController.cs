using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace MvcApp.Controllers
{
    [AllowAnonymous] //[Authorize(Roles = "UserAdministrators")] 
    public class PagesController : Controller
    {
        private readonly ContentDbContext _context;
        private int culture = MvcApp.Controllers.SelectLanguageController.GetLanguage();

        public PagesController(ContentDbContext context)
        {
            _context = context;
        }
        private void PageDropDownList(object selectedPage = null)
        {
            var pagesQuery = from d in _context.Page
                             orderby d.Id
                             select new { d.Id, d.Name };
            ViewBag.ParentId = new SelectList(pagesQuery, "Id", "Name", selectedPage);
        }

        // GET: Page
        public async Task<IActionResult> Index()
        {
            var pages = await _context.Page
                .Include(l=>l.Language)
                .Where(l=>l.LanguageId==culture)
                .ToListAsync();
            return View(pages);
        }

        // GET: Page/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Page
                .SingleOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Page/Create
        public IActionResult Create()
        {
            PageDropDownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParentId, Name")] Page page)
        {
            if (ModelState.IsValid)
            {
                page.LanguageId = culture;
                _context.Add(page);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(page);
        }

        // GET: Page/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Page.SingleOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }
            PageDropDownList(page.ParentId);
            return View(page);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Name")] Page page)
        {
            if (id != page.Id)
            {
                return NotFound();
            }

            if (page.Name==string.Empty)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    page.LanguageId = culture;
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(page);
        }

        // GET: Page/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Page
                .SingleOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Page/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.Page.SingleOrDefaultAsync(m => m.Id == id);
            _context.Page.Remove(page);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PageExists(int id)
        {
            return _context.Page.Any(e => e.Id == id);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
