using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.API;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class RecieveBatteryReportCommand : RecieveBaseCommand, INukiBatteryReport
    {
        public ushort BatteryDrain => GetData<ushort>(nameof(BatteryDrain));

        public ushort BatteryVoltage => GetData<ushort>(nameof(BatteryVoltage));

        public bool CriticalBatteryState => GetData<bool>(nameof(CriticalBatteryState));
        public RecieveBatteryReportCommand()
            : base(NukiCommandType.BatteryReport, CreateFields())
        {
        }

        private static IEnumerable<FieldParserBase> CreateFields()
        {
            yield return new FieldParser<ushort>(nameof(BatteryDrain), sizeof(ushort), (buffer, start, length) => BitConverter.ToUInt16(buffer,start));
            yield return new FieldParser<ushort>(nameof(BatteryVoltage), sizeof(ushort), (buffer, start, length) => BitConverter.ToUInt16(buffer, start));
            yield return new FieldParser<bool>(nameof(CriticalBatteryState),1, (buffer, start, length) => buffer[start] != 0);
        }

    }
}
