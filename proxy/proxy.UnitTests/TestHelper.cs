using Microsoft.Extensions.Configuration;
using proxy.AuthServices;
using proxy.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace proxy.UnitTests
{
    class TestHelper
    {

        private Dictionary<string, string> _testData;

        public TestHelper()
        {
            _testData = new Dictionary<string, string>();
            SetTestData();
        }

        public void SetTestData()
        {

            // .auth cookie
            string auth = "6EBEDFA7199C0F0FE9FAE1A366895321C2D885B6F2788CD3BBF559FCE9A2D82F20BB14465E6024148C9B1652FCA3BF344BCCF092DCB86232ABFC3D34F6D5AD42304DC0DF08F2D0074DAE0AFF8FF51B926A3CE2301141288AD026327ACCDF6BF9332AC89B5E113CAA7954A5592EE4374CDF59F5CA29CFDBEBFDE77B2FE342A2B057C614A347D26C8C50BFFCBDE25C342994CD669C";
            _testData.Add("auth", auth);
            // ASP.NET_SessionId cookie
            string sessionId = "apqmpj30shpian5geljh0c3v";
            _testData.Add("sessionId", sessionId);


            // ExtranetAuthServiceTests data
            // user login
            string login = "brunets1997";
            _testData.Add("login", login);
            //user password
            string password = "2512367";
            _testData.Add("password", password);


            // ExtranetUserRepositoryTests data
            // user name
            string userName = "Andrii";
            _testData.Add("userName", userName);
            // project alias
            string projectAlias = "TTI";
            _testData.Add("projectAlias", projectAlias);
            // user id
            string userId = "e981a503-1536-4a48-920d-6c464f596cbc";
            _testData.Add("userId", userId);


            //ExtranetTimelogsRepositoryTests data
            // project id
            string projectId = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            _testData.Add("projectId", projectId);
            // timelog id
            string timelogId = "8415db4a-8a0a-488b-bd86-62c427b57922";
            _testData.Add("timelogId", timelogId);
            //task id
            string taskId = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            _testData.Add("taskId", taskId);
            // timelog id to delete timelog
            string deleteTimelogId = "7134bf13-c3ca-402a-9c20-a134a49eadcc";
            _testData.Add("deleteTimelogId", deleteTimelogId);

        }

        public Dictionary<string,string> GetTestData()
        {
            return _testData;
        }
    }
}
