using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace UF.AssessmentProject.Helper
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class SignatureHelper
    {
        /// <summary>
        /// Check if signature source string is from same given hash
        /// </summary>
        public static bool IsSignatureSame(string source, string hash)
        {
            using (var sha256Hash = SHA256.Create())
            {
                return VerifyHash(sha256Hash, source, hash);
            }
        }

        /// <summary>
        /// Check if signature source string is from same given hash
        /// </summary>
        public static string GenerateNewSignature(string source)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var sourceHash = GetHash(sha256Hash, source);
                var sourceBytes = Encoding.UTF8.GetBytes(sourceHash);
                return Convert.ToBase64String(sourceBytes);
            }
        }

        /// <summary>
        /// Generate new hash 
        /// </summary>
        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verify a hash against a string.
        /// </summary>
        public static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);

            // Create a StringComparer an compare the hashes.
            var comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
