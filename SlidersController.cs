using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace MvcApp.Controllers
{
    [AllowAnonymous] 
    public class SlidersController : Controller
    {
        private readonly ContentDbContext _context;
        private IHostingEnvironment _env;

        public SlidersController(ContentDbContext context,  IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Slider
        public async Task<IActionResult> Index()
        {
            return View(await _context.Slider.ToListAsync());
        }

        // GET: Slider/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Slider
                .SingleOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Slider/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Slider/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameImg,Alt,Desc,Link,LanguageId")] Slider slider, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images");
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);

                        }
                        slider.NameImg = fileName;
                    }
                    _context.Add(slider);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(slider);
        }

        // GET: Slider/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Slider.SingleOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Slider/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameImg,Alt,Desc,Link,LanguageId")] Slider slider,IFormFile file)
        {
            if (id != slider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images");
                    if (file != null && file.Length > 0)
                    {
                        slider.NameImg = "";
                        var fileName = Path.GetFileName(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);

                        }
                        slider.NameImg = fileName;
                    }
                    _context.Update(slider);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.Id))
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
            return View(slider);
        }

        // GET: Slider/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Slider
                .SingleOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Slider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _context.Slider.SingleOrDefaultAsync(m => m.Id == id);
            _context.Slider.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SliderExists(int id)
        {
            return _context.Slider.Any(e => e.Id == id);
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
