using System;
using Spectre.Console;

namespace certificate_tools
{
    public interface IConsoleCommand
    {
        public void Do(IAnsiConsole console);
    }
}
