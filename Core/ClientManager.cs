namespace RPIBBS.Core;

public class ClientManager
{
    private readonly Commands _commands = new Commands();

    public void HandleClient(TcpClient client)
    {
        using(client)
        using(NetworkStream stream = client.GetStream())
        {
            _commands.WriteCopyright(stream);
            _commands.DisableTelnetEcho(stream);

            string input = _commands.ReadPassword(stream);
            Console.WriteLine(input);
        }
    }
}