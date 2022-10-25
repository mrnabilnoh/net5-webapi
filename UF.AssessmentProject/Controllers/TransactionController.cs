using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UF.AssessmentProject.Helper;
using UF.AssessmentProject.Repository;
using Swashbuckle.AspNetCore.Annotations;
using UF.AssessmentProject.Model.Transaction;

namespace UF.AssessmentProject.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    [SwaggerTag("Transaction Middleware Controller to keep transactional data in Log Files")]
    public class TransactionController : ControllerBase
    {
        private readonly IDbPartnersRepository _repository;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IDbPartnersRepository repository, ILogger<TransactionController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        ///     Submit Transaction data
        /// </summary>
        /// <remarks>
        ///     Ensure all parameter needed and responded as per IDD.
        ///     Ensure all possible validation is done.
        ///     API purpose: To ensure all data is validated and only valid
        ///     partner with valid signature are able to access to this API
        /// </remarks>
        /// <param name="req">RequestMessage Object</param>
        /// <returns></returns>
        [HttpPost("submittrxmessage")]
        [SwaggerResponse(StatusCodes.Status200OK, "Submit Transaction Message successfully", typeof(ResponseMessage))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized, Request")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Oops! Can't get your Post right now")]
        public ActionResult<ResponseMessage> SubmitTRansaction(RequestMessage req)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(req));

            var results = new ResponseMessage
            {
                // By default, we always assume it failed unless proof otherwise by validation
                result = DataDictionary.ResponseResult.Failed
            };

            try
            {
                // Validation 1: Check for authorized partner, then we will check if signature valid
                if (_repository.IsValid(req.partnerkey, req.partnerpassword) && isSignatureValid(req))
                {
                    // Validation 2: Check for total amount match with items amount
                    if (isTotalAmountValid(req))
                    {
                        // Validation 3: Check request timestamp within +-5min server time
                        if (isRequestNotExpired(req))
                        {
                            results.result = DataDictionary.ResponseResult.Success;
                            results.resultmessage = ResponseMessage.MessageRequestValid;
                        }
                        else
                        {
                            results.resultmessage = ResponseMessage.MessageExpired;
                        }
                    }
                    else
                    {
                        results.resultmessage = ResponseMessage.MessageInvalidTotalAmount;
                    }
                }
                else
                {
                    results.resultmessage = ResponseMessage.MessageAccessDenied;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                // If exception happen, then automatic response fail
                results.result = DataDictionary.ResponseResult.Failed;
            }

            _logger.LogInformation(JsonConvert.SerializeObject(results));

            return Ok(results);
        }

        /// <summary>
        ///     Validate signature match with request item.
        /// </summary>
        /// <param name="req">RequestMessage Object</param>
        /// <returns></returns>
        private bool isSignatureValid(RequestMessage req)
        {
            try
            {
                // Based on the task requirement, we assume the string is in Base64 format.
                // Thus we need to decode it first before able to check if it SHA256 string.
                var decodedBase64 = Convert.FromBase64String(req.sig);
                var decodedSha256 = Encoding.UTF8.GetString(decodedBase64);

                // Generate new signature for existing RequestMessage.
                // signature: sigtimestamp + partnerkey + partnerrefno + totalamount + partnerpassword
                var signature = req.RealTimeStamp.ToUniversalTime().ToString("yyyyMMddHHmmss") + req.partnerkey + req.partnerrefno + req.totalamount + req.partnerpassword;

                return SignatureHelper.IsSignatureSame(signature, decodedSha256);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return false;
        }

        /// <summary>
        ///     Validate total amount match with item actual amount.
        /// </summary>
        /// <param name="req">RequestMessage Object</param>
        /// <returns></returns>
        private bool isTotalAmountValid(RequestMessage req)
        {
            long itemActualAmount = 0;

            if (req.items != null && req.items.Length > 0)
            {
                itemActualAmount = req.items.Sum(x => x.qty * x.unitprice);
            }
            
            return req.totalamount == itemActualAmount;
        }

        /// <summary>
        ///     Validate request executing time within allowed +- 5 minute server time.
        /// </summary>
        /// <param name="req">RequestMessage Object</param>
        /// <returns></returns>
        private bool isRequestNotExpired(RequestMessage req)
        {
            // Get +-5 minute range from current time.
            var currentDateTime = DateTime.Now;
            var past5MinuteDateTime = currentDateTime.AddMinutes(-5);
            var next5MinuteDateTime = currentDateTime.AddMinutes(5);

            return req.RealTimeStamp >= past5MinuteDateTime && req.RealTimeStamp < next5MinuteDateTime;
        }

        /// <summary>
        ///     Test this controller is active
        /// </summary>
        /// <remarks>
        ///     Test API to check API is Alive or not
        /// </remarks>
        /// <returns></returns>
        [HttpGet("testapi")]
        public ActionResult<string> TestAPI()
        {
            const string result = "Hello World!";
            return Ok(result);
        }
    }
}
