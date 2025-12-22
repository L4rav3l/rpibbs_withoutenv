using System.Text;
using System.Runtime.InteropServices;

namespace RPIBBS.Core;

public class Commands
{
    public void WriteLine(NetworkStream stream, string text)
    {
        byte[] data = Encoding.ASCII.GetBytes(text + "\r\n");
        stream.Write(data, 0, data.Length);
    }

    public void Write(NetworkStream stream, string text)
    {
        byte[] data = Encoding.ASCII.GetBytes(text);
        stream.Write(data, 0, data.Length);
    }

    public void WriteCopyright(NetworkStream stream)
    {
        string os = RuntimeInformation.OSDescription;

        WriteLine(stream, $"- RPIBBS v1.0 OS/{os} (OpenSource)");
        WriteLine(stream, $"- COPYRIGHT (C) 2025-{DateTime.Now.Year} by L4rav3l\r\n");
        WriteLine(stream, $"Welcome to {Environment.GetEnvironmentVariable("BBS_NAME")}\r\n");
        WriteLine(stream, $"TELNET: {Environment.GetEnvironmentVariable("BBS_HOSTNAME")}:{Environment.GetEnvironmentVariable("BBS_HOSTPORT")}");

    }

    public string ReadLine(NetworkStream stream)
    {
        StringBuilder sb = new StringBuilder();

        while(true)
        {
            int b = stream.ReadByte();
            if (b == -1) return null;
            if (b == '\n') break;
            if (b != '\r') sb.Append((char)b);
        }

        return sb.ToString();
    }
}