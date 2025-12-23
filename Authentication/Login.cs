using Npgsql;
using RPIBBS.Core;

namespace RPIBBS.Authentication;

public class Login
{

    private Commands _commands;
    private Encrypting _encrypt = new Encrypting();

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

                    _commands.EnableTelnetEcho(stream);

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