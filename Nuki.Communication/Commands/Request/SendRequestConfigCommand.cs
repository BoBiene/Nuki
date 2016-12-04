using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public class SendRequestConfigCommand : SendBaseCommandWithContext
    {
        public SmartLockNonce ChallengeNonce { get { return GetData<SmartLockNonce>(nameof(ChallengeNonce)); } }

        /// <summary>
        /// Public Key (0x0003) 
        /// </summary>
        /// <param name="byPublicKey">The public key of the sender. </param>
        public SendRequestConfigCommand(IConnectionContext context)
            : base(CommandTypes.RequestConfig,context, 3)
        {
            AddField(nameof(ChallengeNonce), context.SmartLockNonce);
        }

    }
}
