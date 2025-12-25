using Npgsql;
using System.Threading;
using RPIBBS.Core;
using MimeKit;

namespace RPIBBS.Authentication;

public class Login
{

    private Commands _commands;
    private Encrypting _encrypt = new Encrypting();
    private Mail _mail = new Mail();

    public async Task<int?> Run(string username, NetworkStream stream, Commands commands, Postgresql _connection)
    {
        _commands = commands;
        int status = 0;

        for(int i = 1; i < 6; i++)
        {
            _commands.Write(stream, "Password: ");

            _commands.DisableTelnetEcho(stream);

            string password = _commands.ReadPassword(stream);
            string encrypted_passwd = _encrypt.Sha512Generate(password);

            await using(var conn = await _connection.GetOpenConnectionAsync())
            {
                await using(var login = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", conn))
                {
                    login.Parameters.AddWithValue("username", username);
                    login.Parameters.AddWithValue("password", encrypted_passwd);

                    await using(var reader = await login.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            status = reader.GetInt32(reader.GetOrdinal("id"));
                            break;
                        } else {
                            status = 0;
                        }
                    }
                }
            }

            _commands.Write(stream, $"\rTRY #{i}");
            _commands.Write(stream, "\r\n\r\n");
        }

        _commands.EnableTelnetEcho(stream);

        if(status == 0)
        {
            while(true)
            {
                _commands.Write(stream, $"Do you want to send a mail to {Environment.GetEnvironmentVariable("BBS_ADMIN")}? Y/N ");

                string input = _commands.ReadLine(stream);

                if(input.ToLower() == "y")
                {
                    _commands.DisableTelnetEcho(stream);

                    _commands.Write(stream, "\x1B[2J\x1B[H");

                    _commands.WriteLine(stream, "\r\nSubject: Password recorvery\r\n");

                    string data = _commands.ReadMail(stream, 3);

                    using(var SmtpClient = _mail.CreateSmtpClient())
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("System", Environment.GetEnvironmentVariable("SMTP_USERNAME")));
                        message.To.Add(new MailboxAddress("Admin", Environment.GetEnvironmentVariable("BBS_ADMIN_MAIL")));

                        message.Subject = "Password recorvery";
                        message.Body = new TextPart("plain")
                        {
                            Text = data
                        };

                        SmtpClient.Send(message);
                        SmtpClient.Disconnect(true);
                    }

                    _commands.EnableTelnetEcho(stream);
                    _commands.Write(stream, "\x1B[2J\x1B[H");
                    _commands.Write(stream, "A password recovery email has been sent.");
                    _commands.Write(stream, "\x1B[2J\x1B[H");

                    Thread.Sleep(1000);

                    break;
                } else if(input.ToLower() == "n")
                {
                    _commands.Write(stream, "\r\n");
                    break;
                }
            }
        }

        return status;
    }
}