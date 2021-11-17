using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using RasputinTMFaSessionSession.models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task<Session[]> FindUserSessions(ILogger log, CloudTable tblSession, Guid userID, bool open)
        {
            log.LogInformation($"FindUserSessions by user {userID}");
            List<Session> result = new List<Session>();
            TableQuery<Session> query = new TableQuery<Session>().Where(TableQuery.GenerateFilterConditionForGuid("UserID", QueryComparisons.Equal, userID));
            TableContinuationToken continuationToken = null;
            try
            {
                do
                {
                    var page = await tblSession.ExecuteQuerySegmentedAsync(query, continuationToken);
                    continuationToken = page.ContinuationToken;
                    result.AddRange(page.Results);
                } while (continuationToken != null);
                return result.Where(x => x.Open == open).ToArray();
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "FindUserSessions");
                return null;
            }
        }
    }
}
