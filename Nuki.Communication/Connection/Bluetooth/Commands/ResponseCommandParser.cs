using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Commands.Response;
using System.Diagnostics;
using Windows.Storage.Streams;
using MetroLog;

namespace Nuki.Communication.Commands
{
    public static class ResponseCommandParser
    {
        private static ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger(nameof(ResponseCommandParser));
        public static RecieveBaseCommand Parse(IDataReader reader)
        {
            RecieveBaseCommand cmd = null;
            CommandTypes cmdType =(CommandTypes) reader.ReadUInt16();
            switch (cmdType)
            {
                case CommandTypes.AuthorizationID:
                    cmd = new RecieveAuthorizationIDCommand();
                    break;
                case CommandTypes.Challenge:
                    cmd = new RecieveChallengeCommand();
                    break;
                case CommandTypes.ErrorReport:
                    cmd = new RecieveErrorReportCommand();
                    break;
                case CommandTypes.PublicKey:
                    cmd = new RecievePublicKeyCommand();
                    break;
                case CommandTypes.Status:
                    cmd = new RecieveStatusCommand();
                    break;
                case CommandTypes.NukiStates:
                    cmd = new RecieveNukiStatesCommand();
                    break;
                case CommandTypes.Config:
                    cmd = new RecieveConfigCommand();
                    break;
                default:
                    Log.Error($"Command {cmdType} is not handelt!");
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
