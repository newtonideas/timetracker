using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using proxy.AuthServices;
using proxy.Models;
using proxy.Services;
using System;
using System.Collections.Generic;

namespace proxy.UnitTests
{
    [TestClass]
    public class ExtranetTimelogsRepositoryTests
    {
        private Mock<IAuthService> mockAuthService;
        private Mock<IConfiguration> mockConfiguration;
        private Dictionary<string, string> authCookies;
        private ExtranetTimelogsRepository extranetTimelogsRepository;

        private Dictionary<string, string> _testData;


        [TestInitialize]
        public void SetupMocks()
        {
            TestHelper testHelper = new TestHelper();
            _testData = testHelper.GetTestData();

            //stubs for DI objects
            mockAuthService = new Mock<IAuthService>();
            mockConfiguration = new Mock<IConfiguration>();

            extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);

            authCookies = new Dictionary<string, string>();
            authCookies.Add(".auth", _testData["auth"]);
            authCookies.Add("ASP.NET_SessionId", _testData["sessionId"]);
            //stub for Iconfiguration object that should return extranet domain
            mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");
            //stub for IAuthService getAuthCredentials method that should return valid auth cookies
            mockAuthService.Setup(m => m.getAuthCredentials(It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(authCookies));
        }

        [TestMethod]
        public void GetAll_ValidTokenAndProjectId_ReturnsAllTimelogs()
        {
            //Arrange
            string projectId = _testData["projectId"];

            //Act
            List<Timelog> timelogs = (List<Timelog>)extranetTimelogsRepository.GetAll("", projectId).Result;
            var checkList = new List<string>();
            foreach (var t in timelogs)
            {
                checkList.Add(t.Id);
                if (t.Project_id.Equals(projectId))
                {
                    checkList.Add(t.Project_id);
                }
                else
                {
                    checkList.Add(null);
                }
                checkList.Add(t.Task_id);
                checkList.Add(t.Title);
                checkList.Add(t.User_id);
            }

            //Assert
            CollectionAssert.AllItemsAreNotNull(checkList);
        }

        [TestMethod]
        public void GetAll_ValidTokenAndInvalidProjectId_ReturnsEmptyList()
        {
            //Arrange
            string projectId = "invalid project id";

            //Act
            List<Timelog> timelogs = (List<Timelog>)extranetTimelogsRepository.GetAll("", projectId).Result;
            var count = timelogs.Count;

            //Assert
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void GetById_ValidIdAndTokenAndProjectId_ReturnsTimelog()
        {
            //Arrange
            string projectId = _testData["projectId"];
            string id = _testData["timelogId"];

            //Act
            Timelog timelog = extranetTimelogsRepository.GetById(id, "", projectId).Result;
            bool check = timelog != null && timelog.Id.Equals(id);

            //Assert
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void GetById_InvalidIdAndValidTokenAndProjectId_ReturnsNull()
        {
            //Arrange
            string projectId = _testData["projectId"];
            string id = "invalid timelog";

            //Act
            Timelog timelog = extranetTimelogsRepository.GetById(id, "", projectId).Result;

            //Assert
            Assert.IsNull(timelog);
        }

        [TestMethod]
        public void GetById_InvalidProjectIdAndValidIdAndToken_ReturnsNull()
        {
            //Arrange
            string projectId = "invalid project id";
            string id = _testData["timelogId"];

            //Act
            Timelog timelog = extranetTimelogsRepository.GetById(id, "", projectId).Result;

            //Assert
            Assert.IsNull(timelog);
        }

        [TestMethod]
        public void Create_ValidTokenTimelogAndProjectId_ReturnsCreatedTimelog()
        {
            //Arrange
            Timelog newTimelog = new Timelog();
            newTimelog.Project_id = _testData["projectId"];
            newTimelog.Task_id = _testData["taskId"];
            newTimelog.User_id = _testData["userId"];
            newTimelog.Title = "create unit test";
            DateTime start = new DateTime(2018,3,13,11,0,0,0);
            DateTime finish = new DateTime(2018,3,13,12,0,0,0);
            newTimelog.Start_on = start;
            newTimelog.Finish_on = finish;

            //Act
            Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;
            
            //Assert
            Assert.IsNotNull(res.Id);
        }

        [TestMethod]
        public void Create_ValidTokenAndTimelogInvalidProjectId_ThrowsException()
        {
            //Arrange
            Timelog newTimelog = new Timelog();
            newTimelog.Project_id = "invalid";
            newTimelog.Task_id = _testData["taskId"];
            newTimelog.User_id = _testData["userId"];
            newTimelog.Title = "unit test";
            DateTime start = new DateTime(2018, 3, 12, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 12, 8, 0, 0, 0);
            newTimelog.Start_on = start;
            newTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;
                Assert.IsTrue(false);
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid timelog input data");
            }
        }

        [TestMethod]
        public void Create_ValidTokenAndProjectIdInvalidTimelogData_ThrowsException()
        {
            //Arrange
            Timelog newTimelog = new Timelog();
            newTimelog.Project_id = _testData["projectId"];
            newTimelog.Task_id = _testData["taskId"];
            newTimelog.User_id = "invalid user id";
            newTimelog.Title = "unit test";
            DateTime start = new DateTime(2018, 03, 13, 07, 00, 00, 000);
            DateTime finish = new DateTime(2018, 03, 13, 08, 00, 00, 000);
            newTimelog.Start_on = start;
            newTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;
                Assert.IsTrue(false);
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid timelog input data");
            }
        }

