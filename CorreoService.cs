using System.Net.Mail;
using System.Net;

public class CorreoService
{
    private readonly IConfiguration _config;

    public CorreoService(IConfiguration config)
    {
        _config = config;
    }

    public void EnviarPin(string destino, string pin)
    {
        var remitente = _config["Correo:Remitente"];
        var clave = _config["Correo:Clave"];
        var smtpHost = _config["Correo:Smtp"];
        var puerto = int.Parse(_config["Correo:Puerto"]);

        Debug.WriteLine($"Hola {remitente}");

        var mensaje = new MailMessage(remitente, destino, "Recuperación de PIN",
            $"Hola,\n\nSu nuevo PIN es: {pin}");

        using var smtp = new SmtpClient(smtpHost, puerto)
        {
            Credentials = new NetworkCredential(remitente, clave),
            EnableSsl = true
        };

        smtp.Send(mensaje);
    }
}

