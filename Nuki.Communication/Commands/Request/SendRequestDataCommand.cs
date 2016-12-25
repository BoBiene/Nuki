﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public class SendRequestDataCommand : SendBaseCommand
    {
        public CommandTypes ReqestedCommand {  get { return GetData<CommandTypes>(nameof(ReqestedCommand)); } }

        /// <summary>
        /// Request Data (0x0001) 
        /// </summary>
        /// <param name="requestedCommand">The identifier of the command to be executed by the NukiSmartlock.</param>
        public SendRequestDataCommand(CommandTypes requestedCommand)
            : base(CommandTypes.RequestData,1)
        {
            AddField(nameof(ReqestedCommand), requestedCommand);
        }
    }
}