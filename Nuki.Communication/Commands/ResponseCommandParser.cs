using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Commands.Response;
using System.Diagnostics;

namespace Nuki.Communication.Commands
{
    public class ResponseCommandParser
    {

        public RecieveBaseCommand Parse(byte[] data)
        {
            RecieveBaseCommand cmd = null;
            CommandTypes cmdType = GetCommandType(data);
            switch (cmdType)
            {
                case CommandTypes.PublicKey:
                    cmd = new RecievePublicKeyCommand(data);
                    break;
                default:
                    Debug.WriteLine($"Command {cmdType} is not handelt!");
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
