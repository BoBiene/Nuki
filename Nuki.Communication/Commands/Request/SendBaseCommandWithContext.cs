using Nuki.Communication.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public abstract class SendBaseCommandWithContext : SendBaseCommand
    {
        public IConnectionContext ConnectionContext { get; private set; }
        public SendBaseCommandWithContext(CommandTypes type, IConnectionContext connectionContext, int nNumberOfFields)
            : base(type, nNumberOfFields)
        {
            ConnectionContext = connectionContext;
        }
    }
}
