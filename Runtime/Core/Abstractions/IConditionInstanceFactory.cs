using System;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionInstanceFactory
    {
        IConditionHandler<TContext> CreateHandler<TContext>(
            Type handlerType,
            IConditionDefinition definition,
            TContext context)
            where TContext : IConditionContext;

        IConditionAction CreateAction<TContext>(
            Type actionType,
            IConditionActionDefinition definition,
            TContext context)
            where TContext : IConditionContext;

        IConditionAggregation CreateAggregation(
            Type aggregationType,
            IConditionAggregationDefinition definition);
    }
}