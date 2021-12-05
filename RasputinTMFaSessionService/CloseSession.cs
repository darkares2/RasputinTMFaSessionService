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
using RasputinTMFaSessionService;

namespace Rasputin.TM
{
    public static class CloseSession
    {
        [FunctionName("CloseSession")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
                                                    [Table("tblSessions")] CloudTable tblSession,
                                                    ILogger log)
        {
            log.LogInformation("CloseSession called");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SessionCloseRequest data = (SessionCloseRequest)JsonConvert.DeserializeObject(requestBody, typeof(SessionCloseRequest));
            Session appointment = await SessionService.CloseSession(log, tblSession, data.SessionID);

            return new OkObjectResult(JsonConvert.SerializeObject(appointment));
        }
    }
}
