// gradnjapro.Controllers.PartnersController
using System.Linq;
using System.Threading.Tasks;
using gradnjapro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "UserAdmin, Korisnik, SuperAdmin")]
public class PartnersController : Controller
{
	private readonly ApplicationDbContext _context;

	public PartnersController(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		return View(await _context.Partner.ToListAsync());
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		Partner partner = await _context.Partner.FirstOrDefaultAsync((Partner m) => (int?)m.Id == id);
		if (partner == null)
		{
			return NotFound();
		}
		return View(partner);
	}

	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind(new string[] { "Id,Naziv,Tip,Adresa,PIB,TekuciRacun,MB,Napomena" })] Partner partner)
	{
		if (_context.Partner.Any((Partner d) => d.Naziv == partner.Naziv))
		{
			base.ModelState.AddModelError("Naziv", "Naziv već postoji");
		}
		if (_context.Partner.Any((Partner d) => d.PIB == partner.PIB) && partner.PIB != null)
		{
			base.ModelState.AddModelError("PIB", "PIB već postoji");
		}
		if (_context.Partner.Any((Partner d) => d.MB == partner.MB) && partner.MB != null)
		{
			base.ModelState.AddModelError("MB", "MB već postoji");
		}
		if (_context.Partner.Any((Partner d) => d.TekuciRacun == partner.TekuciRacun) && partner.TekuciRacun != null)
		{
			base.ModelState.AddModelError("TekuciRacun", "Tekuci racun već postoji");
		}
		if (base.ModelState.IsValid)
		{
			_context.Add(partner);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		return View(partner);
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		Partner partner = await _context.Partner.FindAsync(id);
		if (partner == null)
		{
			return NotFound();
		}
		return View(partner);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind(new string[] { "Id,Naziv,Tip,Adresa,PIB,TekuciRacun,MB,Napomena" })] Partner partner)
	{
		if (id != partner.Id)
		{
			return NotFound();
		}
		if (_context.Partner.Where((Partner d) => d.Id != partner.Id).Any((Partner d) => d.Naziv == partner.Naziv))
		{
			base.ModelState.AddModelError("Naziv", "Naziv već postoji");
		}
		if (_context.Partner.Where((Partner d) => d.Id != partner.Id).Any((Partner d) => d.PIB == partner.PIB) && partner.PIB != null)
		{
			base.ModelState.AddModelError("PIB", "PIB već postoji");
		}
		if (_context.Partner.Where((Partner d) => d.Id != partner.Id).Any((Partner d) => d.MB == partner.MB) && partner.MB != null)
		{
			base.ModelState.AddModelError("MB", "MB već postoji");
		}
		if (_context.Partner.Where((Partner d) => d.Id != partner.Id).Any((Partner d) => d.TekuciRacun == partner.TekuciRacun) && partner.TekuciRacun != null)
		{
			base.ModelState.AddModelError("TekuciRacun", "Tekuci racun već postoji");
		}
		if (base.ModelState.IsValid)
		{
			try
			{
				_context.Update(partner);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PartnerExists(partner.Id))
				{
					return NotFound();
				}
				throw;
			}
		}
		return View(partner);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		Partner partner = await _context.Partner.FirstOrDefaultAsync((Partner m) => (int?)m.Id == id);
		if (partner == null)
		{
			return NotFound();
		}
		return View(partner);
	}

	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		IQueryable<Dokument> source = _context.Dokument.Where((Dokument d) => d.PartnerId == (int?)id);
		if (source.Any())
		{
			return CustomErrorMessage("Ne možete izbrisatu poslovnog partnera zato što postoji u dokumentima", "Index");
		}
		Partner entity = await _context.Partner.FindAsync(id);
		_context.Partner.Remove(entity);
		await _context.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	private bool PartnerExists(int id)
	{
		return _context.Partner.Any((Partner e) => e.Id == id);
	}

	public IActionResult CustomErrorMessage(string e, string url, string c = "Partners")
	{
		CustomErrorViewModel customErrorViewModel = new CustomErrorViewModel
		{
			ErrorMessage = e,
			returnUrl = url,
			controller = c
		};
		return RedirectToAction("CustomError", new
		{
			error = customErrorViewModel.ErrorMessage,
			returnUrl = customErrorViewModel.returnUrl,
			controller = customErrorViewModel.controller
		});
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult CustomError(string error, string returnUrl, string controller)
	{
		base.ViewData["ErrorMessage"] = error;
		base.ViewData["returnUrl"] = returnUrl;
		base.ViewData["controller"] = controller;
		return View("CustomError");
	}
}
