using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace NetCoreProject.Domain.Util
{
    public class CertDecryptUtil
    {
        public static byte[] PublicKeyDecrypt(byte[] dataBinary, AsymmetricKeyParameter asymmetricKeyParameter, int decryptionBufferSize)
        {
            throw new NotImplementedException("the normal way is to encrypt with the public key and decrypt with the private key");
            //var rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            //var rsaParameters = new RSAParameters()
            //{
            //    Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned(),
            //    Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned()
            //};
            //return BaseDecrypt(dataBinary, rsaParameters, decryptionBufferSize);
        }
        public static byte[] PrivateKeyDecrypt(byte[] dataBinary, AsymmetricKeyParameter asymmetricKeyParameter, int encryptionKeySize, int decryptionBufferSize)
        {
            var raPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)asymmetricKeyParameter;
            var rsaParameters = DotNetUtilities.ToRSAParameters(raPrivateCrtKeyParameters);
            var cspParameters = new CspParameters();
            cspParameters.KeyContainerName = "PriKeyContainer";
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(encryptionKeySize, cspParameters);
            rsaCryptoServiceProvider.ImportParameters(rsaParameters);
            return BaseDecrypt(dataBinary, rsaCryptoServiceProvider.ExportParameters(true), decryptionBufferSize);
        }
        private static byte[] BaseDecrypt(byte[] dataBinary, RSAParameters rsaParameters, int decryptionBufferSize)
        {
            var result = default(byte[]);
            // Create a new instance of RSACryptoServiceProvider.
            using (var rsaCryptoServiceProvider = new RSACryptoServiceProvider())
            {
                // Import the RSA Key information. This needs to include the private key information.
                rsaCryptoServiceProvider.ImportParameters(rsaParameters);
                // Decrypt the passed byte array and specify OAEP padding.  
                // OAEP padding is only available on Microsoft Windows XP or later.  
                using (var memoryStream = new MemoryStream(dataBinary.Length))
                {
                    // The buffer that will hold the encrypted chunks
                    var bufferArray = new byte[decryptionBufferSize];
                    var pos = 0;
                    var copyLength = bufferArray.Length;
                    while (true)
                    {
                        // Copy a chunk of encrypted data / iteration
                        Array.Copy(dataBinary, pos, bufferArray, 0, copyLength);
                        // Set the next start position
                        pos += copyLength;
                        // Decrypt the data using the private key
                        // We need to store the decrypted data temporarily because we don't know the size of it; 
                        // unlike with encryption where we know the size is 128 bytes. 
                        // The only thing we know is that it's between 1-117 bytes
                        var responByteArray = rsaCryptoServiceProvider.Decrypt(bufferArray, false);
                        memoryStream.Write(responByteArray, 0, responByteArray.Length);
                        // Cleat the buffers
                        Array.Clear(responByteArray, 0, responByteArray.Length);
                        Array.Clear(bufferArray, 0, copyLength);
                        if (pos >= dataBinary.Length)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
        public static byte[] PublicKeyDecrypt(byte[] dataBinary, string publicKeyBase64String)
        {
            throw new NotImplementedException("the normal way is to encrypt with the public key and decrypt with the private key");
            //var x509Certificate2 = CertUtil.ToX509Certificate2(publicKeyBase64String);
            //var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            //rsaCryptoServiceProvider.FromXmlString(x509Certificate2.PublicKey.Key.ToXmlString(false));
            //return rsaCryptoServiceProvider.Decrypt(dataBinary, true);
        }
        public static byte[] PrivateKeyDecrypt(byte[] dataBinary, AsymmetricKeyParameter asymmetricKeyParameter, int encryptionKeySize)
        {
            var raPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)asymmetricKeyParameter;
            var rsaParameters = DotNetUtilities.ToRSAParameters(raPrivateCrtKeyParameters);
            var cspParameters = new CspParameters();
            cspParameters.KeyContainerName = "PriKeyContainer";
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(encryptionKeySize, cspParameters);
            rsaCryptoServiceProvider.ImportParameters(rsaParameters);
            return rsaCryptoServiceProvider.Decrypt(dataBinary, true);
        }
    }
}
