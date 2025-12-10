using System.Collections.Generic;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionAggregation
    {
        bool Aggregate(IEnumerable<bool> states);
    }
}
