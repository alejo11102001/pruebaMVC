namespace SanVicenteHospital.Services;
using MailKit.Net.Smtp;
using MimeKit;
using SanVicenteHospital.Models;
using SanVicenteHospital.Data;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly PgDbContext _context;

    public EmailService(IConfiguration config, PgDbContext context)
    {
        _config = config;
        _context = context;
    }

    public async Task SendAppointmentEmail(Appointment appointment)
    {
        // Obtener datos del paciente y doctor
        var patient = _context.patients.FirstOrDefault(p => p.id == appointment.patientid);
        var doctor = _context.doctors.FirstOrDefault(d => d.id == appointment.doctorid);

        if (patient == null || string.IsNullOrEmpty(patient.email))
            return;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("San Vicente Hospital", _config["EmailSettings:SenderEmail"]));
        message.To.Add(new MailboxAddress(patient.fullname, patient.email));
        message.Subject = "Your Appointment Confirmation";

        message.Body = new TextPart("plain")
        {
            Text = $"Dear {patient.fullname},\n\n" +
                   $"Your appointment with Dr. {doctor.fullname} is scheduled for " +
                   $"{appointment.appointmentdate.ToLocalTime():f}.\n\n" +
                   "Thank you for choosing San Vicente Hospital."
        };

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]), false);
                await client.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                // Registrar en EmailHistory
                var history = new EmailHistory
                {
                    appointmentid = appointment.id,
                    status = "Sent",
                    message = $"Email sent successfully to {patient.email}"
                };
                _context.Add(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Si hay error, registrar tambiÃ©n
                var history = new EmailHistory
                {
                    appointmentid = appointment.id,
                    status = "Error",
                    message = ex.Message
                };
                _context.Add(history);
                _context.SaveChanges();
            }
        }
    }
}