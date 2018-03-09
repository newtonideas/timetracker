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
            mockTokenStorage = new Mock<ITokenStorage>();
            mockConfiguration = new Mock<IConfiguration>();
        } 

        [TestMethod]
        public void GetAuthCredentials_ValidToken_ReturnsAuthCredentials()
        {

            AccessToken accessToken = new AccessToken() { Auth = "auth", SessionId = "sessionId", Token = "token" };
            mockTokenStorage.Setup(m => m.SingleOrDefaultAsync(It.IsAny<string>())).Returns(Task.FromResult((object)accessToken));
            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);


            Dictionary<string, string> res = extranetAuthService.getAuthCredentials("").Result;


            CollectionAssert.AllItemsAreNotNull(res.Values);
        }

        [ExpectedException(typeof(AggregateException))]
        [TestMethod]
        public void GetAuthCredentials_InvalidToken_ThrowsException()
        {

            mockTokenStorage.Setup(m => m.SingleOrDefaultAsync(It.IsAny<string>())).Returns(Task.FromResult((object)null));
            ExtranetAuthService extranetAuthService = new ExtranetAuthService(mockConfiguration.Object, mockTokenStorage.Object);
            

            var res = extranetAuthService.getAuthCredentials("").Result;

        }
    }
}
