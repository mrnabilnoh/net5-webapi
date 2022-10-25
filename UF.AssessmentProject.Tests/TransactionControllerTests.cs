using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UF.AssessmentProject.Controllers;
using UF.AssessmentProject.Model.Transaction;
using UF.AssessmentProject.Repository;
using Xunit;
using Moq;
using UF.AssessmentProject.Helper;
using RequestMessage = UF.AssessmentProject.Model.Transaction.RequestMessage;

namespace UF.AssessmentProject.Tests
{
    public class TransactionControllerTests
    {
       private readonly Mock<IDbPartnersRepository> _repositoryStub = new Mock<IDbPartnersRepository>();
       private readonly Mock<ILogger<TransactionController>> _loggerStub = new Mock<ILogger<TransactionController>>();
       private static Random _random = new Random();

        [Fact]
        public void RequestMessage_PartnerKeyEmpty_ReturnsPartnerKeyIsRequired()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.partnerkey = "";

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The partnerkey field is required."));
        }

        [Fact]
        public void RequestMessage_PartnerKeyStringMoreThan50_ReturnsPartnerKeyRequireValidKey()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.partnerkey = randomString(51);

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The partnerkey require valid key."));
        }

        [Fact]
        public void RequestMessage_PartnerRefnoEmpty_ReturnsPartnerRefnoIsRequired()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.partnerrefno = "";

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The partnerrefno field is required."));
        }

        [Fact]
        public void RequestMessage_PartnerRefnoStringMoreThan50_ReturnsPartnerRefnoRequireValidReferenceNumber()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.partnerrefno = randomString(51);

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The partnerrefno require valid reference number."));
        }

        [Fact]
        public void RequestMessage_PartnerPasswordEmpty_ReturnsPartnerPasswordIsRequired()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.partnerpassword = "";

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The partnerpassword field is required."));
        }

        [Fact]
        public void RequestMessage_TotalAmountZero_ReturnsTotalAmountRequirePositiveValue()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.totalamount = 0;

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The totalamount require positive value."));
        }

        [Fact]
        public void RequestMessage_SigEmpty_ReturnsSigIsRequired()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.sig = "";

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The sig field is required."));
        }

        [Fact]
        public void RequestMessage_SigInvalidFormat_ReturnsSigRequireValidFormat()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.sig = randomString(64);

            // Act
            var lstErrors = validateModel(expectedRequestMessage);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The sig require valid signature format."));
        }

        [Fact]
        public void RequestMessage_ItemsItemRefEmpty_ReturnsItemsItemRefIsRequired()
        {
            // Arrange
            var expectedRequestMessageItem = (createRequestMessage()).items[0];
            expectedRequestMessageItem.itemref = "";

            // Act
            var lstErrors = validateModel(expectedRequestMessageItem);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The itemref field is required."));
        }

        [Fact]
        public void RequestMessage_ItemsNameEmpty_ReturnsItemsNameIsRequired()
        {
            // Arrange
            var expectedRequestMessageItem = (createRequestMessage()).items[0];
            expectedRequestMessageItem.name = "";

            // Act
            var lstErrors = validateModel(expectedRequestMessageItem);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The name field is required."));
        }

        [Fact]
        public void RequestMessage_ItemsQtyMoreThan10_ReturnsItemsQtyMustBeBetween1and10()
        {
            // Arrange
            var expectedRequestMessageItem = (createRequestMessage()).items[0];
            expectedRequestMessageItem.qty = 11;

            // Act
            var lstErrors = validateModel(expectedRequestMessageItem);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The field qty must be between 1 and 10."));
        }

        [Fact]
        public void RequestMessage_ItemsUnitPriceZero_ReturnsItemsUnitPriceRequirePositiveValue()
        {
            // Arrange
            var expectedRequestMessageItem = (createRequestMessage()).items[0];
            expectedRequestMessageItem.unitprice = 0;

            // Act
            var lstErrors = validateModel(expectedRequestMessageItem);

            // Assert
            Assert.Contains(lstErrors, item => item.ErrorMessage.Contains("The unitprice require positive value."));
        }

        [Fact]
        public void SubmitTRansaction_RequestMessagePartnerUnauthorized_ReturnsAccessDenied()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();

            _repositoryStub.Setup(repo => repo.IsValid(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var controller = new TransactionController(_repositoryStub.Object, _loggerStub.Object);


            // Act
            var actionResult = controller.SubmitTRansaction(expectedRequestMessage);


            // Assert
            if (actionResult.Result is OkObjectResult result)
            {
                Assert.IsType<ResponseMessage>(result.Value);
                if (result.Value is ResponseMessage responseMessage)
                {
                    Assert.Equal(DataDictionary.ResponseResult.Failed, responseMessage.result);
                    Assert.Equal(ResponseMessage.MessageAccessDenied, responseMessage.resultmessage);
                }
                else
                {
                    Assert.Fail("SubmitTRansaction should return ResponseMessage object.");
                }
            }
            else
            {
                Assert.Fail("SubmitTRansaction should return OkObjectResult object.");
            }

        }

        [Fact]
        public void SubmitTRansaction_RequestMessageSignatureInvalid_ReturnsAccessDenied()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.totalamount = expectedRequestMessage.totalamount + 10;

            _repositoryStub.Setup(repo => repo.IsValid(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var controller = new TransactionController(_repositoryStub.Object, _loggerStub.Object);


            // Act
            var actionResult = controller.SubmitTRansaction(expectedRequestMessage);


            // Assert
            if (actionResult.Result is OkObjectResult result)
            {
                Assert.IsType<ResponseMessage>(result.Value);
                if (result.Value is ResponseMessage responseMessage)
                {
                    Assert.Equal(DataDictionary.ResponseResult.Failed, responseMessage.result);
                    Assert.Equal(ResponseMessage.MessageAccessDenied, responseMessage.resultmessage);
                }
                else
                {
                    Assert.Fail("SubmitTRansaction should return ResponseMessage object.");
                }
            }
            else
            {
                Assert.Fail("SubmitTRansaction should return OkObjectResult object.");
            }

        }

        [Fact]
        public void SubmitTRansaction_RequestMessageTotalAmountInvalid_ReturnsInvalidTotalAmount()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();
            expectedRequestMessage.items[0].unitprice = 100;

            _repositoryStub.Setup(repo => repo.IsValid(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var controller = new TransactionController(_repositoryStub.Object, _loggerStub.Object);


            // Act
            var actionResult = controller.SubmitTRansaction(expectedRequestMessage);


            // Assert
            if (actionResult.Result is OkObjectResult result)
            {
                Assert.IsType<ResponseMessage>(result.Value);
                if (result.Value is ResponseMessage responseMessage)
                {
                    Assert.Equal(DataDictionary.ResponseResult.Failed, responseMessage.result);
                    Assert.Equal(ResponseMessage.MessageInvalidTotalAmount, responseMessage.resultmessage);
                }
                else
                {
                    Assert.Fail("SubmitTRansaction should return ResponseMessage object.");
                }
            }
            else
            {
                Assert.Fail("SubmitTRansaction should return OkObjectResult object.");
            }

        }

        [Fact]
        public void SubmitTRansaction_RequestMessageExpired_ReturnsExpired()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage(6);

            _repositoryStub.Setup(repo => repo.IsValid(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var controller = new TransactionController(_repositoryStub.Object, _loggerStub.Object);


            // Act
            var actionResult = controller.SubmitTRansaction(expectedRequestMessage);


            // Assert
            if (actionResult.Result is OkObjectResult result)
            {
                Assert.IsType<ResponseMessage>(result.Value);
                if (result.Value is ResponseMessage responseMessage)
                {
                    Assert.Equal(DataDictionary.ResponseResult.Failed, responseMessage.result);
                    Assert.Equal(ResponseMessage.MessageExpired, responseMessage.resultmessage);
                }
                else
                {
                    Assert.Fail("SubmitTRansaction should return ResponseMessage object.");
                }
            }
            else
            {
                Assert.Fail("SubmitTRansaction should return OkObjectResult object.");
            }

        }

        [Fact]
        public void SubmitTRansaction_RequestMessageValid_ReturnsRequestValid()
        {
            // Arrange
            var expectedRequestMessage = createRequestMessage();

            _repositoryStub.Setup(repo => repo.IsValid(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var controller = new TransactionController(_repositoryStub.Object, _loggerStub.Object);


            // Act
            var actionResult = controller.SubmitTRansaction(expectedRequestMessage);


            // Assert
            if (actionResult.Result is OkObjectResult result)
            {
                Assert.IsType<ResponseMessage>(result.Value);
                if (result.Value is ResponseMessage responseMessage)
                {
                    Assert.Equal(DataDictionary.ResponseResult.Success, responseMessage.result);
                    Assert.Equal(ResponseMessage.MessageRequestValid, responseMessage.resultmessage);
                }
                else
                {
                    Assert.Fail("SubmitTRansaction should return ResponseMessage object.");
                }
            }
            else
            {
                Assert.Fail("SubmitTRansaction should return OkObjectResult object.");
            }

        }

        private static RequestMessage createRequestMessage(int timestampMinuteAdjustment = 0)
        {
            var currentDateTime = DateTime.Now;
            var adjustDateTime = currentDateTime.AddMinutes(timestampMinuteAdjustment);

            var requestMessage = new RequestMessage
            {
                partnerkey = "FAKEGOOGLE",
                partnerrefno = "FA0000001",
                partnerpassword = "FAKEPASSWORD1234",
                totalamount = 1000,
                timestamp = adjustDateTime.ToString("o"),
                items = new []
                {
                    new itemdetail()
                    {
                        itemref = "i-00001",
                        name = "Pen",
                        qty = 2,
                        unitprice = 500
                    }, 
                }
            };

            // Generate new plain signature for existing RequestMessage.
            // signature: sigtimestamp + partnerkey + partnerrefno + totalamount + partnerpassword
            var signature = requestMessage.RealTimeStamp.ToUniversalTime().ToString("yyyyMMddHHmmss") + requestMessage.partnerkey + requestMessage.partnerrefno + requestMessage.totalamount + requestMessage.partnerpassword;

            // Encode signature 
            requestMessage.sig = SignatureHelper.GenerateNewSignature(signature);



            return requestMessage;
        }

        private static string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static IEnumerable<ValidationResult> validateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }


    }
}
