using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;

namespace NetCoreProject.Domain.Util
{
    public class CertEncryptUtil
    {
        public static byte[] PublicKeyEncrypt(byte[] dataBinary, AsymmetricKeyParameter publicAsymmetricKeyParameter, int encryptionBufferSize, int decryptionBufferSize)
        {
            var rsaKeyParameters = (RsaKeyParameters)publicAsymmetricKeyParameter;
            var rsaParameters = new RSAParameters()
            {
                Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned(),
                Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned()
            };
            return BaseEncrypt(dataBinary, rsaParameters, encryptionBufferSize, decryptionBufferSize);
        }
        public static byte[] PublicKeyEncrypt(byte[] dataBinary, X509Certificate2 x509Certificate2, int encryptionBufferSize, int decryptionBufferSize)
        {
            return BaseEncrypt(dataBinary, x509Certificate2.GetRSAPublicKey().ExportParameters(false), encryptionBufferSize, decryptionBufferSize);
        }
        public static byte[] PublicKeyEncrypt(byte[] dataBinary, string publicKeyBase64String, int encryptionBufferSize, int decryptionBufferSize)
        {
            var x509Certificate2 = CertUtil.ToX509Certificate2(publicKeyBase64String);
            return BaseEncrypt(dataBinary, x509Certificate2.GetRSAPublicKey().ExportParameters(false), encryptionBufferSize, decryptionBufferSize);
        }
        public static byte[] PivateKeyEncrypt(byte[] dataBinary, AsymmetricKeyParameter asymmetricKeyParameter, int encryptionKeySize, int encryptionBufferSize, int decryptionBufferSize)
        {
            throw new NotImplementedException("the normal way is to encrypt with the public key and decrypt with the private key");
            //var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)asymmetricKeyParameter;
            //var rsaParameters = DotNetUtilities.ToRSAParameters(rsaPrivateCrtKeyParameters);
            //var rsaCryptoServiceProvider = new RSACryptoServiceProvider(encryptionKeySize);
            //rsaCryptoServiceProvider.ImportParameters(rsaParameters);
            //return BaseEncrypt(dataBinary, rsaCryptoServiceProvider.ExportParameters(true), encryptionBufferSize, decryptionBufferSize);
        }
        private static byte[] BaseEncrypt(byte[] dataBinary, RSAParameters rsaParameters, int encryptionBufferSize, int decryptionBufferSize)
        {
            var result = default(byte[]);
            //Create a new instance of RSACryptoServiceProvider.
            using (var rsaCryptoServiceProvider = new RSACryptoServiceProvider())
            {
                //Import the RSA Key information. This only needs toinclude the public key information.
                rsaCryptoServiceProvider.ImportParameters(rsaParameters);
                ////Encrypt the passed byte array and specify OAEP padding.  
                ////OAEP padding is only available on Microsoft Windows XP or later.  
                using (var memoryStream = new MemoryStream())
                {
                    var bufferByteArray = new byte[encryptionBufferSize];
                    var pos = 0;
                    var copyLength = bufferByteArray.Length;
                    while (true)
                    {
                        // Check if the bytes left to read is smaller than the buffer size, 
                        // then limit the buffer size to the number of bytes left
                        if (pos + copyLength > dataBinary.Length)
                        {
                            copyLength = dataBinary.Length - pos;
                        }
                        // Create a new buffer that has the correct size
                        bufferByteArray = new byte[copyLength];
                        //Copy as many bytes as the algorithm can handle at a time, 
                        //iterate until the whole input array is encoded
                        Array.Copy(dataBinary, pos, bufferByteArray, 0, copyLength);
                        //Start from here in next iteration
                        pos += copyLength;
                        //Encrypt the data using the public key and add it to the memory buffer
                        memoryStream.Write(rsaCryptoServiceProvider.Encrypt(bufferByteArray, false), 0, decryptionBufferSize);
                        // Clear the content of the buffer, 
                        // otherwise we could end up copying the same data during the last iteration
                        Array.Clear(bufferByteArray, 0, copyLength); 
                        // Check if we have reached the end, then exit
                        if (pos >= dataBinary.Length)
                        {
                            break;
                        }
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
        public static byte[] PublicKeyEncrypt(byte[] dataBinary, string publicKeyBase64String)
        {
            var x509Certificate2 = CertUtil.ToX509Certificate2(publicKeyBase64String);
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.FromXmlString(x509Certificate2.PublicKey.Key.ToXmlString(false));
            return rsaCryptoServiceProvider.Encrypt(dataBinary, true);
        }
        public static byte[] PivateKeyEncrypt(byte[] dataBinary, AsymmetricKeyParameter asymmetricKeyParameter, int encryptionKeySize)
        {
            throw new NotImplementedException("the normal way is to encrypt with the public key and decrypt with the private key");
            //var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)asymmetricKeyParameter;
            //var rsaParameters = DotNetUtilities.ToRSAParameters(rsaPrivateCrtKeyParameters);
            //var rsaCryptoServiceProvider = new RSACryptoServiceProvider(encryptionKeySize);
            //rsaCryptoServiceProvider.ImportParameters(rsaParameters);
            //return rsaCryptoServiceProvider.Encrypt(dataBinary, true);
        }
    }
}
