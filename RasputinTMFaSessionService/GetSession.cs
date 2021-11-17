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

namespace RasputinTMFaSessionService
{
    public static class GetSession
    {
        [FunctionName("GetSession")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("tblSessions")] CloudTable tblSession,
            ILogger log)
        {
            log.LogInformation("GetSession called.");

            string responseMessage = null;
            string userIDString = req.Query["UserID"];
            if (userIDString != null && !userIDString.Equals(""))
            {
                Session[] sessions = await SessionService.FindUserSessions(log, tblSession, Guid.Parse(userIDString), true);
                responseMessage = JsonConvert.SerializeObject(sessions);

            }

            return new OkObjectResult(responseMessage);
        }
    }
}
