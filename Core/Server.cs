using System.Threading;

namespace RPIBBS.Core;

public class Server
{
    private ClientManager _clientManager = new ClientManager();

    public void Main(int port)
    {
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();

        while(true)
        {
            TcpClient client = server.AcceptTcpClient();
            new Thread(() => _clientManager.HandleClient(client)).Start();
        }
    }
}