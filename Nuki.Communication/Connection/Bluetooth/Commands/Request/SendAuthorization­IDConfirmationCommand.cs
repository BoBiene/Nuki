﻿using Nuki.Communication.API;
using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public class SendAuthorization­IDConfirmationCommand : SendBaseCommandAuthenticated
    {
        public UniqueClientID UniqueClientID => GetData<UniqueClientID>(nameof(UniqueClientID));
        public SmartLockNonce SmartLockNonce => GetData<SmartLockNonce>(nameof(SmartLockNonce));

        public SendAuthorization­IDConfirmationCommand(UniqueClientID uuid, IConnectionContext context)
            : base(NukiCommandType.AuthorizationIDConfirmation,context,2)
        {
            AddField(nameof(SmartLockUUID), uuid);
            AddField(nameof(SmartLockNonce), context.SmartLockNonce,FieldFlags.PartOfAuthentication); 
        }
    }
}
