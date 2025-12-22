global using System;
global using System.Net;
global using System.Net.Sockets;

using DotNetEnv;

using RPIBBS.Core;

public class Program
{

    public static void Main()
    {
        Env.Load();
        
        Server server = new Server();
        server.Main(9999);
    }
}