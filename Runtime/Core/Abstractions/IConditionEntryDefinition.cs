namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionEntryDefinition
    {
        IConditionDefinition Condition { get; }
        ConditionUsage Usage { get; }
    }
}
