using System;
using ConditionSystem.Runtime.Core.Abstractions;
using R3;

namespace ConditionSystem.Runtime.Core.Handlers
{
    public abstract class ConditionHandler<TContext> : IConditionHandler<TContext>
        where TContext : IConditionContext
    {
        #region Properties

        public TContext Context { get; }

        #endregion

        #region Constructors

        protected ConditionHandler(TContext context) =>
            Context = context ?? throw new ArgumentNullException(nameof(context));

        #endregion

        #region IConditionHandler<TContext>

        public virtual void RequestConditionActivation()
        {
        }

        public virtual bool IsConditionActive() => false;

        public virtual Observable<bool> ObserveConditionState() => Observable.Return(false);

        public virtual Observable<IConditionDescription> ObserveDescription() =>
            Observable.Empty<IConditionDescription>();

        #endregion
    }
}