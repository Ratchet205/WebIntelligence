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
        Bad2FACode,
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

        private static TwoFactorAuthResult CompareTwoFactorCodes(string twoFactorCode1, string twoFactorCode2, int earlierAllowedIntervals = 1, int laterAllowedIntervals = 1)
        {
            if (string.IsNullOrEmpty(twoFactorCode1) || string.IsNullOrEmpty(twoFactorCode2) 
                || twoFactorCode1.Length != 6 || twoFactorCode2.Length != 6 
                || !int.TryParse(twoFactorCode1, out _) || !int.TryParse(twoFactorCode2, out _)) 
                return TwoFactorAuthResult.Bad2FACode;


            throw new NotImplementedException();
        }
    }
}
