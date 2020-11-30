using System;
using System.CommandLine;

namespace certificate_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rootCommand = new RootCommand();

            rootCommand.AddChild<MatchKeyAndCertificate>("match", command =>
                Console.WriteLine("Match: {0}", command.AreValid().ToString()));

            rootCommand.AddChild<ChainVerificationCommand>("verify", command =>
                Console.WriteLine("Valid: {0}", command.IsValid().ToString()));

            rootCommand.Invoke(args);
        }
    }
}
