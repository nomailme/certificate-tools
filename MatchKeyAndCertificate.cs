using System;
using System.CommandLine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    [Description("Verify that key and certificate matches")]
    public class MatchKeyAndCertificate
    {
        [Description("Certificate file")]
        [Name("certificate")]
        [Required]
        public FileInfo Certificate { get; set; }

        [Description("Key file")]
        [Name("key")]
        [Required]
        public FileInfo Key { get; set; }

        public void Do(RootCommand rootCommand)
        {

        }

        public bool AreValid()
        {
            X509Certificate2 certificate = PemTools.LoadFromPemFile(Certificate).First();
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(Key.FullName));
            certificate.CopyWithPrivateKey(rsa);
            return true;
        }
    }
}
