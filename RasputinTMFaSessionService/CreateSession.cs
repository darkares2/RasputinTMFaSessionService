using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using RasputinTMFaSessionSession.models;
using RasputinTMFaSessionService.models;

namespace RasputinTMFaSessionService
{
    public static class CreateSession
    {
        [FunctionName("CreateSession")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("tblSessions")] CloudTable tblSession,
            ILogger log)
        {
            log.LogInformation("CreateSession called.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateSessionRequest data = (CreateSessionRequest)JsonConvert.DeserializeObject(requestBody, typeof(CreateSessionRequest));

            Session session = await SessionService.InsertSession(log, tblSession, data.UserID, data.ConsultantID);

            string responseMessage = JsonConvert.SerializeObject(session);
            return new OkObjectResult(responseMessage);
        }
    }
}
