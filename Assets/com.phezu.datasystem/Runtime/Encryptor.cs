using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Phezu.Data {

    public static class Encryptor {
        public static string AEC_key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
        public static string AEC_iv = "1234567887654321";
        public static string DES_key = "ABCDEFGH";
        public static string MD5_key = "Password@username:userID";

        public static string AESEncrypt(string inputData, string key, string iv) {
            AesCryptoServiceProvider AEScryptoProvider = new() {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            byte[] txtByteData = Encoding.ASCII.GetBytes(inputData);
            ICryptoTransform transform = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

            byte[] encrypted = transform.TransformFinalBlock(txtByteData, 0, txtByteData.Length);

            return Convert.ToBase64String(encrypted);
        }

        public static string AESDecrypt(string inputData, string key, string iv) {
            AesCryptoServiceProvider AEScryptoProvider = new() {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            byte[] txtByteData = Convert.FromBase64String(inputData);
            ICryptoTransform transform = AEScryptoProvider.CreateDecryptor();

            byte[] decrypted = transform.TransformFinalBlock(txtByteData, 0, txtByteData.Length);

            return Encoding.ASCII.GetString(decrypted);
        }

        public static string DESEncrypt(string inputData, string key) {
            byte[] txtByteData = Encoding.ASCII.GetBytes(inputData);
            byte[] keyByteData = Encoding.ASCII.GetBytes(key);

            DESCryptoServiceProvider DEScryptoProvider = new();
            ICryptoTransform transform = DEScryptoProvider.CreateEncryptor(keyByteData, keyByteData);
            CryptoStreamMode mode = CryptoStreamMode.Write;

            MemoryStream mStream = new();
            CryptoStream cStream = new(mStream, transform, mode);
            cStream.Write(txtByteData, 0, txtByteData.Length);
            cStream.FlushFinalBlock();

            byte[] result = new byte[mStream.Length];
            mStream.Position = 0;
            mStream.Read(result, 0, result.Length);

            return Convert.ToBase64String(result);
        }

        public static string DESDecrypt(string inputData, string key) {
            byte[] txtByteData = Convert.FromBase64String(inputData);
            byte[] keyByteData = Encoding.ASCII.GetBytes(key);

            DESCryptoServiceProvider DEScryptoProvider = new();
            ICryptoTransform trnsfrm = DEScryptoProvider.CreateDecryptor(keyByteData, keyByteData);
            CryptoStreamMode mode = CryptoStreamMode.Write;

            MemoryStream mStream = new();
            CryptoStream cStream = new(mStream, trnsfrm, mode);
            cStream.Write(txtByteData, 0, txtByteData.Length);
            cStream.FlushFinalBlock();

            byte[] result = new byte[mStream.Length];
            mStream.Position = 0;
            mStream.Read(result, 0, result.Length);

            return Encoding.ASCII.GetString(result);
        }

        public static string MD5Encrypt(string inputData, string key) {
            byte[] bData = Encoding.UTF8.GetBytes(inputData);

            MD5CryptoServiceProvider md5 = new();
            TripleDESCryptoServiceProvider tripalDES = new();

            tripalDES.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            tripalDES.Mode = CipherMode.ECB;

            ICryptoTransform trnsfrm = tripalDES.CreateEncryptor();
            byte[] encrypted = trnsfrm.TransformFinalBlock(bData, 0, bData.Length);

            return Convert.ToBase64String(encrypted);
        }

        public static string MD5Decrypt(string inputData, string key) {
            byte[] bData = Convert.FromBase64String(inputData);

            MD5CryptoServiceProvider md5 = new();
            TripleDESCryptoServiceProvider tripalDES = new();

            tripalDES.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            tripalDES.Mode = CipherMode.ECB;

            ICryptoTransform trnsfrm = tripalDES.CreateDecryptor();
            byte[] decrypted = trnsfrm.TransformFinalBlock(bData, 0, bData.Length);

            return Encoding.ASCII.GetString(decrypted);
        }
    }
}
