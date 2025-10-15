namespace SanVicenteHospital.Models;

public class Patient
{
    public int id { get; set; }
    public string fullname { get; set; }
    public string document { get; set; }
    public int age { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
}