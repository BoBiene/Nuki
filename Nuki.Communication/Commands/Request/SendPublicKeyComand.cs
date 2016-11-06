using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public class SendPublicKeyComand : SendBaseCommand
    {
        public byte[] PublicKey { get { return GetData<byte[]>(nameof(PublicKey)); } }
        /// <summary>
        /// Public Key (0x0003) 
        /// </summary>
        /// <param name="byPublicKey">The public key of the sender. </param>
        public SendPublicKeyComand(byte[] byPublicKey)
            : base(CommandTypes.PublicKey, 1)
        {
            AddField(nameof(PublicKey), byPublicKey);
        }
    }
}
