using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{
    public class RecieveBaseCommandEncrypted : RecieveBaseCommand
    {
        protected RecieveBaseCommandEncrypted(CommandTypes type, byte[] data, IEnumerable<FieldParserBase> fields)
            : base(type, data, fields)
        {

        }
    }
}
