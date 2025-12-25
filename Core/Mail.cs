using Mailkit.Net.Smtp;
using Mailkit.Security;

namespace RPIBBS.Core;

public class Mail
{
    public SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient();

        client.Connect(Environment.GetEnvironmentVariable("SMTP_HOSTNAME"), Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_HOSTPORT")), SecureSocketOptions.SslOnConnect);
        client.Authenticate(Environment.GetEnvironmentVariable("SMTP_USERNAME"), Environment.GetEnvironmentVariable("SMTP_PASSWORD"));

        return client;
    }
}