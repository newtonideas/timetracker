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
        private Mock<IAuthService> _mockAuthService;
        private Mock<IConfiguration> _mockConfiguration;
        private ExtranetUsersRepository _extranetUsersRepository;
        private Mock<IProjectRepository> _mockProjectRepository;

        private Dictionary<string, string> _testData;

        [TestInitialize]
        public void SetupMocks()
        {
            TestHelper testHelper = new TestHelper();
            _testData = testHelper.GetTestData();

            //stubs for DI objects
            _mockAuthService = new Mock<IAuthService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockProjectRepository = new Mock<IProjectRepository>();

            Dictionary<string, string> authCookies = new Dictionary<string, string>();
            authCookies.Add(".auth", _testData["auth"]);
            authCookies.Add("ASP.NET_SessionId", _testData["sessionId"]);

            //stub for IConfiguration object that should return extranet domain
            _mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");

            //stub for IAuthService getAuthCredentials method that should return valid auth cookies
            _mockAuthService.Setup(m => m.getAuthCredentials(It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(authCookies));

            _extranetUsersRepository = new ExtranetUsersRepository(_mockAuthService.Object, _mockConfiguration.Object, _mockProjectRepository.Object);

        }

        [TestMethod]
        public void GetAll_ValidTokenWithoutFilters_ReturnsAllUsers()
        {
            //Arrange
            
            //Act
            var users = (List<User>)_extranetUsersRepository.GetAll("").Result;
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
            string name = _testData["userName"];

            //Act
            var users = (List<User>)_extranetUsersRepository.GetAll("", name).Result;

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
        public void GetAllByProject_ValidTokenAndProjectIdWithoutFilters_ReturnsAllUsersFromProject()
        {
            //Arrange
            Project project = new Project();
            project.Alias = _testData["projectAlias"];
            //stub for IProjectService GetById method which returns project
            _mockProjectRepository.Setup(m => m.GetById(It.IsAny<string>(), It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(project));

            //Act
            List<User> users = (List<User>)_extranetUsersRepository.GetAllByProject("", "").Result;
            List<string> check = new List<string>();
            foreach(var u in users)
            {
                check.Add(u.Id);
                check.Add(u.Name);
            }

            //Assert
            CollectionAssert.AllItemsAreNotNull(check);
        }

        [TestMethod]
        public void GetById_ValidTokenAndId_ReturnsUser()
        {
            //Arrange
            string id = _testData["userId"];

            //Act
            var user = _extranetUsersRepository.GetById(id, "").Result;
            bool check = user != null && user.Id.Equals(id);

            //Assert
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void GetById_ValidTokenInvalidId_ReturnsNull()
        {
            //Arrange
            string id = "invalid";

            //Act
            var user = _extranetUsersRepository.GetById(id, "").Result;

            //Assert
            Assert.IsNull(user);
        }
    }
}
