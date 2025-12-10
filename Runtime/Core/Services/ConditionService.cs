using System;
using System.Collections.Generic;
using ConditionSystem.Runtime.Core.Abstractions;
using R3;
using UnityEngine;

namespace ConditionSystem.Runtime.Core.Services
{
    public sealed class ConditionService<TContext, TKey> : IConditionService<TContext, TKey>
        where TContext : IConditionContext
    {
        #region Fields

        private readonly IConditionStateService<TContext> _stateService;
        private readonly IConditionActionFactory<TContext> _actionFactory;

        private readonly Dictionary<TKey, Registration> _registrations = new();

        #endregion

        #region Constructors

        public ConditionService(
            IConditionStateService<TContext> stateService,
            IConditionActionFactory<TContext> actionFactory)
        {
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
        }

        #endregion

        #region IConditionService<TContext, TKey>

        public void Register(
            TKey key,
            IConditionDefinition definition,
            TContext context)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Unregister(key);

            var handler = _stateService.Register(definition, context);

            IDisposable triggerSubscription = null;
            IReadOnlyList<IConditionAction> actions = Array.Empty<IConditionAction>();

            if (definition is IConditionReactionDefinition { Actions: { Count: > 0 } } triggerDefinition)
            {
                actions = BuildActions(triggerDefinition.Actions, context);

                var conditionState = _stateService
                    .ObserveConditionState(handler)
                    .DistinctUntilChanged();

                triggerSubscription = conditionState
                    .Where(isMet => isMet)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        ExecuteAll(actions);
                        Unregister(key);
                    });
            }

            _registrations[key] = new Registration(
                handler: handler,
                triggerSubscription: triggerSubscription,
                actions: actions);
        }

        public void Unregister(TKey key)
        {
            if (key is null)
                return;

            if (_registrations.TryGetValue(key, out var registration) == false)
                return;

            registration.Dispose();
            _stateService.Unregister(registration.Handler);
            _registrations.Remove(key);
        }

        public bool IsConditionActive(TKey key) =>
            _registrations.TryGetValue(key, out var registration) &&
            _stateService.IsConditionActive(registration.Handler);

        public Observable<bool> ObserveConditionState(TKey key) =>
            _registrations.TryGetValue(key, out var registration) == false
                ? Observable.Return(false)
                : _stateService.ObserveConditionState(registration.Handler);

        public Observable<TDescription> ObserveDescription<TDescription>(TKey key)
            where TDescription : IConditionDescription =>
            _registrations.TryGetValue(key, out var registration) == false
                ? Observable.Empty<TDescription>()
                : _stateService.ObserveDescription<TDescription>(registration.Handler);

        public void RequestConditionActivation(TKey key)
        {
            if (_registrations.TryGetValue(key, out var registration) == false)
                return;

            _stateService.RequestConditionActivation(registration.Handler);
        }

        #endregion

        #region Private Methods

        private static void ExecuteAll(IReadOnlyList<IConditionAction> actions)
        {
            if (actions == null)
                return;

            foreach (var action in actions)
            {
                if (action == null)
                    continue;

                try
                {
                    action.Execute();
                }
                catch (Exception exception)
                {
                    Debug.LogError(
                        $"[{nameof(ConditionService<TContext, TKey>)}] Action {action.GetType().Name} failed: {exception}");
                }
            }
        }

        private IConditionAction[] BuildActions(
            IReadOnlyList<IConditionActionDefinition> definitions,
            TContext context)
        {
            if (definitions == null || definitions.Count == 0)
                return Array.Empty<IConditionAction>();

            var result = new IConditionAction[definitions.Count];

            for (var i = 0; i < definitions.Count; i++)
            {
                var def = definitions[i]
                          ?? throw new InvalidOperationException(
                              $"Action definition at index {i} is null.");

                result[i] = _actionFactory.Create(def, context);
            }

            return result;
        }

        #endregion

        #region Nested Types

        private sealed class Registration : IDisposable
        {
            #region Fields

            private readonly IDisposable _triggerSubscription;

            #endregion

            #region Properties

            public IConditionHandler<TContext> Handler { get; }
            public IReadOnlyList<IConditionAction> Actions { get; }

            #endregion

            #region Constructors

            public Registration(
                IConditionHandler<TContext> handler,
                IDisposable triggerSubscription,
                IReadOnlyList<IConditionAction> actions)
            {
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
                _triggerSubscription = triggerSubscription;
                Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            }

            #endregion

            #region IDisposable

            public void Dispose() => _triggerSubscription?.Dispose();

            #endregion
        }

        #endregion
    }
}