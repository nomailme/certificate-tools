using System;
using System.CommandLine;

namespace certificate_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var matchCommand = new Command("match").ConfigureFromClass<MatchKeyAndCertificate>(command =>
                Console.WriteLine("Match: {0}", command.AreValid().ToString()));
            var verifyCommand = new Command("verify").ConfigureFromClass<VerifyCertificate>(command =>
                Console.WriteLine("Verification successful: {0}", command.IsValid().ToString()));

            rootCommand.AddCommand(matchCommand);
            rootCommand.AddCommand(verifyCommand);
            rootCommand.Invoke(args);
        }
    }
}
