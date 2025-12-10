using System;
using System.Collections.Generic;
using ConditionSystem.Runtime.Core.Abstractions;
using R3;

namespace ConditionSystem.Runtime.Core.Services
{
    public sealed class ConditionStateService<TContext> : IConditionStateService<TContext>
        where TContext : IConditionContext
    {
        #region Fields

        private readonly IConditionHandlerFactory<TContext> _handlerFactory;
        private readonly HashSet<IConditionHandler<TContext>> _handlers = new();

        #endregion

        #region Constructors

        public ConditionStateService(IConditionHandlerFactory<TContext> handlerFactory) => _handlerFactory =
            handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

        #endregion

        #region IConditionStateService<TContext>

        public IConditionHandler<TContext> Register(IConditionDefinition definition, TContext context)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var handler = _handlerFactory.Create(definition, context);
            _handlers.Add(handler);
            return handler;
        }

        public void Unregister(IConditionHandler<TContext> handler)
        {
            if (handler == null)
                return;

            if (_handlers.Remove(handler) && handler is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public bool IsConditionActive(IConditionHandler<TContext> handler) =>
            handler != null &&
            _handlers.Contains(handler) &&
            handler.IsConditionActive();

        public Observable<bool> ObserveConditionState(IConditionHandler<TContext> handler)
        {
            if (handler == null || !_handlers.Contains(handler))
                return Observable.Return(false);

            return handler.ObserveConditionState();
        }

        public Observable<TDescription> ObserveDescription<TDescription>(IConditionHandler<TContext> handler)
            where TDescription : IConditionDescription
        {
            if (handler == null || !_handlers.Contains(handler))
                return Observable.Empty<TDescription>();

            return handler
                .ObserveDescription()
                .Where(description => description is TDescription)
                .Select(description => (TDescription)description);
        }

        public void RequestConditionActivation(IConditionHandler<TContext> handler)
        {
            if (handler == null || !_handlers.Contains(handler))
                return;

            handler.RequestConditionActivation();
        }

        #endregion
    }
}