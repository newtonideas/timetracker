using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using proxy.AuthServices;
using proxy.Models;
using proxy.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace proxy.UnitTests
{
    [TestClass]
    public class ExtranetUsersRepositoryTests
    {
        private Mock<IAuthService> mockAuthService;
        private Mock<IConfiguration> mockConfiguration;
        private ExtranetUsersRepository extranetUsersRepository;

        [TestInitialize]
        public void SetupMocks()
        {
            //stubs for DI objects
            mockAuthService = new Mock<IAuthService>();
            mockConfiguration = new Mock<IConfiguration>();

            Dictionary<string, string> authCookies = new Dictionary<string, string>();
            //Input valid auth cookie here
            authCookies.Add(".auth", "4D6EB947E818880113FD9065F047680084EFDB66FE09DFDEECB8347A91F33B5548DAC793569937855ACDBFFC14E1AFB8C2BBD509D080231896C3D4922B907CCA0C633FF43669EB75922F6CCE85DF9A3925EE7CFA8826D0632C914A750D291C2F19D2E3DA0D7DA55973CBBAA3E708AA2F7FD051CBEFA351DB8B6BB346E74F2099EE5263407B50F4A685E606FCEC1F10DAD4EAA4A8");
            //Input valid sessionId cookie here
            authCookies.Add("ASP.NET_SessionId", "zcb1fcvtd1rv01b2lpeh3ny5");

            //stub for IConfiguration object that should return extranet domain
            mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");

            //stub for IAuthService getAuthCredentials method that should return valid auth cookies
            mockAuthService.Setup(m => m.getAuthCredentials(It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(authCookies));

            extranetUsersRepository = new ExtranetUsersRepository(mockAuthService.Object, mockConfiguration.Object);

        }

        [TestMethod]
        public void GetAll_ValidTokenWithoutFilters_ReturnsAllUsers()
        {
            //Arrange
            
            //Act
            var users = (List<User>)extranetUsersRepository.GetAll("").Result;
            var checkList = new List<string>();
            foreach(var u in users)
            {
                checkList.Add(u.Id);
                checkList.Add(u.Name);
            }

            //Assert
            CollectionAssert.AllItemsAreNotNull(checkList);
        }


        [TestMethod]
        public void GetAll_ValidTokenWithNameFilter_ReturnsUsersFilteredByName()
        {
            //Arrange
            //input name here
            string name = "Andrii";

            //Act
            var users = (List<User>)extranetUsersRepository.GetAll("", name).Result;

            bool check = true;
            foreach(var u in users)
            {
                if (!u.Name.Contains(name))
                    check = false;
            }

            //Assert
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void GetById_ValidTokenAndId_ReturnsUser()
        {
            //Arrange
            //input user id here
            string id = "e981a503-1536-4a48-920d-6c464f596cbc";

            //Act
            var user = extranetUsersRepository.GetById(id, "").Result;
            bool check = user != null && user.Id.Equals(id);

            //Assert
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void GetById_ValidTokenInvalidId_ReturnsNull()
        {
            //Arrange
            //input invalid user id here
            string id = "invalid";

            //Act
            var user = extranetUsersRepository.GetById(id, "").Result;

            //Assert
            Assert.IsNull(user);
        }
    }
}
