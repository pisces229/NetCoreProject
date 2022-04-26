using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using System;
using System.IO;
using System.Linq;

namespace NetCoreProject.Domain.Util
{
    public class CertUtil
    {
        public static bool CheckCert(X509Certificate x509Certificate)
        {
            var resule = true;
            var over = DateTime.Compare(DateTime.Now, Convert.ToDateTime(x509Certificate.NotAfter));
            if (over > 0)
            {
                resule = false;
            }
            return resule;
        }
        public static AsymmetricKeyParameter GetPrivateKey(string pfxPath, string pinCode)
        {
            var pkcs12Store = new Pkcs12StoreBuilder().Build();
            using (var fileStream = new FileStream(pfxPath, FileMode.Open, FileAccess.Read))
            {
                pkcs12Store.Load(fileStream, pinCode.ToArray());
            }
            var alia = string.Empty;
            foreach (string Aliase in pkcs12Store.Aliases)
            {
                if (pkcs12Store.IsKeyEntry(Aliase))
                {
                    alia = Aliase;
                }
            }
            return pkcs12Store.GetKey(alia).Key;
        }
        public static X509Certificate GetPublicKey(string pfxPath, string pinCode)
        {
            var pkcs12Store = new Pkcs12StoreBuilder().Build();
            using (var fileStream = new FileStream(pfxPath, FileMode.Open, FileAccess.Read))
            {
                pkcs12Store.Load(fileStream, pinCode.ToArray());
            }
            var alia = string.Empty;
            foreach (string Aliase in pkcs12Store.Aliases)
            {
                if (pkcs12Store.IsKeyEntry(Aliase))
                {
                    alia = Aliase;
                }
            }
            var result = pkcs12Store.GetCertificate(alia).Certificate;
            result.CheckValidity();
            result.Verify(result.GetPublicKey());
            return result;
        }
        private static X509Certificate ToPortableX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate2)
        {
            return new X509CertificateParser().ReadCertificate(x509Certificate2.GetRawCertData());
        }
        public static X509Certificate ToPortableX509Certificate(string publicKeyBase64String)
        {
            return ToPortableX509Certificate(Convert.FromBase64String(publicKeyBase64String));
        }
        public static X509Certificate ToPortableX509Certificate(byte[] byteArray)
        {
            return new X509CertificateParser().ReadCertificate(byteArray);
        }
        public static System.Security.Cryptography.X509Certificates.X509Certificate ToX509Certificate(X509Certificate x509Certificate)
        {
            return ToX509Certificate(x509Certificate.GetEncoded());
        }
        public static System.Security.Cryptography.X509Certificates.X509Certificate ToX509Certificate(string publicKeyBase64String)
        {
            return ToX509Certificate(Convert.FromBase64String(publicKeyBase64String));
        }
        public static System.Security.Cryptography.X509Certificates.X509Certificate ToX509Certificate(byte[] byteArray)
        {
            var x509Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate(byteArray);
            return x509Certificate;
        }
        public static System.Security.Cryptography.X509Certificates.X509Certificate2 ToX509Certificate2(X509Certificate x509Certificate)
        {
            return ToX509Certificate2(x509Certificate.GetEncoded());
        }
        public static System.Security.Cryptography.X509Certificates.X509Certificate2 ToX509Certificate2(string publicKeyBase64String)
        {
            return ToX509Certificate2(Convert.FromBase64String(publicKeyBase64String));
        }
        public static System.Security.Cryptography.X509Certificates.X509Certificate2 ToX509Certificate2(byte[] byteArray)
        {
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(byteArray);
        }
        public static void GenerateSelfCert(string filepath, string filename, string pincode, int encryptionKeySize = 2048)
        {
            // 產生憑證密碼
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), encryptionKeySize));
            var asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();
            // 發行者
            var issuerX509Name = new X509Name("OU=TPK,O=ACER,C=TW");
            // 憑證持有者
            var subjectX509Name = new X509Name("C=TW, CN=TEST");
            var nowDateTime = DateTime.Now;
            // 憑證資訊
            var asn1SignatureFactory = new Asn1SignatureFactory("SHA256WithRSAEncryption", asymmetricCipherKeyPair.Private);
            var x509V3CertificateGenerator = new X509V3CertificateGenerator();
            x509V3CertificateGenerator.SetSerialNumber(new BigInteger(120, new Random()));
            x509V3CertificateGenerator.SetIssuerDN(issuerX509Name);
            x509V3CertificateGenerator.SetNotBefore(nowDateTime.AddHours(-8));
            x509V3CertificateGenerator.SetNotAfter(DateTime.Parse("9999-12-31").AddHours(23).AddMinutes(59).AddSeconds(59).AddHours(-8));
            x509V3CertificateGenerator.SetSubjectDN(subjectX509Name);
            x509V3CertificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);
            x509V3CertificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(asymmetricCipherKeyPair.Public));
            x509V3CertificateGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment));
            var x509Certificate = x509V3CertificateGenerator.Generate(asn1SignatureFactory);
            x509Certificate.Verify(asymmetricCipherKeyPair.Public);
            // 建立憑證
            var pkcs12Store = new Pkcs12StoreBuilder().Build();
            var certEntry = new X509CertificateEntry(x509Certificate);
            pkcs12Store.SetCertificateEntry(x509Certificate.SubjectDN.ToString(), certEntry);
            var asymmetricKeyEntry = new AsymmetricKeyEntry(asymmetricCipherKeyPair.Private);
            pkcs12Store.SetKeyEntry(x509Certificate.SubjectDN.ToString() + " KEY", asymmetricKeyEntry, new X509CertificateEntry[] { certEntry });
            using (var fileStream = File.Create(Path.Combine(filepath, filename + ".pfx")))
            {
                pkcs12Store.Save(fileStream, pincode.ToCharArray(), new SecureRandom());
            }
            using (var memoryStream = new MemoryStream(10000))
            {
                pkcs12Store.Save(memoryStream, pincode.ToCharArray(), new SecureRandom());
                var x509Certificate2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(memoryStream.GetBuffer(), pincode, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);
                var publicKeyByteArray = x509Certificate2.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert);
                using (var fileStream = new FileStream(Path.Combine(filepath, filename + ".cer"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.Write(publicKeyByteArray, 0, publicKeyByteArray.Length);
                }
            }
        }
    }
}