        [TestMethod]
        public void Update_ValidTokenAndIdAndTimelogAndProjectId_ReturnsEditedTimelog()
        {
            //Arrange
            Timelog editedTimelog = new Timelog();
            editedTimelog.Id = _testData["timelogId"];
            editedTimelog.Project_id = _testData["projectId"];
            editedTimelog.Task_id = _testData["taskId"];
            editedTimelog.User_id = _testData["userId"];
            editedTimelog.Title = "unit test update";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;

            //Assert
            Assert.IsNotNull(res.Id);
        }

        [TestMethod]
        public void Update_ValidTokenAndTimelogAndProjectIdInvalidId_ThrowsException()
        {
            //Arrange
            Timelog editedTimelog = new Timelog(); 
            editedTimelog.Id = "invalid id";
            editedTimelog.Project_id = _testData["projectId"];
            editedTimelog.Task_id = _testData["taskId"];
            editedTimelog.User_id = _testData["userId"];
            editedTimelog.Title = "unit test update";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;
                Assert.IsTrue(false);
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Timelog not found");
            }
        }

        [TestMethod]
        public void Update_ValidTokenAndTimelogAndIdInvalidProjectId_ThrowsException()
        {
            //Arrange
            Timelog editedTimelog = new Timelog();
            editedTimelog.Project_id = "invalid";
            editedTimelog.Id = _testData["timelogId"];
            editedTimelog.Task_id = _testData["taskId"];
            editedTimelog.User_id = _testData["userId"];
            editedTimelog.Title = "unit test update";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;
                Assert.IsTrue(false);
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid timelog input data");
            }
        }

        [TestMethod]
        public void Update_ValidTokenAndIdAndProjectIdInvalidTimelogData_ThrowsException()
        {
            //Arrange
            Timelog editedTimelog = new Timelog();
            editedTimelog.Id = _testData["timelogId"];
            editedTimelog.Project_id = _testData["projectId"];
            editedTimelog.Task_id = _testData["taskId"];
            editedTimelog.User_id = "invalid";
            editedTimelog.Title = "unit test update v3";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;
                Assert.IsTrue(false);
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid timelog input data");
            }
        }

        [TestMethod]
        public void Delete_ValidTokenAndId_ReturnsTrue()
        {
            //Arrange
            string id = _testData["deleteTimelogId"];

            //Act
            bool res = extranetTimelogsRepository.Delete("", id).Result;

            //Assert
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void Delete_ValidTokenInvalidId_ThrowsException()
        {
            //Arrange
            string id = "invalid";

            //Act
            try
            {
                bool res = extranetTimelogsRepository.Delete("", id).Result;
                Assert.IsTrue(false);
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Timelog not found");
            }
        }
    }
}
