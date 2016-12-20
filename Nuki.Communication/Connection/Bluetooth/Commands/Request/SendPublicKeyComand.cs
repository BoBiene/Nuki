using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public class SendPublicKeyComand : SendBaseCommand
    {
        public ClientPublicKey PublicKey { get { return GetData<ClientPublicKey>(nameof(PublicKey)); } }
        /// <summary>
        /// Public Key (0x0003) 
        /// </summary>
        /// <param name="byPublicKey">The public key of the sender. </param>
        public SendPublicKeyComand(ClientPublicKey byPublicKey)
            : base(CommandTypes.PublicKey, 1)
        {
            AddField(nameof(PublicKey), byPublicKey);
        }
    }
}
