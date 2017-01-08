namespace Nuki.Communication.API
{
    public interface INukiLogEntryCount
    {
        /// <summary>
        /// Total number of log entries 
        /// </summary>
        ushort Count { get; }
        /// <summary>
        /// This flag indicates whether or not logging is enabled.
        /// </summary>
        bool LoggingEnabled { get; }
    }
}