using System;

namespace Discans.Enum
{
    [Flags]
    public enum UpdateStatus : short
    {
        Success = 0b01,
        Error =    0b10
    }
}
