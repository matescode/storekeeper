using System;

namespace CommonBase.Application
{
    public interface IApplicationInfo
    {
        string Name { get; }

        string LongName { get; }

        string Description { get; }

        Version Version { get; }

        string Company { get; }

        string Web { get; }
    }
}