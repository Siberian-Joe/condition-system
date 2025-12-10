using R3;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionStateService<TContext>
        where TContext : IConditionContext
    {
        IConditionHandler<TContext> Register(IConditionDefinition definition, TContext context);

        void Unregister(IConditionHandler<TContext> handler);

        bool IsConditionActive(IConditionHandler<TContext> handler);

        Observable<bool> ObserveConditionState(IConditionHandler<TContext> handler);

        Observable<TDescription> ObserveDescription<TDescription>(IConditionHandler<TContext> handler)
            where TDescription : IConditionDescription;

        void RequestConditionActivation(IConditionHandler<TContext> handler);
    }
}