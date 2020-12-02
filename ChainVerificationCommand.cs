using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Spectre.Console;

namespace certificate_tools
{
    [Description("Verify certificate chain")]
    public class ChainVerificationCommand : IConsoleCommand
    {
        private readonly string[] certificateExtensions = { ".crt", ".pem" };

        [Description("Directory that contains root certificates")]
        [Name("ca")]
        public DirectoryInfo Ca { get; set; }

        [Description("Certificate file")]
        [Name("certificate")]
        [Required]
        public FileInfo Certificate { get; set; }

        [Description("Use system certificate store")]
        [Name("nostore")]
        public bool DoNotUseSystemCertificateStore { get; set; }

        public void Do(IAnsiConsole console)
        {
            var result = IsValid();
            var resultMessage = result.IsValid switch
            {
                true => "Result: [green]ok[/]",
                false => $"Result: [red]fail[/]{Environment.NewLine}Error: {GetError(result)}"
            };

            console.MarkupLine(resultMessage);
        }

        private string GetError((bool isValid, List<X509ChainStatus> chainStatuses) result)
        {
            StringBuilder builder = new StringBuilder();
            result.chainStatuses.ForEach(x => builder.AppendLine($"{x.StatusInformation}"));
            return builder.ToString();
        }

        private (bool IsValid, List<X509ChainStatus> ChainStatuses) IsValid()
        {
            try
            {
                var certificateCollection = PemTools.LoadCeretificateFromPemFile(Certificate);
                var certificate = certificateCollection.First();
                if (certificate == null)
                {
                    throw new ArgumentException("Unable to load certificate");
                }

                var trustMode = DoNotUseSystemCertificateStore
                    ? X509ChainTrustMode.CustomRootTrust
                    : X509ChainTrustMode.System;

                X509Chain chain = new X509Chain
                {
                    ChainPolicy =
                    {
                        TrustMode = trustMode,
                        RevocationMode = X509RevocationMode.NoCheck,
                        VerificationFlags = X509VerificationFlags.NoFlag
                    }
                };

                var intermediateCertificates = certificateCollection.Skip(1);

                Ca?.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly)
                    .Where(x => certificateExtensions.Contains(x.Extension))
                    .Select(PemTools.LoadCeretificateFromPemFile)
                    .SelectMany(x => x.OfType<X509Certificate2>())
                    .Where(x => string.Equals(x.Issuer, x.Subject))
                    .GroupBy(x => x.Thumbprint)
                    .Select(x => x.First())
                    .ToList()
                    .ForEach(x => chain.ChainPolicy.CustomTrustStore.Add(x));

                chain.ChainPolicy.ExtraStore.AddRange(intermediateCertificates);
                var isValid = chain.Build(certificate);
                return (isValid, chain.ChainStatus.ToList());
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Unable to load certificate from file");
            }
        }
    }
}
