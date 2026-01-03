namespace RPIBBS.Core;

public class Menu
{
    public async Task Run(string _username, Commands _commands, NetworkStream stream)
    {
        var terminalHeight = _commands.GetScreenSize(stream);
        Console.Write($"{terminalHeight.width}, {terminalHeight.height}");
    }
}