using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHttpsServer.server
{
    public class UserNotFoundException : Exception
    {

    }

    public static class DataBaseService
    {

        /// <summary>
        /// Uses a primary key to get other user data out of a database.
        /// </summary>
        /// <param name="email">Uses the email as a primary key.</param>
        /// <returns>Tuple containing (id, name, hashed password, salt, uses 2fa)</returns>
        public static (int, string, string, byte[], bool) GetUserData(string email)
        {
            //byte[] salt = Encoding.UTF8.GetBytes(saltString);
            throw new NotImplementedException();
        }

        public static string GetUserTwoFactorAuthSecret(string accountHash)
        {
            throw new NotImplementedException();
        }
    }
}
