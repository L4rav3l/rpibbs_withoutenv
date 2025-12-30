using Npgsql;
using RPIBBS.Core;

namespace RPIBBS.Authentication;

public class AuthManager
{
    private Commands _commands;
    private Postgresql _conn;
    private Login _login;
    private Registration _registration;

    public async Task<int> Run(NetworkStream stream, Commands commands)
    {
        _commands = commands;
        _conn = new Postgresql();
        _login = new Login();
        _registration = new Registration();

        int? id = null;
        bool userTaken = false;

        while(true)
        {
            _commands.Write(stream, "Username: ");
            string username = _commands.ReadLine(stream);

            if(username != null)
            {
                await using(var conn = await _conn.GetOpenConnectionAsync())
                {
                    await using(var usernameTaken = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", conn))
                    {
                        usernameTaken.Parameters.AddWithValue("username", username);

                        await using(var reader = await usernameTaken.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                userTaken = true;
                            } else {
                                userTaken = false;
                            }
                        }
                    }
                }
            } else {

            }

            if(userTaken)
            {
                id = await _login.Run(username, stream, _commands, _conn);

                if(id != null)
                {
                    break;
                } else {

                }

            } else {
                while(true)
                {
                    _commands.Write(stream, $"Do you want to register {username}? Y/N ");

                    string respone = _commands.ReadLine(stream);

                    if(respone.ToLower() == "y")
                    {
                        id = await _registration.Run(username, stream, _commands, _conn);
                        break;
                    }

                    if(respone.ToLower() == "n")
                    {
                        break;
                    }
                }

                if(id != null)
                {
                    break;
                } else {

                }
            }
        }

        return id.Value;
    }
}