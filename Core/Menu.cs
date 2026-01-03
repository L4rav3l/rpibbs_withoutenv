namespace RPIBBS.Core;

public class Menu
{
    public async Task Run(string _username, Commands _commands, NetworkStream stream)
    {
        _commands.ClearConsole(stream);
     
        var terminal = _commands.GetScreenSize(stream);
        int options = 1;
     
        _commands.SetRow(stream, terminal.height / 4);
        _commands.SetColumn(stream, terminal.width / 2 - 29);
        _commands.Write(stream, "____________________.___  ____________________  _________\n");

        _commands.SetColumn(stream, terminal.width / 2 - 29);
        _commands.Write(stream, "\\______   \\______   \\   | \\______   \\______   \\/   _____/\n");

        _commands.SetColumn(stream, terminal.width / 2 - 29);
        _commands.Write(stream, " |       _/|     ___/   |  |    |  _/|    |  _/\\_____  \\\n");

        _commands.SetColumn(stream, terminal.width / 2 - 29);
        _commands.Write(stream, " |    |   \\|    |   |   |  |    |   \\|    |   \\/        \\\n");

        _commands.SetColumn(stream, terminal.width / 2 - 29);
        _commands.Write(stream, " |____|_  /|____|   |___|  |______  /|______  /_______  /\n");

        _commands.SetColumn(stream, terminal.width / 2 - 29);
        _commands.Write(stream, "        \\/                        \\/        \\/        \\/ ");

        _commands.DisableTelnetEcho(stream);

        while(true)
        {
            int b1 = stream.ReadByte();
            if(b1 == 0x1B)
            {
                int b2 = stream.ReadByte();

                if(b2 == '[')
                {
                    int b3 = stream.ReadByte();

                    if(b3 == 'A')
                    {
                        if(1 <= options-1)
                        {
                            options--;
                        }

                    } else if(b3 == 'B') {
                        
                        if(5 >= options+1)
                        {
                            options++;
                        }
                    }
                }
            }

            _commands.SetRow(stream, terminal.height / 2);
            _commands.SetColumn(stream, terminal.width / 2 - 6);

            _commands.Write(stream, "  Messages  \n");
            
            _commands.SetColumn(stream, terminal.width / 2 - 5);
            _commands.Write(stream, "  Boards  \n");

            _commands.SetColumn(stream, terminal.width / 2 - 4);
            _commands.Write(stream, "  Game  \n");

            _commands.SetColumn(stream, terminal.width / 2 - 6);
            _commands.Write(stream, "  Settings  \n");

            _commands.SetColumn(stream, terminal.width / 2 - 4);
            _commands.Write(stream, "  Exit  ");

            if(options == 1)
            {
                _commands.SetRow(stream, terminal.height / 2);
                _commands.SetColumn(stream, terminal.width / 2 - 6);
                _commands.Write(stream, "> Messages <\n");

            } else if(options == 2)
            {
                _commands.SetRow(stream, terminal.height / 2 +1);
                _commands.SetColumn(stream, terminal.width / 2 - 5);
                _commands.Write(stream, "> Boards <\n");

            } else if(options == 3)
            {
                _commands.SetRow(stream, terminal.height / 2 + 2);
                _commands.SetColumn(stream, terminal.width / 2 - 4);
                _commands.Write(stream, "> Game <\n");
            } else if(options == 4)
            {
                _commands.SetRow(stream, terminal.height / 2 + 3);
                _commands.SetColumn(stream, terminal.width / 2 - 6);
                _commands.Write(stream, "> Settings <\n");
            } else if(options == 5)
            {
                _commands.SetRow(stream, terminal.height / 2 + 4);
                _commands.SetColumn(stream, terminal.width / 2 - 4);
                _commands.Write(stream, "> Exit <");
            }
        }
    }
}