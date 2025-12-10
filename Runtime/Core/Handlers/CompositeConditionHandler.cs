using System;
using System.Collections.Generic;
using ConditionSystem.Runtime.Core.Abstractions;
using ConditionSystem.Runtime.Core.Extensions;
using R3;

namespace ConditionSystem.Runtime.Core.Handlers
{
    public sealed class CompositeConditionHandler<TContext> : ConditionHandler<TContext>, IDisposable
        where TContext : IConditionContext
    {
        #region Fields

        private readonly IConditionEntryDefinition[] _entries;
        private readonly IConditionHandler<TContext>[] _handlers;
        private readonly IConditionAggregation _aggregation;
        private readonly Observable<bool>[] _stateStreams;

        #endregion

        #region Constructors

        public CompositeConditionHandler(
            TContext context,
            ICompositeConditionDefinition definition,
            IConditionHandlerFactory<TContext> handlerFactory,
            IConditionAggregationFactory aggregationFactory)
            : base(context)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (handlerFactory == null)
                throw new ArgumentNullException(nameof(handlerFactory));

            if (aggregationFactory == null)
                throw new ArgumentNullException(nameof(aggregationFactory));

            InitializeEntries(
                context,
                definition,
                handlerFactory,
                out _entries,
                out _handlers,
                out _stateStreams);

            _aggregation = CreateAggregation(definition, aggregationFactory);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var handler in _handlers)
            {
                if (handler is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        #endregion

        #region Public API

        public override bool IsConditionActive()
        {
            return _handlers.Length == 0 || _aggregation.Aggregate(EnumerateStates());

            IEnumerable<bool> EnumerateStates()
            {
                foreach (var handler in _handlers)
                    yield return handler.IsConditionActive();
            }
        }

        public override Observable<bool> ObserveConditionState() => _handlers.Length == 0
            ? Observable.Return(true)
            : _aggregation.Aggregate(_stateStreams);

        public override void RequestConditionActivation()
        {
            for (var i = 0; i < _handlers.Length; i++)
            {
                var usage = _entries[i].Usage;

                if (usage.HasFlag(ConditionUsage.ForActivation) == false)
                    continue;

                var handler = _handlers[i];

                if (handler.IsConditionActive())
                    continue;

                handler.RequestConditionActivation();
                break;
            }
        }

        public override Observable<IConditionDescription> ObserveDescription()
        {
            if (_handlers.Length == 0)
                return Observable.Empty<IConditionDescription>();

            return Observable
                .CombineLatest(_stateStreams)
                .Select(states =>
                {
                    var allSatisfied = _aggregation.Aggregate(states);
                    if (allSatisfied)
                        return Observable.Empty<IConditionDescription>();

                    for (var i = 0; i < states.Length; i++)
                    {
                        if (states[i])
                            continue;

                        var usage = _entries[i].Usage;
                        if (!usage.HasFlag(ConditionUsage.ForDisplay))
                            continue;

                        var descStream = _handlers[i].ObserveDescription();
                        return descStream ?? Observable.Empty<IConditionDescription>();
                    }

                    return Observable.Empty<IConditionDescription>();
                })
                .DistinctUntilChanged()
                .Switch();
        }

        #endregion

        #region Private Methods

        private static void InitializeEntries(
            TContext context,
            ICompositeConditionDefinition definition,
            IConditionHandlerFactory<TContext> handlerFactory,
            out IConditionEntryDefinition[] entries,
            out IConditionHandler<TContext>[] handlers,
            out Observable<bool>[] stateStreams)
        {
            var sourceEntries = definition.Entries;

            if (sourceEntries == null || sourceEntries.Count == 0)
            {
                entries = Array.Empty<IConditionEntryDefinition>();
                handlers = Array.Empty<IConditionHandler<TContext>>();
                stateStreams = Array.Empty<Observable<bool>>();
                return;
            }

            var count = sourceEntries.Count;

            entries = new IConditionEntryDefinition[count];
            handlers = new IConditionHandler<TContext>[count];
            stateStreams = new Observable<bool>[count];

            for (var i = 0; i < count; i++)
            {
                var entry = sourceEntries[i] ?? throw new ArgumentException(
                    "Condition entry cannot be null.", nameof(definition));

                entries[i] = entry;

                var handler = handlerFactory.Create(entry.Condition, context);
                handlers[i] = handler;

                stateStreams[i] = handler
                    .ObserveConditionState();
            }
        }

        private static IConditionAggregation CreateAggregation(
            ICompositeConditionDefinition definition,
            IConditionAggregationFactory aggregationFactory)
        {
            var aggregationDefinition = definition.Aggregation
                                        ?? throw new ArgumentNullException(nameof(definition.Aggregation));

            return aggregationFactory.Create(aggregationDefinition);
        }

        #endregion
    }
}