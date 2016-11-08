
using Nuki.Communication.Commands;
using System;



namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiClientTypeID : byte
    {
        App = 0,
        Bridge = 1,
        Fob = 2
    }
}