using System;
using System.Collections.Generic;
using System.Text;

namespace RasputinTMFaSessionService.models
{
    public class CreateSessionRequest
    {
        public Guid UserID { get; set; }
        public Guid ConsultantID { get; set; }
    }
}
