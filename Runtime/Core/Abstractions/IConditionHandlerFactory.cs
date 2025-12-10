namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionHandlerFactory<TContext> where TContext : IConditionContext
    {
        IConditionHandler<TContext> Create(IConditionDefinition definition, TContext context);
    }
}
