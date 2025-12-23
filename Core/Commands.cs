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

        WriteLine(stream, "[== JUST FUN AND EDUCATION PURPOSES ==]\r\n");
    }

    public void EnableTelnetEcho(NetworkStream stream)
    {
        byte[] cmds =
        {
            0xFF, 0xFC, 0x03,
            0xFF, 0xFE, 0x03,
            0xFF, 0xFC, 0x01
        };

        stream.Write(cmds, 0, cmds.Length);
    }

    public void DisableTelnetEcho(NetworkStream stream)
    {
        byte[] cmds =
        {
            0xFF, 0xFB, 0x03,
            0xFF, 0xFD, 0x03,
            0xFF, 0xFB, 0x01 
        };

        stream.Write(cmds, 0, cmds.Length);
    }

    public string ReadLine(NetworkStream stream)
    {
        StringBuilder sb = new StringBuilder();

        while(true)
        {
            int b = stream.ReadByte();

            if(b == 255)
            {
                stream.ReadByte();
                stream.ReadByte();

                continue;
            }

            if (b == -1) return null;
            if (b == '\n') break;
            if (b != '\r') sb.Append((char)b);
        }

        return sb.ToString();
    }

    public string ReadMail(NetworkStream stream, int StartLine)
    {
        var lines = new List<StringBuilder> { new StringBuilder() };
        int line = 0;
        int col = 0;

        for(int i = 0; i < StartLine; i++)
        {
            lines.Insert(++line, new StringBuilder());
        }

        while (true)
        {
            int b = stream.ReadByte();
            if (b == -1) return null;

            if (b == 255)
            {
                stream.ReadByte();
                stream.ReadByte();
                continue;
            }

            if(b == 127 && col > 0)
            {
                lines[line].Remove(col -1, 1);
                col--;
                
                if(line-- > StartLine)
                {
                    Write(stream, "\b \b");
                }
                
                continue;
            }

            if (b == 27)
            {
                int b2 = stream.ReadByte();
                if (b2 == 91)
                {
                    int arrow = stream.ReadByte();

                    if (arrow == 65 && line > StartLine) line--;
                    if (arrow == 66 && line < lines.Count - 1) line++;
                    if (arrow == 68 && col > 0) col--;
                    if (arrow == 67 && col < lines[line].Length) col++;

                    col = Math.Min(col, lines[line].Length);
                    Write(stream, $"\x1b[{line + 1};{col + 1}H");
                    Write(stream, "\b");
                    continue;
                }
            }

            if (b == 13)
            {
                lines.Insert(++line, new StringBuilder());
                col = 0;
                Write(stream, "\r\n");
                continue;
            }

            if (b == 4)
                break;

            if (b != '\r')
            {
                lines[line].Insert(col, (char)b);
                Write(stream, ((char)b).ToString());
                col++;
            }
        }

        return string.Join("\r\n", lines.Select(l => l.ToString()));
    }

    public string ReadPassword(NetworkStream stream)
    {
        StringBuilder password = new StringBuilder();

        while(true)
        {
            int b = stream.ReadByte();

            if(b == -1) return null;

            if(b == 255)
            {
                stream.ReadByte();
                stream.ReadByte();

                continue;
            }
            
            if(b == '\r')
            {
                stream.ReadByte();
                Write(stream, "\n");

                break;
            }

            if(b == 8 || b == 127)
            {
                if(password.Length > 0)
                {
                    password.Length--;
                    Write(stream, "\b \b");
                }

                continue;
            }

            password.Append((char)b);
            Write(stream, "#");
        }

        return password.ToString();
    }
}