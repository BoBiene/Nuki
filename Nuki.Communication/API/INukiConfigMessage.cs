
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.API
{
    public interface INukiConfigMessage
    {
        bool AutoUnlatch { get; }
        bool ButtonEnabled { get; }
        NukiTimeStamp CurrentTime { get; }
        NukiDSTSetting DSTMode { get; }
        NukiFobAction FobAction1 { get; }
        NukiFobAction FobAction2 { get; }
        NukiFobAction FobAction3 { get; }
        bool HasFob { get; }
        float Latitude { get; }
        byte LEDBrightness { get; }
        bool LEDEnabled { get; }
        float Longitude { get; }
        string Name { get; }
        bool PairingEnabled { get; }
        UniqueClientID UniqueClientID { get; }
        short UTCOffset { get; }
    }
}