using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Static_AES
{
    // 제이슨 암호화
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine(Encrypt("abc@naver.com", "_dhqxlak2010_"));
        //}

        public static string Encrypt(string textToEncrypt, string key)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;

                rijndaelCipher.KeySize = 128;
                rijndaelCipher.BlockSize = 128;
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;

                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }
                Array.Copy(pwdBytes, keyBytes, len);

                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

                byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
                return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
            }
            catch (Exception ex)
            {
                // 암호화 실패
                Debug.LogError("Encrypt Error : " + ex.Message);
                return null;
            }
        }

        public static string Decrypt(string textToDecrypt, string key)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;

                rijndaelCipher.KeySize = 128;
                rijndaelCipher.BlockSize = 128;
                byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }

                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;

                byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return Encoding.UTF8.GetString(plainText);
            }
            catch (Exception ex)
            {
                // 복화 실패
                Debug.LogError("Decrypt Error : " + ex.Message);
                return null;
            }
        }
        //public static string Encrypt(string textToEncrypt, string key)
        //{
        //    RijndaelManaged rijndaelCipher = GetRijndaelCipher(key);
        //    byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
        //    return Convert.ToBase64String(rijndaelCipher.CreateEncryptor().TransformFinalBlock(plainText, 0, plainText.Length));
        //}

        //    public static string Decrypt(string textToDecrypt, string key)
        //{
        //    RijndaelManaged rijndaelCipher = GetRijndaelCipher(key);
        //    byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
        //    byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        //    return Encoding.UTF8.GetString(plainText);
        //}

        //static RijndaelManaged GetRijndaelCipher(string key)
        //{
        //    byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        //    byte[] keyBytes = new byte[16];
        //    int len = pwdBytes.Length;
        //    if (len > keyBytes.Length) len = keyBytes.Length;
        //    Array.Copy(pwdBytes, keyBytes, len);

        //    return new RijndaelManaged
        //    {
        //        Mode = CipherMode.CBC,
        //        Padding = PaddingMode.PKCS7,
        //        KeySize = 128,
        //        BlockSize = 128,
        //        Key = keyBytes,
        //        IV = keyBytes
        //    };
        //}
    }
}