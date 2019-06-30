using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR.Azure.Serverless
{
    // because arguments is a an array of objects, json.net may have issues deserializing the array as type info is unknown
    // keep this in mind, especially if you add int64s, etc.
    public class SignalRMessage
    {
        public string target { get; set; }
        public IEnumerable<object> arguments { get; set; }
    }
}
