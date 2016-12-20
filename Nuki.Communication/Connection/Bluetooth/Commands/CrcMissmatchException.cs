using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands
{
    public class CrcMissmatchException : Exception
    {
        public CrcMissmatchException() { }
        public CrcMissmatchException(string message) : base(message) { }
        public CrcMissmatchException(string message, Exception inner) : base(message, inner) { }
    }
}
