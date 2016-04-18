using System;

namespace CommonBase.Log
{
    [Flags]
    public enum LogMode
    {
        None = 0x0,
        Console = 0x1,
        File = 0x2,
        SystemLog = 0x4,
        Database = 0x8,
        Control = 0x10,
        Application = 0x20
    }
}