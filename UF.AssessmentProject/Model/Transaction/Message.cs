using System.ComponentModel.DataAnnotations;

namespace UF.AssessmentProject.Model.Transaction
{
    public class RequestMessage : Model.RequestMessage
    {
        public itemdetail[] items { get; set; } 
    }

    public class itemdetail
    {
        /// <summary>
        ///     Unique reference ID the of an item.
        ///     - Cannot be null or empty
        /// </summary>
        /// <example>string</example>
        /// <returns></returns>
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The {0} require valid reference number.")]
        public string itemref { get; set; }

        /// <summary>
        ///     Name of the item.
        ///     - Cannot be null or empty
        /// </summary>
        /// <example>string</example>
        /// <returns></returns>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string name { get; set; }

        /// <summary>
        ///     Quantity of the item bought.
        ///     - Only allow value to be >= 1.
        ///     - Quantity must not exceed 10.
        /// </summary>
        /// <example>1</example>
        /// <returns></returns>
        [Required]
        [Range(1, 10)]
        public int qty { get; set; }

        /// <summary>
        ///     Price of one unit of the item in the currency of the transaction (MYR).
        ///     - Only allow positive value.
        ///     - Value is in cents (exp: 1000 = MYR 10.00).
        /// </summary>
        /// <example>1000</example>
        /// <returns></returns>
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "The {0} require positive value.")]
        public long unitprice { get; set; }
    }

    public class ResponseMessage : Model.ResponseMessage
    {
        public const string MessageAccessDenied = "Access Denied!";
        public const string MessageInvalidTotalAmount = "Invalid Total Amount.";
        public const string MessageExpired = "Expired.";
        public const string MessageRequestValid = "Request data is valid.";
    }
}
