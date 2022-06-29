// gradnjapro.Controllers.ObjektiController
using System.Linq;
using System.Threading.Tasks;
using gradnjapro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "UserAdmin, Korisnik, SuperAdmin")]
public class ObjektiController : Controller
{
	private readonly ApplicationDbContext _context;

	public ObjektiController(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		return View(await _context.Objekat.ToListAsync());
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		Objekat objekat = await _context.Objekat.FirstOrDefaultAsync((Objekat m) => (int?)m.Id == id);
		if (objekat == null)
		{
			return NotFound();
		}
		return View(objekat);
	}

	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind(new string[] { "Id,Naziv,Tip,Opis,Adresa,VrstaCene" })] Objekat objekat)
	{
		if (base.ModelState.IsValid)
		{
			IQueryable<Objekat> source = _context.Objekat.Where((Objekat d) => d.Naziv.ToLower() == objekat.Naziv.ToLower());
			if (source.Any())
			{
				base.ModelState.AddModelError("Naziv", "Unetei naziv postoji");
				return View(objekat);
			}
			_context.Add(objekat);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		return View(objekat);
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		Objekat objekat = await _context.Objekat.FindAsync(id);
		if (objekat == null)
		{
			return NotFound();
		}
		return View(objekat);
	}

	[Authorize(Roles = "SuperAdmin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind(new string[] { "Id,Naziv,Tip,Opis,Adresa,VrstaCene" })] Objekat objekat)
	{
		if (id != objekat.Id)
		{
			return NotFound();
		}
		if (base.ModelState.IsValid)
		{
			try
			{
				IQueryable<Objekat> source = _context.Objekat.Where((Objekat d) => d.Naziv.ToLower() == objekat.Naziv.ToLower());
				if (source.Any())
				{
					base.ModelState.AddModelError("Naziv", "Unetei naziv postoji");
					return View(objekat);
				}
				_context.Update(objekat);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ObjekatExists(objekat.Id))
				{
					return NotFound();
				}
				throw;
			}
			return RedirectToAction("Index");
		}
		return View(objekat);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		Objekat objekat = await _context.Objekat.FirstOrDefaultAsync((Objekat m) => (int?)m.Id == id);
		if (objekat == null)
		{
			return NotFound();
		}
		return View(objekat);
	}

	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		Objekat entity = await _context.Objekat.FindAsync(id);
		_context.Objekat.Remove(entity);
		await _context.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	private bool ObjekatExists(int id)
	{
		return _context.Objekat.Any((Objekat e) => e.Id == id);
	}
}
