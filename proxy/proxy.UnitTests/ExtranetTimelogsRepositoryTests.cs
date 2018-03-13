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


        [TestInitialize]
        public void SetupMocks()
        {
            //stubs for DI objects
            mockAuthService = new Mock<IAuthService>();
            mockConfiguration = new Mock<IConfiguration>();

            extranetTimelogsRepository = new ExtranetTimelogsRepository(mockAuthService.Object, mockConfiguration.Object);

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
            Timelog newTimelog = new Timelog();
            //input timelog data here
            newTimelog.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            newTimelog.Task_id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            newTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            newTimelog.Title = "create";
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
            try
            {
                Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;
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
            //input some invalid timelog data here
            Timelog newTimelog = new Timelog();
            newTimelog.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            newTimelog.Task_id = "invalid task id";
            newTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            newTimelog.Title = "unit test";
            DateTime start = new DateTime(2018, 03, 13, 07, 00, 00, 000);
            DateTime finish = new DateTime(2018, 03, 13, 08, 00, 00, 000);
            newTimelog.Start_on = start;
            newTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Create("", newTimelog, newTimelog.Project_id).Result;
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
            //input timelog data to update here
            editedTimelog.Id = "f65f8c5f-9831-41aa-929d-35ae8ba668a6";
            //editedTimelog.Id = "96628a0e-594c-4ffd-af44-090318a6d03d";
            editedTimelog.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            editedTimelog.Task_id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            editedTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
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
            //input invalid id here
            editedTimelog.Id = "invalid id";
            //input timelog data to update here
            editedTimelog.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            editedTimelog.Task_id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            editedTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            editedTimelog.Title = "unit test update";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid timelog input data");
            }
        }

        [TestMethod]
        public void Update_ValidTokenAndTimelogAndIdInvalidProjectId_ThrowsException()
        {
            //Arrange
            Timelog editedTimelog = new Timelog();
            //input invalid project id here
            editedTimelog.Project_id = "invalid";
            //input timelog data to update here
            editedTimelog.Id = "6833cd15-c199-4a00-add3-f149a824ff9c";            
            editedTimelog.Task_id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            editedTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            editedTimelog.Title = "unit test update";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;
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
            //input some invalid timelog data here
            editedTimelog.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            editedTimelog.Id = "6833cd15-c199-4a00-add3-f149a824ff9c";
            editedTimelog.Task_id = "invalid";
            editedTimelog.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            editedTimelog.Title = "unit test update";
            DateTime start = new DateTime(2018, 3, 13, 7, 0, 0, 0);
            DateTime finish = new DateTime(2018, 3, 13, 10, 0, 0, 0);
            editedTimelog.Start_on = start;
            editedTimelog.Finish_on = finish;

            //Act
            try
            {
                Timelog res = extranetTimelogsRepository.Update("", editedTimelog.Id, editedTimelog, editedTimelog.Project_id).Result;
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
            //input timelog id to delete
            string id = "f65f8c5f-9831-41aa-929d-35ae8ba668a6";

            //Act
            bool res = extranetTimelogsRepository.Delete("", id).Result;

            //Assert
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void Delete_ValidTokenInvalidId_ThrowsException()
        {
            //Arrange
            //input invalid id here
            string id = "invalid";

            //Act
            try
            {
                bool res = extranetTimelogsRepository.Delete("", id).Result;
            }
            catch (AggregateException ae)
            {
                //Assert
                Assert.AreEqual(ae.InnerException.Message, "Invalid id");
            }
        }
    }
}
