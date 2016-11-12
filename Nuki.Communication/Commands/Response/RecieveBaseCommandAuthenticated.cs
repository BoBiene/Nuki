using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{
    public abstract class RecieveBaseCommandAuthenticated : RecieveBaseCommand
    {
        public MessageAuthentication Authentication { get { return GetData<MessageAuthentication>(nameof(Authentication)); } }
        protected RecieveBaseCommandAuthenticated(CommandTypes type,IEnumerable<FieldParserBase> fields)
            : base(type, InitFields(fields))
        {

        }

        private static IEnumerable<FieldParserBase> InitFields(IEnumerable<FieldParserBase> fields)
        {
            yield return new FieldParser<MessageAuthentication>(nameof(Authentication), 32, SeperateSemanticType((b) => new MessageAuthentication(b)));
            foreach (var field in fields)
                yield return field;
        }

        public bool IsValid(IConnectionContext connectionContext, ClientNonce nonce)
        {
            if (Complete)
            {
                byte[] byToSign = new byte[Data.Length
                                                        - 2 //Remove CRC
                                                        - 2]; //Remove Command Ident


                Array.Copy(Data, 34, byToSign, 0, byToSign.Length - 32); //Remove Authentication,CommandIdent and CRC

                Array.Copy(nonce.Value, 0, byToSign, byToSign.Length - 32, nonce.Value.Length);

                MessageAuthentication calculatedAuth = new MessageAuthentication(
                    Sodium.SecretKeyAuth.SignHmacSha256(
                    byToSign, connectionContext.SharedKey));

                return base.IsValid() && Authentication == calculatedAuth;
            }
            else
            {
                return false;
            }
        }
    }
}
