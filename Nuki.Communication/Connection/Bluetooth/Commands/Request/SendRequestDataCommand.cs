using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public class SendRequestDataCommand : SendBaseCommand
    {
        public NukiCommandType ReqestedCommand {  get { return GetData<NukiCommandType>(nameof(ReqestedCommand)); } }

        /// <summary>
        /// Request Data (0x0001) 
        /// </summary>
        /// <param name="requestedCommand">The identifier of the command to be executed by the NukiSmartlock.</param>
        public SendRequestDataCommand(NukiCommandType requestedCommand)
            : base(NukiCommandType.RequestData,1)
        {
            AddField(nameof(ReqestedCommand), requestedCommand);
        }
    }
}
