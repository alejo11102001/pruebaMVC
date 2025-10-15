using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SanVicenteHospital.Controllers;
using SanVicenteHospital.Data;
using SanVicenteHospital.Models;

public class EmailHistoryController: Controller
{
    private readonly PgDbContext _context;

    public EmailHistoryController(PgDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var histories = _context.emailhistory
            .Include(e => e.appointment)
            .OrderByDescending(e => e.sentat)
            .ToList();

        return View(histories);
    }
}