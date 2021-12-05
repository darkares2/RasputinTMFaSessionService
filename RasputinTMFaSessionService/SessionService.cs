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

        public static async Task<Session> CloseSession(ILogger log, CloudTable tblSession, Guid sessionID)
        {
            Session session = await FindSession(log, tblSession, sessionID);
            session.Open = false;
            TableOperation operation = TableOperation.Replace(session);
            await tblSession.ExecuteAsync(operation);
            return session;
        }

        public static async Task<Session> FindSession(ILogger log, CloudTable tblSession, Guid SessionID)
        {
            string pk = "p1";
            string rk = SessionID.ToString();
            log.LogInformation($"FindSession: {pk},{rk}");
            TableOperation operation = TableOperation.Retrieve(pk, rk);
            try
            {
                var tableResult = await tblSession.ExecuteAsync(operation);
                return tableResult.Result as Session != null ? tableResult.Result as Session : (Session)tableResult;
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "FindSession", SessionID);
                return null;
            }
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
