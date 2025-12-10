using System.Collections.Generic;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionReactionDefinition : IConditionDefinition
    {
        IReadOnlyList<IConditionActionDefinition> Actions { get; }
    }
}
