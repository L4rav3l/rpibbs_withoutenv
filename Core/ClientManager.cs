using RPIBBS.Authentication;
using Npgsql;

namespace RPIBBS.Core;

public class ClientManager
{
    private readonly Commands _commands = new Commands();
    private readonly AuthManager _authManager = new AuthManager();
    private readonly Menu _menu = new Menu();
    private readonly Postgresql _postgresql = new Postgresql();

    public async void HandleClient(TcpClient client)
    {
        try
        {
            using(client)
            using(NetworkStream stream = client.GetStream())
            {
                _commands.WriteCopyright(stream);

                Thread.Sleep(500);

                int id = await _authManager.Run(stream, _commands);
                string username = "";

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                    await using(var getUsername = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", conn))
                    {
                        getUsername.Parameters.AddWithValue("id", id);

                        await using(var reader = await getUsername.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                username = reader.GetString(reader.GetOrdinal("username"));
                            }
                        }
                    }
                }

                await _menu.Run(username, _commands, stream);
            }
        }
        catch(Exception ex)
        {

        }
    }
}