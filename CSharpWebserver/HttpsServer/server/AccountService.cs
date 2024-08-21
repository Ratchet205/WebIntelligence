using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebHttpsServer.server
{
    public enum LoginAnswer
    {
        UserNotFound,
        WrongPassword,
        LoginSuccess,
        LoginError
    }

    public static class AccountService
    {
        public static LoginAnswer Login(string email, ref string password)
        {
            (int, string, string, byte[], bool) userData;

            try
            {
                userData = DataBaseService.GetUserData(email);
            }
            catch (UserNotFoundException)
            {
                return LoginAnswer.UserNotFound;
            }

            int id = userData.Item1;
            string name = userData.Item2;
            byte[] hashedPasswordDataBase = Encoding.ASCII.GetBytes(userData.Item3);
            byte[] salt = userData.Item4;
            bool uses2FA = userData.Item5;

            byte[] hashedPasswordCode = HashPassword(password, salt);
            byte[] accountHash = GenerateAccountHash(password, name, id);

            password = String.Empty;

            if (!CompareHashes(hashedPasswordDataBase, hashedPasswordCode)) return LoginAnswer.WrongPassword;

            if (!uses2FA) return LoginAnswer.LoginSuccess;

            if(TwoFactorAuthService.ValidateTwoFactorCode(accountHash)) return LoginAnswer.LoginSuccess;

            return LoginAnswer.LoginError;
        }

        private static byte[] GenerateAccountHash(string password, string name, int id)
        {
            string accountString = password + name + id;

            byte[] accountStringBytes = Encoding.UTF8.GetBytes(accountString);
            byte[] hashedBytes = SHA256.HashData(accountStringBytes);
            return hashedBytes;
            
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            byte[] hashedBytes = SHA256.HashData(saltedPassword);

            return hashedBytes;
        }

        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            bool hashesEqual = false;
            if (hash1.Length == hash2.Length)
            {
                int i = 0;
                while ((i < hash1.Length) && (hash1[i] == hash2[i]))
                {
                    i += 1;
                }
                if (i == hash1.Length)
                {
                    hashesEqual = true;
                }
            }
            return hashesEqual;
        }

        private static byte[] GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[8];
            rng.GetBytes(salt);
            return salt;
        }
    }
}
