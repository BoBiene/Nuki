using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands
{

    public class MessageParseException : Exception
    {
        public MessageParseException() { }
        public MessageParseException(string message) : base(message) { }
        public MessageParseException(string message, Exception inner) : base(message, inner) { }
    }
}
