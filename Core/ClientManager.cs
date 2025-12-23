using RPIBBS.Authentication;

namespace RPIBBS.Core;

public class ClientManager
{
    private readonly Commands _commands = new Commands();
    private readonly AuthManager _authManager = new AuthManager();

    public async void HandleClient(TcpClient client)
    {
        using(client)
        using(NetworkStream stream = client.GetStream())
        {
            _commands.WriteCopyright(stream);

            Thread.Sleep(500);

            await _authManager.Run(stream, _commands);
        }
    }
}