using Npgsql;
using RPIBBS.Core;

namespace RPIBBS.Authentication;

public class AuthManager
{
    private Commands _commands;
    private Postgresql _conn;
    private Login _login;
    private Registration _registration;
    private Encrypting _encrypting;

    public async Task<bool> Run(NetworkStream stream, Commands commands)
    {
        _commands = commands;
        _conn = new Postgresql();
        _login = new Login();
        _registration = new Registration();
        _encrypting = new Encrypting();

        _commands.Write(stream, "Username: ");
        string username = _commands.ReadLine(stream);

        await using(var conn = await _conn.GetOpenConnectionAsync())
        {
            await using(var usernameTaken = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", conn))
            {
                usernameTaken.Parameters.AddWithValue("username", username);

                await using(var reader = await usernameTaken.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {

                    } else {
                        
                    }
                }
            }
        }
        
        _commands.DisableTelnetEcho(stream);

        _commands.Write(stream, "Password: ");
        string password = _commands.ReadPassword(stream);

        return true;
    }
}