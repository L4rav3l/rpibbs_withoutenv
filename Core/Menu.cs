namespace RPIBBS.Core;

public class Menu
{
    public async Task Run(string _username, Commands _commands, NetworkStream stream)
    {
        _commands.ClearConsole(stream);
     
        var terminal = _commands.GetScreenSize(stream);
     
        _commands.SetRow(stream, terminal.height / 4);
        _commands.Write(stream, "cica");

        while(true)
        {

        }
    }
}