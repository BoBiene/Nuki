using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public class SendRequestCalibrationCommand : SendBaseCommandWithContext
    {
        public SmartLockNonce ChallengeNonce { get { return GetData<SmartLockNonce>(nameof(ChallengeNonce)); } }
        public UInt16 SecurityPIN {  get { return GetData<UInt16>(nameof(SecurityPIN)); } }

        /// <summary>
        /// Public Key (0x0003) 
        /// </summary>
        /// <param name="byPublicKey">The public key of the sender. </param>
        public SendRequestCalibrationCommand(IConnectionContext context, UInt16 securityPin)
            : base(CommandTypes.RequestCalibration,context, 2)
        {
            AddField(nameof(ChallengeNonce), context.SmartLockNonce);
            AddField(nameof(SecurityPIN), securityPin);
        }

    }
}
