using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace NetCoreProject.Domain.Util
{
    public class SecurityUtil
    {
        public static byte[] CreateSecurityKey()
        {
            var secureRandom = new SecureRandom();
            var key = new byte[256 / 8];
            secureRandom.NextBytes(key);
            return key;
        }
        public static byte[] HexToByte(string hexStr)
        {
            var result = new byte[hexStr.Length / 2];
            for (var i = 0; i < (hexStr.Length / 2); i++)
            {
                var firstNibble = byte.Parse(hexStr.Substring((2 * i), 1), NumberStyles.HexNumber);
                var secondNibble = byte.Parse(hexStr.Substring((2 * i) + 1, 1), NumberStyles.HexNumber);
                result[i] = Convert.ToByte((firstNibble << 4) | (secondNibble));
            }
            return result;
        }
        public static string ToHex(byte[] data)
        {
            var result = string.Empty;
            foreach (var c in data)
            {
                result += c.ToString("X2");
            }
            return result;
        }
        public static string ToHex(string asciiString)
        {
            var result = string.Empty;
            foreach (var c in asciiString)
            {
                result += string.Format("{0:x2}", Convert.ToUInt32(c.ToString()));
            }
            return result;
        }
        public static SecureString StringToSecure(string value)
        {
            var result = new SecureString();
            foreach (char c in value)
            {
                result.AppendChar(c);
            }
            return result;
        }

        public static string SecureToString(SecureString value)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        public static string MD5Hash(string text, string salt)
        {
            var result = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(text + salt));

                for (var i = 0; i < data.Length; i++)
                {
                    result.Append(data[i].ToString("x2"));
                }
            }
            return result.ToString();
        }
        public static string Encrypt(string plainText, string securityKey)
        {
            var keyBinary = HexToByte(securityKey);
            var viBinary = GetViBinary(keyBinary);
            var plainBinary = Encoding.UTF8.GetBytes(plainText);
            var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            var aeadParameters = new AeadParameters(new KeyParameter(keyBinary), 128, viBinary, null);
            gcmBlockCipher.Init(true, aeadParameters);
            var encryptedBinary = new byte[gcmBlockCipher.GetOutputSize(plainBinary.Length)];
            var len = gcmBlockCipher.ProcessBytes(plainBinary, 0, plainBinary.Length, encryptedBinary, 0);
            gcmBlockCipher.DoFinal(encryptedBinary, len);
            return Convert.ToBase64String(encryptedBinary, Base64FormattingOptions.None);
        }
        public static string Decrypt(string encryptedText, string securityKey)
        {
            var keyBinary = HexToByte(securityKey);
            var viBinary = GetViBinary(keyBinary);
            var encryptedBinary = Convert.FromBase64String(encryptedText);
            var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            var aeadParameters = new AeadParameters(new KeyParameter(keyBinary), 128, viBinary, null);
            gcmBlockCipher.Init(false, aeadParameters);
            var plainBinary = new byte[gcmBlockCipher.GetOutputSize(encryptedBinary.Length)];
            var len = gcmBlockCipher.ProcessBytes(encryptedBinary, 0, encryptedBinary.Length, plainBinary, 0);
            gcmBlockCipher.DoFinal(plainBinary, len);
            return Encoding.UTF8.GetString(plainBinary);
        }
        private static byte[] GetViBinary(byte[] keyBinary)
        {
            var result = new byte[keyBinary.Length / 2];
            for (var i = 0; i < keyBinary.Length / 2; ++i)
            {
                result[i] = Convert.ToByte(keyBinary[i * 2] ^ keyBinary[i * 2 + 1]);
            }
            return result;
        }
    }
}
