using Nuki.Communication.API;

namespace Nuki.Communication.API
{
    public interface INukiErrorMessage
    {
        NukiErrorCode ErrorCode { get; }
        NukiCommandType FailedCommand { get; }
    }
}