using System;
using System.CommandLine;
using Spectre.Console;

namespace certificate_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rootCommand = new RootCommand();

            rootCommand.AddChild<MatchKeyAndCertificate>("match", AnsiConsole.Console);
            rootCommand.AddChild<ChainVerificationCommand>("verify", AnsiConsole.Console);

            rootCommand.Invoke(args);
        }
    }
}
