using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Xml.Linq;

namespace NetCoreProject.Domain.Util
{
    public class CertSignatureUtil
    {
        public static byte[] GenerateSignature(byte[] originalDataBinary, string publicKeyBase64String)
        {
            throw new NotImplementedException("the normal way is to signature with the private key");
            //var x509Certificate = CertUtil.ToPortableX509Certificate(publicKeyBase64String);
            //return GenerateSignature(originalDataBinary, x509Certificate.GetPublicKey());
        }
        public static byte[] GenerateSignature(byte[] originalDataBinary, AsymmetricKeyParameter privateKeyParameter)
        {
            var signer = SignerUtilities.GetSigner("SHA256WithRSAEncryption");
            // 引數為true加簽，引數為false驗籤
            signer.Init(true, privateKeyParameter);
            signer.BlockUpdate(originalDataBinary, 0, originalDataBinary.Length);
            return signer.GenerateSignature();
        }
        public static bool VerifySignature(XDocument signatureXDocument, byte[] originalDataBinary)
        {
            // 取得簽體
            var signatureBase64String = signatureXDocument.Descendants("Signature")
                .Descendants("SignatureValue").FirstOrDefault().Value;
            // 取得公鑰
            var publicKeyBase64String = signatureXDocument.Descendants("Signature")
                .Descendants("X509Certificate").FirstOrDefault().Value;
            return VerifySignature(signatureBase64String, publicKeyBase64String, originalDataBinary);
        }
        public static bool VerifySignature(string signatureBase64String, string publicKeyBase64String, byte[] originalDataBinary)
        {
            var signatureBinary = Convert.FromBase64String(signatureBase64String);
            return VerifySignature(signatureBinary, publicKeyBase64String, originalDataBinary);
        }
        public static bool VerifySignature(byte[] signatureBinary, string publicKeyBase64String, byte[] originalDataBinary)
        {
            var x509Certificate = CertUtil.ToPortableX509Certificate(publicKeyBase64String);
            return VerifySignature(signatureBinary, x509Certificate.GetPublicKey(), originalDataBinary);
        }
        public static bool VerifySignature(byte[] signatureBinary, AsymmetricKeyParameter asymmetricKeyParameter, byte[] originalDataBinary)
        {
            var signer = SignerUtilities.GetSigner("SHA256WithRSAEncryption");
            // 引數為true加簽，引數為false驗籤
            signer.Init(false, asymmetricKeyParameter);
            signer.BlockUpdate(originalDataBinary, 0, originalDataBinary.Length);
            return signer.VerifySignature(signatureBinary);
        }
    }
}
