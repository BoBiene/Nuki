using Nuki.Communication.API;

namespace Nuki.Communication.API
{
    public interface INukiDeviceStateMessage
    {
        bool CriticalBattery { get; }
        NukiTimeStamp CurrentTime { get; }
        NukiLockState LockState { get; }
        NukiState NukiState { get; }
        NukiLockStateChangeTrigger Trigger { get; }
        short UTCOffset { get; }
    }
}