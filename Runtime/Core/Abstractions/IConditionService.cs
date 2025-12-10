using R3;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionService<in TContext, in TKey> where TContext : IConditionContext
    {
        void Register(
            TKey key,
            IConditionDefinition definition,
            TContext context);

        void Unregister(TKey key);

        bool IsConditionActive(TKey key);

        Observable<bool> ObserveConditionState(TKey key);

        Observable<TDescription> ObserveDescription<TDescription>(TKey key)
            where TDescription : IConditionDescription;

        void RequestConditionActivation(TKey key);
    }
}