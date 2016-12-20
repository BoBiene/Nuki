using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Connection;
using Nuki.Communication.API;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Commands.Request
{
    public class SendAuthorizationDataCommand : SendBaseCommandAuthenticated
    {

        public NukiClientTypeID ClientIDType { get { return GetData<NukiClientTypeID>(nameof(ClientIDType)); } }
        public UniqueClientID UniqueClientID { get { return GetData<UniqueClientID>(nameof(UniqueClientID)); } }
        public string ClientName { get { return GetData<string>(nameof(ClientName)); } }

        public SmartLockNonce SmartLockNonce { get { return GetData<SmartLockNonce>(nameof(SmartLockNonce)); } }
        public ClientNonce ClientNonce { get { return GetData<ClientNonce>(nameof(ClientNonce)); } }

        public SendAuthorizationDataCommand(string strName, IConnectionContext connectionContext) :
            base(CommandTypes.AuthorizationData, connectionContext, 5)
        {
            AddField(nameof(ClientIDType), NukiClientTypeID.App);
            AddField(nameof(UniqueClientID), connectionContext.UniqueClientID);
            AddField(nameof(ClientName), strName, 32, FieldFlags.All);
            AddField(nameof(ClientNonce), connectionContext.CreateNonce());
            AddField(nameof(SmartLockNonce), connectionContext.SmartLockNonce, FieldFlags.PartOfAuthentication);

        }


    }
}
