using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gradnjapro.Models;
using gradnjapro.Models.DokumentViewModels;
using gradnjapro.Models.ReportViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

[Authorize(Roles = "SuperAdmin, UserAdmin, Korisnik")]
public class DokumentStavkeController : Controller
{
	public static int stanje = 2;

	private readonly ApplicationDbContext _context;

	private readonly UserManager<ApplicationUser> _userManager;

	private readonly IWebHostEnvironment _env;

	public DokumentStavkeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
	{
		_context = context;
		_userManager = userManager;
		_env = env;
	}

	[Authorize(Roles = "UserAdministrator, Korisnik, Magacioner")]
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

	[Authorize(Roles = "UserAdministrator, Korisnik, Magacioner")]
	public async Task<IActionResult> IndexPotvrda(int? selectedVD, StatusDokumenta? selectedStatus, StatusPotvrde? selectedPotvrda)
	{
		List<VrstaDokumenta> items = (from d in _context.VrstaDokumenta
									  where d.Skracenica != "PS"
									  orderby d.Naziv
									  select d).ToList();
		base.ViewBag.selectedVD = new SelectList(items, "Id", "Naziv", selectedVD).OrderBy((SelectListItem d) => d.Text);
		var items2 = Enum.GetValues(typeof(StatusDokumenta)).Cast<StatusDokumenta>().Select(delegate (StatusDokumenta s)
		{
			StatusDokumenta statusDokumenta = s;
			return new
			{
				ID = s,
				Name = statusDokumenta.ToString()
			};
		});
		base.ViewBag.selectedStatus = new SelectList(items2, "ID", "Name", selectedStatus).OrderBy((SelectListItem d) => d.Text);
		var items3 = Enum.GetValues(typeof(StatusPotvrde)).Cast<StatusPotvrde>().Select(delegate (StatusPotvrde s)
		{
			StatusPotvrde statusPotvrde = s;
			return new
			{
				ID = s,
				Name = statusPotvrde.ToString()
			};
		});
		base.ViewBag.selectedPotvrda = new SelectList(items3, "ID", "Name", selectedPotvrda).OrderBy((SelectListItem d) => d.Text);
		int vrstaId = selectedVD.GetValueOrDefault();
		StatusDokumenta? statusId = (selectedStatus.HasValue ? new StatusDokumenta?(selectedStatus.GetValueOrDefault()) : selectedStatus);
		IQueryable<DokumentView> source = from d in _context.Dokument.Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat).Include((Dokument d) => d.VrstaDokumenta)
				.Include((Dokument d) => d.ApplicationUser)
										  where d.VrstaDokumenta.Skracenica != "PS" && (!selectedVD.HasValue || d.VrstaDokumentaId == vrstaId) && (!selectedStatus.HasValue || (int?)d.Status == (int?)statusId) && (int)d.Validacija == 0
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
											  BrojVeznogDokumenta = dokumenti.BrojVeznogDokumenta,
											  Broj = dokumenti.Broj
										  };
		return View(await source.ToListAsync());
	}

	[Authorize(Roles = "UserAdministrator, Korisnik, Magacioner")]
	public async Task<IActionResult> IndexZatvoreni(int? selectedVD)
	{
		List<VrstaDokumenta> items = (from d in _context.VrstaDokumenta
									  where d.Skracenica != "PS"
									  orderby d.Naziv
									  select d).ToList();
		base.ViewBag.selectedVD = new SelectList(items, "Id", "Naziv", selectedVD).OrderBy((SelectListItem d) => d.Text);
		int vrstaId = selectedVD.GetValueOrDefault();
		IQueryable<DokumentView> source = from d in _context.Dokument.Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat).Include((Dokument d) => d.VrstaDokumenta)
				.Include((Dokument d) => d.ApplicationUser)
										  where d.VrstaDokumenta.Skracenica != "PS" && (!selectedVD.HasValue || d.VrstaDokumentaId == vrstaId) && (int)d.Status == 1 && (int)d.Validacija != 0
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
											  BrojVeznogDokumenta = dokumenti.BrojVeznogDokumenta,
											  Broj = dokumenti.Broj
										  };
		return View(await source.ToListAsync());
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> Details(int? id)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DokumentStavkeViewModel viewModel = new DokumentStavkeViewModel();
		IQueryable<DokumentView> source = from dokument in _context.Dokument
										  where (int?)dokument.Id == id
										  select new DokumentView
										  {
											  Id = dokument.Id,
											  VrstaDokumenta = dokument.VrstaDokumenta,
											  DateCreated = dokument.DateCreated,
											  DateDoc = dokument.DateDoc,
											  DateUpdated = dokument.DateUpdated,
											  ObjekatId = dokument.ObjekatId,
											  Objekat = dokument.Objekat,
											  PartnerId = dokument.PartnerId.GetValueOrDefault(),
											  PrevoznikId = dokument.PrevoznikId.GetValueOrDefault(),
											  Partner = dokument.Partner,
											  Status = dokument.Status,
											  Opis = dokument.Opis,
											  UserId = dokument.UserId,
											  ApplicationUser = dokument.ApplicationUser,
											  VozacId = dokument.VozacId,
											  VoziloId = dokument.VoziloId,
											  BrojVeznogDokumenta = dokument.BrojVeznogDokumenta,
											  Broj = dokument.Broj
										  };
		IQueryable<DokumentStavkeView> results = from stavke in _context.DokumentStavka
												 where stavke.DokumentId == id
												 select new DokumentStavkeView
												 {
													 Id = stavke.Id,
													 Stavka = stavke.Stavka,
													 ZaliheId = stavke.ZaliheId,
													 Zalihe = stavke.Zalihe,
													 Kolicina = stavke.Kolicina,
													 JedMere = stavke.JedMere,
													 Cena = stavke.Cena,
													 Vrednost = stavke.Cena * stavke.Kolicina
												 };
		DokumentStavkeViewModel dokumentStavkeViewModel = viewModel;
		dokumentStavkeViewModel.DokumentView = await source.SingleOrDefaultAsync();
		dokumentStavkeViewModel = viewModel;
		dokumentStavkeViewModel.DokumentStavke = await results.ToListAsync();
		if (viewModel == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		return View(viewModel);
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public IActionResult Create()
	{
		DropDownList();
		return View();
	}

	public void DropDownList()
	{
		List<Partner> list = (from d in _context.Partner
							  where (int)d.Tip != 3
							  orderby d.Naziv
							  select d).ToList();
		list.Insert(0, new Partner
		{
			Naziv = "Izaberi poslovnog partnera"
		});
		base.ViewBag.PartnerId = new SelectList(list, "Id", "Naziv");
		List<SelectListItem> list2 = new SelectList(from d in _context.Partner
													where (int)d.Tip == 3 || (int)d.Tip == 5 || (int)d.Tip == 6 || (int)d.Tip == 7
													orderby d.Naziv
													select d, "Id", "Naziv").ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi prevoznika",
			Value = null,
			Selected = true
		});
		base.ViewData["PrevoznikId"] = list2;
		list2 = new SelectList(_context.Objekat.OrderBy((Objekat d) => d.Naziv), "Id", "Naziv").ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi objekat skladištenja",
			Value = null,
			Selected = true
		});
		base.ViewData["ObjekatId"] = list2;
		list2 = new SelectList(from d in _context.VrstaDokumenta.ToList()
							   where d.Naziv != "Početno stanje"
							   orderby d.Naziv
							   select new { d.Id, d.Naziv }, "Id", "Naziv").OrderBy((SelectListItem d) => d.Text).ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi vrstu dokumenta",
			Value = null,
			Selected = true
		});
		base.ViewData["VrstaDokumentaId"] = list2;
		IQueryable<string> queryable = from table in _context.ApplicationUser
									   where table.Id == _userManager.GetUserId(User)
									   orderby string.Concat(table.FirstName + " ", table.LastName)
									   select string.Concat(table.FirstName + " ", table.LastName);
		base.ViewData["UserId"] = GetUser();
		list2 = new SelectList(from d in _context.Zalihe.ToList()
							   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
							   orderby (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
							   select new
							   {
								   Id = d.Id,
								   Naziv = (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
							   }, "Id", "Naziv").OrderBy((SelectListItem d) => d.Text).ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi stavku",
			Value = null,
			Selected = true
		});
		base.ViewData["ZaliheId"] = list2;
		list2 = new SelectList(from d in _context.VrstaDokumenta.ToList()
							   orderby d.Naziv
							   select new { d.Id, d.Naziv }, "Id", "Naziv").ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi vezni dokument",
			Value = null,
			Selected = true
		});
		base.ViewData["VezniDokumentId"] = list2;
		list2 = new SelectList(from d in _context.Vozila.ToList()
							   orderby d.RegBroj
							   select new { d.Id, d.RegBroj }, "Id", "RegBroj").ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi reg. broj vozila",
			Value = null,
			Selected = true
		});
		base.ViewData["VoziloId"] = list2;
		list2 = new SelectList(from d in _context.Vozac.ToList()
							   orderby d.Prezime + " " + d.Ime
							   select new
							   {
								   Id = d.Id,
								   Naziv = d.Prezime + " " + d.Ime
							   }, "Id", "Naziv").ToList();
		list2.Insert(0, new SelectListItem
		{
			Text = "Izaberi  vozača",
			Value = null,
			Selected = true
		});
		base.ViewData["VozacId"] = list2;
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> Create(DokumentStavkeViewModel model)
	{
		base.ModelState.Clear();
		try
		{
			model.DokumentView.DateCreated = DateTime.Today;
			model.DokumentView.DateUpdated = DateTime.Today;
			if (model.DokumentView.DateDoc.ToString() == "0001-01-01 00:00:00")
			{
				model.DokumentView.DateDoc = DateTime.Today;
			}
			model.DokumentView.UserId = _userManager.GetUserId(base.User);
			model.DokumentView.Status = StatusDokumenta.Otvoren;
			if (base.ModelState.IsValid)
			{
				if (model.DokumentView.UserId == null)
				{
					return CustomErrorMessage("Ne možete da kreirate dokumenat", "Create");
				}
				if (model.DokumentView.ObjekatId == 0)
				{
					return CustomErrorMessage("Izaberite objekat skladištenja", "Create");
				}
				if (model.DokumentView.PartnerId == 0)
				{
					model.DokumentView.PartnerId = null;
				}
				if (model.DokumentView.VrstaDokumentaId == 0)
				{
					return CustomErrorMessage("Izaberite vrstu dokumenta.", "Create");
				}
				if (model.DokumentView.Broj == string.Empty)
				{
					return CustomErrorMessage("Ne možete kreirati dokument bez unetog broja", "Create");
				}
				if (model.DokumentStavke.Count == 0)
				{
					return CustomErrorMessage("Ne možete kreirati dokument bez stavki", "Create");
				}
				using (new ApplicationDbContext())
				{
					Dokument dokument = new Dokument
					{
						VrstaDokumentaId = model.DokumentView.VrstaDokumentaId,
						DateCreated = model.DokumentView.DateCreated,
						DateUpdated = model.DokumentView.DateUpdated,
						DateDoc = model.DokumentView.DateDoc,
						Opis = model.DokumentView.Opis,
						UserId = model.DokumentView.UserId,
						Status = model.DokumentView.Status,
						ObjekatId = model.DokumentView.ObjekatId,
						PartnerId = model.DokumentView.PartnerId,
						PrevoznikId = model.DokumentView.PrevoznikId,
						VrstaVezniDokumentId = model.DokumentView.VrstaVezniDokumentId,
						VezniDokumentId = model.DokumentView.VezniDokumentId,
						KorisnikId = model.DokumentView.KorisnikId,
						Validacija = model.DokumentView.Validacija,
						DateValidated = model.DokumentView.DateValidated,
						Napomena = model.DokumentView.Napomena,
						Broj = model.DokumentView.Broj,
						VozacId = model.DokumentView.VozacId,
						VoziloId = model.DokumentView.VoziloId,
						BrojVeznogDokumenta = model.DokumentView.BrojVeznogDokumenta,
						GenDoc = model.DokumentView.GenDoc
					};
					_context.Dokument.Add(dokument);
					await _context.SaveChangesAsync();
					int id = dokument.Id;
					if (model.DokumentStavke[0].ZaliheId != 0 || model.DokumentStavke[0].Kolicina != 0.0)
					{
						int k = 0;
						foreach (DokumentStavkeView item2 in model.DokumentStavke)
						{
							k++;
							if (item2.ZaliheId == 0)
							{
								continue;
							}
							if (item2.Kolicina > 0.0 || item2.Kolicina.ToString() != string.Empty)
							{
								item2.DokumentId = id;
								_context.DokumentStavka.Add(new DokumentStavka
								{
									DokumentId = item2.DokumentId,
									Stavka = k,
									ZaliheId = item2.ZaliheId,
									Kolicina = item2.Kolicina,
									Cena = item2.Cena,
									JedMere = _context.Zalihe.FirstOrDefault((Zalihe d) => d.Id == item2.ZaliheId).JedMere
								});
								await _context.SaveChangesAsync();
							}
							else
							{
								base.ModelState.AddModelError("Kolicina", "Količina mora biti veća od 0");
							}
						}
					}
					new Dokument();
					if (dokument.GenDoc && dokument.VrstaDokumentaId == VrstaDokumenta("O"))
					{
						Dokument im = new Dokument
						{
							VrstaDokumentaId = VrstaDokumenta("IM"),
							DateCreated = model.DokumentView.DateCreated,
							DateUpdated = model.DokumentView.DateUpdated,
							DateDoc = model.DokumentView.DateDoc,
							UserId = model.DokumentView.UserId,
							Status = StatusDokumenta.Otvoren,
							Validacija = ((model.DokumentView.Validacija == StatusPotvrde.Otpremio) ? StatusPotvrde.Otpremio : StatusPotvrde.Na_cekanju),
							ObjekatId = model.DokumentView.ObjekatId,
							VrstaVezniDokumentId = VrstaDokumenta("O"),
							VezniDokumentId = id,
							BrojVeznogDokumenta = model.DokumentView.Broj,
							Broj = model.DokumentView.Broj,
							KorisnikId = model.DokumentView.KorisnikId,
							Napomena = "Automatski generisan dokument na osnovu otpremnice " + model.DokumentView.Broj
						};
						_context.Dokument.Add(im);
						await _context.SaveChangesAsync();
						List<DokumentStavkeView> list = new List<DokumentStavkeView>();
						int k = 0;
						foreach (DokumentStavkeView item in model.DokumentStavke)
						{
							Receptura receptura = (from d in _context.Receptura
												   where d.ProizvodId == item.ZaliheId
												   orderby d.DateDoc, d.Id
												   select d).LastOrDefault();
							if (receptura == null)
							{
								continue;
							}
							List<RecepturaStavka> list2 = _context.RecepturaStavka.Where((RecepturaStavka d) => d.RecepturaId == receptura.Id).ToList();
							foreach (RecepturaStavka item3 in list2)
							{
								if (item.Kolicina > 0.0 || item.Kolicina.ToString() != string.Empty)
								{
									list.Add(new DokumentStavkeView());
									list[k].DokumentId = im.Id;
									list[k].Stavka = k + 1;
									list[k].ZaliheId = item3.MaterijalId;
									list[k].Kolicina = item3.Kolicina - item3.Skart;
									list[k].JedMere = item3.JedMere;
									k++;
								}
							}
						}
						IEnumerable<DokumentStavkeView> enumerable = from r in list
																	 group r by new { r.ZaliheId, r.JedMere } into g
																	 select new DokumentStavkeView
																	 {
																		 ZaliheId = g.Key.ZaliheId,
																		 JedMere = g.Key.JedMere,
																		 Kolicina = g.Sum((DokumentStavkeView d) => d.Kolicina)
																	 };
						k = 0;
						foreach (DokumentStavkeView item4 in enumerable)
						{
							k++;
							_context.DokumentStavka.Add(new DokumentStavka
							{
								DokumentId = im.Id,
								Stavka = k,
								ZaliheId = item4.ZaliheId,
								Kolicina = item4.Kolicina,
								JedMere = item4.JedMere
							});
							await _context.SaveChangesAsync();
						}
						new Dokument();
						if (dokument.GenDoc && dokument.VrstaDokumentaId == VrstaDokumenta("O"))
						{
							Dokument pp = new Dokument
							{
								VrstaDokumentaId = VrstaDokumenta("PP"),
								DateCreated = model.DokumentView.DateCreated,
								DateUpdated = model.DokumentView.DateUpdated,
								DateDoc = model.DokumentView.DateDoc,
								UserId = model.DokumentView.UserId,
								Status = StatusDokumenta.Otvoren,
								Validacija = ((model.DokumentView.Validacija == StatusPotvrde.Otpremio) ? StatusPotvrde.Primio : StatusPotvrde.Na_cekanju),
								ObjekatId = model.DokumentView.ObjekatId,
								VrstaVezniDokumentId = VrstaDokumenta("O"),
								VezniDokumentId = id,
								BrojVeznogDokumenta = model.DokumentView.Broj,
								Broj = model.DokumentView.Broj,
								KorisnikId = model.DokumentView.KorisnikId,
								Napomena = "Automatski generisan dokument na osnovu otpremnice " + model.DokumentView.Broj
							};
							_context.Dokument.Add(pp);
							await _context.SaveChangesAsync();
							k = 0;
							foreach (DokumentStavkeView item5 in model.DokumentStavke)
							{
								if (item5.ZaliheId != 0)
								{
									_context.DokumentStavka.Add(new DokumentStavka
									{
										DokumentId = pp.Id,
										Stavka = k + 1,
										ZaliheId = item5.ZaliheId,
										Kolicina = item5.Kolicina,
										JedMere = item5.JedMere
									});
									k++;
								}
							}
							await _context.SaveChangesAsync();
						}
					}
				}
			}
			return RedirectToAction("Edit", new
			{
				id = model.DokumentStavke.First().DokumentId
			});
		}
		catch (RetryLimitExceededException)
		{
			base.ModelState.AddModelError("", "Promene nisu sačuvane. Pokušajte ponovo, ako problem i dalje postoji obratite se administratoru.");
		}
		DropDownList();
		return View("Index");
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
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

	[Authorize(Roles = "UserAdministrator, Korisnik, Magacioner")]
	public async Task<IActionResult> EditPotvrda(int? id)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DokumentStavkeViewModel model = new DokumentStavkeViewModel();
		IQueryable<DokumentView> source = from d in _context.Dokument
										  where d.Id == (int)id
										  select d into dokumenti
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
											  PartnerId = dokumenti.PartnerId,
											  PrevoznikId = dokumenti.PrevoznikId,
											  UserId = dokumenti.UserId,
											  ApplicationUser = dokumenti.ApplicationUser,
											  VrstaVezniDokumentId = dokumenti.VrstaVezniDokumentId,
											  VezniDokumentId = dokumenti.VezniDokumentId,
											  KorisnikId = _userManager.GetUserId(User),
											  Validacija = dokumenti.Validacija,
											  DateValidated = DateTime.Today,
											  Napomena = dokumenti.Napomena,
											  VozacId = dokumenti.VozacId,
											  VoziloId = dokumenti.VoziloId,
											  BrojVeznogDokumenta = dokumenti.BrojVeznogDokumenta,
											  Broj = dokumenti.Broj
										  };
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
		DokumentStavkeViewModel dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentView = await source.SingleAsync();
		dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentStavke = await items.ToListAsync();
		if (model == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DropDownList();
		base.ViewData["DateDoc"] = DateTime.Today.ToString("dd/MM/yyyy");
		base.ViewData["DateValidated"] = DateTime.Today.ToString("dd/MM/yyyy");
		return View(model);
	}

	[Authorize(Roles = "UserAdministrator, Korisnik, Magacioner")]
	[HttpPost]
	[ActionName("EditPotvrda")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditPotvrda(int id, DokumentStavkeViewModel model)
	{
		if (id != model.DokumentView.Id)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		try
		{
			Dokument dokument = _context.Dokument.FirstOrDefault((Dokument d) => d.Id == id);
			dokument.KorisnikId = GetUser();
			dokument.DateValidated = model.DokumentView.DateValidated;
			dokument.Validacija = model.DokumentView.Validacija;
			dokument.Napomena = model.DokumentView.Napomena;
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!DokumentStavkeViewModelExists(model.DokumentView.Id))
			{
				return CustomErrorMessage("Ne postoji traženi dokument", "Index");
			}
			throw;
		}
		return RedirectToAction("IndexPotvrda");
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> Copy(int id)
	{
		try
		{
			DokumentStavkeViewModel model = new DokumentStavkeViewModel();
			IQueryable<DokumentView> source = from dokumenti in _context.Dokument.Where((Dokument d) => d.Id == id).Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat)
					.Include((Dokument d) => d.VrstaDokumenta)
					.Include((Dokument d) => d.ApplicationUser)
											  select new DokumentView
											  {
												  VrstaDokumenta = dokumenti.VrstaDokumenta,
												  VrstaDokumentaId = dokumenti.VrstaDokumentaId,
												  DateCreated = DateTime.Today,
												  DateUpdated = DateTime.Today,
												  DateDoc = DateTime.Today,
												  Opis = dokumenti.Opis,
												  Status = StatusDokumenta.Otvoren,
												  ObjekatId = dokumenti.ObjekatId,
												  Objekat = dokumenti.Objekat,
												  PartnerId = dokumenti.PartnerId.GetValueOrDefault(),
												  PrevoznikId = dokumenti.PrevoznikId.GetValueOrDefault(),
												  Partner = dokumenti.Partner,
												  UserId = _userManager.GetUserId(User),
												  ApplicationUser = dokumenti.ApplicationUser,
												  VrstaVezniDokumentId = dokumenti.VrstaVezniDokumentId,
												  VozacId = dokumenti.VozacId,
												  VoziloId = dokumenti.VoziloId,
												  BrojVeznogDokumenta = dokumenti.BrojVeznogDokumenta
											  };
			IQueryable<DokumentStavkeView> items = from stavke in _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id).Include((DokumentStavka d) => d.Zalihe)
												   select new DokumentStavkeView
												   {
													   Stavka = stavke.Stavka,
													   ZaliheId = stavke.ZaliheId,
													   Zalihe = stavke.Zalihe,
													   Marka = stavke.Zalihe.Marka,
													   Cena = stavke.Cena,
													   Kolicina = stavke.Kolicina,
													   JedMere = stavke.JedMere,
													   Vrednost = stavke.Cena * stavke.Kolicina
												   };
			DokumentStavkeViewModel dokumentStavkeViewModel = model;
			dokumentStavkeViewModel.DokumentView = await source.FirstAsync();
			dokumentStavkeViewModel = model;
			dokumentStavkeViewModel.DokumentStavke = await items.ToListAsync();
			if (model == null)
			{
				return CustomErrorMessage("Ne postoji traženi dokument", "Index");
			}
			DropDownList();
			base.ViewData["DateDoc"] = DateTime.Today.ToString("dd/MM/yyyy");
			return View(model);
		}
		catch (RetryLimitExceededException)
		{
			base.ModelState.AddModelError("", "Promene nisu sačuvane. Pokušajte ponovo, ako problem i dalje postoji obratite se administratoru.");
		}
		return RedirectToAction("Index");
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> Delete(int? id)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DokumentStavkeViewModel model = new DokumentStavkeViewModel();
		IQueryable<DokumentView> source = from dokument in _context.Dokument
										  where (int?)dokument.Id == id
										  select new DokumentView
										  {
											  Id = dokument.Id,
											  VrstaDokumentaId = dokument.VrstaDokumentaId,
											  DateCreated = dokument.DateCreated,
											  DateUpdated = dokument.DateUpdated,
											  DateDoc = dokument.DateDoc,
											  Status = dokument.Status,
											  Opis = dokument.Opis,
											  VezniDokumentId = dokument.VezniDokumentId.GetValueOrDefault(),
											  UserId = dokument.UserId,
											  ObjekatId = dokument.ObjekatId,
											  PartnerId = dokument.PartnerId.GetValueOrDefault(),
											  PrevoznikId = dokument.PrevoznikId.GetValueOrDefault(),
											  ApplicationUser = dokument.ApplicationUser,
											  Partner = dokument.Partner,
											  Objekat = dokument.Objekat,
											  VrstaDokumenta = dokument.VrstaDokumenta
										  };
		IQueryable<DokumentStavkeView> modelStavke = from stavke in _context.DokumentStavka
													 where stavke.DokumentId == id
													 select new DokumentStavkeView
													 {
														 Id = stavke.Id,
														 Stavka = stavke.Stavka,
														 DokumentId = stavke.DokumentId,
														 Kolicina = stavke.Kolicina,
														 JedMere = stavke.JedMere,
														 Cena = stavke.Cena
													 };
		DokumentStavkeViewModel dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentView = await source.SingleOrDefaultAsync();
		dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentStavke = await modelStavke.ToListAsync();
		if (model == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		return View(model);
	}

	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		Dokument dokument = _context.Dokument.Where((Dokument d) => d.Id == id).FirstOrDefault();
		if (dokument == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		if (dokument.Status == StatusDokumenta.Zatvoren)
		{
			return CustomErrorMessage("Ne možete obrisati dokument koji je zatvoren ", "Index");
		}
		IQueryable<DokumentStavka> queryable = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id);
		if (queryable == null)
		{
			return CustomErrorMessage("Ne postoje stavke dokumenta", "Index");
		}
		foreach (DokumentStavka item in queryable)
		{
			_context.DokumentStavka.Remove(item);
		}
		_context.Dokument.Remove(dokument);
		await _context.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	private bool DokumentStavkeViewModelExists(int id)
	{
		return _context.Dokument.Any((Dokument e) => e.Id == id);
	}

	public int GetStavke(int id)
	{
		int num = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id).Count();
		return num + 1;
	}

	public void RenumerateItems(int id)
	{
		int num = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id).Count();
		if (num != 0)
		{
			List<DokumentStavka> list = (from d in _context.DokumentStavka
										 where d.DokumentId == (int?)id
										 orderby d.Stavka
										 select d).ToList();
			for (int i = 1; i <= num; i++)
			{
				list[i].Stavka = i;
			}
		}
		_context.SaveChanges();
	}

	public double GetCena(int idzalihe, int idobjekat)
	{
		double result = 0.0;
		int? vrstaZalihaId = _context.Zalihe.Where((Zalihe z) => z.Id == idzalihe).SingleOrDefault().VrstaZalihaId;
		ObjekatZalihe objekatZalihe = _context.ObjekatZalihe.Where((ObjekatZalihe z) => z.ObjekatId == (int?)idobjekat && z.ZaliheId == idzalihe).SingleOrDefault();
		if (objekatZalihe != null)
		{
			result = objekatZalihe.Cena;
		}
		return result;
	}

	[AcceptVerbs(new string[] { })]
	public IActionResult GetItemsCena(int idzalihe, int? idobjekat)
	{
		DokumentStavkeView dokumentStavkeView = new DokumentStavkeView();
		Zalihe zalihe = _context.Zalihe.Where((Zalihe z) => z.Id == idzalihe).FirstOrDefault();
		dokumentStavkeView.JedMere = zalihe.JedMere;
		dokumentStavkeView.Marka = zalihe.Marka;
		ObjekatZalihe objekatZalihe = _context.ObjekatZalihe.Where((ObjekatZalihe z) => z.ObjekatId == idobjekat && z.ZaliheId == idzalihe).FirstOrDefault();
		if (objekatZalihe == null)
		{
			dokumentStavkeView.Cena = 0.0;
		}
		else
		{
			dokumentStavkeView.JedMere = zalihe.JedMere;
			dokumentStavkeView.Marka = zalihe.Marka;
			dokumentStavkeView.Cena = objekatZalihe.Cena;
		}
		dokumentStavkeView.Vrednost = dokumentStavkeView.Kolicina * dokumentStavkeView.Cena;
		return Json(dokumentStavkeView);
	}

	public double GetProsecnaCena(int idzalihe, int idobjekat)
	{
		double result = 0.0;
		int? vrstaZalihaId = _context.Zalihe.Where((Zalihe z) => z.Id == idzalihe).SingleOrDefault().VrstaZalihaId;
		ObjekatZalihe objekatZalihe = _context.ObjekatZalihe.Where((ObjekatZalihe z) => z.ObjekatId == (int?)idobjekat && z.ZaliheId == idzalihe).SingleOrDefault();
		if (objekatZalihe != null)
		{
			result = objekatZalihe.Cena;
		}
		return result;
	}

	[AcceptVerbs(new string[] { })]
	public IActionResult GetVrednost(double cena, double kolicina)
	{
		DokumentStavkeView data = new DokumentStavkeView
		{
			Vrednost = cena * kolicina
		};
		return Json(data);
	}

	public string GetUser()
	{
		IQueryable<string> source = from table in _context.ApplicationUser
									where table.Id == _userManager.GetUserId(User) && (User.IsInRole("Korisnik") || User.IsInRole("UserAdministrator"))
									select string.Concat(table.FirstName + " ", table.LastName);
		return source.SingleOrDefault();
	}

	public string GetKorisnik()
	{
		IQueryable<string> source = from table in _context.ApplicationUser
									where table.Id == _userManager.GetUserId(User) && User.IsInRole("Magacioner")
									select string.Concat(table.FirstName + " ", table.LastName);
		return source.SingleOrDefault();
	}

	public IActionResult CustomErrorMessage(string e, string url, string c = "DokumentStavke")
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
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Save(DokumentStavkeViewModel model)
	{
		await Create(model);
		await Final(model.DokumentStavke.First().DokumentId);
		return RedirectToAction("IndexZatvoreni");
	}

	public async Task<IActionResult> Final(int? id)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DokumentStavkeViewModel model = new DokumentStavkeViewModel();
		IQueryable<DokumentView> source = from dokument in _context.Dokument
										  where (int?)dokument.Id == id
										  select new DokumentView
										  {
											  Id = dokument.Id,
											  VrstaDokumentaId = dokument.VrstaDokumentaId,
											  DateCreated = dokument.DateCreated,
											  DateUpdated = dokument.DateUpdated,
											  DateDoc = dokument.DateDoc,
											  Status = dokument.Status,
											  Opis = dokument.Opis,
											  VezniDokumentId = dokument.VezniDokumentId.GetValueOrDefault(),
											  UserId = dokument.UserId,
											  ObjekatId = dokument.ObjekatId,
											  PartnerId = dokument.PartnerId.GetValueOrDefault(),
											  PrevoznikId = dokument.PrevoznikId.GetValueOrDefault(),
											  ApplicationUser = dokument.ApplicationUser,
											  Partner = dokument.Partner,
											  Objekat = dokument.Objekat,
											  VrstaDokumenta = dokument.VrstaDokumenta
										  };
		IQueryable<DokumentStavkeView> modelStavke = from stavke in _context.DokumentStavka
													 where stavke.DokumentId == id
													 select new DokumentStavkeView
													 {
														 Id = stavke.Id,
														 Stavka = stavke.Stavka,
														 DokumentId = stavke.Id,
														 Kolicina = stavke.Kolicina,
														 JedMere = stavke.JedMere,
														 Cena = stavke.Cena
													 };
		DokumentStavkeViewModel dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentView = await source.SingleOrDefaultAsync();
		dokumentStavkeViewModel = model;
		dokumentStavkeViewModel.DokumentStavke = await modelStavke.ToListAsync();
		if (model == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		return View(model);
	}

	[HttpPost]
	[ActionName("Final")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> FinalVersion(int id)
	{
		Dokument doc = (from d in _context.Dokument.Include((Dokument d) => d.VrstaDokumenta)
						where d.Id == id
						select d).SingleOrDefault();
		if (doc.Validacija == StatusPotvrde.Na_cekanju && doc.VrstaDokumenta.Skracenica != "PS")
		{
			return CustomErrorMessage("Nije potvrdjen status zaliha", "Index");
		}
		if (doc == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		if (doc.Status == StatusDokumenta.Zatvoren)
		{
			if (doc.VrstaDokumenta.Skracenica == "PS")
			{
				return RedirectToAction("IndexStanje");
			}
			return RedirectToAction("Index");
		}
		List<DokumentStavka> list = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id).ToList();
		if (list == null)
		{
			return CustomErrorMessage("Ne postoje stavke dokumenta", "Index");
		}
		foreach (DokumentStavka item in list)
		{
			ObjekatZalihe objekatZalihe = _context.ObjekatZalihe.Where((ObjekatZalihe d) => d.ObjekatId == doc.ObjekatId && d.ZaliheId == item.ZaliheId).FirstOrDefault();
			if (objekatZalihe == null || objekatZalihe.Kolicina <= 0.0)
			{
				if (doc.VrstaDokumenta.Skracenica != "O" && doc.VrstaDokumenta.Skracenica != "IM")
				{
					ObjekatZalihe entity = new ObjekatZalihe
					{
						ObjekatId = doc.ObjekatId,
						ZaliheId = item.ZaliheId,
						Kolicina = item.Kolicina,
						Cena = item.Cena
					};
					_context.ObjekatZalihe.Add(entity);
					continue;
				}
				return CustomErrorMessage("Na stanju ne postoje zalihe ", "Index");
			}
			int num = (int)doc.VrstaDokumenta.Stanje;
			int cena = (int)doc.VrstaDokumenta.Cena;
			switch (num)
			{
				case 0:
					objekatZalihe.ObjekatId = doc.ObjekatId;
					objekatZalihe.ZaliheId = item.ZaliheId;
					objekatZalihe.Kolicina = objekatZalihe.Kolicina;
					switch (cena)
					{
						case 0:
							objekatZalihe.Cena = objekatZalihe.Cena;
							break;
						case 1:
							objekatZalihe.Cena = item.Cena;
							break;
						default:
							objekatZalihe.Cena = (objekatZalihe.Kolicina * objekatZalihe.Cena + item.Kolicina * item.Cena) / (objekatZalihe.Kolicina + item.Kolicina);
							break;
					}
					break;
				case 1:
					objekatZalihe.ObjekatId = doc.ObjekatId;
					objekatZalihe.ZaliheId = item.ZaliheId;
					objekatZalihe.Kolicina = item.Kolicina;
					objekatZalihe.Cena = item.Cena;
					break;
				case 2:
					{
						double cena2 = item.Cena;
						if (cena == 2)
						{
							cena2 = (objekatZalihe.Kolicina * objekatZalihe.Cena + item.Kolicina * item.Cena) / (objekatZalihe.Kolicina + item.Kolicina);
						}
						objekatZalihe.ObjekatId = doc.ObjekatId;
						objekatZalihe.ZaliheId = item.ZaliheId;
						objekatZalihe.Kolicina += item.Kolicina;
						objekatZalihe.Cena = cena2;
						break;
					}
				case 3:
					objekatZalihe.ObjekatId = doc.ObjekatId;
					objekatZalihe.ZaliheId = item.ZaliheId;
					objekatZalihe.Kolicina -= item.Kolicina;
					break;
			}
		}
		doc.Status = StatusDokumenta.Zatvoren;
		doc.UserId = _userManager.GetUserId(base.User);
		await _context.SaveChangesAsync();
		if (doc.VrstaDokumenta.Skracenica == "PS")
		{
			return RedirectToAction("IndexStanje");
		}
		return RedirectToAction("Index", doc.VrstaDokumentaId);
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public IActionResult CreateItem(int? id, int objekatId)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		DokumentStavkeView model = new DokumentStavkeView
		{
			DokumentId = id,
			ObjekatId = objekatId
		};
		List<SelectListItem> list = new SelectList(from d in _context.Zalihe
												   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
												   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   select new
												   {
													   Id = d.Id,
													   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   }, "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi stavku",
			Value = null,
			Selected = true
		});
		base.ViewData["ZaliheId"] = list;
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> CreateItem(DokumentStavkeView model, int id)
	{
		try
		{
			if (base.ModelState.IsValid && model != null)
			{
				_context.DokumentStavka.Add(new DokumentStavka
				{
					Stavka = GetStavke(id),
					DokumentId = id,
					ZaliheId = model.ZaliheId,
					Kolicina = model.Kolicina,
					Cena = model.Cena,
					JedMere = model.JedMere
				});
			}
			await _context.SaveChangesAsync();
			return RedirectToAction("Edit", new { id });
		}
		catch (RetryLimitExceededException)
		{
			base.ModelState.AddModelError("", "Promene nisu sačuvane. Pokušajte ponovo, ako problem i dalje postoji obratite se administratoru.");
		}
		SelectList selectList = new SelectList(from d in _context.Zalihe
											   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
											   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
											   select new
											   {
												   Id = d.Id,
												   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
											   }, "Id", "Naziv", _context.Zalihe);
		selectList.Append(new SelectListItem
		{
			Text = "Unesi stavku",
			Value = null
		});
		base.ViewData["ZaliheId"] = selectList;
		return View("Edit");
	}

	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> EditStavka(int? id)
	{
		if (!id.HasValue)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		IQueryable<DokumentStavkeView> queryable = from stavke in _context.DokumentStavka.Where((DokumentStavka d) => (int?)d.Id == id).Include((DokumentStavka d) => d.Zalihe)
												   select new DokumentStavkeView
												   {
													   Id = stavke.Id,
													   Stavka = stavke.Stavka,
													   ZaliheId = stavke.ZaliheId,
													   Zalihe = stavke.Zalihe,
													   Cena = stavke.Cena,
													   Kolicina = stavke.Kolicina,
													   JedMere = stavke.JedMere,
													   Vrednost = stavke.Cena * stavke.Kolicina,
													   DokumentId = stavke.DokumentId
												   };
		if (queryable == null)
		{
			return CustomErrorMessage("Ne postoji traženi dokument", "Index");
		}
		List<SelectListItem> list = new SelectList(from d in _context.Zalihe
												   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
												   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   select new
												   {
													   Id = d.Id,
													   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   }, "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Selected = true,
			Text = string.Empty,
			Value = string.Empty
		});
		base.ViewData["ZaliheId"] = list;
		return View(await queryable.SingleOrDefaultAsync());
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> EditStavka(int id, DokumentStavkeView model)
	{
		if (id != model.Id)
		{
			return NotFound();
		}
		if (base.ModelState.IsValid)
		{
			try
			{
				_context.DokumentStavka.Update(new DokumentStavka
				{
					Stavka = model.Stavka,
					ZaliheId = model.ZaliheId,
					Kolicina = model.Kolicina,
					Cena = model.Cena,
					JedMere = model.JedMere
				});
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!DokumentStavkeViewModelExists(model.Id))
				{
					return NotFound();
				}
				throw;
			}
			return RedirectToAction("Index");
		}
		base.ViewData["ZaliheId"] = new SelectList(from d in _context.Zalihe
												   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
												   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   select new
												   {
													   Id = d.Id,
													   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   }, "Id", "Naziv", model.ZaliheId);
		return View(model);
	}

	[HttpPost]
	[ActionName("DeleteItem")]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "UserAdministrator, Korisnik")]
	public async Task<IActionResult> DeleteItemConfirmed(int id)
	{
		DokumentStavka dokumentStavka = await _context.DokumentStavka.FindAsync(id);
		int? dok = dokumentStavka.DokumentId;
		_context.DokumentStavka.Remove(dokumentStavka);
		IQueryable<DokumentStavkeView> queryable = from stavke in _context.DokumentStavka
												   where stavke.Id == id
												   select new DokumentStavkeView
												   {
													   Stavka = stavke.Stavka,
													   DokumentId = stavke.Id,
													   Kolicina = stavke.Kolicina,
													   JedMere = stavke.JedMere,
													   Cena = stavke.Cena,
													   Vrednost = stavke.Cena * stavke.Kolicina
												   };
		if (!queryable.Any())
		{
			return NotFound();
		}
		int num = 1;
		foreach (DokumentStavkeView item in queryable)
		{
			item.Stavka = num;
			num++;
		}
		await _context.SaveChangesAsync();
		return RedirectToAction("Edit", new
		{
			id = dok
		});
	}

	public async Task<IActionResult> IndexStanje()
	{
		IQueryable<DokumentStanjeView> source = from d in _context.Dokument.Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat).Include((Dokument d) => d.VrstaDokumenta)
				.Include((Dokument d) => d.ApplicationUser)
												where d.VrstaDokumenta.Skracenica == "PS"
												orderby d.DateDoc descending, d.Id descending
												select d into dokumenti
												select new DokumentStanjeView
												{
													Id = dokumenti.Id,
													VrstaDokumenta = dokumenti.VrstaDokumenta,
													VrstaDokumentaId = dokumenti.VrstaDokumentaId,
													DateCreated = dokumenti.DateCreated,
													DateDoc = dokumenti.DateDoc,
													DateUpdated = dokumenti.DateUpdated,
													ObjekatId = dokumenti.ObjekatId,
													Objekat = dokumenti.Objekat,
													Status = dokumenti.Status,
													Opis = dokumenti.Opis,
													UserId = dokumenti.UserId,
													ApplicationUser = dokumenti.ApplicationUser
												};
		return View(await source.ToListAsync());
	}

	public async Task<IActionResult> DetailsStanje(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		DokumentStanjeViewModel viewModel = new DokumentStanjeViewModel();
		IQueryable<DokumentStanjeView> source = from dokument in _context.Dokument
												where (int?)dokument.Id == id && dokument.VrstaDokumenta.Skracenica == "PS"
												select new DokumentStanjeView
												{
													Id = dokument.Id,
													VrstaDokumenta = dokument.VrstaDokumenta,
													VrstaDokumentaId = dokument.VrstaDokumentaId,
													DateCreated = dokument.DateCreated,
													DateDoc = dokument.DateDoc,
													DateUpdated = dokument.DateUpdated,
													ObjekatId = dokument.ObjekatId,
													Objekat = dokument.Objekat,
													Status = dokument.Status,
													Opis = dokument.Opis,
													UserId = dokument.UserId,
													ApplicationUser = dokument.ApplicationUser
												};
		IQueryable<DokumentStavkeView> results = from stavke in _context.DokumentStavka
												 where stavke.DokumentId == id
												 select new DokumentStavkeView
												 {
													 Id = stavke.Id,
													 Stavka = stavke.Stavka,
													 ZaliheId = stavke.ZaliheId,
													 Zalihe = stavke.Zalihe,
													 Kolicina = stavke.Kolicina,
													 JedMere = stavke.JedMere,
													 Cena = stavke.Cena,
													 Vrednost = stavke.Cena * stavke.Kolicina
												 };
		DokumentStanjeViewModel dokumentStanjeViewModel = viewModel;
		dokumentStanjeViewModel.DokumentStanje = await source.SingleOrDefaultAsync();
		dokumentStanjeViewModel = viewModel;
		dokumentStanjeViewModel.DokumentStavkeView = await results.ToListAsync();
		if (viewModel == null)
		{
			return NotFound();
		}
		return View(viewModel);
	}

	public IActionResult CreateStanje()
	{
		List<SelectListItem> list = new SelectList(_context.Objekat.OrderBy((Objekat d) => d.Naziv), "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi objekat skladištenja",
			Value = null,
			Selected = true
		});
		base.ViewData["ObjekatId"] = list;
		list = new SelectList(from d in _context.VrstaDokumenta.ToList()
							  where d.Naziv == "Početno stanje"
							  orderby d.Naziv
							  select new { d.Id, d.Naziv }, "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi vrstu dokumenta",
			Value = null,
			Selected = true
		});
		base.ViewData["VrstaDokumentaId"] = list;
		IQueryable<string> queryable = from table in _context.ApplicationUser
									   where table.Id == _userManager.GetUserId(User)
									   orderby string.Concat(table.FirstName + " ", table.LastName)
									   select string.Concat(table.FirstName + " ", table.LastName);
		base.ViewData["UserId"] = GetUser();
		list = new SelectList(from d in _context.Zalihe.ToList()
							  join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
							  orderby (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
							  select new
							  {
								  Id = d.Id,
								  Naziv = (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
							  }, "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi stavku",
			Value = null,
			Selected = true
		});
		base.ViewData["ZaliheId"] = list;
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> CreateStanje(DokumentStanjeViewModel model)
	{
		base.ModelState.Clear();
		model.DokumentStanje.DateCreated = DateTime.Today;
		model.DokumentStanje.DateUpdated = DateTime.Today;
		model.DokumentStanje.UserId = _userManager.GetUserId(base.User);
		model.DokumentStanje.Status = StatusDokumenta.Otvoren;
		if (model.DokumentStanje.ObjekatId == 0)
		{
			base.ModelState.AddModelError("ObjekatId", "Izaberite objekat skladištenja.");
		}
		if (model.DokumentStanje.VrstaDokumentaId == 0)
		{
			base.ModelState.AddModelError("VrstaDokumentaId", "Izaberite vrstu dokumenta početno stanje.");
		}
		if (model.DokumentStanje.Broj == string.Empty)
		{
			base.ModelState.AddModelError("VrstaDokumentaId", "Unesite broj dokumenta");
		}
		if (base.ModelState.IsValid)
		{
			try
			{
				using (new ApplicationDbContext())
				{
					Dokument dokument = new Dokument
					{
						VrstaDokumentaId = VrstaDokumenta("PS"),
						DateCreated = model.DokumentStanje.DateCreated,
						DateUpdated = model.DokumentStanje.DateUpdated,
						DateDoc = model.DokumentStanje.DateDoc,
						Opis = model.DokumentStanje.Opis,
						UserId = model.DokumentStanje.UserId,
						Status = model.DokumentStanje.Status,
						ObjekatId = model.DokumentStanje.ObjekatId,
						Broj = model.DokumentStanje.Broj
					};
					_context.Dokument.Add(dokument);
					await _context.SaveChangesAsync();
					if (dokument.Id <= 0)
					{
						return CustomErrorMessage("Ne možete sačuvati dokument", "IndexStanje");
					}
					int id = dokument.Id;
					if (model.DokumentStavkeView != null)
					{
						int i = 0;
						foreach (DokumentStavkeView item in model.DokumentStavkeView)
						{
							if (item.ZaliheId > 0)
							{
								item.DokumentId = id;
								_context.DokumentStavka.Add(new DokumentStavka
								{
									DokumentId = item.DokumentId,
									Stavka = i + 1,
									ZaliheId = item.ZaliheId,
									Kolicina = item.Kolicina,
									Cena = item.Cena,
									JedMere = item.JedMere
								});
								await _context.SaveChangesAsync();
							}
						}
					}
					return RedirectToAction("EditStanje", new
					{
						id = model.DokumentStavkeView.First().DokumentId
					});
				}
			}
			catch (RetryLimitExceededException)
			{
				return CustomErrorMessage("Ne možete sačuvati dokument", "IndexStanje");
			}
		}
		base.ModelState.AddModelError("", "Promene nisu sačuvane. Pokušajte ponovo, ako problem i dalje postoji obratite se administratoru.");
		base.ViewData["ObjekatId"] = new SelectList(_context.Objekat.OrderBy((Objekat d) => d.Naziv), "Id", "Naziv", model.DokumentStanje.ObjekatId);
		base.ViewData["VrstaDokumentaId"] = new SelectList(from d in _context.VrstaDokumenta.ToList()
														   where d.Naziv != "Početno stanje"
														   orderby d.Naziv
														   select new { d.Id, d.Naziv }, "Id", "Naziv", model.DokumentStanje.VrstaDokumentaId);
		base.ViewData["UserId"] = _context.ApplicationUser.SingleOrDefault((ApplicationUser z) => z.Id == _userManager.GetUserId(User));
		SelectList selectList = new SelectList(from d in _context.Zalihe
											   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
											   orderby d.Naziv
											   select new
											   {
												   Id = d.Id,
												   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
											   }, "Id", "Naziv");
		selectList.Append(new SelectListItem
		{
			Text = "Unesi stavku",
			Value = null
		});
		base.ViewData["ZaliheId"] = selectList;
		return View("IndexStanje");
	}

	public async Task<IActionResult> EditStanje(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		DokumentStanjeViewModel model = new DokumentStanjeViewModel();
		IQueryable<DokumentStanjeView> source = from dokumenti in _context.Dokument.Where((Dokument d) => (int?)d.Id == id).Include((Dokument d) => d.Partner).Include((Dokument d) => d.Objekat)
				.Include((Dokument d) => d.VrstaDokumenta)
				.Include((Dokument d) => d.ApplicationUser)
												select new DokumentStanjeView
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
													UserId = dokumenti.UserId,
													ApplicationUser = dokumenti.ApplicationUser,
													Broj = dokumenti.Broj
												};
		IQueryable<DokumentStavkeView> items = from stavke in _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == id).Include((DokumentStavka d) => d.Zalihe)
											   select new DokumentStavkeView
											   {
												   Id = stavke.Id,
												   Stavka = stavke.Stavka,
												   ZaliheId = stavke.ZaliheId,
												   Zalihe = stavke.Zalihe,
												   Cena = stavke.Cena,
												   Kolicina = stavke.Kolicina,
												   JedMere = stavke.JedMere,
												   Vrednost = stavke.Cena * stavke.Kolicina,
												   DokumentId = stavke.DokumentId
											   };
		DokumentStanjeViewModel dokumentStanjeViewModel = model;
		dokumentStanjeViewModel.DokumentStanje = await source.FirstAsync();
		dokumentStanjeViewModel = model;
		dokumentStanjeViewModel.DokumentStavkeView = await (items?.ToListAsync());
		if (model == null)
		{
			return CustomErrorMessage("Ne postoji dokument", "IndexStanje");
		}
		base.ViewData["ObjekatId"] = new SelectList(_context.Objekat.OrderBy((Objekat d) => d.Naziv), "Id", "Naziv", model.DokumentStanje.ObjekatId);
		base.ViewData["VrstaDokumentaId"] = new SelectList(from d in _context.VrstaDokumenta.ToList()
														   where d.Naziv == "Početno stanje"
														   orderby d.Naziv
														   select new { d.Id, d.Naziv }, "Id", "Naziv", model.DokumentStanje.VrstaDokumentaId);
		base.ViewData["UserId"] = _context.ApplicationUser.SingleOrDefault((ApplicationUser z) => z.Id == _userManager.GetUserId(User));
		List<SelectListItem> list = new SelectList(from d in _context.Zalihe.ToList()
												   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
												   orderby d.Naziv
												   select new
												   {
													   Id = d.Id,
													   Naziv = (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
												   }, "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi stavku",
			Value = null,
			Selected = true
		});
		base.ViewData["ZaliheId"] = list;
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditStanje(int id, DokumentStanjeViewModel model)
	{
		if (id != model.DokumentStanje.Id)
		{
			return NotFound();
		}
		model.DokumentStanje.DateUpdated = DateTime.Today;
		model.DokumentStanje.UserId = _userManager.GetUserId(base.User);
		if (base.ModelState.IsValid)
		{
			try
			{
				Dokument dokument = _context.Dokument.FirstOrDefault((Dokument d) => d.Id == id);
				dokument.DateUpdated = model.DokumentStanje.DateUpdated;
				dokument.DateDoc = model.DokumentStanje.DateDoc;
				dokument.Status = model.DokumentStanje.Status;
				dokument.ObjekatId = model.DokumentStanje.ObjekatId;
				dokument.Opis = model.DokumentStanje.Opis;
				dokument.UserId = model.DokumentStanje.UserId;
				IQueryable<DokumentStavka> queryable = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id);
				if (model.DokumentStavkeView != null)
				{
					int num = 0;
					foreach (DokumentStavka item in queryable)
					{
						item.DokumentId = id;
						item.Stavka = num + 1;
						item.ZaliheId = model.DokumentStavkeView[num].ZaliheId;
						item.Kolicina = model.DokumentStavkeView[num].Kolicina;
						item.Cena = model.DokumentStavkeView[num].Cena;
						item.JedMere = model.DokumentStavkeView[num].JedMere;
						num++;
					}
				}
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!DokumentStavkeViewModelExists(model.DokumentStanje.Id))
				{
					return NotFound();
				}
				throw;
			}
			return RedirectToAction("IndexStanje");
		}
		base.ViewData["ObjekatId"] = new SelectList(_context.Objekat.OrderBy((Objekat d) => d.Naziv), "Id", "Naziv", model.DokumentStanje.ObjekatId);
		base.ViewData["VrstaDokumentaId"] = new SelectList(from d in _context.VrstaDokumenta.ToList()
														   where d.Naziv == "Početno stanje"
														   orderby d.Naziv
														   select new { d.Id, d.Naziv }, "Id", "Naziv", model.DokumentStanje.VrstaDokumentaId);
		base.ViewData["UserId"] = _context.ApplicationUser.SingleOrDefault((ApplicationUser z) => z.Id == _userManager.GetUserId(User));
		base.ViewData["ZaliheId"] = new SelectList(from d in _context.Zalihe
												   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
												   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " - $ "), d.Marka ?? string.Empty)
												   select new
												   {
													   Id = d.Id,
													   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " - $ "), d.Marka ?? string.Empty)
												   }, "Id", "Naziv", model.DokumentStavkeView.FirstOrDefault().ZaliheId);
		return View(model);
	}

	public async Task<IActionResult> DeleteStanje(int? id)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		DokumentStanjeViewModel model = new DokumentStanjeViewModel();
		IQueryable<DokumentStanjeView> queryable = from dokument in _context.Dokument
												   where (int?)dokument.Id == id
												   select new DokumentStanjeView
												   {
													   Id = dokument.Id,
													   VrstaDokumentaId = dokument.VrstaDokumentaId,
													   DateCreated = dokument.DateCreated,
													   DateUpdated = dokument.DateUpdated,
													   DateDoc = dokument.DateDoc,
													   Status = dokument.Status,
													   Opis = dokument.Opis,
													   UserId = dokument.UserId,
													   ObjekatId = dokument.ObjekatId,
													   ApplicationUser = dokument.ApplicationUser,
													   Objekat = dokument.Objekat,
													   VrstaDokumenta = dokument.VrstaDokumenta
												   };
		if (queryable == null)
		{
			return NotFound();
		}
		IQueryable<DokumentStavkeView> modelStavke = from stavke in _context.DokumentStavka
													 where stavke.DokumentId == id
													 select new DokumentStavkeView
													 {
														 Id = stavke.Id,
														 Stavka = stavke.Stavka,
														 DokumentId = stavke.Id,
														 Kolicina = stavke.Kolicina,
														 JedMere = stavke.JedMere,
														 Cena = stavke.Cena
													 };
		if (modelStavke == null)
		{
			return NotFound();
		}
		DokumentStanjeViewModel dokumentStanjeViewModel = model;
		dokumentStanjeViewModel.DokumentStanje = await queryable.SingleOrDefaultAsync();
		dokumentStanjeViewModel = model;
		dokumentStanjeViewModel.DokumentStavkeView = await modelStavke.ToListAsync();
		if (model == null)
		{
			return NotFound();
		}
		return View(model);
	}

	[HttpPost]
	[ActionName("DeleteStanje")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteStanjeConfirmed(int id)
	{
		Dokument dokument = await _context.Dokument.FindAsync(id);
		if (dokument == null)
		{
			return NotFound();
		}
		if (dokument.Status == StatusDokumenta.Zatvoren)
		{
			return CustomErrorMessage("Ne možete obrisati dokument koji je zatvoren", "IndexStanje");
		}
		IEnumerable<DokumentStavka> enumerable = _context.DokumentStavka.Where((DokumentStavka d) => d.DokumentId == (int?)id);
		if (enumerable == null)
		{
			return NotFound();
		}
		foreach (DokumentStavka item in enumerable)
		{
			_context.DokumentStavka.Remove(item);
		}
		_context.Dokument.Remove(dokument);
		await _context.SaveChangesAsync();
		return RedirectToAction("IndexStanje");
	}

	public IActionResult CreateStanjeItem(int? id, int objekatId)
	{
		if (!id.HasValue)
		{
			return NotFound();
		}
		DokumentStavkeView model = new DokumentStavkeView
		{
			DokumentId = id,
			ObjekatId = objekatId
		};
		List<SelectListItem> list = new SelectList(from d in _context.Zalihe
												   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
												   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   select new
												   {
													   Id = d.Id,
													   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
												   }, "Id", "Naziv").ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi stavku",
			Value = null,
			Selected = true
		});
		base.ViewData["ZaliheId"] = list;
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> CreateStanjeItem(DokumentStavkeView model, int id)
	{
		try
		{
			if (base.ModelState.IsValid && model != null)
			{
				_context.DokumentStavka.Add(new DokumentStavka
				{
					Stavka = GetStavke(id),
					DokumentId = id,
					ZaliheId = model.ZaliheId,
					Kolicina = model.Kolicina,
					Cena = model.Cena,
					JedMere = model.JedMere
				});
			}
			await _context.SaveChangesAsync();
			return RedirectToAction("EditStanje", new { id });
		}
		catch (RetryLimitExceededException)
		{
			base.ModelState.AddModelError("", "Promene nisu sačuvane. Pokušajte ponovo, ako problem i dalje postoji obratite se administratoru.");
		}
		SelectList selectList = new SelectList(from d in _context.Zalihe
											   join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
											   orderby string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
											   select new
											   {
												   Id = d.Id,
												   Naziv = string.Concat(string.Concat(string.Concat((vz.Naziv ?? string.Empty) + " ", d.Naziv), " "), d.Marka ?? string.Empty)
											   }, "Id", "Naziv", _context.Zalihe);
		selectList.Append(new SelectListItem
		{
			Text = "Unesi stavku",
			Value = null
		});
		base.ViewData["ZaliheId"] = selectList;
		return View("EditStanje");
	}

	[HttpPost]
	[ActionName("DeleteStanjeItem")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteStanjeItemConfirmed(int id)
	{
		DokumentStavka dokumentStavka = await _context.DokumentStavka.FindAsync(id);
		int? dok = dokumentStavka.DokumentId;
		_context.DokumentStavka.Remove(dokumentStavka);
		IQueryable<DokumentStavkeView> queryable = from stavke in _context.DokumentStavka
												   where stavke.Id == id
												   select new DokumentStavkeView
												   {
													   Stavka = stavke.Stavka,
													   DokumentId = stavke.Id,
													   Kolicina = stavke.Kolicina,
													   JedMere = stavke.JedMere,
													   Cena = stavke.Cena,
													   Vrednost = stavke.Cena * stavke.Kolicina
												   };
		queryable.Any();
		int num = 1;
		foreach (DokumentStavkeView item in queryable)
		{
			item.Stavka = num;
			num++;
		}
		await _context.SaveChangesAsync();
		return RedirectToAction("EditStanje", new
		{
			id = dok
		});
	}

	public IActionResult PromeneZaliheView()
	{
		PromeneZaliheView promeneZaliheView = new PromeneZaliheView();
		List<SelectListItem> list = new SelectList(_context.Objekat.OrderBy((Objekat d) => d.Naziv), "Id", "Naziv", promeneZaliheView.ObjekatId).ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi skladište",
			Value = null,
			Selected = true
		});
		base.ViewData["ObjekatId"] = list;
		list = new SelectList(_context.VrstaDokumenta.OrderBy((VrstaDokumenta d) => d.Naziv), "Id", "Naziv", promeneZaliheView.DokumentId).ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi vrstu dokumenta",
			Value = null,
			Selected = true
		});
		base.ViewData["VrstaDokumentaId"] = list;
		list = new SelectList(_context.VrstaZaliha.OrderBy((VrstaZaliha d) => d.Naziv), "Id", "Naziv", promeneZaliheView.VrstaZalihaId).ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi vrstu zaliha",
			Value = null,
			Selected = true
		});
		base.ViewData["VrstaZalihaId"] = list;
		list = new SelectList(from d in _context.Zalihe.ToList()
							  join vz in _context.VrstaZaliha on d.VrstaZalihaId equals vz.Id
							  orderby (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
							  select new
							  {
								  Id = d.Id,
								  Naziv = (vz.Naziv ?? string.Empty) + " " + d.Naziv + " " + (d.Marka ?? string.Empty)
							  }, "Id", "Naziv", promeneZaliheView.ZaliheId).ToList();
		list.Insert(0, new SelectListItem
		{
			Text = "Izaberi zalihe",
			Value = null,
			Selected = true
		});
		base.ViewData["ZaliheId"] = list;
		promeneZaliheView.OdDatuma = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		promeneZaliheView.DoDatuma = DateTime.Today;
		return View(promeneZaliheView);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> PromeneZaliheView(PromeneZaliheView model)
	{
		Objekat objekat = await _context.Objekat.Where((Objekat d) => d.Id == model.ObjekatId).FirstOrDefaultAsync();
		if (objekat != null)
		{
			model.ObjekatNaziv = objekat.Naziv;
		}
		else
		{
			model.ObjekatNaziv = "Svi objekti skladištenja";
		}
		VrstaDokumenta vrstaDokumenta = await _context.VrstaDokumenta.Where((VrstaDokumenta d) => d.Id == model.DokumentId).FirstOrDefaultAsync();
		if (vrstaDokumenta != null)
		{
			model.DokumentNaziv = vrstaDokumenta.Naziv;
		}
		else
		{
			model.DokumentNaziv = "Svi dokumenti";
		}
		VrstaZaliha vrstaZaliha = await _context.VrstaZaliha.Where((VrstaZaliha d) => d.Id == model.VrstaZalihaId).FirstOrDefaultAsync();
		if (vrstaZaliha != null)
		{
			model.VrstaZalihaNaziv = vrstaZaliha.Naziv;
		}
		else
		{
			model.VrstaZalihaNaziv = "Sve vrste zaliha";
		}
		Zalihe zalihe = await _context.Zalihe.Where((Zalihe d) => d.Id == model.ZaliheId).FirstOrDefaultAsync();
		if (zalihe != null)
		{
			model.ZaliheNaziv = zalihe.Naziv;
		}
		else
		{
			model.ZaliheNaziv = "Sve zalihe";
		}
		return RedirectToAction("ZaliheIzvestaj", new { model.OdDatuma, model.DoDatuma, model.ObjekatId, model.DokumentId, model.VrstaZalihaId, model.ZaliheId });
	}

	public IActionResult ZaliheIzvestaj(DateTime odDatuma, DateTime doDatuma, int? objekatId, int? dokumentId, int? vrstaZalihaId, int? zaliheId)
	{
		base.ViewData["OdDatuma"] = odDatuma.ToShortDateString();
		base.ViewData["DoDatuma"] = doDatuma.ToShortDateString();
		base.ViewData["ObjekatId"] = ((objekatId != 0) ? _context.Objekat.FirstOrDefault((Objekat d) => (int?)d.Id == objekatId).Naziv : "Svi objekti");
		base.ViewData["VrstaDokumentaId"] = ((dokumentId != 0) ? _context.VrstaDokumenta.FirstOrDefault((VrstaDokumenta d) => (int?)d.Id == dokumentId).Naziv : "Svi dokumenti");
		base.ViewData["VrstaZalihaId"] = ((vrstaZalihaId != 0) ? _context.VrstaZaliha.FirstOrDefault((VrstaZaliha d) => (int?)d.Id == vrstaZalihaId).Naziv : "Sve vrste zaliha");
		base.ViewData["ZaliheId"] = ((zaliheId != 0) ? _context.Zalihe.FirstOrDefault((Zalihe d) => (int?)d.Id == zaliheId).Naziv : "Sve zalihe");
		IQueryable<KolicinaZaliha> queryable = from ds in _context.DokumentStavka
											   join z in _context.Zalihe on ds.ZaliheId equals z.Id
											   join d in _context.Dokument on ds.DokumentId equals d.Id
											   join vd in _context.VrstaDokumenta on d.VrstaDokumentaId equals vd.Id
											   join vz in _context.VrstaZaliha on z.VrstaZalihaId equals vz.Id
											   where (objekatId == (int?)0 || d.ObjekatId == objekatId) && (dokumentId == (int?)0 || (int?)d.VrstaDokumentaId == dokumentId) && (vrstaZalihaId == (int?)0 || (int?)vz.Id == vrstaZalihaId) && (zaliheId == (int?)0 || (int?)z.Id == zaliheId) && d.DateDoc >= odDatuma && d.DateDoc <= doDatuma
											   group ds by new { d.ObjekatId, d.VrstaDokumentaId, vd.Skracenica, z.VrstaZalihaId, ds.ZaliheId, z.JedMere } into groupStavka
											   select new KolicinaZaliha
											   {
												   IdObjekta = groupStavka.Key.ObjekatId,
												   IdDokumenta = groupStavka.Key.VrstaDokumentaId,
												   Skracenica = groupStavka.Key.Skracenica,
												   IdVrstaZaliha = groupStavka.Key.VrstaZalihaId,
												   IdZalihe = groupStavka.Key.ZaliheId,
												   JedMere = groupStavka.Key.JedMere,
												   Kolicina = groupStavka.Sum((DokumentStavka d) => d.Kolicina),
												   UkupnaKolicina = groupStavka.Sum((DokumentStavka d) => d.Kolicina)
											   };
		if (!queryable.Any())
		{
			return CustomErrorMessage("Nema podataka za prikaz", "PromeneZaliheView");
		}
		IQueryable<KolicinaZalihaView> queryable2 = from km in queryable
													join o in _context.Objekat on km.IdObjekta equals o.Id
													join z in _context.Zalihe on km.IdZalihe equals z.Id
													join vd in _context.VrstaDokumenta on km.IdDokumenta equals vd.Id
													join vz in _context.VrstaZaliha on km.IdVrstaZaliha equals vz.Id
													select new KolicinaZalihaView
													{
														IdObjekta = km.IdObjekta,
														Objekat = o.Naziv,
														Skracenica = vd.Skracenica,
														IdVrstaZaliha = km.IdVrstaZaliha,
														VrstaZaliha = vz.Naziv,
														IdZalihe = km.IdZalihe,
														Zalihe = string.Concat(string.Concat(string.Concat((z.VrstaZaliha.Naziv ?? string.Empty) + " ", z.Naziv), " "), z.Marka ?? string.Empty),
														JedMere = km.JedMere,
														Kolicina = ((km.Skracenica == "O" || km.Skracenica == "IM") ? (-1.0 * km.Kolicina) : km.Kolicina),
														UkupnaKolicina = ((km.Skracenica == "O" || km.Skracenica == "IM") ? (-1.0 * km.Kolicina) : km.Kolicina)
													};
		if (!queryable2.Any())
		{
			return CustomErrorMessage("Nema podataka za prikaz", "PromeneZaliheView");
		}
		return View("ZaliheIzvestaj", queryable2);
	}

	public int VrstaDokumenta(string skr)
	{
		return _context.VrstaDokumenta.FirstOrDefault((VrstaDokumenta d) => d.Skracenica == skr).Id;
	}

	public string Automatski(int? id)
	{
		string text = "";
		IQueryable<Dokument> queryable = from d in _context.Dokument
										 join vd in _context.VrstaDokumenta on d.VrstaDokumentaId equals vd.Id
										 join dd in _context.Dokument on d.Id equals dd.VezniDokumentId
										 where (int?)d.Id == id
										 select dd;
		if (!queryable.Any())
		{
			return string.Empty;
		}
		foreach (Dokument item in queryable)
		{
			text = text + " dokument " + item.VrstaDokumenta.Naziv + " broj " + item.Broj + ",";
		}
		if (text != string.Empty)
		{
			text += "automatski je generisan pre zatvaranja otpremnice zatvorite ova dokumenta prvo Izdavanje materijala, zatim prijem proizvoda";
		}
		return text;
	}
}