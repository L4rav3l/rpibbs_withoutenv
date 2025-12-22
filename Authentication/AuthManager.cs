using Npgsql;
using RPIBBS.Core;

namespace RPIBBS.Authentication;

public class AuthManager
{

    private Commands _commands;

    public bool Run(NetworkStream stream, Commands commands)
    {
        _commands = commands;

        _commands.Write(stream, "Username: ");
        string username = _commands.ReadLine(stream);
        
        _commands.DisableTelnetEcho(stream);

        _commands.Write(stream, "Password: ");
        string password = _commands.ReadPassword(stream);

        return true;
    }
}