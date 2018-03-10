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

        [TestInitialize]
        public void SetupMocks()
        {
            //stubs for DI objects
            mockTokenStorage = new Mock<ITokenStorage>();
            mockConfiguration = new Mock<IConfiguration>();
        } 

        [TestMethod]
        public void GetAuthCredentials_ValidToken_ReturnsAuthCredentials()
        {
            //Arrange
            AccessToken accessToken = new AccessToken() { Auth = "auth", SessionId = "sessionId", Token = "token" };

            //stub for SingleOrDefaultAsync() method that should return AccessToken from the database
            mockTokenStorage.Setup(m => m.SingleOrDefaultAsync(It.IsAny<string>())).Returns(Task.FromResult((object)accessToken));

            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);

            //Act
            Dictionary<string, string> res = extranetAuthService.getAuthCredentials("").Result;

            //Assert
            CollectionAssert.AllItemsAreNotNull(res.Values);
        }

        [ExpectedException(typeof(AggregateException))]
        [TestMethod]
        public void GetAuthCredentials_InvalidToken_ThrowsException()
        {

            //Arrange
            //stub for SingleOrDefaultAsync() method that should return AccessToken from the database
            mockTokenStorage.Setup(m => m.SingleOrDefaultAsync(It.IsAny<string>())).Returns(Task.FromResult((object)null));
            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);
            
            //Act
            //Exception is expected here
            var res = extranetAuthService.getAuthCredentials("").Result;

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

        [ExpectedException(typeof(AggregateException))]
        [TestMethod]
        public void CreateAuthCredentials_InvalidLoginAndPassword_ThrowsException()
        {
            //Arrange
            //stub for SingleOrDefaultAsync() method that should return AccessToken from the database
            mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");
            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);
            string validLogin = "Invalid login";
            string validPassword = "Invalid password";

            //Act
            //Exception is expected here
            var token = extranetAuthService.createAuthCredentials(validLogin, validPassword).Result;
        }
    }
}
