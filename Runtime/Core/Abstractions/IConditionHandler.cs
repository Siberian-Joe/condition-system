using R3;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionHandler
    {
        void RequestConditionActivation();
        bool IsConditionActive();
        Observable<bool> ObserveConditionState();
        Observable<IConditionDescription> ObserveDescription();
    }

    public interface IConditionHandler<out TContext> : IConditionHandler
        where TContext : IConditionContext
    {
        TContext Context { get; }
    }
}