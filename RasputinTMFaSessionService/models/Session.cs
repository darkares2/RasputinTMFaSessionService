using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace RasputinTMFaSessionSession.models
{
    public class Session : TableEntity
    {
        public Session(Guid userID, Guid consultantID)
        {
            this.PartitionKey = "p1";
            this.RowKey = Guid.NewGuid().ToString();
            this.UserID = userID;
            this.ConsultantID = consultantID;
            this.Open = true;
        }
        public Session() { }
        public bool? Open { get; set; }

        public Guid? UserID { get; set; }
        public Guid? ConsultantID { get; set; }

        public Guid SessionID { get { return Guid.Parse(RowKey); } }

        public static explicit operator Session(TableResult v)
        {
            DynamicTableEntity entity = (DynamicTableEntity)v.Result;
            Session SessionProfile = new Session();
            SessionProfile.PartitionKey = entity.PartitionKey;
            SessionProfile.RowKey = entity.RowKey;
            SessionProfile.Timestamp = entity.Timestamp;
            SessionProfile.ETag = entity.ETag;
            SessionProfile.Open = entity.Properties["Open"].BooleanValue;
            SessionProfile.UserID = entity.Properties["UserID"].GuidValue;
            SessionProfile.ConsultantID = entity.Properties["ConsultantID"].GuidValue;

            return SessionProfile;
        }
    }
}
