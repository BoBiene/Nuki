using Nuki.Communication.API;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class   RecieveConfigCommand : RecieveBaseCommand
    {
        public UniqueClientID UniqueClientID { get { return GetData<UniqueClientID>(nameof(UniqueClientID)); } }
        public string Name { get { return GetData<string>(nameof(Name)); } }
        /// <summary>
        /// The latitude of the Nuki Smartlock’s geoposition
        /// </summary>
        public float Latitude { get { return GetData<float>(nameof(Latitude)); } }
        /// <summary>
        /// The longitude of the Nuki Smartlock’s geoposition.
        /// </summary>
        public float Longitude { get { return GetData<float>(nameof(Longitude)); } }
        /// <summary>
        /// This flag indicates whether or not the door shall 
        /// be unlatched bymanually operatinga doorhandle fromthe outside.
        /// </summary>
        public bool AutoUnlatch { get { return GetData<bool>(nameof(AutoUnlatch)); } }
        /// <summary>
        /// This flag indicates whether or not the pairing mode should beenabled.
        /// </summary>
        public bool PairingEnabled  { get { return GetData<bool>(nameof(PairingEnabled)); } }
        /// <summary>
        /// This flag indicates whether or not the button should be enabled.
        /// </summary>
        public bool ButtonEnabled  { get { return GetData<bool>(nameof(ButtonEnabled)); } }
        /// <summary>
        /// This flag indicates whether or not the LED should be enabled to signal an unlocked door.
        /// </summary>
        public bool LEDEnabled  { get { return GetData<bool>(nameof(LEDEnabled)); } }

        /// <summary>
        /// The LED brightness level. Possible values are 0 to 5 
        /// 0 = off, …, 5 = max
        /// </summary>
        public byte LEDBrightness { get { return GetData<byte>(nameof(LEDBrightness)); } }

        /// <summary>
        /// 
        /// </summary>
        public NukiTimeStamp CurrentTime { get { return GetData<NukiTimeStamp>(nameof(CurrentTime)); } }
        public Int16 UTCOffset { get { return GetData<Int16>(nameof(UTCOffset)); } }
        public NukiDSTSetting DSTMode { get { return GetData<NukiDSTSetting>(nameof(DSTMode)); } }
        /// <summary>
        /// This flag indicates whether or not a Nuki Fob hasbeen pairedto thisNuki.
        /// </summary>
        public bool HasFob  { get { return GetData<bool>(nameof(HasFob)); } }
        /// <summary>
        /// The desired action, if a Nuki Fob is pressed once.
        /// </summary>
        public NukiFobAction FobAction1 { get { return GetData<NukiFobAction>(nameof(FobAction1)); } }
        /// <summary>
        /// The desired action, if a Nuki Fob is pressed twice.
        /// </summary>
        public NukiFobAction FobAction2 { get { return GetData<NukiFobAction>(nameof(FobAction2)); } }
        /// <summary>
        /// The desired action, if a Nuki Fob is pressed three times.
        /// </summary>
        public NukiFobAction FobAction3 { get { return GetData<NukiFobAction>(nameof(FobAction3)); } }


        public RecieveConfigCommand()
            : base(NukiCommandType.Config, InitFields())
        {
        }

        protected static IEnumerable<FieldParserBase> InitFields()
        {
            yield return new FieldParser<UniqueClientID>(nameof(UniqueClientID), 4,
             (buffer, start, length) => new UniqueClientID(BitConverter.ToUInt32(buffer, start)));

            yield return new FieldParser<string>(nameof(Name), 32, (b, s, l) => Encoding.ASCII.GetString(b, s, l));
            yield return new FieldParser<float>(nameof(Latitude), 4, (b, s, l) => BitConverter.ToSingle(b,s));
            yield return new FieldParser<float>(nameof(Longitude), 4, (b, s, l) => BitConverter.ToSingle(b, s));
            yield return new FieldParser<bool>(nameof(AutoUnlatch), 1, (b, s, l) => b[s] != 0);
            yield return new FieldParser<bool>(nameof(PairingEnabled), 1, (b, s, l) => b[s] != 0);
            yield return new FieldParser<bool>(nameof(ButtonEnabled), 1, (b, s, l) => b[s] != 0);
            yield return new FieldParser<bool>(nameof(LEDEnabled), 1, (b, s, l) => b[s] != 0);
            yield return new FieldParser<byte>(nameof(LEDBrightness), 1, (b, s, l) => b[s]);
            yield return new FieldParser<NukiTimeStamp>(nameof(CurrentTime), 7, (b, s, l) => NukiTimeStamp.FromBytes(b, s));
            yield return new FieldParser<Int16>(nameof(UTCOffset), 2, (b, s, l) => BitConverter.ToInt16(b, s));
            yield return new FieldParser<NukiDSTSetting>(nameof(DSTMode), 1, (b, s, l) => (NukiDSTSetting) b[s]);
            yield return new FieldParser<bool>(nameof(HasFob), 1, (b, s, l) => b[s] != 0);
            yield return new FieldParser<NukiFobAction>(nameof(FobAction1), 1, (b, s, l) => (NukiFobAction)b[s]);
            yield return new FieldParser<NukiFobAction>(nameof(FobAction2), 1, (b, s, l) => (NukiFobAction)b[s]);
            yield return new FieldParser<NukiFobAction>(nameof(FobAction3), 1, (b, s, l) => (NukiFobAction)b[s]);
        }
    }
}
