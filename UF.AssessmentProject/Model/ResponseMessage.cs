 
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using UF.AssessmentProject.Helper;

namespace UF.AssessmentProject.Model
{
    public class ResponseMessage
    {
        /// <summary>
        ///     Response Result
        ///     1 - is successful
        ///     0 - if errors encountered.
        /// </summary>
        /// <example>1</example>
        [Required]
        public DataDictionary.ResponseResult result { get; set; }

        /// <summary>
        /// Result message if the operation was a Success or Failure
        /// </summary>
        /// <example>string</example>
        [Required]
        public string resultmessage { get; set; }

    }
}
