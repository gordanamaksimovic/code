using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
.....
	public async Task<IActionResult> Index(int? selectedVD, StatusDokumenta? selectedStatus, StatusPotvrde? selectedPotvrda)
	{
		List<VrstaDokumenta> items = (from d in _context.VrstaDokumenta
									  where d.Skracenica != "PS"
									  orderby d.Naziv
									  select d).ToList();
		base.ViewBag.selectedVD = new SelectList(items, "Id", "Naziv", selectedVD);
		var items2 = Enum.GetValues(typeof(StatusDokumenta)).Cast<StatusDokumenta>().Select(delegate (StatusDokumenta s)
		{
			StatusDokumenta statusDokumenta = s;
			return new
			{
				ID = s,
				Name = statusDokumenta.ToString()
			};
		});
		base.ViewBag.selectedStatus = new SelectList(items2, "ID", "Name", selectedStatus);
		var items3 = Enum.GetValues(typeof(StatusPotvrde)).Cast<StatusPotvrde>().Select(delegate (StatusPotvrde s)
		{
			StatusPotvrde statusPotvrde = s;
			return new
			{
				ID = s,
				Name = statusPotvrde.ToString()
			};
		});
		base.ViewBag.selectedPotvrda = new SelectList(items3, "ID", "Name", selectedPotvrda);
		int vrstaId = selectedVD.GetValueOrDefault();
		StatusDokumenta? statusId = (selectedStatus.HasValue ? new StatusDokumenta?(selectedStatus.GetValueOrDefault()) : selectedStatus);
		StatusPotvrde? potvrdaId = (selectedPotvrda.HasValue ? new StatusPotvrde?(selectedPotvrda.GetValueOrDefault()) : selectedPotvrda);
		IQueryable<DokumentView> source = from d in _context.Dokument.Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat).Include((Dokument d) => d.VrstaDokumenta)
				.Include((Dokument d) => d.ApplicationUser)
										  where d.VrstaDokumenta.Skracenica != "PS" && (!selectedVD.HasValue || d.VrstaDokumentaId == vrstaId) && (!selectedStatus.HasValue || (int?)d.Status == (int?)statusId) && (!selectedPotvrda.HasValue || (int?)d.Validacija == (int?)potvrdaId) && (int)d.Status == 0 && (int)d.Validacija != 0
										  orderby d.DateDoc descending, d.Id descending
										  select d into dokumenti
										  select new DokumentView
										  {
											  Id = dokumenti.Id,
											  VrstaDokumenta = dokumenti.VrstaDokumenta,
											  VrstaVezniDokument = dokumenti.VrstaVezniDokument,
											  DateCreated = dokumenti.DateCreated,
											  DateDoc = dokumenti.DateDoc,
											  DateUpdated = dokumenti.DateUpdated,
											  ObjekatId = dokumenti.ObjekatId,
											  Objekat = dokumenti.Objekat,
											  PartnerId = dokumenti.PartnerId.GetValueOrDefault(),
											  PrevoznikId = dokumenti.PrevoznikId.GetValueOrDefault(),
											  Partner = dokumenti.Partner,
											  Status = dokumenti.Status,
											  Opis = dokumenti.Opis,
											  UserId = dokumenti.UserId,
											  ApplicationUser = dokumenti.ApplicationUser,
											  VrstaVezniDokumentId = dokumenti.VrstaVezniDokumentId.GetValueOrDefault(),
											  VezniDokumentId = dokumenti.VezniDokumentId.GetValueOrDefault(),
											  Validacija = dokumenti.Validacija,
											  Prevoznik = dokumenti.Prevoznik,
											  VozacId = dokumenti.VozacId,
											  Vozaci = dokumenti.Vozaci,
											  VoziloId = dokumenti.VoziloId,
											  Vozila = dokumenti.Vozila,
											  Broj = ((dokumenti.Broj == "0") ? string.Empty : dokumenti.Broj),
											  BrojVeznogDokumenta = dokumenti.BrojVeznogDokumenta
										  };
		return View(await source.ToListAsync());
	}
