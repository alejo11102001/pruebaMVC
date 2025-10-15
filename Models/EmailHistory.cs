namespace SanVicenteHospital.Models;

public class EmailHistory
{
    public int id { get; set; }
    public int? appointmentid { get; set; }
    public DateTime sentat { get; set; } = DateTime.UtcNow;
    public string status { get; set; }
    public string message { get; set; }
    
    public Appointment appointment { get; set; }
}