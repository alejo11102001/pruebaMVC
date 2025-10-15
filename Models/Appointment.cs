namespace SanVicenteHospital.Models;

public class Appointment
{
    public int id { get; set; }
    public int patientid { get; set; }
    public int doctorid { get; set; }
    public DateTime appointmentdate { get; set; }
    public string status { get; set; } = "Pending";
    
    public Patient? patient { get; set; }
    public Doctor? doctor { get; set; }
    public EmailHistory emailhistory { get; set; }
}