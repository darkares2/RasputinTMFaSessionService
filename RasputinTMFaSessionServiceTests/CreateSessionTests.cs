using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RasputinTMFaSessionService;
using RasputinTMFaSessionService.models;
using RasputinTMFaSessionSession.models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RasputinTMFaSessionServiceTests
{
    public class CreateSessionTests
    {

        private Stream Serialize(object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            return new MemoryStream(Encoding.Default.GetBytes(jsonString));
        }

        [Fact]
        public async Task CreateSessionNew()
        {
            //Arrange
            CreateSessionRequest createSessionRequest = new CreateSessionRequest() { UserID = Guid.NewGuid(), ConsultantID = Guid.NewGuid() };
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = Serialize(createSessionRequest);
            var iLoggerMock = new Mock<ILogger>();
            var tblSessionMock = new Mock<CloudTable>(new Uri("https://fake.com"), null);
            TableOperation operation = null;
            tblSessionMock.Setup(_ => _.ExecuteAsync(It.IsAny<TableOperation>()))
                    .Callback<TableOperation>((obj) => operation = obj);

            // Act
            OkObjectResult result = (OkObjectResult)await CreateSession.Run(request, tblSessionMock.Object , iLoggerMock.Object);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Session sessionResult = (Session)JsonConvert.DeserializeObject((string)result.Value, typeof(Session));
            Assert.Equal(createSessionRequest.UserID, sessionResult.UserID);
            Assert.True(sessionResult.Open);
            tblSessionMock.Verify(_ => _.ExecuteAsync(It.IsAny<TableOperation>()), Times.Exactly(1));
            Assert.NotNull(operation.Entity);
            Assert.Equal(createSessionRequest.UserID, ((Session)operation.Entity).UserID);
            Assert.Equal(createSessionRequest.ConsultantID, ((Session)operation.Entity).ConsultantID);
        }
    }
}
