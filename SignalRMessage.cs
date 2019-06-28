using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR.Azure.Serverless
{
    public class SignalRMessage
    {
        public string target { get; set; }
        public IEnumerable<string> arguments { get; set; }
    }
}
