using System.Collections.Generic;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface ICompositeConditionDefinition : IConditionDefinition
    {
        IReadOnlyList<IConditionEntryDefinition> Entries { get; }
        IConditionAggregationDefinition Aggregation { get; }
    }
}