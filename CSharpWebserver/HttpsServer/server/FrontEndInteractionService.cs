using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHttpsServer.server
{
    public static class FrontEndInteractionService
    {
        public static string RequestTwoFactorAuthCode()
        {
            // interact with frontend for 2FA code request
            throw new NotImplementedException();
        }

        public static bool SetSessionCookie(string sessionToken)
        {
            // interaction with client setting session cookie using session Token
            throw new NotImplementedException();
        }
    }
}
