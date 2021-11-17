using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using RasputinTMFaSessionSession.models;
using System;
using System.Threading.Tasks;

namespace RasputinTMFaSessionService
{
    public static class SessionService
    {
        public static async Task<Session> InsertSession(ILogger log, CloudTable tblSession, Guid userID, Guid consultantID)
        {
            Session session = new Session(userID, consultantID);
            TableOperation operation = TableOperation.Insert(session);
            await tblSession.ExecuteAsync(operation);
            return session;
        }

    }
}
