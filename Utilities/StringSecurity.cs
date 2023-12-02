using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace StickyNotesJ.Utilities
{
    internal class StringSecurity
    {

        private static byte[] DeriveKeyFromPassword(string password)
        {
            byte[] salt = new byte[16];
            byte[] pwdHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            byte[] pwdHash2 = SHA256.HashData(pwdHash);

            if (pwdHash2.Length < 16)
            {
                byte[] extra = Encoding.UTF8.GetBytes("!@*__fj37");
                Buffer.BlockCopy(pwdHash2, 0, salt, 0, pwdHash2.Length);
                Buffer.BlockCopy(extra, 0, salt, pwdHash2.Length, 16 - pwdHash2.Length);
            }
            else
            {
                Buffer.BlockCopy(pwdHash2, 0, salt, 0, 16);
            }

            Rfc2898DeriveBytes keyDerivation = new(password, salt, 100000, HashAlgorithmName.SHA256);

            return keyDerivation.GetBytes(32);
        }

        public static string EncryptString(string plainText, string password)
        {
            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(password);
            aes.GenerateIV();

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] ivPlainBytes = new byte[inputBytes.Length + aes.IV.Length];
            Buffer.BlockCopy(aes.IV, 0, ivPlainBytes, 0, aes.IV.Length);
            Buffer.BlockCopy(inputBytes, 0, ivPlainBytes, aes.IV.Length, inputBytes.Length);

            using MemoryStream ms = new();
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(ivPlainBytes, 0, ivPlainBytes.Length);
            }

            byte[] cipherText = ms.ToArray();

            string encrypted = Convert.ToBase64String(cipherText);
            return encrypted;
        }


        public static string DecryptString(string encryptedText, string password)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);

            using Aes aes = Aes.Create();
            byte[] iv = new byte[16];
            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);

            byte[] cipher = new byte[cipherBytes.Length - iv.Length];
            Buffer.BlockCopy(cipherBytes, iv.Length, cipher, 0, cipher.Length);

            aes.Key = DeriveKeyFromPassword(password);
            aes.IV = iv;
            try
            {
                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                byte[] plainBytes = ms.ToArray();

                string plainText = Encoding.UTF8.GetString(plainBytes);
                return plainText;
            }
            catch (CryptographicException)
            {
                MessageBox.Show("The key you entered is incorrect", "Incorrect Key", MessageBoxButton.OK);
                return encryptedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK);
                return encryptedText;
            }
        }
    }
}
