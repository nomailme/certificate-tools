using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    public static class PemTools
    {
        public static X509Certificate2Collection LoadCeretificateFromPemFile(FileInfo filePath)
        {
            var rawData = File.ReadAllText(filePath.FullName);
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.ImportFromPem(rawData);
            return certificateCollection;
        }

        public static X509Certificate2Collection LoadCeretificateFromPemFile(FileSystemInfo filePath)
        {
            return LoadCeretificateFromPemFile(new FileInfo(filePath.FullName));
        }
    }
}
