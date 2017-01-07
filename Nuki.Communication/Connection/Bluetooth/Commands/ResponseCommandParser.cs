using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using System.Diagnostics;
using Windows.Storage.Streams;
using MetroLog;
using Nuki.Communication.API;

namespace Nuki.Communication.Connection.Bluetooth.Commands
{
    public static class ResponseCommandParser
    {
        private static ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger(nameof(ResponseCommandParser));
        public static RecieveBaseCommand Parse(IDataReader reader)
        {
            RecieveBaseCommand cmd = null;
            NukiCommandType cmdType =(NukiCommandType) reader.ReadUInt16();
            switch (cmdType)
            {
                case NukiCommandType.AuthorizationID:
                    cmd = new RecieveAuthorizationIDCommand();
                    break;
                case NukiCommandType.Challenge:
                    cmd = new RecieveChallengeCommand();
                    break;
                case NukiCommandType.ErrorReport:
                    cmd = new RecieveErrorReportCommand();
                    break;
                case NukiCommandType.PublicKey:
                    cmd = new RecievePublicKeyCommand();
                    break;
                case NukiCommandType.Status:
                    cmd = new RecieveStatusCommand();
                    break;
                case NukiCommandType.NukiStates:
                    cmd = new RecieveNukiStatesCommand();
                    break;
                case NukiCommandType.Config:
                    cmd = new RecieveConfigCommand();
                    break;
                case NukiCommandType.BatteryReport:
                    cmd = new RecieveBatteryReportCommand();
                    break;
                default:
                    Log.Error($"Command {cmdType} is not handelt!");
                    break;
            }

            return cmd;
        }

        private static NukiCommandType GetCommandType(byte[] data)
        {
           return  (NukiCommandType)BitConverter.ToUInt16(data, 0);
        }
    }
}
