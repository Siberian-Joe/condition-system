namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionActionFactory<in TContext> where TContext : IConditionContext
    {
        IConditionAction Create(
            IConditionActionDefinition definition,
            TContext context);
    }
}