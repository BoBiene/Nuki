using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Nuki.Communication.Commands.Response;

namespace Nuki.Communication.Commands
{
    public class ResponseCommandParser
    {

        public RecieveBaseCommand Parse(byte[] data)
        {
            RecieveBaseCommand cmd = null;
            switch (GetCommandType(data))
            {
                case CommandTypes.PublicKey:
                    cmd = new RecievePublicKeyCommand(data);
                    break;
            }

            return cmd;
        }

        private static CommandTypes GetCommandType(byte[] data)
        {
           return  (CommandTypes)BitConverter.ToUInt16(data, 0);
        }
    }
}
