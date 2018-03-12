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

        [TestInitialize]
        public void SetupMocks()
        {
            //stubs for DI objects
            mockAuthService = new Mock<IAuthService>();
            mockConfiguration = new Mock<IConfiguration>();

            authCookies = new Dictionary<string, string>();
            //Input valid auth cookie here
            authCookies.Add(".auth", "4D6EB947E818880113FD9065F047680084EFDB66FE09DFDEECB8347A91F33B5548DAC793569937855ACDBFFC14E1AFB8C2BBD509D080231896C3D4922B907CCA0C633FF43669EB75922F6CCE85DF9A3925EE7CFA8826D0632C914A750D291C2F19D2E3DA0D7DA55973CBBAA3E708AA2F7FD051CBEFA351DB8B6BB346E74F2099EE5263407B50F4A685E606FCEC1F10DAD4EAA4A8");
            //Input valid sessionId cookie here
            authCookies.Add("ASP.NET_SessionId", "zcb1fcvtd1rv01b2lpeh3ny5"); //
            //stub for Iconfiguration object that should return extranet domain
            mockConfiguration.Setup(m => m["ExtranetDomain"]).Returns("https://extranet.newtonideas.com/");
            //stub for IAuthService getAuthCredentials method that should return valid auth cookies
            mockAuthService.Setup(m => m.getAuthCredentials(It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(authCookies));
        }

        [TestMethod]
        public void GetAll_ValidTokenAndProjectId_ReturnsAllTimelogs()
        {
            //Arrange
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            //input project id here
            string projectId = "295070f3-8246-492a-bf1f-a2f39b319578";

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
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            //input invalid project id here
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
            mockAuthService.Setup(m => m.getAuthCredentials(It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(authCookies));
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            //input project id here
            string projectId = "295070f3-8246-492a-bf1f-a2f39b319578";
            //input timelog id here
            string id = "8415db4a-8a0a-488b-bd86-62c427b57922";

            //Act
            Timelog timelog = extranetTimelogsRepository.GetById(id, "", projectId).Result;
            bool check = false;
            if(timelog != null && timelog.Id.Equals(id))
            {
                check = true;
            }

            //Assert
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void GetById_InvalidIdAndValidTokenAndProjectId_ReturnsNull()
        {
            //Arrange
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            //input project id here
            string projectId = "295070f3-8246-492a-bf1f-a2f39b319578";
            //input invalid timelog id here
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
            
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            //input invalid project id here
            string projectId = "invalid project id";
            //input timelog id here
            string id = "8415db4a-8a0a-488b-bd86-62c427b57922";

            //Act
            Timelog timelog = extranetTimelogsRepository.GetById(id, "", projectId).Result;

            //Assert
            Assert.IsNull(timelog);
        }

        [TestMethod]
        public void Create_ValidTokenTimelogAndProjectId_ReturnsCreatedTimelog()
        {
            //Arrange
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            Timelog newTimelog = new Timelog();
            //input timelog data here
            newTimelog.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            newTimelog.Task_id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            newTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            newTimelog.Title = "unit test";
            DateTime start = new DateTime(2018,3,12,7,0,0,0);
            DateTime finish = new DateTime(2018,3,12,8,0,0,0);
            newTimelog.Start_on = start;
            newTimelog.Finish_on = finish;

            //Act
            Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;

            //Assert
            Assert.IsNotNull(newTimelog.Id);
        }

        [ExpectedException(typeof(AggregateException))]
        [TestMethod]
        public void Create_ValidTokenAndTimelogInvalidProjectId_ReturnsCreatedTimelog()
        {
            //Arrange
            ExtranetTimelogsRepository extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);
            //input timelog data here
            Timelog newTimelog = new Timelog();
            //input invalid project id
            newTimelog.Project_id = "invalid";
            newTimelog.Task_id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            newTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            newTimelog.Title = "unit test";
            DateTime start = new DateTime(2018, 3, 12, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 12, 8, 0, 0, 0);
            newTimelog.Start_on = start;
            newTimelog.Finish_on = finish;

            //Act
            Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;

        }
    }
}
