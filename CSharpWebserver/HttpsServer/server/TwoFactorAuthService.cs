using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebHttpsServer.server
{
    public enum TwoFactorAuthResult
    {
        UserNotFound,
        Old2FACode,
        Wrong2FACode,
        SuccessfulAuthentication
    }

    public static class TwoFactorAuthService
    {
        public static bool ValidateTwoFactorCode(byte[] accountHash)
        {
            string user2FACode = FrontEndInteractionService.RequestTwoFactorAuthCode();
            string secret = DataBaseService.GetUserTwoFactorAuthSecret(Encoding.ASCII.GetString(accountHash));
            string generated2FACode = GenerateTwoFactorCode(secret);


            // UNDONE multiple comparison, if wrong code, request new code


            if (CompareTwoFactorCodes(user2FACode, generated2FACode) == TwoFactorAuthResult.SuccessfulAuthentication) return true; // TEMP
            return false; // TEMP
        }

        private static string GenerateTwoFactorCode(string secret)
        {
            throw new NotImplementedException();
        }

        private static TwoFactorAuthResult CompareTwoFactorCodes(string twoFactorCode1, string twoFactorCode2) // potentially unnecessary
        {
            throw new NotImplementedException();
        }
    }
}
