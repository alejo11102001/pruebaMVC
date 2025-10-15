using Microsoft.AspNetCore.Mvc;
using SanVicenteHospital.Data;
using SanVicenteHospital.Models;
namespace SanVicenteHospital.Controllers;

public class PatientsController: Controller
{
    private readonly PgDbContext _context;
    public PatientsController(PgDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var patients = _context.patients.ToList();
        return View(patients);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Store([Bind("fullname, document, age, phone, email")] Patient patient)
    {
        if (ModelState.IsValid)
        {
            var existsDocument = _context.patients.Any(p => p.document == patient.document );
            if (existsDocument)
            {
                ModelState.AddModelError("document", "Document already exists");
                return View("Create", patient);
            }
            
            var existsPhone = _context.patients.Any(p => p.phone == patient.phone );
            if (existsPhone)
            {
                ModelState.AddModelError("phone", "Phone already exists");
                return View("Create", patient);
            }
            
            var existsEmail = _context.patients.Any(p => p.email == patient.email );
            if (existsEmail)
            {
                ModelState.AddModelError("email", "Email already exists");
                return View("Create", patient);
            }
            
            _context.Add(patient);
            _context.SaveChanges();
            TempData["message"] = "Patient created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View("Create", patient);
    }

    public IActionResult Destroy(int id)
    {
        var patient = _context.patients.Find(id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.patients.Remove(patient);
        _context.SaveChanges();
        TempData["message"] = "Patient removed successfully";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var patient = _context.patients.Find(id);
        if (patient == null)
        {
            return NotFound();
        }
        return View(patient);
    }

    [HttpPost]
    public IActionResult Update(int id, Patient updatePatient)
    {
        var existingPatient = _context.patients.Find(id);
        if (existingPatient == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            existingPatient.fullname = updatePatient.fullname;
            existingPatient.document = updatePatient.document;
            existingPatient.age = updatePatient.age;
            existingPatient.phone = updatePatient.phone;
            existingPatient.email = updatePatient.email;
            
            _context.SaveChanges();
            TempData["message"] = "Patient edited successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(updatePatient);
    }
}