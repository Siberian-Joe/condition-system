namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionAggregationFactory
    {
        IConditionAggregation Create(IConditionAggregationDefinition definition);
    }
}
