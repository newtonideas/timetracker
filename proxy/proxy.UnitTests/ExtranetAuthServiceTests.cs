using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using proxy.AuthServices;
using proxy.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace proxy.UnitTests
{
    [TestClass]
    public class ExtranetAuthServiceTests
    {
        private Mock<ITokenStorage> mockTokenStorage;
        private Mock<IConfiguration> mockConfiguration;

        private ExtranetAuthService extranetAuthService;

        [TestInitialize]
        public void SetupMocks()
        {
            //stubs for DI objects
            mockTokenStorage = new Mock<ITokenStorage>();
            mockConfiguration = new Mock<IConfiguration>();
            extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);

        }

        [TestMethod]
        public void GetAuthCredentials_ValidToken_ReturnsAuthCredentials()
        {
            //Arrange
            AccessToken accessToken = new AccessToken() { Auth = "auth", SessionId = "sessionId", Token = "token" };
            //stub for SingleOrDefaultAsync() method that should return AccessToken from the database
            mockTokenStorage.Setup(m => m.SingleOrDefaultAsync(It.IsAny<string>())).Returns(Task.FromResult((object)accessToken));

            //Act
            Dictionary<string, string> res = extranetAuthService.getAuthCredentials("").Result;

            //Assert
            CollectionAssert.AllItemsAreNotNull(res.Values);
        }

        [TestMethod]
        public void GetAuthCredentials_InvalidToken_ThrowsException()
        {
            //Arrange
            //stub for SingleOrDefaultAsync() method that should return AccessToken from the database
            mockTokenStorage.Setup(m => m.SingleOrDefaultAsync(It.IsAny<string>())).Returns(Task.FromResult((object)null));


            //Act
            try
            {
                var res = extranetAuthService.getAuthCredentials("").Result;
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Unauthorized");
            }

        }

        [TestMethod]
        public void CreateAuthCredentials_ValidLoginAndPassword_ReturnsAuthToken()
        {
            //Arrange
            //stub for Iconfiguration object that should return extranet domain
            mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");
            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);
            //input your login here
            string validLogin = "login";
            //input your password here
            string validPassword = "password";

            //Act
            var token = extranetAuthService.createAuthCredentials(validLogin, validPassword).Result;

            //Assert
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void CreateAuthCredentials_InvalidLoginAndPassword_ThrowsException()
        {
            //Arrange
            //stub for SingleOrDefaultAsync() method that should return AccessToken from the database
            mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");
            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);
            string invalidLogin = "Invalid login";
            string invalidPassword = "Invalid password";

            //Act
            try
            {
                var token = extranetAuthService.createAuthCredentials(invalidLogin, invalidPassword).Result;
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid login/password");
            }
        }
    }
}
