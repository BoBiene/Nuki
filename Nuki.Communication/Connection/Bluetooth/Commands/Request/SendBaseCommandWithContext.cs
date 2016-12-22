using Nuki.Communication.API;
using Nuki.Communication.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public abstract class SendBaseCommandWithContext : SendBaseCommand
    {
        public IConnectionContext ConnectionContext { get; private set; }
        public SendBaseCommandWithContext(NukiCommandType type, IConnectionContext connectionContext, int nNumberOfFields)
            : base(type, nNumberOfFields)
        {
            ConnectionContext = connectionContext;
        }
    }
}
