using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using UF.AssessmentProject.Helper;

namespace UF.AssessmentProject.Model
{
    public class RequestMessage
    {
        /// <summary>
        ///     The allowed partner's key.
        /// </summary>
        /// <example>string</example>
        /// <returns></returns>
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The {0} require valid key.")]
        public string partnerkey { get; set; }

        /// <summary>
        ///     Partner's unique reference number for this transaction.
        /// </summary>
        /// <remarks>
        ///     Sample reference number `FA0000001` length is 9, we use it as minimum length check.
        /// </remarks>
        /// <example>FA0000001</example>
        /// <returns></returns>
        [Required]
        [StringLength(50, MinimumLength = 9, ErrorMessage = "The {0} require valid reference number.")]
        public string partnerrefno { get; set; }

        /// <summary>
        ///     The allowed partner's password.
        /// </summary>
        /// <remarks>
        ///     This field is not define in task table but we know it exist because sample request have this column field.
        ///     As column field is not define in the table, we assume it do not have max string length.
        /// </remarks>
        /// <example>string</example>
        /// <returns></returns>
        [Required(AllowEmptyStrings = false)]
        public string partnerpassword { get; set; }

        /// <summary>
        ///     Total amount of payment in MYR.
        ///     - Only allow positive value.
        ///     - Value is in cents (exp: 1000 = MYR 10.00).
        /// </summary>
        /// <example>1000</example>
        /// <returns></returns>
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "The {0} require positive value.")]
        public long totalamount { get; set; }

        /// <summary>
        ///     Time in the ISO format. ie. 2014-04-14T12:34:23.00+0800 Preferably set using RealTimeStamp instead.
        /// </summary>
        /// <example>2020-07-28T12:34:23.00+0800</example>
        /// <returns></returns>
        [Required]
        public string timestamp
        {
            get => _realTimeStamp.ToUniversalTime().ToString("o");

            // For invalid datetime string, this will throw an error.
            // But we will not handle it, we let global exception handler return standard ProblemDetail object.
            set => _realTimeStamp = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        /// <summary>
        ///     Message signature result from joined string that been converted to SHA256 hash, then encode it again with Base64.
        ///     - Joint String Between sigtimestamp + partnerkey + partnerrefno + totalamount + partnerpassword.
        ///     - sigtimestamp is from timestamp but in `yyyyMMddHHmmss` format.
        /// </summary>
        /// <example>1000</example>
        /// <returns></returns>
        [Required]
        [SignatureRegex]
        public string sig { get; set; } 

        private DateTime _realTimeStamp;

        /// <summary>
        /// When this field is set, automatically converts to the string 
        /// for the timestamp property
        /// </summary> 
        /// <remarks>Internal use</remarks>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Text.Json.Serialization.JsonIgnore()]
        public DateTime RealTimeStamp
        {
            get { return _realTimeStamp; }
        }
    } 
}
