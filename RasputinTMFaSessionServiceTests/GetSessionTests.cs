using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using RasputinTMFaSessionService;
using RasputinTMFaSessionService.models;
using RasputinTMFaSessionSession.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RasputinTMFaSessionServiceTests
{
    public class GetSessionTests
    {

        private Stream Serialize(object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            return new MemoryStream(Encoding.Default.GetBytes(jsonString));
        }

        [Fact]
        public async Task GetSessionByUserID()
        {
            //Arrange
            var context = new DefaultHttpContext();
            var request = context.Request;
            Guid userID = Guid.NewGuid();
            var qs = new Dictionary<string, StringValues>
            {
                { "UserID", userID.ToString() }
            };
            request.Query = new QueryCollection(qs);
            var iLoggerMock = new Mock<ILogger>();
            var tblSessionMock = new Mock<CloudTable>(new Uri("https://fake.com"), null);
            Session session1 = new Session() { RowKey = Guid.NewGuid().ToString(), UserID = Guid.NewGuid(), ConsultantID = Guid.NewGuid(), Open = true };
            Session session2 = new Session() { RowKey = Guid.NewGuid().ToString(), UserID = Guid.NewGuid(), ConsultantID = Guid.NewGuid(), Open = true };
            List<Session> sessions = new List<Session>() { session1, session2 };
            var resultMock = new Mock<TableQuerySegment<Session>>(sessions);
            tblSessionMock.Setup(_ => _.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<Session>>(), It.IsAny<TableContinuationToken>())).ReturnsAsync(resultMock.Object);

            // Act
            OkObjectResult result = (OkObjectResult)await GetSession.Run(request, tblSessionMock.Object , iLoggerMock.Object);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Session[] sessionResult = (Session[])JsonConvert.DeserializeObject((string)result.Value, typeof(Session[]));
            Assert.Equal(2, sessionResult.Length);
            Assert.Equal(session1.UserID, sessionResult[0].UserID);
            Assert.Equal(session1.ConsultantID, sessionResult[0].ConsultantID);
            Assert.Equal(session2.UserID, sessionResult[1].UserID);
            Assert.Equal(session2.ConsultantID, sessionResult[1].ConsultantID);
        }

        [Fact]
        public async Task GetSessionByUserIDNotClosed()
        {
            //Arrange
            var context = new DefaultHttpContext();
            var request = context.Request;
            Guid userID = Guid.NewGuid();
            var qs = new Dictionary<string, StringValues>
            {
                { "UserID", userID.ToString() }
            };
            request.Query = new QueryCollection(qs);
            var iLoggerMock = new Mock<ILogger>();
            var tblSessionMock = new Mock<CloudTable>(new Uri("https://fake.com"), null);
            Session session1 = new Session() { RowKey = Guid.NewGuid().ToString(), UserID = Guid.NewGuid(), ConsultantID = Guid.NewGuid(), Open = true };
            Session session2 = new Session() { RowKey = Guid.NewGuid().ToString(), UserID = Guid.NewGuid(), ConsultantID = Guid.NewGuid(), Open = true };
            Session session3 = new Session() { RowKey = Guid.NewGuid().ToString(), UserID = Guid.NewGuid(), ConsultantID = Guid.NewGuid(), Open = false };
            List<Session> sessions = new List<Session>() { session1, session2 };
            var resultMock = new Mock<TableQuerySegment<Session>>(sessions);
            tblSessionMock.Setup(_ => _.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<Session>>(), It.IsAny<TableContinuationToken>())).ReturnsAsync(resultMock.Object);

            // Act
            OkObjectResult result = (OkObjectResult)await GetSession.Run(request, tblSessionMock.Object, iLoggerMock.Object);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Session[] sessionResult = (Session[])JsonConvert.DeserializeObject((string)result.Value, typeof(Session[]));
            Assert.Equal(2, sessionResult.Length);
            Assert.Equal(session1.UserID, sessionResult[0].UserID);
            Assert.Equal(session1.ConsultantID, sessionResult[0].ConsultantID);
            Assert.Equal(session2.UserID, sessionResult[1].UserID);
            Assert.Equal(session2.ConsultantID, sessionResult[1].ConsultantID);
        }
    }
}
