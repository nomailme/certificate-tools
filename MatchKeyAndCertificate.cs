using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

namespace certificate_tools
{
    [Description("Verify that key and certificate matches")]
    public class MatchKeyAndCertificate : IConsoleCommand
    {
        [Description("Certificate file")]
        [Name("certificate")]
        [Required]
        public FileInfo Certificate { get; set; }

        [Description("Key file")]
        [Name("key")]
        [Required]
        public FileInfo Key { get; set; }

        public bool AreValid()
        {
            X509Certificate2 certificate = PemTools.LoadCeretificateFromPemFile(Certificate).First();
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(Key.FullName));
            certificate.CopyWithPrivateKey(rsa);
            return true;
        }

        public void Do(IAnsiConsole console)
        {
            var result = AreValid();
            var resultMessage = result switch
            {
                true => "Result: [green]ok[/]",
                false => $"Result: [red]fail[/]"
            };

            console.MarkupLine(resultMessage);
        }
    }
}
