using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TracerDemo.Helpers
{
    public class HasherHelper
    {
        /// <summary>
        /// Generate a SHA256 hash, based on input string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = SHA256.Create();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }
}
