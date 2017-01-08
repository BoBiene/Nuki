using Nuki.Communication.API;

namespace Nuki.Communication.API
{
    public interface INukiReturnMessage
    {
        NukiErrorCode StatusCode { get; }
    }
}