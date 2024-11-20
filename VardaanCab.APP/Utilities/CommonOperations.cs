using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace VardaanCab.APP.Utilities
{
    public class CommonOperations
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        public string GenerateRandomOtp(int length = 6)
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            byte[] randomBytes = new byte[length];
            _rng.GetBytes(randomBytes);

            return new string(randomBytes.Select(b => chars[b % chars.Length]).ToArray());
        }
    }
}