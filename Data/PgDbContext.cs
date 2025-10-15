using Microsoft.EntityFrameworkCore;
using SanVicenteHospital.Models;
namespace SanVicenteHospital.Data;

public class PgDbContext: DbContext
{
    public PgDbContext(DbContextOptions<PgDbContext> options) : base(options)
    {
    }
    
    public DbSet<Patient> patients { get; set; }
    public DbSet<Doctor> doctors { get; set; }
    public DbSet<Appointment> appointments { get; set; }
    public DbSet<EmailHistory> emailhistory { get; set; }
}