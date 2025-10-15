using Microsoft.AspNetCore.Mvc;
using SanVicenteHospital.Models;
using SanVicenteHospital.Data;
namespace SanVicenteHospital.Controllers;

public class DoctorsController: Controller
{
    private readonly PgDbContext _context;
    public DoctorsController(PgDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string specialty)
    {
        var specialties = _context.doctors
            .Select(d => d.specialty)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        ViewBag.Specialties = specialties;
        
        var doctors = string.IsNullOrEmpty(specialty)
            ? _context.doctors.ToList()
            : _context.doctors.Where(d => d.specialty == specialty).ToList();

        ViewBag.SelectedSpecialty = specialty;

        return View(doctors);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Store([Bind("fullname, document, specialty, phone, email")] Doctor doctor)
    {
        if (ModelState.IsValid)
        {
            var existsDocument = _context.doctors.Any(d => d.document == doctor.document );
            if (existsDocument)
            {
                ModelState.AddModelError("document", "Document already exists");
                return View("Create", doctor);
            }
            
            var existsCombination = _context.doctors.Any(d =>
                d.fullname.ToLower() == doctor.fullname.ToLower() &&
                d.specialty.ToLower() == doctor.specialty.ToLower()
            );
            if (existsCombination)
            {
                ModelState.AddModelError("FullName", "A doctor with the same name and specialty already exists.");
                return View("Create", doctor);
            }
            
            _context.Add(doctor);
            _context.SaveChanges();
            TempData["message"] = "Doctor created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View("Create", doctor);
    }

    public IActionResult Destroy(int id)
    {
        var doctor = _context.doctors.Find(id);
        if (doctor == null)
        {
            return NotFound();
        }

        _context.doctors.Remove(doctor);
        _context.SaveChanges();
        TempData["message"] = "Doctor removed successfully";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var doctor = _context.doctors.Find(id);
        if (doctor == null)
        {
            return NotFound();
        }
        return View(doctor);
    }

    [HttpPost]
    public IActionResult Update(int id, Doctor updateDoctor)
    {
        var existingDoctor = _context.doctors.Find(id);
        if (existingDoctor == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            existingDoctor.fullname = updateDoctor.fullname;
            existingDoctor.document = updateDoctor.document;
            existingDoctor.specialty = updateDoctor.specialty;
            existingDoctor.phone = updateDoctor.phone;
            existingDoctor.email = updateDoctor.email;
            
            _context.SaveChanges();
            TempData["message"] = "Doctor edited successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(updateDoctor);
    }
}