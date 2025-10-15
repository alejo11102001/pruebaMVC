using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanVicenteHospital.Data;
using SanVicenteHospital.Models;
using SanVicenteHospital.Services;


namespace SanVicenteHospital.Controllers;

public class AppointmentsController: Controller
{
    private readonly PgDbContext _context;
    private readonly EmailService _emailService;

    public AppointmentsController(PgDbContext context,  EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    public IActionResult Index(int? patientId, int? doctorId)
    {
        var appointments = _context.appointments
            .Include(a => a.patient)
            .Include(a => a.doctor)
            .AsQueryable();
        
        if (patientId.HasValue)
            appointments = appointments.Where(a => a.patientid == patientId);

        if (doctorId.HasValue)
            appointments = appointments.Where(a => a.doctorid == doctorId);

        ViewBag.Patients = _context.patients.ToList();
        ViewBag.Doctors = _context.doctors.ToList();

        return View(appointments.ToList());
    }
    
    public IActionResult Create()
    {
        ViewBag.Patients = _context.patients.ToList();
        ViewBag.Doctors = _context.doctors.ToList();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Store([Bind("patientid, doctorid, appointmentdate, status")]Appointment appointment)
    {
        
        appointment.appointmentdate = appointment.appointmentdate.ToUniversalTime();
        
        if (appointment.appointmentdate < DateTime.UtcNow)
        {
            TempData["error"] = "The date and time of the appointment cannot be earlier than the current date and time.";
            ViewBag.Patients = _context.patients.ToList();
            ViewBag.Doctors = _context.doctors.ToList();
            return View("Create", appointment);
        }

        
        bool conflict = _context.appointments.Any(a =>
            (a.doctorid == appointment.doctorid && a.appointmentdate == appointment.appointmentdate) ||
            (a.patientid == appointment.patientid && a.appointmentdate == appointment.appointmentdate)
        );

        if (conflict)
        {
            TempData["error"] = "The doctor or patient already has an appointment at this time.";
            ViewBag.Patients = _context.patients.ToList();
            ViewBag.Doctors = _context.doctors.ToList();
            return View("Create", appointment); // Stay in Create view
        }

        _context.appointments.Add(appointment);
        _context.SaveChanges();

        await _emailService.SendAppointmentEmail(appointment);
        
        TempData["message"] = "Appointment successfully scheduled and email sent.";
        return RedirectToAction("Index");
    }

    public IActionResult Cancel(int id)
    {
        var appointment = _context.appointments.Find(id);
        if (appointment != null)
        {
            appointment.status = "Cancelled";
            _context.SaveChanges();
            TempData["message"] = "Appointment cancelled.";
        }
        return RedirectToAction("Index");
    }

    public IActionResult Attend(int id)
    {
        var appointment = _context.appointments.Find(id);
        if (appointment != null)
        {
            appointment.status = "Attended";
            _context.SaveChanges();
            TempData["message"] = "Appointment marked as attended.";
        }
        return RedirectToAction("Index");
    }
}
