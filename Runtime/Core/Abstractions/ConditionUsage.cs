using System;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    [Flags]
    public enum ConditionUsage
    {
        None = 0,
        ForActivation = 1 << 0,
        ForDisplay = 1 << 1
    }
}