..............
	public async Task<IActionResult> Edit(int? id)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DokumentStavkeViewModel model = new DokumentStavkeViewModel();
		IQueryable<DokumentView> source = from dokumenti in _context.Dokument.Where((Dokument d) => (int?)d.Id == id).Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat)
				.Include((Dokument d) => d.VrstaDokumenta)
				.Include((Dokument d) => d.ApplicationUser)
										  select new DokumentView
										  {
											  Id = dokumenti.Id,
											  VrstaDokumenta = dokumenti.VrstaDokumenta,
											  VrstaDokumentaId = dokumenti.VrstaDokumentaId,
											  DateCreated = dokumenti.DateCreated,
											  DateUpdated = dokumenti.DateUpdated,
											  DateDoc = dokumenti.DateDoc,
											  Opis = dokumenti.Opis,
											  Status = dokumenti.Status,
											  ObjekatId = dokumenti.ObjekatId,
											  Objekat = dokumenti.Objekat,
											  PartnerId = dokumenti.PartnerId.GetValueOrDefault(),
											  PrevoznikId = dokumenti.PrevoznikId.GetValueOrDefault(),
											  Partner = dokumenti.Partner,
											  UserId = dokumenti.UserId,
											  ApplicationUser = dokumenti.ApplicationUser,
											  VrstaVezniDokumentId = dokumenti.VrstaVezniDokumentId,
											  VezniDokumentId = dokumenti.VezniDokumentId,
											  KorisnikId = dokumenti.KorisnikId,
											  Validacija = dokumenti.Validacija,
											  DateValidated = dokumenti.DateValidated,
											  Napomena = dokumenti.Napomena,
											  Broj = dokumenti.Broj,
											  VozacId = dokumenti.VozacId,
											  VoziloId = dokumenti.VoziloId,
											  BrojVeznogDokumenta = dokumenti.BrojVeznogDokumenta
										  };
		_ = base.ModelState.IsValid;
		if (!source.Any())
		{
			return CustomErrorMessage("Greška u otvaranju dokumenta", "Index");
		}
		IQueryable<DokumentStavkeView> items = from stavke in _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == id).Include((DokumentStavka d) => d.Zalihe)
											   select new DokumentStavkeView
											   {
												   Id = stavke.Id,
												   Stavka = stavke.Stavka,
												   ZaliheId = stavke.ZaliheId,
												   Zalihe = stavke.Zalihe,
												   Marka = stavke.Zalihe.Marka,
												   Cena = stavke.Cena,
												   Kolicina = stavke.Kolicina,
												   JedMere = stavke.JedMere,
												   Vrednost = stavke.Cena * stavke.Kolicina,
												   DokumentId = stavke.DokumentId
											   };
		if (!items.Any())
		{
			return CustomErrorMessage("Greška u stavkama dokumenta", "Index");
		}
		DokumentStavkeViewModel dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentView = await source.FirstAsync();
		dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentStavke = await items.ToListAsync();
		if (model == null)
		{
			return CustomErrorMessage("Greška u dokumentu", "Index");
		}
		DropDownList();
		base.ViewData["DateDoc"] = DateTime.Today.ToString("dd/MM/yyyy");
		base.ViewData["Message"] = Automatski(id);
		return View(model);
	}

	[HttpPost]
	[ActionName("Edit")]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> Edit(int id, DokumentStavkeViewModel model)
	{
		if (id != model.DokumentView.Id)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		if (base.ModelState.IsValid)
		{
			if (model.DokumentView.ObjekatId == 0)
			{
				return CustomErrorMessage("Izaberite objekat skladištenja", "Edit");
			}
			if (model.DokumentView.PartnerId == 0)
			{
				model.DokumentView.PartnerId = null;
			}
			if (model.DokumentView.VrstaDokumentaId == 0)
			{
				return CustomErrorMessage("Izaberite vrstu dokumenta.", "Edit");
			}
			if (model.DokumentView.Broj == string.Empty)
			{
				return CustomErrorMessage("Unesite broj dokumenta.", "Edit");
			}
			if (model.DokumentStavke.Count == 0)
			{
				return CustomErrorMessage("Ne možete kreirati dokument bez stavki", "Edit");
			}
			try
			{
				Dokument dokument = _context.Dokument.FirstOrDefault((Dokument d) => d.Id == id);
				dokument.DateUpdated = model.DokumentView.DateUpdated;
				dokument.DateDoc = model.DokumentView.DateDoc;
				dokument.Status = model.DokumentView.Status;
				dokument.PartnerId = model.DokumentView.PartnerId;
				dokument.PrevoznikId = model.DokumentView.PrevoznikId;
				dokument.ObjekatId = model.DokumentView.ObjekatId;
				dokument.Opis = model.DokumentView.Opis;
				dokument.UserId = model.DokumentView.UserId;
				dokument.VrstaVezniDokumentId = model.DokumentView.VrstaVezniDokumentId;
				dokument.VezniDokumentId = model.DokumentView.VezniDokumentId;
				dokument.KorisnikId = model.DokumentView.KorisnikId;
				dokument.DateValidated = model.DokumentView.DateValidated;
				dokument.Validacija = model.DokumentView.Validacija;
				dokument.Napomena = model.DokumentView.Napomena;
				dokument.Broj = model.DokumentView.Broj;
				dokument.VozacId = model.DokumentView.VozacId;
				dokument.VoziloId = model.DokumentView.VoziloId;
				dokument.BrojVeznogDokumenta = model.DokumentView.BrojVeznogDokumenta;
				IQueryable<DokumentStavka> queryable = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id);
				if (model.DokumentStavke != null)
				{
					int num = 0;
					foreach (DokumentStavka item in queryable)
					{
						if (item.ZaliheId != 0)
						{
							item.DokumentId = id;
							item.Stavka = num + 1;
							if (model.DokumentStavke[num].ZaliheId == 0)
							{
								base.ModelState.AddModelError("ZaliheId", "Unesite zalihe");
							}
							item.ZaliheId = model.DokumentStavke[num].ZaliheId;
							if (model.DokumentStavke[num].Kolicina == 0.0)
							{
								base.ModelState.AddModelError("Kolicina", "Unesite kolicinu");
							}
							item.Kolicina = model.DokumentStavke[num].Kolicina;
							item.Cena = model.DokumentStavke[num].Cena;
							item.JedMere = model.DokumentStavke[num].JedMere;
							num++;
						}
					}
				}
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!DokumentStavkeViewModelExists(model.DokumentView.Id))
				{
					return CustomErrorMessage("Ne postoje stavke u dokumentu", "Index");
				}
				throw;
			}
			return RedirectToAction("Edit", new { id });
		}
		DropDownList();
		return RedirectToAction("Edit", new { id });
	}
