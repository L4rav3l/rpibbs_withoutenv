using Npgsql;
using RPIBBS.Core;

namespace RPIBBS.Authentication;

public class Registration
{
    private Commands _commands;
    private Encrypting _encrypt = new Encrypting();

    public async Task<int> Run(string username, NetworkStream stream, Commands commands, Postgresql _connection)
    {
        _commands = commands;
        string password = "";
        bool stop = false;
        int? id = null;

        for(int i = 0; i < 5; i++)
        {
            _commands.Write(stream, "\rPassword: ");
            _commands.DisableTelnetEcho(stream);

            password = _commands.ReadPassword(stream);

            _commands.Write(stream, "\rPassword again: ");

            string password_again = _commands.ReadPassword(stream);

            if(_encrypt.Sha512Generate(password) == _encrypt.Sha512Generate(password_again))
            {
                break;
            } else {
                _commands.Write(stream, "\r\nThe passwords don't match.\n\r");
            }

            if(i == 5)
            {
                stop = true;
            }
        }

        if(stop)
        {
            return 0;
        } else {
            _commands.EnableTelnetEcho(stream);
            _commands.Write(stream, "\rYour country: ");
            string country = _commands.ReadLine(stream);
            
            _commands.Write(stream, "\rYour town: ");
            string town = _commands.ReadLine(stream);

            await using(var conn = await _connection.GetOpenConnectionAsync())
            {
                await using(var transaction = await conn.BeginTransactionAsync())
                {
                    try{
                        await using(var insertUser = new NpgsqlCommand("INSERT INTO users (username, password, country, town) VALUES (@username, @password, @country, @town) RETURNING id", conn, transaction))
                        {
                            insertUser.Parameters.AddWithValue("username", username);
                            insertUser.Parameters.AddWithValue("password", _encrypt.Sha512Generate(password));
                            insertUser.Parameters.AddWithValue("country", country);
                            insertUser.Parameters.AddWithValue("town", town);

                            await using(var reader = await insertUser.ExecuteReaderAsync())
                            {
                                if(await reader.ReadAsync())
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id"));
                                }
                            }
                        }

                        await transaction.CommitAsync();
                    }

                    catch(Exception ex)
                    {
                        await transaction.RollbackAsync();
                    }
                }

            }
        }

        return id.Value;
    }
}