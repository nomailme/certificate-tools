using System;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    public static class X509CertificateCollectionExtensions
    {
        public static X509Certificate2 First(this X509Certificate2Collection collection)
        {
            if (collection.Count == 0)
            {
                return default;
            }

            return collection[0];
        }

        public static X509CertificateCollection Skip(this X509Certificate2Collection collection, int skip)
        {
            var result = new X509Certificate2Collection();
            for (var i = skip; i < collection.Count - 1; i++)
            {
                result.Add(collection[i]);
            }

            return result;
        }
    }
}
