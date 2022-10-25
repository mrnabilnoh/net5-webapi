using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UF.AssessmentProject.Helper
{
    /// <summary>
    ///     Check if string in correct signature format
    ///     that is SHA256 string encoded in Base64.
    /// </summary>
    public class SignatureRegexAttribute : ValidationAttribute
    {
        private readonly Regex _regex;

        public SignatureRegexAttribute() : base("The {0} require valid signature format.")
        {
            // Regex pattern for SHA256 
            _regex = new Regex(@"[A-Fa-f0-9]{64}", RegexOptions.Compiled);
        }

        public override bool IsValid(object value)
        {
            var strValue = value as string;

            if (string.IsNullOrEmpty(strValue))
            {
                // If value is null or empty, no need to proceed.
                // We let other data annotation validation handle it (if exist), such as: [Required]
                return true;
            }

            try
            {
                // Based on the task requirement, we assume the string is in Base64 format.
                // Thus we need to decode it first before able to check if it SHA256 string.
                var data = Convert.FromBase64String(strValue);
                var decodedString = Encoding.UTF8.GetString(data);
                return _regex.IsMatch(decodedString);
            }
            catch
            {
                // If it throw exception, then the string is not a valid Base64 string.
                return false;
            }
        }
    }
} 
